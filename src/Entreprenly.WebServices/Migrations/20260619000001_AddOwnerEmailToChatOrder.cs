using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerEmailToChatOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "owner_email",
                table: "chat_orders",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "owner_email",
                table: "chat_orders");
        }
    }
}
