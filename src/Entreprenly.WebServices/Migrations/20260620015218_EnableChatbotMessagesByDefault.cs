using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class EnableChatbotMessagesByDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The chatbot now only auto-replies when notifications_chatbot_messages is enabled. Existing
            // profiles were created with this flag off (it used to be cosmetic), so enable it for everyone
            // to preserve the current behavior where the bot always responds. Owners can opt out afterwards.
            migrationBuilder.Sql("UPDATE profiles SET notifications_chatbot_messages = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE profiles SET notifications_chatbot_messages = 0;");
        }
    }
}
