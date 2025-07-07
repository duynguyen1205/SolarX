using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.OrderServices;

namespace SolarX.SERVICE.Abstractions.IOrderServices;

public interface IOrderServices
{
    Task<Result> CreateB2BOrder(Guid sellerAgencyId, Guid buyerAgencyId, RequestModel.CreateOrderReq request);
    Task<Result> CreateB2COrder(Guid agencyId, RequestModel.PublicOrderRequest request);

    Task<Result<PagedResult<ResponseModel.OrderResponseModel>>> GetAllOrder(Guid agencyId, string? searchTerm,
        DateTimeOffset? dateOrder,
        OrderStatus? status, bool seller, int pageIndex,
        int pageSize
    );
    
    Task<Result<List<ResponseModel.OrderItemResponseModel?>>> GetOrderDetail(Guid orderId);
}