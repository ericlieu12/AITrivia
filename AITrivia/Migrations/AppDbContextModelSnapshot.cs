﻿// <auto-generated />
using System;
using AITrivia.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AITrivia.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AITrivia.DBModels.Lobby", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("UrlString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDone")
                        .HasColumnType("bit");

                    b.Property<bool>("isStarted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Lobbys");
                });

            modelBuilder.Entity("AITrivia.DBModels.TriviaAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("TriviaQuestionId")
                        .HasColumnType("int");

                    b.Property<string>("answerString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TriviaQuestionId")
                        .IsUnique();

                    b.ToTable("TriviaAnswer");
                });

            modelBuilder.Entity("AITrivia.DBModels.TriviaQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LobbyId")
                        .HasColumnType("int");

                    b.Property<string>("questionString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.ToTable("TriviaQuestion");
                });

            modelBuilder.Entity("AITrivia.DBModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsLeader")
                        .HasColumnType("bit");

                    b.Property<int>("LobbyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TriviaQuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LobbyId");

                    b.HasIndex("TriviaQuestionId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AITrivia.DBModels.TriviaAnswer", b =>
                {
                    b.HasOne("AITrivia.DBModels.TriviaQuestion", null)
                        .WithOne("correctAnswer")
                        .HasForeignKey("AITrivia.DBModels.TriviaAnswer", "TriviaQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AITrivia.DBModels.TriviaQuestion", b =>
                {
                    b.HasOne("AITrivia.DBModels.Lobby", null)
                        .WithMany("triviaQuestions")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AITrivia.DBModels.User", b =>
                {
                    b.HasOne("AITrivia.DBModels.Lobby", null)
                        .WithMany("users")
                        .HasForeignKey("LobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AITrivia.DBModels.TriviaQuestion", null)
                        .WithMany("users")
                        .HasForeignKey("TriviaQuestionId");
                });

            modelBuilder.Entity("AITrivia.DBModels.Lobby", b =>
                {
                    b.Navigation("triviaQuestions");

                    b.Navigation("users");
                });

            modelBuilder.Entity("AITrivia.DBModels.TriviaQuestion", b =>
                {
                    b.Navigation("correctAnswer")
                        .IsRequired();

                    b.Navigation("users");
                });
#pragma warning restore 612, 618
        }
    }
}
