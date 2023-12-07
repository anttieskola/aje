﻿// <auto-generated />
using System;
using AJE.Service.NewsFixer.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AJE.Service.NewsFixer.Migrations
{
    [DbContext(typeof(NewsFixerContext))]
    [Migration("20231206130521_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AJE.Service.NewsFixer.Infra.ArticleRow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean")
                        .HasColumnName("isValid")
                        .HasAnnotation("Relational:JsonPropertyName", "isValid");

                    b.Property<string>("Reasoning")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reasoning")
                        .HasAnnotation("Relational:JsonPropertyName", "reasoning");

                    b.HasKey("Id");

                    b.ToTable("articles");
                });
#pragma warning restore 612, 618
        }
    }
}