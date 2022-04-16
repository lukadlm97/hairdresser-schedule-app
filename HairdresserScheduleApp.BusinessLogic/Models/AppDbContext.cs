using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HairdresserScheduleApp.BusinessLogic.Models
{
    public class AppDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> Roles { get; set; }
        public DbSet<DailySchedule> DailySchedules { get; set; }
        public DbSet<ScheduleItem> ScheduleItems { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(x => x.UserRole);
            modelBuilder.Entity<DailySchedule>().HasMany(j => j.ScheduleItems);
            modelBuilder.Entity<ScheduleItem>().HasOne(x => x.DailySchedule);
            modelBuilder.Entity<Reservation>().HasOne(x => x.User);
            modelBuilder.Entity<Reservation>().HasOne(x => x.ScheduleItem);

        }
    }
}
