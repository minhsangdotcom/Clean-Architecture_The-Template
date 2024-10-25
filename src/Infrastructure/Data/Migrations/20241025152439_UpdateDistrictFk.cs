using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDistrictFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "district_id1",
                table: "commune",
                type: "character varying(26)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_commune_district_id1",
                table: "commune",
                column: "district_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_commune_district_district_id1",
                table: "commune",
                column: "district_id1",
                principalTable: "district",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_commune_district_district_id1",
                table: "commune");

            migrationBuilder.DropIndex(
                name: "ix_commune_district_id1",
                table: "commune");

            migrationBuilder.DropColumn(
                name: "district_id1",
                table: "commune");
        }
    }
}
