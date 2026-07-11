using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entreprenly.WebServices.Migrations
{
    /// <inheritdoc />
    public partial class BackfillWhatsappSessionSellerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // One-off backfill for data written before the fix in PR #85 ("scope inbound conversations
            // by the owner's IAM seller id"). That fix stops new rows from being saved under the
            // WhatsApp bridge's own seller id (which lives in a different id space than the IAM user
            // id the dashboard reads scope by), but it does not touch rows already saved that way.
            // Scoped to the one account known to have connected the chatbot so far; safe to extend to
            // other owner_email values the same way if more accounts connect before ever registering
            // a conversation/order under the healed seller id.
            migrationBuilder.Sql("""
                SET @owner_email := 'test.beta@entreprenly.online';
                SET @stale_seller_id := (SELECT seller_id FROM whatsapp_sessions WHERE owner_email = @owner_email LIMIT 1);
                SET @correct_seller_id := (SELECT id FROM users WHERE email = @owner_email LIMIT 1);

                UPDATE conversations
                SET seller_id = @correct_seller_id
                WHERE seller_id = @stale_seller_id AND @correct_seller_id IS NOT NULL AND @stale_seller_id IS NOT NULL;

                UPDATE chat_orders
                SET seller_id = @correct_seller_id
                WHERE seller_id = @stale_seller_id AND @correct_seller_id IS NOT NULL AND @stale_seller_id IS NOT NULL;

                UPDATE whatsapp_sessions
                SET seller_id = @correct_seller_id
                WHERE owner_email = @owner_email AND @correct_seller_id IS NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not reversible: the stale seller id is not recorded anywhere once this runs, so there is
            // nothing to restore it from.
        }
    }
}
