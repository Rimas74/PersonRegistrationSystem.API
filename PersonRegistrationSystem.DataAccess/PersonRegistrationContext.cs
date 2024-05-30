﻿using Microsoft.EntityFrameworkCore;
using PersonRegistrationSystem.DataAccess.Entities;

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
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.PlaceOfResidence)
                .WithOne(pr => pr.Person)
                .HasForeignKey<PlaceOfResidence>(pr => pr.PersonId)
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
