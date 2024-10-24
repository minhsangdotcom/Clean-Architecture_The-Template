using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "address",
                table: "user",
                newName: "address_street");

            migrationBuilder.AddColumn<string>(
                name: "address_commune_id",
                table: "user",
                type: "character varying(26)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_district_id",
                table: "user",
                type: "character varying(26)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_province_id",
                table: "user",
                type: "character varying(26)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_address_commune_id",
                table: "user",
                column: "address_commune_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_address_district_id",
                table: "user",
                column: "address_district_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_address_province_id",
                table: "user",
                column: "address_province_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_commune_address_commune_id",
                table: "user",
                column: "address_commune_id",
                principalTable: "commune",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_district_address_district_id",
                table: "user",
                column: "address_district_id",
                principalTable: "district",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_province_address_province_id",
                table: "user",
                column: "address_province_id",
                principalTable: "province",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_commune_address_commune_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_district_address_district_id",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "fk_user_province_address_province_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_address_commune_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_address_district_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_address_province_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "address_commune_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "address_district_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "address_province_id",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "address_street",
                table: "user",
                newName: "address");
        }
    }
}
