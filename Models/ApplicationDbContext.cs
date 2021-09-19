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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasNoKey();
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();
            
        }
        public DbSet<smartpalika.Models.LoginVM> LoginVM { get; set; }
        public DbSet<smartpalika.Models.CreateRoleVM> CreateRoleVM { get; set; }
        public DbSet<smartpalika.Models.EditUserVM> EditUserVM { get; set; }

    }
}
