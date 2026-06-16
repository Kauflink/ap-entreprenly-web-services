using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unit_lots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    code_qr = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    entry_date = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    expiry_date = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_unit_lots", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "unit_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    code_qr = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    price = table.Column<double>(type: "double", nullable: false),
                    weight_grams = table.Column<double>(type: "double", nullable: false),
                    brand = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_unit_products", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "weight_lots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    code_qr = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    entry_date = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    quantity_kg = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_weight_lots", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "weight_products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    owner_email = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    code_qr = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    price_per_kg = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_weight_products", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_unit_lots_owner_email",
                table: "unit_lots",
                column: "owner_email");

            migrationBuilder.CreateIndex(
                name: "i_x_unit_products_owner_email",
                table: "unit_products",
                column: "owner_email");

            migrationBuilder.CreateIndex(
                name: "i_x_weight_lots_owner_email",
                table: "weight_lots",
                column: "owner_email");

            migrationBuilder.CreateIndex(
                name: "i_x_weight_products_owner_email",
                table: "weight_products",
                column: "owner_email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unit_lots");

            migrationBuilder.DropTable(
                name: "unit_products");

            migrationBuilder.DropTable(
                name: "weight_lots");

            migrationBuilder.DropTable(
                name: "weight_products");
        }
    }
}
