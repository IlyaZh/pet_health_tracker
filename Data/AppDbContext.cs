using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Data;

public class AppDbContext : DbContext
{
    public  AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
}