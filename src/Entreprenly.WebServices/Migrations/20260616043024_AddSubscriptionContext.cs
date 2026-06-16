using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    default_billing_cycle = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    current_plan_id = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    current_plan_name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    current_plan_short_description = table.Column<string>(type: "MEDIUMTEXT", nullable: false),
                    current_plan_monthly_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    current_plan_annual_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    current_plan_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    current_plan_status_label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    current_plan_badge_label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    current_plan_recommended = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    current_period_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    current_period_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    recommended_plan_id = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    recommended_plan_name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    recommended_plan_short_description = table.Column<string>(type: "MEDIUMTEXT", nullable: false),
                    recommended_plan_monthly_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    recommended_plan_annual_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    recommended_plan_status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    recommended_plan_status_label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    recommended_plan_badge_label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    recommended_plan_recommended = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    recommended_period_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    recommended_period_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    payment_method_title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    payment_method_description = table.Column<string>(type: "MEDIUMTEXT", nullable: false),
                    payment_method_action_label = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    fiscal_data_title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    fiscal_data_description = table.Column<string>(type: "MEDIUMTEXT", nullable: false),
                    fiscal_data_action_label = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    has_payment_method = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    has_fiscal_data = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    fiscal_document_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    fiscal_document_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    fiscal_business_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true),
                    fiscal_receipt_email = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: true),
                    fiscal_address = table.Column<string>(type: "MEDIUMTEXT", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_activities",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    activity_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    detail = table.Column<string>(type: "MEDIUMTEXT", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_activities", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscription_activities_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_current_plan_features",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: false),
                    available = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_current_plan_features", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscription_current_plan_features_subscriptions_subscriptio~",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_limits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    limit_id = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    label = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    used_value = table.Column<int>(type: "int", nullable: false),
                    max_value = table.Column<int>(type: "int", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_limits", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscription_limits_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    payment_method_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    card_brand = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    last_four = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    holder_name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    expiry_month = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    expiry_year = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    is_default = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_payment_methods", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscription_payment_methods_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_recommended_plan_features",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: false),
                    available = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscription_recommended_plan_features", x => x.id);
                    table.ForeignKey(
                        name: "f_k_subscription_recommended_plan_features_subscriptions_subscri~",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_activities_subscription_id",
                table: "subscription_activities",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_current_plan_features_subscription_id",
                table: "subscription_current_plan_features",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_limits_subscription_id",
                table: "subscription_limits",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_payment_methods_subscription_id",
                table: "subscription_payment_methods",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscription_recommended_plan_features_subscription_id",
                table: "subscription_recommended_plan_features",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_activities");

            migrationBuilder.DropTable(
                name: "subscription_current_plan_features");

            migrationBuilder.DropTable(
                name: "subscription_limits");

            migrationBuilder.DropTable(
                name: "subscription_payment_methods");

            migrationBuilder.DropTable(
                name: "subscription_recommended_plan_features");

            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
