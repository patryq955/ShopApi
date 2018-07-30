using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users {get;set;}
    }
}