using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAL.Data
{
    public class DatabaseContext : IdentityDbContext<User> 
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Entities.Task> Tasks { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }
        public DbSet<TeamUser> TeamUser { get; set; }
        public DbSet<ProjectTeam> ProjectTeam { get; set; }


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Team>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Teams)
                .UsingEntity<TeamUser>
                (tu => tu.HasOne<User>().WithMany().HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.NoAction),
                tu => tu.HasOne<Team>().WithMany().HasForeignKey(t => t.TeamId).OnDelete(DeleteBehavior.NoAction));

            builder.Entity<Project>()
                .HasMany(p => p.Teams)
                .WithMany(t => t.Projects)
                .UsingEntity<ProjectTeam>
                (pt => pt.HasOne<Team>().WithMany().HasForeignKey(pt => pt.TeamId).OnDelete(DeleteBehavior.NoAction),
                pt => pt.HasOne<Project>().WithMany().HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior.NoAction));

            builder.Entity<Entities.Task>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.Tasks)
                .HasForeignKey(u => u.AssigneeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WorkLog>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
