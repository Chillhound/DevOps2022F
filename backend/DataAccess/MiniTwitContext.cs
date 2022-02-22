using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MiniTwitContext : DbContext
    {
        public MiniTwitContext(DbContextOptions<MiniTwitContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follower>().HasKey(e => new { e.WhoId,e.WhomId});

            modelBuilder.Entity<Follower>().HasOne(e => e.Who).WithMany(e => e.Following).HasForeignKey(e => e.WhoId);
            modelBuilder.Entity<Follower>().HasOne(e => e.Whom).WithMany(e => e.Followers).HasForeignKey(e => e.WhomId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Follower>().HasOne(e => e.Whom).WithMany(e => e.Following).HasForeignKey(e => e.WhomId).OnDelete(DeleteBehavior.Restrict);
        }

    }
}
