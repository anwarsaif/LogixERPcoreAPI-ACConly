using System.Globalization;
using System.IO.Compression;
using Logix.Application.Common;
using Logix.Application.Wrapper;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Logix.MVC.LogixAPIs.Main
{
    public class BackupController : BaseMainApiController
    {
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;

        public BackupController(IWebHostEnvironment env,
            IConfiguration configuration)
        {
            this.env = env;
            this.configuration = configuration;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                List<BackupVM> files = new();
                string folderBackup = Path.Join(env.ContentRootPath, FilesPath.BackupDbPath);
                // Check if the directory exists
                if (Directory.Exists(folderBackup))
                {
                    // get all files that in backup folder
                    string[] filePaths = Directory.GetFiles(folderBackup);

                    foreach (string filePath in filePaths)
                    {
                        string fileName = Path.GetFileName(filePath);
                        files.Add(new BackupVM() { Name = fileName, Path = FilesPath.BackupDbPath + "/" + fileName });
                    }
                }

                return Ok(await Result<List<BackupVM>>.SuccessAsync(files));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"===Exp, {ex.Message}"));
            }
        }

        [HttpGet("Create")]
        public async Task<ActionResult> Create()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                string folderBackup; string fileNameZip; string folderName;

                using (var objCnn = new SqlConnection(configuration.GetConnectionString("LogixLocal")))
                {
                    await objCnn.OpenAsync();
                    string dataBaseName = objCnn.Database;
                    string name = $"{dataBaseName}-{DateTime.Now:yyyy-MM-dd-HH-mm}";
                    string fileName = $"{name}.bak";
                    fileNameZip = $"{name}.zip";
                    folderBackup = Path.Join(env.ContentRootPath, FilesPath.BackupDbPath);
                    folderName = Path.Combine(folderBackup, name);

                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    using (var objCmd = objCnn.CreateCommand())
                    {
                        objCmd.CommandTimeout = 66666;
                        objCmd.CommandType = System.Data.CommandType.Text;
                        objCmd.CommandText = "BACKUP DATABASE @name TO DISK = @fileName";
                        objCmd.Parameters.AddWithValue("@name", dataBaseName);
                        objCmd.Parameters.AddWithValue("@fileName", Path.Combine(folderName, fileName));
                        objCmd.ExecuteNonQuery();
                    }
                }

                ZipFile.CreateFromDirectory(folderName, Path.Combine(folderBackup, fileNameZip), CompressionLevel.Fastest, false);

                return Ok(await Result.SuccessAsync());
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"===Exp, {ex.Message}"));
            }
        }
    }
}