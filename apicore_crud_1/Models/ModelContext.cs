using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apicore_crud_1.Models
{
    public class ModelContext : DbContext
    {
        public ModelContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }    
        public DbSet<Order> Orders { get; set; }
        public DbSet<Detail> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                        .Navigation(a => a.Details)
                        .AutoInclude();
            modelBuilder.Entity<Detail>()
                      .Navigation(a => a.Product)
                      .AutoInclude();
        }
    }
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();
    }
    public class Order
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string CustomerName { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required, StringLength(50)]
        public string Picture { get; set; }

        public bool IsDelivered { get; set; }

        // A Patient can have many Appointments
        public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();
    }
    public class Detail
    {
        public int Id { get; set; }

        //[Required, StringLength(50)]
        //public string Location { get; set; }
        
        [Required, ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [Required, ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
        
    }
}
