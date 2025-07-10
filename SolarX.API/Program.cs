using Microsoft.EntityFrameworkCore;
using SolarX.API.Behaviors;
using SolarX.API.Extensions;
using SolarX.API.Middlewares;
using SolarX.REPOSITORY;
using SolarX.REPOSITORY.Abstractions;
using SolarX.SERVICE.Abstractions.IAgencyServices;
using SolarX.SERVICE.Abstractions.IAuthServices;
using SolarX.SERVICE.Abstractions.IBlogServices;
using SolarX.SERVICE.Abstractions.ICategoryServices;
using SolarX.SERVICE.Abstractions.IConsultingRequestServices;
using SolarX.SERVICE.Abstractions.IFaqServices;
using SolarX.SERVICE.Abstractions.IInventoryServices;
using SolarX.SERVICE.Abstractions.IInventoryTransactionServices;
using SolarX.SERVICE.Abstractions.IJwtServices;
using SolarX.SERVICE.Abstractions.IOrderServices;
using SolarX.SERVICE.Abstractions.IPasswordHasherServices;
using SolarX.SERVICE.Abstractions.IProductServices;
using SolarX.SERVICE.Abstractions.IWalletService;
using SolarX.SERVICE.Services.AgencyServices;
using SolarX.SERVICE.Services.AuthServices;
using SolarX.SERVICE.Services.BlogServices;
using SolarX.SERVICE.Services.CategoryServices;
using SolarX.SERVICE.Services.CloudinaryServices;
using SolarX.SERVICE.Services.ConsultingRequestServices;
using SolarX.SERVICE.Services.FaqServices;
using SolarX.SERVICE.Services.InventoryServices;
using SolarX.SERVICE.Services.InventoryTransactionServices;
using SolarX.SERVICE.Services.JwtServices;
using SolarX.SERVICE.Services.OrderServices;
using SolarX.SERVICE.Services.PasswordHasherServices;
using SolarX.SERVICE.Services.ProductServices;
using SolarX.SERVICE.Services.WalletService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSwaggerServices();
builder.Services.AddJwtServices(builder.Configuration);
builder.Services.AddCloudinaryServices(builder.Configuration);

builder.Services.AddCors(options =>
    options.AddPolicy("CorsPolicy",
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        // .AllowCredentials() open when user SignR
    )
);
builder.Services
    .AddTransient(typeof(IBaseRepository<,>), typeof(BaseRepository<,>))
    .AddTransient<IGlobalTransactionsBehaviors, GlobalTransactionsBehaviors>()
    .AddTransient<IJwtServices, JwtServices>()
    .AddTransient<GlobalExceptionHandlingMiddleware>()
    .AddTransient<IPasswordHasherServices, PasswordHasherService>()
    .AddTransient<IAuthServices, AuthServices>()
    .AddTransient<ICategoryServices, CategoryServices>()
    .AddTransient<IProductServices, ProductServices>()
    .AddTransient<IAgencyServices, AgencyServices>()
    .AddTransient<IWalletService, WalletService>()
    .AddTransient<IOrderServices, OrderServices>()
    .AddTransient<IInventoryServices, InventoryServices>()
    .AddTransient<IInventoryTransactionServices, InventoryTransactionServices>()
    .AddTransient<IFaqServices, FaqServices>()
    .AddTransient<IBlogServices, BlogServices>()
    .AddTransient<IConsultingRequestServices, ConsultingRequestServices>();


var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseMiddleware<AgencySlugMiddleware>();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply pending migrations automatically
}

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//
// }

app.UseCors("CorsPolicy");
app.UseSwagger();
app.UseSwaggerApi();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();