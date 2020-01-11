using DRMAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class DRMContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public DRMContext(DbContextOptions<DRMContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Contact>().ToTable("contacts");
            builder.Entity<Contact>().HasKey(c => c.Id);
            builder.Entity<Contact>().Property(c => c.Id).HasColumnName("contact_id").ValueGeneratedOnAdd();
            builder.Entity<Contact>().Property(c => c.Email).HasColumnName("email");
            builder.Entity<Contact>().Property(c => c.Name).HasColumnName("name");
            builder.Entity<Contact>().Property(c => c.Message).HasColumnName("message");
            builder.Entity<Contact>().Property(c => c.Company).HasColumnName("company");
        }
    }
}
