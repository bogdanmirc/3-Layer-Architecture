using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Data
{
    public class TradeMarketDbContext : DbContext
    {
        public TradeMarketDbContext()
        {
            
        }
        public TradeMarketDbContext(DbContextOptions<TradeMarketDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptsDetails { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>().HasOne(c => c.Customer).WithOne(c => c.Person).HasForeignKey<Person>(c => c.CustomerId);
            modelBuilder.Entity<Customer>().HasOne(c => c.Person).WithOne(c => c.Customer).HasForeignKey<Customer>(c => c.PersonId);
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.ProductCategoryId);
            modelBuilder.Entity<Receipt>().HasOne(r => r.Customer).WithMany(c => c.Receipts).HasForeignKey(r => r.CustomerId);
            modelBuilder.Entity<ReceiptDetail>().HasOne(rd => rd.Receipt).WithMany(r => r.ReceiptDetails).HasForeignKey(rd => rd.ReceiptId);
            modelBuilder.Entity<ReceiptDetail>().HasOne(rd => rd.Product).WithMany(p => p.ReceiptDetails).HasForeignKey(rd => rd.ProductId);
        }
    }
}
