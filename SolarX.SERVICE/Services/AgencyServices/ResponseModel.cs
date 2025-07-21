namespace SolarX.SERVICE.Services.AgencyServices;

public static class ResponseModel
{
    public record AgencyResponseModel(
        Guid AgencyId,
        string AgencyName,
        string AgencySlug,
        string AgencyLogo,
        string AgencyBanner,
        string Address,
        string Email,
        string ThemeColor,
        string Hotline,
        bool DisplayWithMarkup,
        decimal CreditLimit
    );

    public record AgencyDetailResponseModel(
        string LogoUrl,
        string Phone,
        string Email,
        string Address,
        string Name
    );

}