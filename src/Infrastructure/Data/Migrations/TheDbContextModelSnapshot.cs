﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(TheDbContext))]
    partial class TheDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Aggregates.Users.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Guard")
                        .HasColumnType("text")
                        .HasColumnName("guard");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_role");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_role_name");

                    b.ToTable("role", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.RoleClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claim");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claim_role_id");

                    b.ToTable("role_claim", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<string>("Avatar")
                        .HasColumnType("text")
                        .HasColumnName("avatar");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("date")
                        .HasColumnName("day_of_birth");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasDatabaseName("ix_user_user_name");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserClaim", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("RoleClaimId")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("role_claim_id");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claim");

                    b.HasIndex("RoleClaimId")
                        .HasDatabaseName("ix_user_claim_role_claim_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claim_user_id");

                    b.ToTable("user_claim", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserRole", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("role_id");

                    b.Property<string>("UserId")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("user_id");

                    b.HasKey("RoleId", "UserId")
                        .HasName("pk_user_role");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_role_user_id");

                    b.ToTable("user_role", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserToken", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("ClientIp")
                        .HasColumnType("text")
                        .HasColumnName("client_ip");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset>("ExpiredTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expired_time");

                    b.Property<string>("FamilyId")
                        .HasColumnType("text")
                        .HasColumnName("family_id");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_blocked");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text")
                        .HasColumnName("refresh_token");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<string>("UserAgent")
                        .HasColumnType("text")
                        .HasColumnName("user_agent");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_token");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_token_user_id");

                    b.ToTable("user_token", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Users.RoleClaim", b =>
                {
                    b.HasOne("Domain.Aggregates.Users.Role", "Role")
                        .WithMany("RoleClaims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_role_claim_role_role_id");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserClaim", b =>
                {
                    b.HasOne("Domain.Aggregates.Users.RoleClaim", "RoleClaim")
                        .WithMany("UserClaims")
                        .HasForeignKey("RoleClaimId")
                        .HasConstraintName("fk_user_claim_role_claim_role_claim_id");

                    b.HasOne("Domain.Aggregates.Users.User", "User")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_claim_user_user_id");

                    b.Navigation("RoleClaim");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserRole", b =>
                {
                    b.HasOne("Domain.Aggregates.Users.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_role_role_id");

                    b.HasOne("Domain.Aggregates.Users.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_user_user_id");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserToken", b =>
                {
                    b.HasOne("Domain.Aggregates.Users.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_token_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.Role", b =>
                {
                    b.Navigation("RoleClaims");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.RoleClaim", b =>
                {
                    b.Navigation("UserClaims");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.User", b =>
                {
                    b.Navigation("UserClaims");

                    b.Navigation("UserRoles");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
