using CrewOnDemand.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrewOnDemand.Database
{
    public class CrewOnDemandContext : DbContext
    {
        public CrewOnDemandContext(DbContextOptions<CrewOnDemandContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var basesSeed = new List<Base>
            {
                new Base { Id = 1, Name = "munich" },
                new Base { Id = 2, Name = "berlin" },
            };

            var pilotsSeed = new List<Pilot>
            {
                new Pilot { Id = 1, Name = "Andy", BaseId = 1 },
                new Pilot { Id = 2, Name = "Betty", BaseId = 1 },
                new Pilot { Id = 3, Name = "Callum", BaseId = 1 },
                new Pilot { Id = 4, Name = "Daphne", BaseId = 1 },
                new Pilot { Id = 5, Name = "Elvis", BaseId = 2 },
                new Pilot { Id = 6, Name = "Freida", BaseId = 2 },
                new Pilot { Id = 7, Name = "Greg", BaseId = 2 },
                new Pilot { Id = 8, Name = "Hermione", BaseId = 2 },
            };

            var workDaysSeed = new List<WorkDay>
            {
                // Andy
                new WorkDay { DayOfTheWeek = 1, PilotId = 1 },
                new WorkDay { DayOfTheWeek = 2, PilotId = 1 },
                new WorkDay { DayOfTheWeek = 4, PilotId = 1 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 1 },
                // Betty
                new WorkDay { DayOfTheWeek = 1, PilotId = 2 },
                new WorkDay { DayOfTheWeek = 2, PilotId = 2 },
                new WorkDay { DayOfTheWeek = 3, PilotId = 2 },
                new WorkDay { DayOfTheWeek = 5, PilotId = 2 },
                // Callum
                new WorkDay { DayOfTheWeek = 3, PilotId = 3 },
                new WorkDay { DayOfTheWeek = 4, PilotId = 3 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 3 },
                new WorkDay { DayOfTheWeek = 0, PilotId = 3 },
                // Daphne
                new WorkDay { DayOfTheWeek = 5, PilotId = 4 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 4 },
                new WorkDay { DayOfTheWeek = 0, PilotId = 4 },
                // Elvis
                new WorkDay { DayOfTheWeek = 1, PilotId = 5 },
                new WorkDay { DayOfTheWeek = 2, PilotId = 5 },
                new WorkDay { DayOfTheWeek = 4, PilotId = 5 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 5 },
                // Freida
                new WorkDay { DayOfTheWeek = 1, PilotId = 6 },
                new WorkDay { DayOfTheWeek = 2, PilotId = 6 },
                new WorkDay { DayOfTheWeek = 3, PilotId = 6 },
                new WorkDay { DayOfTheWeek = 5, PilotId = 6 },
                // Greg
                new WorkDay { DayOfTheWeek = 3, PilotId = 7 },
                new WorkDay { DayOfTheWeek = 4, PilotId = 7 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 7 },
                new WorkDay { DayOfTheWeek = 0, PilotId = 7 },
                // Hermione
                new WorkDay { DayOfTheWeek = 5, PilotId = 8 },
                new WorkDay { DayOfTheWeek = 6, PilotId = 8 },
                new WorkDay { DayOfTheWeek = 0, PilotId = 8 },
            };

            modelBuilder.Entity<Base>()
                .Property(b => b.CreationDate)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Base>().HasData(basesSeed);

            modelBuilder.Entity<WorkDay>()
                .HasKey(x => new { x.DayOfTheWeek, x.PilotId });
            modelBuilder.Entity<WorkDay>().HasData(workDaysSeed);

            modelBuilder.Entity<Pilot>()
                .HasKey(c => new { c.Id });
            modelBuilder.Entity<Pilot>()
                .Property(b => b.CreationDate)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Pilot>()
                .HasMany(x => x.WorkDays)
                .WithOne(x => x.Pilot);
            modelBuilder.Entity<Pilot>()
                .HasMany(x => x.Bookings);
            modelBuilder.Entity<Pilot>()
                .HasOne(x => x.Base);
            modelBuilder.Entity<Pilot>().HasData(pilotsSeed);

            modelBuilder.Entity<Booking>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<Booking>()
                .Property(b => b.CreationDate)
                .HasDefaultValueSql("getdate()");
        }

        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Base> Bases { get; set; }
        public DbSet<WorkDay> WorkDays { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
