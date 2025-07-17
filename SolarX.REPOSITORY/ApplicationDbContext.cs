using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.REPOSITORY.Interceptions;

namespace SolarX.REPOSITORY;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Agency> Agencies { get; set; }
    public DbSet<AgencyWallet> AgencyWallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ConsultingRequest> ConsultingRequests { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSpecification> ProductSpecifications { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration == null)
        {
            throw new Exception("❌ IConfiguration is not injected into ApplicationDbContext.");
        }

        var connectionString = _configuration["ConnectionStrings:DefaultConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("❌ Connection string is missing! Ensure it is set in .env or appsettings.json.");
        }

        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => { sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
        );

        optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));

        optionsBuilder.AddInterceptors(new UpdateAuditableInterceptor());
        optionsBuilder.AddInterceptors(new DeleteAuditableInterceptor());
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        modelBuilder.Entity<Order>()
            .HasOne(o => o.SellerAgency)
            .WithMany(a => a.OrdersAsSeller)
            .HasForeignKey(o => o.SellerAgencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.BuyerAgency)
            .WithMany(a => a.OrdersAsBuyer)
            .HasForeignKey(o => o.BuyerAgencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.BasePrice)
            .HasPrecision(18, 2);


        modelBuilder.Entity<AgencyWallet>(entity =>
        {
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.CurrentDebt).HasPrecision(18, 2);
        });

        modelBuilder.Entity<WalletTransaction>(entity => { entity.Property(e => e.Amount).HasPrecision(18, 2); });

        var agency = new Agency
        {
            Id = new Guid("44444444-4444-4444-4444-444444444444"),
            Name = "SolarX",
            Slug = "Admin",
            LogoUrl = "https://solarx.vn/wp-content/uploads/2021/09/logo-solarx.png",
            BannerUrl = "https://solarx.vn/wp-content/uploads/2021/09/banner-solarx.png",
            ThemeColor = "#000000",
            Hotline = "0952252586",
            MarkupPercent = 0,
            DisplayWithMarkup = false,
            CreatedAt = DateTimeOffset.Now,
            UpdateAt = DateTimeOffset.Now,
            IsDeleted = false
        };

        modelBuilder.Entity<Agency>().HasData(agency);

        var user = new User
        {
            Id = new Guid("55555555-5555-5555-5555-555555555555"),
            UserName = "adminSolarX",
            Password = "nnsY/SJvZg/iBkxHXS/l9g==:0KN/XMNuKmz3nhL40RxTMNcMC9Xe5UE6XuYZ6bZQ1YU=",
            Email = "Admin@gmail.com",
            FullName = "Admin SolarX",
            PhoneNumber = "0952252586",
            AgencyId = agency.Id,
            Role = Role.SystemAdmin,
            CreatedAt = DateTimeOffset.Now,
            UpdateAt = DateTimeOffset.Now,
            IsDeleted = false
        };
        var staffUser = new User
        {
            Id = new Guid("66666666-6666-6666-6666-666666666666"),
            UserName = "staffSolarX",
            Password = "nnsY/SJvZg/iBkxHXS/l9g==:0KN/XMNuKmz3nhL40RxTMNcMC9Xe5UE6XuYZ6bZQ1YU=",
            Email = "Staff@gmail.com",
            FullName = "Staff SolarX",
            PhoneNumber = "0952252587",
            AgencyId = agency.Id,
            Role = Role.SystemStaff,
            CreatedAt = DateTimeOffset.Now,
            UpdateAt = DateTimeOffset.Now,
            IsDeleted = false
        };

        modelBuilder.Entity<User>().HasData(user, staffUser);

        base.OnModelCreating(modelBuilder);
    }
}