using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using EInvoiceKSADemo.Helpers.Zatca;
using EInvoiceKSADemo.Helpers.Zatca.Models;
using Logix.Application.Common;
using Logix.Application.Helpers.Sal;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Sales.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Sales
{
    public class CertificateSettingController : BaseSalesApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ILocalizationService localization;
        private readonly IAccServiceManager accServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IZatcaHelper zatcaHelper;
        private readonly IWebHostEnvironment env;
        private readonly ILogError logError;
        private readonly ICertificateConfiguration certificateConfiguration;

        public CertificateSettingController(IPermissionHelper permission,
            ICurrentData session,
            ISysConfigurationHelper configurationHelper,
            ILocalizationService localization,
            IAccServiceManager accServiceManager,
            IMainServiceManager mainServiceManager,
            IZatcaHelper zatcaHelper,
            IWebHostEnvironment env,
            ILogError logError,
            ICertificateConfiguration certificateConfiguration
            )
        {
            this.permission = permission;
            this.session = session;
            this.configurationHelper = configurationHelper;
            this.localization = localization;
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.zatcaHelper = zatcaHelper;
            this.env = env;
            this.logError = logError;
            this.certificateConfiguration = certificateConfiguration;
        }

        [HttpPost("TestOnboarding")]
        public async Task<IActionResult> TestOnboarding(CertificateSearchVm obj)
        {
            return Ok(await Result<CertificateSearchVm>.SuccessAsync(obj));
        }

        [HttpGet("TestErrorLog")]
        public async Task<IActionResult> TestErrorLog(string errorMessage, string functionName, string className)
        {
            LogError.AddToFile(errorMessage, functionName, className);

            return Ok();
        }

        [HttpPost("TestZatca")]
        public async Task<IActionResult> TestZatca()
        {
            //var x = certificateConfiguration.GetCertificateDetails("1", "1");
            await zatcaHelper.Initiate(1, 1);
            return Ok();
        }

        [HttpGet("Binding")]
        public async Task<IActionResult> Binding()
        {
            try
            {
                var chk1 = await permission.HasPermission(1903, PermissionType.Add);
                var chk2 = await permission.HasPermission(1979, PermissionType.Add);
                if (!chk1 && !chk2)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                CertificateSearchVm obj = new();
                long facilityId = session.FacilityId;
                string environment = await configurationHelper.GetValue(336, facilityId); // Simulation 0 Core 1
                if (string.IsNullOrEmpty(environment))
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("EnvironmentError")}"));
                else
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    if (!string.IsNullOrEmpty(environment) && environment == "1")
                    {
                        // ACC Certificate Settings
                        obj.CertificateSetting = await zatcaHelper.ACC_CertificateSettings(facilityId);
                        obj.BasicData.CsrTypeValue = "Production";
                    }
                    else
                    {
                        // ACC Certificate Settings Simulation
                        obj.CertificateSetting = await zatcaHelper.ACC_CertificateSettings_Simulation(facilityId);
                        obj.BasicData.CsrTypeValue = "PRE";
                    }
                }

                // Get Facility Data
                var getFacilty = await accServiceManager.AccFacilityService.GetOne(x => x.FacilityId == facilityId);
                if (getFacilty.Succeeded && getFacilty.Data != null)
                {
                    obj.BasicData.VatNumber = getFacilty.Data.VatNumber;
                    obj.BasicData.CountryCode = getFacilty.Data.CountryCode;
                    obj.BasicData.FacilityEmail = getFacilty.Data.FacilityEmail;
                }

                // Get Branches Data
                var getBraches = await mainServiceManager.InvestBranchService.GetAll(x => x.Isdel == false && x.FacilityId == facilityId);
                if (getBraches.Succeeded)
                {
                    List<CertificateBranchesVm> branches = new();
                    foreach (var branch in getBraches.Data)
                    {
                        branches.Add(new CertificateBranchesVm()
                        {
                            BranchId = branch.BranchId,
                            BraName = branch.BraName,
                            BraName2 = branch.BraName2,
                            BranchCode = branch.BranchCode,
                            CountryCode = branch.CountryCode,
                            ShortAddress = branch.ShortAddress,
                            OTPCode = ""
                        });
                    }

                    obj.Branches = branches;
                }

                return Ok(await Result<CertificateSearchVm>.SuccessAsync(obj));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp: {ex.Message}"));
            }
        }


        [HttpPost("Onboarding")]
        public async Task<IActionResult> Onboarding(CertificateSearchVm obj)
        {
            try
            {
                string createKeyErrMsgs = "";
                bool success = false; // this variable used to determine the type of result return (success or fail)
                string labMsg = "";
                CertificateDetails certificateDetails = new();
                string environment = await configurationHelper.GetValue(336, session.FacilityId); // Simulation 0 Core 1
                if (string.IsNullOrEmpty(environment))
                    return Ok(await Result.FailAsync($"{localization.GetCommonResource("EnvironmentError")}"));

                HttpContext context = this.HttpContext;
                long UserId = session.UserId;
                long FacilityId = session.FacilityId;

                // get selected branches
                List<CertificateBranchesVm> branchesTable = new();
                if (obj.Branches != null && obj.Branches.Count != 0)
                {
                    branchesTable = obj.Branches.Where(x => x.IsSelected == true).ToList();
                    if (branchesTable.Count == 0)
                        return Ok(await Result.FailAsync($"No branches selected"));
                }

                certificateDetails = GetCertificateDetails();
                string SystemVersion = await GetVersions();
                foreach (var branchRow in branchesTable)
                {
                    success = false;
                    int Branch_ID = branchRow.BranchId;
                    string OTPCode = branchRow.OTPCode ?? "";

                    // Set file paths based on Branch_ID and Facility_ID
                    string PrivateKeyFilePath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{FacilityId}", $"Branch_{Branch_ID}", "PrivateKey.pem");
                    string CSRFilePath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{FacilityId}", $"Branch_{Branch_ID}", "CSR.csr");
                    string CSRContent = "";
                    string PrivateKeyContent = "";

                    // initiate zatca helper properties
                    await zatcaHelper.Initiate(FacilityId, Branch_ID, context);

                    if (SystemVersion != "0")
                    {
                        string labmsg = ""; // send to function by ref, to get the error msg if found
                        bool chk = CreatePrivetKeyAnsCSR(FacilityId, Branch_ID, SystemVersion, ref certificateDetails, branchRow.ShortAddress ?? "", branchRow.CountryCode ?? "", branchRow.BraName2 ?? "", ref labmsg, obj.BasicData);
                        if (!chk)
                        {
                            createKeyErrMsgs += createKeyErrMsgs + " ||";
                        }
                    }
                    else
                        return Ok(await Result.FailAsync("لا يوجد رقم اصدار النظام There is no system version"));

                    // Read CSR and Private Key files as before
                    if (System.IO.File.Exists(CSRFilePath))
                    {
                        try
                        {
                            CSRContent = System.IO.File.ReadAllText(CSRFilePath);
                        }
                        catch (Exception ex)
                        {
                            logError.Add(ex.Message, "Onboarding", GetType().Name);
                            labMsg = "Error reading CSR file: " + ex.Message;
                            continue;
                        }
                    }
                    else
                    {
                        labMsg = "CSR file not found";
                        continue;
                    }

                    if (System.IO.File.Exists(PrivateKeyFilePath))
                    {
                        try
                        {
                            PrivateKeyContent = System.IO.File.ReadAllText(PrivateKeyFilePath);
                        }
                        catch (Exception ex)
                        {
                            logError.Add(ex.Message, "Onboarding", GetType().Name);
                            labMsg = "Error reading Private Key file: " + ex.Message;
                            continue;
                        }
                    }
                    else
                    {
                        labMsg = "Private Key file not found";
                        continue;
                    }

                    if (string.IsNullOrEmpty(CSRContent) || string.IsNullOrEmpty(PrivateKeyContent))
                    {
                        labMsg = "Error in creating the private key and CSR certificate";
                        continue;
                    }

                    // Insert into appropriate environment and handle OnboardingCSID
                    certificateDetails.CSR = Convert.ToBase64String(Encoding.UTF8.GetBytes(CSRContent));
                    certificateDetails.PrivateKey = PrivateKeyContent;
                    certificateDetails.Branch_id = Branch_ID.ToString();

                    if (!string.IsNullOrEmpty(environment) && environment == "1")
                    {
                        var add = await zatcaHelper.InsertACC_CertificateSettings(certificateDetails, UserId, FacilityId);
                    }
                    else if (!string.IsNullOrEmpty(environment) && environment == "0")
                    {
                        var add = await zatcaHelper.InsertACC_CertificateSettings_Simulation(certificateDetails, UserId, FacilityId);
                    }

                    if (!string.IsNullOrEmpty(PrivateKeyContent) && !string.IsNullOrEmpty(CSRContent))
                    {
                        CSIDResultModel? OnboardingCSIDresult = await zatcaHelper.OnboardingCSID(Convert.ToInt32(OTPCode), FacilityId, certificateDetails.CSR, certificateDetails.Branch_id);
                        if (OnboardingCSIDresult != null)
                        {
                            if (!string.IsNullOrEmpty(environment) && environment == "1")
                            {
                                var update = await zatcaHelper.UpdateACC_CertificateSettings(OnboardingCSIDresult, UserId, FacilityId, certificateDetails.Branch_id);
                            }
                            else if (!string.IsNullOrEmpty(environment) && environment == "0")
                            {
                                var update = await zatcaHelper.UpdateACC_CertificateSettings_Simulation(OnboardingCSIDresult, UserId, FacilityId, certificateDetails.Branch_id);
                            }

                            success = true;
                            labMsg = localization.GetResource1("CreateSuccess");
                        }
                        else
                        {
                            labMsg = "Error while onboarding CSID";
                        }
                    }
                    else
                    {
                        labMsg = localization.GetCommonResource("CSRErorr");
                    }
                }

                return success ? Ok(await Result.SuccessAsync(labMsg)) : Ok(await Result.FailAsync(labMsg));
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "Onboarding", GetType().Name);
                return Ok(await Result.FailAsync($"{localization.GetCommonResource("CSRErorr")}"));
            }
        }


        [NonAction]
        private CertificateDetails GetCertificateDetails()
        {
            CertificateDetails certificateDetails = new();
            certificateDetails.Certificate = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("MIID9jCCA5ugAwIBAgITbwAAeCy9aKcLA99HrAABAAB4LDAKBggqhkjOPQQDAjBjMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxEzARBgoJkiaJk/IsZAEZFgNnb3YxFzAVBgoJkiaJk/IsZAEZFgdleHRnYXp0MRwwGgYDVQQDExNUU1pFSU5WT0lDRS1TdWJDQS0xMB4XDTIyMDQxOTIwNDkwOVoXDTI0MDQxODIwNDkwOVowWTELMAkGA1UEBhMCU0ExEzARBgNVBAoTCjMxMjM0NTY3ODkxDDAKBgNVBAsTA1RTVDEnMCUGA1UEAxMeVFNULS05NzA1NjAwNDAtMzEyMzQ1Njc4OTAwMDAzMFYwEAYHKoZIzj0CAQYFK4EEAAoDQgAEYYMMoOaFYAhMO/steotfZyavr6p11SSlwsK9azmsLY7b1b+FLhqMArhB2dqHKboxqKNfvkKDePhpqjui5hcn0aOCAjkwggI1MIGaBgNVHREEgZIwgY+kgYwwgYkxOzA5BgNVBAQMMjEtVFNUfDItVFNUfDMtNDdmMTZjMjYtODA2Yi00ZTE1LWIyNjktN2E4MDM4ODRiZTljMR8wHQYKCZImiZPyLGQBAQwPMzEyMzQ1Njc4OTAwMDAzMQ0wCwYDVQQMDAQxMTAwMQwwCgYDVQQaDANUU1QxDDAKBgNVBA8MA1RTVDAdBgNVHQ4EFgQUO5ZiU7NakU3eejVa3I2S1B2sDwkwHwYDVR0jBBgwFoAUdmCM+wagrGdXNZ3PmqynK5k1tS8wTgYDVR0fBEcwRTBDoEGgP4Y9aHR0cDovL3RzdGNybC56YXRjYS5nb3Yuc2EvQ2VydEVucm9sbC9UU1pFSU5WT0lDRS1TdWJDQS0xLmNybDCBrQYIKwYBBQUHAQEEgaAwgZ0wbgYIKwYBBQUHMAGGYmh0dHA6Ly90c3RjcmwuemF0Y2EuZ292LnNhL0NlcnRFbnJvbGwvVFNaRWludm9pY2VTQ0ExLmV4dGdhenQuZ292LmxvY2FsX1RTWkVJTlZPSUNFLVN1YkNBLTEoMSkuY3J0MCsGCCsGAQUFBzABhh9odHRwOi8vdHN0Y3JsLnphdGNhLmdvdi5zYS9vY3NwMA4GA1UdDwEB/wQEAwIHgDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUHAwMwJwYJKwYBBAGCNxUKBBowGDAKBggrBgEFBQcDAjAKBggrBgEFBQcDAzAKBggqhkjOPQQDAgNJADBGAiEA7mHT6yg85jtQGWp3M7tPT7Jk2+zsvVHGs3bU5Z7YE68CIQD60ebQamYjYvdebnFjNfx4X4dop7LsEBFCNSsLY0IFaQ=="));
            certificateDetails.ExpiredDate = DateTime.Now.AddYears(1);
            certificateDetails.StartedDate = DateTime.Now;
            certificateDetails.Secret = "Xlj15LyMCgSC66ObnEO/qVPfhSbs3kDTjWnGheYhfSs=";
            certificateDetails.UserName = "TUlJRDFEQ0NBM21nQXdJQkFnSVRid0FBZTNVQVlWVTM0SS8rNVFBQkFBQjdkVEFLQmdncWhrak9QUVFEQWpCak1SVXdFd1lLQ1pJbWlaUHlMR1FCR1JZRmJHOWpZV3d4RXpBUkJnb0praWFKay9Jc1pBRVpGZ05uYjNZeEZ6QVZCZ29Ka2lhSmsvSXNaQUVaRmdkbGVIUm5ZWHAwTVJ3d0dnWURWUVFERXhOVVUxcEZTVTVXVDBsRFJTMVRkV0pEUVMweE1CNFhEVEl5TURZeE1qRTNOREExTWxvWERUSTBNRFl4TVRFM05EQTFNbG93U1RFTE1Ba0dBMVVFQmhNQ1UwRXhEakFNQmdOVkJBb1RCV0ZuYVd4bE1SWXdGQVlEVlFRTEV3MW9ZWGxoSUhsaFoyaHRiM1Z5TVJJd0VBWURWUVFERXdreE1qY3VNQzR3TGpFd1ZqQVFCZ2NxaGtqT1BRSUJCZ1VyZ1FRQUNnTkNBQVRUQUs5bHJUVmtvOXJrcTZaWWNjOUhEUlpQNGI5UzR6QTRLbTdZWEorc25UVmhMa3pVMEhzbVNYOVVuOGpEaFJUT0hES2FmdDhDL3V1VVk5MzR2dU1ObzRJQ0p6Q0NBaU13Z1lnR0ExVWRFUVNCZ0RCK3BId3dlakViTUJrR0ExVUVCQXdTTVMxb1lYbGhmREl0TWpNMGZETXRNVEV5TVI4d0hRWUtDWkltaVpQeUxHUUJBUXdQTXpBd01EYzFOVGc0TnpBd01EQXpNUTB3Q3dZRFZRUU1EQVF4TVRBd01SRXdEd1lEVlFRYURBaGFZWFJqWVNBeE1qRVlNQllHQTFVRUR3d1BSbTl2WkNCQ2RYTnphVzVsYzNNek1CMEdBMVVkRGdRV0JCU2dtSVdENmJQZmJiS2ttVHdPSlJYdkliSDlIakFmQmdOVkhTTUVHREFXZ0JSMllJejdCcUNzWjFjMW5jK2FyS2NybVRXMUx6Qk9CZ05WSFI4RVJ6QkZNRU9nUWFBL2hqMW9kSFJ3T2k4dmRITjBZM0pzTG5waGRHTmhMbWR2ZGk1ellTOURaWEowUlc1eWIyeHNMMVJUV2tWSlRsWlBTVU5GTFZOMVlrTkJMVEV1WTNKc01JR3RCZ2dyQmdFRkJRY0JBUVNCb0RDQm5UQnVCZ2dyQmdFRkJRY3dBWVppYUhSMGNEb3ZMM1J6ZEdOeWJDNTZZWFJqWVM1bmIzWXVjMkV2UTJWeWRFVnVjbTlzYkM5VVUxcEZhVzUyYjJsalpWTkRRVEV1WlhoMFoyRjZkQzVuYjNZdWJHOWpZV3hmVkZOYVJVbE9WazlKUTBVdFUzVmlRMEV0TVNneEtTNWpjblF3S3dZSUt3WUJCUVVITUFHR0gyaDBkSEE2THk5MGMzUmpjbXd1ZW1GMFkyRXVaMjkyTG5OaEwyOWpjM0F3RGdZRFZSMFBBUUgvQkFRREFnZUFNQjBHQTFVZEpRUVdNQlFHQ0NzR0FRVUZCd01DQmdnckJnRUZCUWNEQXpBbkJna3JCZ0VFQVlJM0ZRb0VHakFZTUFvR0NDc0dBUVVGQndNQ01Bb0dDQ3NHQVFVRkJ3TURNQW9HQ0NxR1NNNDlCQU1DQTBrQU1FWUNJUUNWd0RNY3E2UE8rTWNtc0JYVXovdjFHZGhHcDdycVNhMkF4VEtTdjgzOElBSWhBT0JOREJ0OSszRFNsaWpvVmZ4enJkRGg1MjhXQzM3c21FZG9HV1ZyU3BHMQ==";
            return certificateDetails;
        }

        [NonAction]
        private bool ReplaceTemplateVariables(string CSRconfigTemplatePath, string SystemVersion, long Branch_ID, ref CertificateDetails CertificateDetails, string ShortAddress, string CountryCode, string BRA_NAME2, CertifiBasicData basicData)
        {
            try
            {
                string CSR_Guid = "";
                try
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        string GuidGeneratingString = BRA_NAME2 + Branch_ID.ToString();
                        byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(GuidGeneratingString));

                        // Resize the byte array to ensure it's 16 bytes in length
                        Array.Resize(ref hashBytes, 16);

                        // Assign the hashed GUID to the Id property
                        Guid hashedGuid = new Guid(hashBytes);
                        CSR_Guid = hashedGuid.ToString();
                    }
                }
                catch (Exception ex)
                {
                    logError.Add(ex.Message, "ReplaceTemplateVariables", GetType().Name);
                    CSR_Guid = Guid.NewGuid().ToString();
                }

                string SN = $"1-LogixERP|2-{SystemVersion}|3-{CSR_Guid}";
                string CN = $"{BRA_NAME2}-{basicData.VatNumber}-{Branch_ID}";

                CertificateDetails.Guid = CSR_Guid;
                CertificateDetails.CN = CN;
                CertificateDetails.SN = SN;
                CertificateDetails.SystemVersion = SystemVersion;
                CertificateDetails.Branch_id = Branch_ID.ToString();
                CertificateDetails.UID = basicData.VatNumber;
                CertificateDetails.OU = (basicData.VatNumber != null && basicData.VatNumber.Length >= 10) ? basicData.VatNumber.Substring(0, 10) : basicData.VatNumber;
                CertificateDetails.O = BRA_NAME2;

                // Read and replace template content
                string templateContent = System.IO.File.ReadAllText(CSRconfigTemplatePath);
                templateContent = templateContent.Replace("@Email", basicData.FacilityEmail)
                    .Replace("@UID", basicData.VatNumber)
                    .Replace("@OU", (basicData.VatNumber != null && basicData.VatNumber.Length >= 10) ? basicData.VatNumber.Substring(0, 10) : basicData.VatNumber)
                    .Replace("@Name", BRA_NAME2)
                    .Replace("@Title", basicData.InvoiceType)
                    .Replace("@ShortAddress", ShortAddress)
                    .Replace("@Category", basicData.Category)
                    .Replace("@CuntryCode", CountryCode).Replace("@SystemVersion", SystemVersion)
                    .Replace("@SN", SN)
                    .Replace("@CN", CN)
                    .Replace("@Pre", basicData.CsrTypeValue == "Production" ? "" : "PRE");

                // Write the updated content back to the file
                System.IO.File.WriteAllText(CSRconfigTemplatePath, templateContent);
                return true;
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "ReplaceTemplateVariables", GetType().Name);
                return false;
            }
        }

        [NonAction]
        private string GetCSRconfigTemplate()
        {
            string CSRconfigTemplate = @"oid_section = OIDs
[OIDs]
certificateTemplateName = 1.3.6.1.4.1.311.20.2
[req]
default_bits = 2048
emailAddress = @Email
req_extensions = v3_req
x509_extensions = v3_ca
prompt = no
default_md = sha256
req_extensions = req_ext
distinguished_name = dn
[dn]
C=@CuntryCode
OU=@OU
O=@Name
CN=@CN
[v3_req]
basicConstraints = CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment
[req_ext]
certificateTemplateName = ASN1:PRINTABLESTRING:@PreZATCA-Code-Signing
subjectAltName = dirName:alt_names
[alt_names]
SN=@SN
UID=@UID
title=@Title
registeredAddress=@ShortAddress
businessCategory=@Category";

            return CSRconfigTemplate;
        }

        [NonAction]
        private bool CreateCSRconfigTemplate(ref string Message, long Facility_ID, string Branch_ID)
        {
            try
            {
                string CSRconfigTemplate = GetCSRconfigTemplate();
                string FolderName = Path.Combine("Files", "Zatca", "CSRconfigTemplate", $"Facility_{Facility_ID}", $"Branch_{Branch_ID}");
                string fullPath = Path.Combine(env.ContentRootPath, FolderName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                string CSRconfigTemplatePath = Path.Combine(fullPath, "CSRconfigTemplate.cnf");
                string fileContent = CSRconfigTemplate;
                try
                {
                    System.IO.File.WriteAllText(CSRconfigTemplatePath, fileContent);
                    return true;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    return false;
                }

            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "CreateCSRconfigTemplate", GetType().Name);
                Message = ex.Message;
                return false;
            }
        }

        [NonAction]
        private async Task<string> GetVersions()
        {
            try
            {
                var getVersions = await mainServiceManager.SysVersionService.GetAll();
                if (getVersions.Succeeded)
                {
                    var version = getVersions.Data.OrderByDescending(x => x.Id).Take(1).Select(x => x.VersionNo).FirstOrDefault();
                    return version ?? "0";
                }
                return "0";
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "GetVersions", GetType().Name);
                return "0";
            }
        }


        [NonAction]
        private bool CreatePrivetKeyAnsCSR(long Facility_ID, long Branch_ID, string SystemVersion, ref CertificateDetails CertificateDetails, string ShortAddress, string CountryCode, string BRA_NAME2, ref string labmsg, CertifiBasicData basicData)
        {
            try
            {
                string Message = "";
                if (CreateCSRconfigTemplate(ref Message, Facility_ID, Branch_ID.ToString()) == false)
                {
                    labmsg = "An error occurred while creating the CSRconfigTemplate file " + Message;
                    return false;
                }

                string CSRconfigTemplatePath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{Facility_ID}", $"Branch_{Branch_ID}", "CSRconfigTemplate.cnf");
                string folderPath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{Facility_ID}", $"Branch_{Branch_ID}");
                if (System.IO.File.Exists(CSRconfigTemplatePath))
                {
                    bool chk = ReplaceTemplateVariables(CSRconfigTemplatePath, SystemVersion, Branch_ID, ref CertificateDetails, ShortAddress, CountryCode, BRA_NAME2, basicData);
                    // here you can exit and return error msg, if ReplaceTemplateVariables faild
                }

                string PrivateKeyPath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{Facility_ID}", $"Branch_{Branch_ID}", "PrivateKey.pem");
                string CSRPath = Path.Combine(env.ContentRootPath, "Files", "Zatca", "CSRconfigTemplate", $"Facility_{Facility_ID}", $"Branch_{Branch_ID}", "CSR.csr");

                if (System.IO.File.Exists(CSRconfigTemplatePath))
                {
                    if (!(System.IO.File.Exists(PrivateKeyPath)))
                    {
                        bool privateKeyCreated = CreatePrivateKey(PrivateKeyPath);
                        if (!privateKeyCreated)
                        {
                            labmsg = "An error occurred while creating the PrivateKey file " + Message;
                            return false;
                        }
                    }
                    if (!(System.IO.File.Exists(CSRPath)))
                    {
                        bool csrCreated = CreateCSR(PrivateKeyPath, CSRPath, CSRconfigTemplatePath);
                        if (!csrCreated)
                        {
                            labmsg = "An error occurred while creating the CSR file " + Message;
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "CreatePrivetKeyAnsCSR", GetType().Name);
                return false;
            }
        }

        [NonAction]
        private bool CreatePrivateKey(string privateKeyPath)
        {
            try
            {
                string rootPath = env.ContentRootPath; // Equivalent to Server.MapPath("~")
                string opensslPath = Path.Combine(rootPath, "OpenSSL-Win64", "bin", "openssl.exe");
                if (!System.IO.File.Exists(opensslPath))
                {
                    // إذا لم يكن موجودًا، استخدم المسار الافتراضي في Program Files
                    opensslPath = @"C:\Program Files\OpenSSL-Win64\bin\openssl.exe";
                }

                string arguments = $"ecparam -name secp256k1 -genkey -noout -out \"{privateKeyPath}\"";
                string result = ExecuteOpenSSLCommand(opensslPath, arguments);
                if (string.IsNullOrEmpty(result))
                    return true;
                else
                {
                    logError.Add(result, "CreatePrivateKey", GetType().Name);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "CreatePrivateKey", GetType().Name);
                return false;
            }
        }

        [NonAction]
        private bool CreateCSR(string privateKeyPath, string csrPath, string CSRconfigTemplatePath)
        {
            try
            {
                string rootPath = env.ContentRootPath; // Equivalent to Server.MapPath("~")
                string opensslPath = Path.Combine(rootPath, "OpenSSL-Win64", "bin", "openssl.exe");
                if (!System.IO.File.Exists(opensslPath))
                {
                    // إذا لم يكن موجودًا، استخدم المسار الافتراضي في Program Files
                    opensslPath = @"C:\Program Files\OpenSSL-Win64\bin\openssl.exe";
                }

                string arguments = $"req -new -sha256 -key \"{privateKeyPath}\" -out \"{csrPath}\" -config \"{CSRconfigTemplatePath}\"";
                string result = ExecuteOpenSSLCommand(opensslPath, arguments);
                if (string.IsNullOrEmpty(result))
                    return true;
                else
                {
                    logError.Add(result, "CreateCSR", GetType().Name);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logError.Add(ex.Message, "CreateCSR", GetType().Name);
                return false;
            }
        }

        [NonAction]
        private string ExecuteOpenSSLCommand(string opensslPath, string arguments)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = opensslPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(processInfo))
                {
                    using (var reader = new StreamReader(process.StandardOutput.BaseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
