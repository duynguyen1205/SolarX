using SolarX.SERVICE.Services.Base;
using SolarX.SERVICE.Services.OrderServices;

namespace SolarX.SERVICE.Abstractions.IOrderServices;

public interface IOrderServices
{
    Task<Result> CreateB2BOrder(Guid sellerAgencyId, Guid buyerAgencyId, RequestModel.CreateOrderReq request);
    Task<Result> CreateB2COrder(Guid agencyId, RequestModel.PublicOrderRequest request);
}