﻿// <auto-generated />
using System;
using CasinoRegistro.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CasinoRegistro.DataAccess.Migrations
{
    [DbContext(typeof(CasinoRegistroDbContext))]
    [Migration("20240930220027_CreacionTablaCajeroPlataforma")]
    partial class CreacionTablaCajeroPlataforma
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CasinoRegistro.Models.CajeroPlataforma", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CajeroUserId")
                        .HasColumnType("int");

                    b.Property<int>("PlataformaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CajeroUserId");

                    b.HasIndex("PlataformaId");

                    b.ToTable("CajeroPlataforma");
                });

            modelBuilder.Entity("CasinoRegistro.Models.CajeroUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DNI")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("DeudaPesosActual")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EsCajero")
                        .HasColumnType("bit");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<int>("FichasCargar")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PorcentajeComision")
                        .HasColumnType("float");

                    b.Property<string>("Rol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlImagen")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DNI" }, "IX_Cajeros_DNI")
                        .IsUnique();

                    b.HasIndex(new[] { "Email" }, "IX_Cajeros_Email")
                        .IsUnique();

                    b.ToTable("Cajero");
                });

            modelBuilder.Entity("CasinoRegistro.Models.Plataforma", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "URL" }, "IX_Plataformas_URL")
                        .IsUnique();

                    b.ToTable("Plataforma");
                });

            modelBuilder.Entity("CasinoRegistro.Models.RegistroMovimiento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CajeroId")
                        .HasColumnType("int");

                    b.Property<decimal?>("Comision")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("EsIngresoFichas")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FichasCargadas")
                        .HasColumnType("int");

                    b.Property<decimal?>("PesosDevueltos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("PesosEntregados")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CajeroId");

                    b.ToTable("RegistroMovimiento");
                });

            modelBuilder.Entity("CasinoRegistro.Models.CajeroPlataforma", b =>
                {
                    b.HasOne("CasinoRegistro.Models.CajeroUser", "CajeroUser")
                        .WithMany()
                        .HasForeignKey("CajeroUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CasinoRegistro.Models.Plataforma", "Plataforma")
                        .WithMany()
                        .HasForeignKey("PlataformaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CajeroUser");

                    b.Navigation("Plataforma");
                });

            modelBuilder.Entity("CasinoRegistro.Models.RegistroMovimiento", b =>
                {
                    b.HasOne("CasinoRegistro.Models.CajeroUser", "CajeroUser")
                        .WithMany()
                        .HasForeignKey("CajeroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CajeroUser");
                });
#pragma warning restore 612, 618
        }
    }
}
