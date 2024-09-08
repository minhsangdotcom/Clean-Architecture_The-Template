using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserNameTypeAndRelationOfRoleClaimAndUserClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role_claim_id",
                table: "user_claim",
                type: "character varying(26)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "user",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_user_claim_role_claim_id",
                table: "user_claim",
                column: "role_claim_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_user_name",
                table: "user",
                column: "user_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_user_claim_role_claim_role_claim_id",
                table: "user_claim",
                column: "role_claim_id",
                principalTable: "role_claim",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_claim_role_claim_role_claim_id",
                table: "user_claim");

            migrationBuilder.DropIndex(
                name: "ix_user_claim_role_claim_id",
                table: "user_claim");

            migrationBuilder.DropIndex(
                name: "ix_user_user_name",
                table: "user");

            migrationBuilder.DropColumn(
                name: "role_claim_id",
                table: "user_claim");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");
        }
    }
}
