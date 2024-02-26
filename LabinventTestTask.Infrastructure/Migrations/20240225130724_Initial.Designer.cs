﻿// <auto-generated />
using LabinventTestTask.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LabinventTestTask.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240225130724_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("LabinventTestTask.Domain.Entities.ModuleData", b =>
                {
                    b.Property<string>("ModuleCategoryID")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ModuleCategoryID");

                    b.ToTable("ModuleData");
                });
#pragma warning restore 612, 618
        }
    }
}