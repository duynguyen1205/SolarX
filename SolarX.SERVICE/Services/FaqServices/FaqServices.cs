using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.SERVICE.Abstractions.IFaqServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.FaqServices;

public class FaqServices : IFaqServices
{

    private readonly IBaseRepository<Faq, Guid> _faqRepository;

    public FaqServices(IBaseRepository<Faq, Guid> faqRepository)
    {
        _faqRepository = faqRepository;
    }

    public async Task<Result<PagedResult<ResponseModel.FaqResponseModel>>> GetFaq(string? searchTerm, int pageIndex, int pageSize)
    {
        var query = _faqRepository.GetAllWithQuery(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Question.Contains(searchTerm) || x.Answear.Contains(searchTerm));
        }

        var listFaq = await PagedResult<Faq>.CreateAsync(query, pageIndex, pageSize);
        var response = listFaq.Items.Select(x => new ResponseModel.FaqResponseModel(
            x.Id,
            x.Question,
            x.Answear
        )).ToList();

        var result = new PagedResult<ResponseModel.FaqResponseModel>(response, listFaq.PageIndex, listFaq.PageSize,
            listFaq.TotalCount);

        return Result<PagedResult<ResponseModel.FaqResponseModel>>.CreateResult("Get faq successfully", 200, result);
    }

    public async Task<Result> CreateFaq(RequestModel.CreateFaqReq request)
    {
        var faq = await _faqRepository.GetAllWithQuery(x => x.Question == request.Question && x.Answear == request.Answer)
            .FirstOrDefaultAsync();
        if (faq != null)
        {
            return Result.CreateResult("Faq already exist", 400);
        }

        var newFaq = new Faq
        {
            Id = Guid.NewGuid(),
            Question = request.Question,
            Answear = request.Answer
        };

        _faqRepository.AddEntity(newFaq);
        return Result.CreateResult("Create faq successfully", 201);
    }

    public async Task<Result> UpdateFaq(Guid faqId, RequestModel.UpdateFaqReq request)
    {
        var faq = await _faqRepository.GetById(faqId);
        if (faq == null || faq.IsDeleted)
        {
            return Result.CreateResult("Faq not found", 404);
        }

        if (request.Question != null && request.Question != faq.Question)
        {
            faq.Question = request.Question;
        }

        if (request.Answer != null && request.Answer != faq.Answear)
        {
            faq.Answear = request.Answer;
        }

        _faqRepository.UpdateEntity(faq);
        return Result.CreateResult("Update faq successfully", 201);
    }

    public async Task<Result> DeleteFaq(Guid faqId)
    {
        var faq = await _faqRepository.GetById(faqId);
        if (faq == null || faq.IsDeleted)
        {
            return Result.CreateResult("Faq not found", 404);
        }

        _faqRepository.RemoveEntity(faq);
        return Result.CreateResult("Delete faq successfully", 201);
    }
}