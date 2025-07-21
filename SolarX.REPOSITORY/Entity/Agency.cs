using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Entity;

public class Agency : BaseEntity<Guid>, IAuditableEntity
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string LogoUrl { get; set; }
    public string BannerUrl { get; set; }
    public string ThemeColor { get; set; }
    public string Hotline { get; set; }
    public string Address { get; set; }
    public float MarkupPercent { get; set; }
    public bool DisplayWithMarkup { get; set; }

    public AgencyWallet? DefaultWallet { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Order> OrdersAsBuyer { get; set; } = new List<Order>();
    public ICollection<Order> OrdersAsSeller { get; set; } = new List<Order>();
    public ICollection<ConsultingRequest> ConsultingRequests { get; set; } = new List<ConsultingRequest>();
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    public ICollection<Setting> Settings { get; set; } = new List<Setting>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}