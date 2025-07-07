using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SolarX.API.Extensions;
public class AgencySlugHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        // Thêm header X-Agency-Slug vào mọi request
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Agency-Slug",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new Microsoft.OpenApi.Any.OpenApiString("admin")
            },
            Description = "Slug đại lý (ví dụ: anphu, vinh, admin, ...)"
        });
    }
}