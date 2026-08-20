using Microsoft.EntityFrameworkCore;
using TechLogistics.Models;

namespace TechLogistics.Data;

public class TechLogisticsDbContext : DbContext
{
    public TechLogisticsDbContext(
        DbContextOptions<TechLogisticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<CentroDistribucion> CentrosDistribucion => Set<CentroDistribucion>();
}