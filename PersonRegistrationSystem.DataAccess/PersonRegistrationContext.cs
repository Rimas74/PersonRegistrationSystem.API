using Microsoft.EntityFrameworkCore;
using PersonRegistrationSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess
{
    public class PersonRegistrationContext : DbContext
    {


        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PlaceOfResidence> PlacesOfResidence { get; set; }

        public PersonRegistrationContext(DbContextOptions<PersonRegistrationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Persons)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.User.Id);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.PlaceOfResidence)
                .WithOne(pr => pr.Person)
                .HasForeignKey<PlaceOfResidence>(pr => pr.PersonId);

            modelBuilder.Entity<PlaceOfResidence>()
                .HasOne(pr => pr.Person)
                .WithOne(p => p.PlaceOfResidence)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "AdminPasword",
                Salt = "salt",
                Role = "Admin"
            });
        }

    }
}
