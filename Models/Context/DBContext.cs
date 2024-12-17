using Microsoft.EntityFrameworkCore;

namespace EIUSmartWarehouse.Models.Context
{
    public class DBContext : DbContext
    {
        public DbSet<StoredProduct> StoredProduct { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
/*        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StoredProduct>().ToTable("StoredProduct");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Warehouse>().ToTable("Warehouse");
            modelBuilder.Entity<Staff>().ToTable("Staff");
        }*/
    }
}
