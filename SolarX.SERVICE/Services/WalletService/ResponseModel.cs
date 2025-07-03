namespace SolarX.SERVICE.Services.WalletService;

public static class ResponseModel
{
    public record WalletResponseModel(decimal Balance, decimal CreditLimit, decimal CurrentDebt, decimal Available);
}