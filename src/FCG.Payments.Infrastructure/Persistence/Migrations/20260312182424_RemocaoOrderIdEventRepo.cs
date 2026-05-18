using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemocaoOrderIdEventRepo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_processed_events",
                schema: "pagamento",
                table: "processed_events");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                schema: "pagamento",
                table: "processed_events",
                newName: "GameId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "pagamento",
                table: "processed_events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_processed_events",
                schema: "pagamento",
                table: "processed_events",
                columns: new[] { "UserId", "GameId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_processed_events",
                schema: "pagamento",
                table: "processed_events");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "pagamento",
                table: "processed_events");

            migrationBuilder.RenameColumn(
                name: "GameId",
                schema: "pagamento",
                table: "processed_events",
                newName: "OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_processed_events",
                schema: "pagamento",
                table: "processed_events",
                column: "OrderId");
        }
    }
}
