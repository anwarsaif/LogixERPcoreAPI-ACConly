using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Services.ACC
{
    public class AccReferenceTypeService: GenericQueryService<AccReferenceType, AccReferenceTypeDto, AccReferenceTypeVw>, IAccReferenceTypeService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
    private readonly IMapper _mapper;
    private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccReferenceTypeService(IQueryRepository<AccReferenceType> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
    {


        this.accRepositoryManager = accRepositoryManager;
        this._mapper = mapper;

        this.session = session;
            this.localization = localization;
        }

    public async Task<IResult<AccReferenceTypeDto>> Add(AccReferenceTypeDto entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) return await Result<AccReferenceTypeDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

        try
        {

            var item = _mapper.Map<AccReferenceType>(entity);
			var newEntity = await accRepositoryManager.AccReferenceTypeRepository.AddAndReturn(item);

            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            var entityMap = _mapper.Map<AccReferenceTypeDto>(newEntity);


            return await Result<AccReferenceTypeDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
        }
        catch (Exception exc)
        {

            return await Result<AccReferenceTypeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
        }

    }

    public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
    {
        var item = await accRepositoryManager.AccReferenceTypeRepository.GetById(Id);
        if (item == null) return Result<AccReferenceTypeDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
        item.FlagDelete = true;
        item.ModifiedOn = DateTime.Now;
        item.ModifiedBy = (int)session.UserId;
        accRepositoryManager.AccReferenceTypeRepository.Update(item);
        try
        {
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            return await Result<AccReferenceTypeDto>.SuccessAsync(_mapper.Map<AccReferenceTypeDto>(item), localization.GetMessagesResource("DeletedSuccess"));
        }
        catch (Exception exp)
        {
            return await Result<AccReferenceTypeDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
        }
    }

    public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
    {
        var item = await accRepositoryManager.AccReferenceTypeRepository.GetById(Id);
        if (item == null) return Result<AccReferenceTypeDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
        item.FlagDelete = true;
        item.ModifiedOn = DateTime.Now;
        item.ModifiedBy = (int)session.UserId;
        accRepositoryManager.AccReferenceTypeRepository.Update(item);
        try
        {
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            return await Result<AccReferenceTypeDto>.SuccessAsync(_mapper.Map<AccReferenceTypeDto>(item), localization.GetMessagesResource("DeletedSuccess"));
        }
        catch (Exception exp)
        {
            return await Result<AccReferenceTypeDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
        }
    }

    public async Task<IResult<AccReferenceTypeEditDto>> Update(AccReferenceTypeEditDto entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) return await Result<AccReferenceTypeEditDto>.FailAsync(localization.GetMessagesResource("UpdateNullEntity"));

        var item = await accRepositoryManager.AccReferenceTypeRepository.GetById(entity.ReferenceTypeId);

        if (item == null) return await Result<AccReferenceTypeEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
        item.FlagDelete = false;
        item.ModifiedOn = DateTime.Now;
        item.ModifiedBy = (int)session.UserId;
        _mapper.Map(entity, item);

        accRepositoryManager.AccReferenceTypeRepository.Update(item);

        try
        {
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            return await Result<AccReferenceTypeEditDto>.SuccessAsync(_mapper.Map<AccReferenceTypeEditDto>(item), localization.GetMessagesResource("success"));
        }
        catch (Exception exp)
        {
            Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            return await Result<AccReferenceTypeEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
        }
    }

}
    
}
