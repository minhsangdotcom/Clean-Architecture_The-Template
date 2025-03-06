﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(TheDbContext))]
    [Migration("20250303055609_RemoveRedundantFk")]
    partial class RemoveRedundantFk
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Application.Common.Interfaces.Services.DistributedCache.DeadLetterQueue", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<object>("ErrorDetail")
                        .HasColumnType("jsonb")
                        .HasColumnName("error_detail");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uuid")
                        .HasColumnName("request_id");

                    b.Property<int>("RetryCount")
                        .HasColumnType("integer")
                        .HasColumnName("retry_count");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.HasKey("Id")
                        .HasName("pk_dead_letter_queue");

                    b.ToTable("dead_letter_queue", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Regions.Commune", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<string>("CustomName")
                        .HasColumnType("text")
                        .HasColumnName("custom_name");

                    b.Property<string>("DistrictId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("district_id");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<string>("FullNameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name_en");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name_en");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.HasKey("Id")
                        .HasName("pk_commune");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_commune_code");

                    b.HasIndex("DistrictId")
                        .HasDatabaseName("ix_commune_district_id");

                    b.ToTable("commune", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Regions.District", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<string>("CustomName")
                        .HasColumnType("text")
                        .HasColumnName("custom_name");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<string>("FullNameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name_en");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name_en");

                    b.Property<string>("ProvinceId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("province_id");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.HasKey("Id")
                        .HasName("pk_district");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_district_code");

                    b.HasIndex("ProvinceId")
                        .HasDatabaseName("ix_district_province_id");

                    b.ToTable("district", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Regions.Province", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<string>("CustomName")
                        .HasColumnType("text")
                        .HasColumnName("custom_name");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<string>("FullNameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name_en");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("NameEn")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name_en");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.HasKey("Id")
                        .HasName("pk_province");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_province_code");

                    b.ToTable("province", (string)null);
                });

            modelBuilder.Entity("Domain.Aggregates.Roles.Role", b =>
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

            modelBuilder.Entity("Domain.Aggregates.Roles.RoleClaim", b =>
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
                        .HasColumnType("citext")
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

                    b.Property<byte>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("username");

                    b.Property<long>("Version")
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_user_email");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_user_username");

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

            modelBuilder.Entity("Domain.Aggregates.Users.UserResetPassword", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(26)")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset>("Expiry")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiry");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(26)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_reset_password");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_reset_password_user_id");

                    b.ToTable("user_reset_password", (string)null);
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

            modelBuilder.Entity("Domain.Aggregates.Regions.Commune", b =>
                {
                    b.HasOne("Domain.Aggregates.Regions.District", "District")
                        .WithMany("Communes")
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_commune_district_district_id");

                    b.Navigation("District");
                });

            modelBuilder.Entity("Domain.Aggregates.Regions.District", b =>
                {
                    b.HasOne("Domain.Aggregates.Regions.Province", null)
                        .WithMany("Districts")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_district_province_province_id");
                });

            modelBuilder.Entity("Domain.Aggregates.Roles.RoleClaim", b =>
                {
                    b.HasOne("Domain.Aggregates.Roles.Role", "Role")
                        .WithMany("RoleClaims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_role_claim_role_role_id");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.User", b =>
                {
                    b.OwnsOne("Domain.Aggregates.Users.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("id");

                            b1.Property<string>("CommuneId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("address_commune_id");

                            b1.Property<string>("DistrictId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("address_district_id");

                            b1.Property<string>("ProvinceId")
                                .HasColumnType("character varying(26)")
                                .HasColumnName("address_province_id");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("address_street");

                            b1.HasKey("UserId");

                            b1.HasIndex("CommuneId")
                                .HasDatabaseName("ix_user_address_commune_id");

                            b1.HasIndex("DistrictId")
                                .HasDatabaseName("ix_user_address_district_id");

                            b1.HasIndex("ProvinceId")
                                .HasDatabaseName("ix_user_address_province_id");

                            b1.ToTable("user");

                            b1.HasOne("Domain.Aggregates.Regions.Commune", "Commune")
                                .WithMany()
                                .HasForeignKey("CommuneId")
                                .HasConstraintName("fk_user_commune_address_commune_id");

                            b1.HasOne("Domain.Aggregates.Regions.District", "District")
                                .WithMany()
                                .HasForeignKey("DistrictId")
                                .HasConstraintName("fk_user_district_address_district_id");

                            b1.HasOne("Domain.Aggregates.Regions.Province", "Province")
                                .WithMany()
                                .HasForeignKey("ProvinceId")
                                .HasConstraintName("fk_user_province_address_province_id");

                            b1.WithOwner()
                                .HasForeignKey("UserId")
                                .HasConstraintName("fk_user_user_id");

                            b1.Navigation("Commune");

                            b1.Navigation("District");

                            b1.Navigation("Province");
                        });

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserClaim", b =>
                {
                    b.HasOne("Domain.Aggregates.Roles.RoleClaim", "RoleClaim")
                        .WithMany("UserClaims")
                        .HasForeignKey("RoleClaimId")
                        .OnDelete(DeleteBehavior.Cascade)
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

            modelBuilder.Entity("Domain.Aggregates.Users.UserResetPassword", b =>
                {
                    b.HasOne("Domain.Aggregates.Users.User", "User")
                        .WithMany("UserResetPasswords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_reset_password_user_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.UserRole", b =>
                {
                    b.HasOne("Domain.Aggregates.Roles.Role", "Role")
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

            modelBuilder.Entity("Domain.Aggregates.Regions.District", b =>
                {
                    b.Navigation("Communes");
                });

            modelBuilder.Entity("Domain.Aggregates.Regions.Province", b =>
                {
                    b.Navigation("Districts");
                });

            modelBuilder.Entity("Domain.Aggregates.Roles.Role", b =>
                {
                    b.Navigation("RoleClaims");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Domain.Aggregates.Roles.RoleClaim", b =>
                {
                    b.Navigation("UserClaims");
                });

            modelBuilder.Entity("Domain.Aggregates.Users.User", b =>
                {
                    b.Navigation("UserClaims");

                    b.Navigation("UserResetPasswords");

                    b.Navigation("UserRoles");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
