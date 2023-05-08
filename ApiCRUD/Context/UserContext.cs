using ApiCRUD.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiCRUD.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
