using DRMAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public class TriviaDRMContext : DbContext
    {
        public DbSet<Clue> Clues { get; set; }

        public TriviaDRMContext(DbContextOptions<TriviaDRMContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Clue>().ToTable("vw_clues_json").HasNoKey();
        }

    }
}
