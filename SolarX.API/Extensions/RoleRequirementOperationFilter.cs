using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SolarX.API.Extensions;

public class RoleRequirementOperationFilter : IOperationFilter
{

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authorizeAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>();

        var roles = authorizeAttributes
            .SelectMany(attr => (attr.Roles ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .ToList();

        if (roles.Count == 0) return;
        var roleText = $"[Roles: {string.Join(", ", roles)}]";
        operation.Summary = string.IsNullOrEmpty(operation.Summary)
            ? roleText
            : $"{operation.Summary} {roleText}";
    }
}