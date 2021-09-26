using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using smartpalika.Models;

namespace smartpalika.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AppointmentUserDetails> Appointment { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasNoKey();
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();
            modelBuilder.Entity<AppointmentUserDetails>()
            .HasOne(o => o.ApplicationUser)
            .WithMany(o => o.Appointments)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
            //.OnDelete(DeleteBehavior.ClientNoAction);



        }



    }
}
