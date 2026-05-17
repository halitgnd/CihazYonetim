using Microsoft.EntityFrameworkCore;
using CihazYonetim.Models; 

namespace CihazYonetim.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Cihazlar> Cihazlar { get; set;}
    public DbSet<CihazLog> CihazLoglar { get; set;}

    // EKSİK OLAN BALYOZ BURASIYDI USTA: Veritabanı tabloları oluşurken kuralları biz koyuyoruz
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // KURAL: Bir cihazın 1 kullanıcısı olur, bir kullanıcının çok cihazı olabilir!
        modelBuilder.Entity<Cihazlar>()
            .HasOne(c => c.User)              
            .WithMany(u => u.Cihazlar)        
            .HasForeignKey(c => c.UserId)     
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
