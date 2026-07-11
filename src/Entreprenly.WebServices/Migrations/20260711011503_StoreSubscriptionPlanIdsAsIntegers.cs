using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class StoreSubscriptionPlanIdsAsIntegers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE subscriptions
                SET current_plan_id = CASE current_plan_id
                        WHEN 'plan-free' THEN '1'
                        WHEN 'plan-control' THEN '2'
                        ELSE current_plan_id
                    END,
                    recommended_plan_id = CASE recommended_plan_id
                        WHEN 'plan-free' THEN '1'
                        WHEN 'plan-control' THEN '2'
                        ELSE recommended_plan_id
                    END;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "recommended_plan_id",
                table: "subscriptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(60)",
                oldMaxLength: 60)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "current_plan_id",
                table: "subscriptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(60)",
                oldMaxLength: 60)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "recommended_plan_id",
                table: "subscriptions",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "current_plan_id",
                table: "subscriptions",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql("""
                UPDATE subscriptions
                SET current_plan_id = CASE current_plan_id
                        WHEN '1' THEN 'plan-free'
                        WHEN '2' THEN 'plan-control'
                        ELSE current_plan_id
                    END,
                    recommended_plan_id = CASE recommended_plan_id
                        WHEN '1' THEN 'plan-free'
                        WHEN '2' THEN 'plan-control'
                        ELSE recommended_plan_id
                    END;
                """);
        }
    }
}
