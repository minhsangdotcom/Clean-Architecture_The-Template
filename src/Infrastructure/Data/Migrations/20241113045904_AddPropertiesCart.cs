using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertiesCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "invalid_reason",
                table: "cart",
                newName: "persistent_error");

            migrationBuilder.AddColumn<bool>(
                name: "is_paid",
                table: "cart",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "payment_result",
                table: "cart",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_paid",
                table: "cart");

            migrationBuilder.DropColumn(
                name: "payment_result",
                table: "cart");

            migrationBuilder.RenameColumn(
                name: "persistent_error",
                table: "cart",
                newName: "invalid_reason");
        }
    }
}
