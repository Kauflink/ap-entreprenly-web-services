using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cash_registers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    total_cash = table.Column<double>(type: "double", nullable: false),
                    total_digital = table.Column<double>(type: "double", nullable: false),
                    sale_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_cash_registers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    seller_id = table.Column<long>(type: "bigint", nullable: false),
                    total = table.Column<double>(type: "double", nullable: false),
                    payment_method = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sales", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sale_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    product_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    weight_kg = table.Column<double>(type: "double", nullable: true),
                    unit_price = table.Column<double>(type: "double", nullable: false),
                    subtotal = table.Column<double>(type: "double", nullable: false),
                    sale_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sale_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sale_items_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sale_payment_receipts",
                columns: table => new
                {
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    method = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    transaction_code = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    amount = table.Column<double>(type: "double", nullable: false),
                    confirmed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sale_payment_receipts", x => x.sale_id);
                    table.ForeignKey(
                        name: "f_k_sale_payment_receipts_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_cash_registers_owner_email_date",
                table: "cash_registers",
                columns: new[] { "owner_email", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sale_items_sale_id",
                table: "sale_items",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sales_owner_email",
                table: "sales",
                column: "owner_email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_registers");

            migrationBuilder.DropTable(
                name: "sale_items");

            migrationBuilder.DropTable(
                name: "sale_payment_receipts");

            migrationBuilder.DropTable(
                name: "sales");
        }
    }
}
