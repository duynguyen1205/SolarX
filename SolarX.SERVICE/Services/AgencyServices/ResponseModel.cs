namespace SolarX.SERVICE.Services.AgencyServices;

public static class ResponseModel
{
    public record AgencyResponseModel(
        Guid AgencyId,
        string AgencyName,
        string AgencySlug,
        string AgencyLogo,
        string AgencyBanner,
        string ThemeColor,
        string Hotline,
        bool DisplayWithMarkup
    );
}