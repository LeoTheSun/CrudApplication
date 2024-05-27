using CrudApplication.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;


namespace CrudApplication.DataAccess
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }


        public DbSet<UserEntity> Users { get; set; }
    }
}