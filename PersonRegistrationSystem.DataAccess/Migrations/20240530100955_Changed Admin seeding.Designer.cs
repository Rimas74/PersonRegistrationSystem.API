﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonRegistrationSystem.DataAccess;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    [DbContext(typeof(PersonRegistrationContext))]
    [Migration("20240530100955_Changed Admin seeding")]
    partial class ChangedAdminseeding
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PersonalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlaceOfResidenceId")
                        .HasColumnType("int");

                    b.Property<string>("ProfilePhotoPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelephoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.PlaceOfResidence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApartmentNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.ToTable("PlacesOfResidence");
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PasswordHash = "9+tsUbFePL+WJrZuWNuDPFiSoH6Ggs9CtTxFwcStCjrrt8OFQEEVRn9I8/OpPWuPVv0wHF9inri2r6u4qByxIQ==",
                            Role = "Admin",
                            Salt = "SqiBlEzScE5MixejdlqEjXFpHphbVfXbRQN79lcsaTKGg7/WYoRclZVHo+dj/a9zvluwzhUVzBQHBNsMJDUIimiOTEp2OREEisYI+Hhk99qCmT2UwYJtZXoD9E4l3OmaDMDguydvfhhO9sKb/vNO8NCcOvNx2CNpPcR20VRFK1E=",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.Person", b =>
                {
                    b.HasOne("PersonRegistrationSystem.DataAccess.Entities.User", "User")
                        .WithMany("Persons")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.PlaceOfResidence", b =>
                {
                    b.HasOne("PersonRegistrationSystem.DataAccess.Entities.Person", "Person")
                        .WithOne("PlaceOfResidence")
                        .HasForeignKey("PersonRegistrationSystem.DataAccess.Entities.PlaceOfResidence", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.Person", b =>
                {
                    b.Navigation("PlaceOfResidence")
                        .IsRequired();
                });

            modelBuilder.Entity("PersonRegistrationSystem.DataAccess.Entities.User", b =>
                {
                    b.Navigation("Persons");
                });
#pragma warning restore 612, 618
        }
    }
}
