using DRMAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class GroceryContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public GroceryContext(DbContextOptions<GroceryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>().Ignore(u => u.Password);
            modelBuilder.Entity<User>().Property(u => u.Email).HasColumnName("email_address");
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("user_id");
            modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasColumnName("password_hash");
            modelBuilder.Entity<User>().Property(u => u.PasswordSalt).HasColumnName("password_salt");

        }
    }
}
