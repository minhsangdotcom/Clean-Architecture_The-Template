using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TicketOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_ticket_ticket_id",
                table: "order");

            migrationBuilder.DropIndex(
                name: "ix_order_ticket_id",
                table: "order");

            migrationBuilder.DropColumn(
                name: "ticket_id",
                table: "order");

            migrationBuilder.RenameColumn(
                name: "available_quantity",
                table: "ticket",
                newName: "used_quantity");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "order",
                newName: "shipping_fee");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "ticket",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "ticket",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "ticket",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "total_amount",
                table: "order",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shipping_address",
                table: "order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "order",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "version",
                table: "order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "customer",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "customer",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "customer",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", nullable: false),
                    customer_id = table.Column<string>(type: "character varying(26)", nullable: false),
                    shipping_address = table.Column<string>(type: "text", nullable: true),
                    shipping_fee = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    total = table.Column<int>(type: "integer", nullable: false),
                    cart_status = table.Column<int>(type: "integer", nullable: false),
                    invalid_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart", x => x.id);
                    table.ForeignKey(
                        name: "fk_cart_customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", nullable: false),
                    tiket_id = table.Column<string>(type: "character varying(26)", nullable: false),
                    order_id = table.Column<string>(type: "character varying(26)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_item_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_item_ticket_tiket_id",
                        column: x => x.tiket_id,
                        principalTable: "ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(26)", nullable: false),
                    tiket_id = table.Column<string>(type: "character varying(26)", nullable: false),
                    cart_id = table.Column<string>(type: "character varying(26)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_cart_item_cart_cart_id",
                        column: x => x.cart_id,
                        principalTable: "cart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cart_item_ticket_tiket_id",
                        column: x => x.tiket_id,
                        principalTable: "ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cart_customer_id",
                table: "cart",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_cart_item_cart_id",
                table: "cart_item",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "ix_cart_item_tiket_id",
                table: "cart_item",
                column: "tiket_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_order_id",
                table: "order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_item_tiket_id",
                table: "order_item",
                column: "tiket_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "ticket");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "order");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "order");

            migrationBuilder.DropColumn(
                name: "shipping_address",
                table: "order");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "order");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "order");

            migrationBuilder.DropColumn(
                name: "version",
                table: "order");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "email",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "customer");

            migrationBuilder.RenameColumn(
                name: "used_quantity",
                table: "ticket",
                newName: "available_quantity");

            migrationBuilder.RenameColumn(
                name: "shipping_fee",
                table: "order",
                newName: "quantity");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_amount",
                table: "order",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ticket_id",
                table: "order",
                type: "character varying(26)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_order_ticket_id",
                table: "order",
                column: "ticket_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_ticket_ticket_id",
                table: "order",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
