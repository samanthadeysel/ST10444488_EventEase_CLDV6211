using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase_CLDV6211_ST10444488_.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Venue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Venue",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
