using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AegisPraxis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Roles");
        }
    }
}
