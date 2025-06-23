using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase_CLDV6211_ST10444488_.Migrations
{
    /// <inheritdoc />
    public partial class updateAzureDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event");

            migrationBuilder.AlterColumn<int>(
                name: "EventTypeID",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event",
                column: "EventTypeID",
                principalTable: "EventType",
                principalColumn: "EventTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event");

            migrationBuilder.AlterColumn<int>(
                name: "EventTypeID",
                table: "Event",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event",
                column: "EventTypeID",
                principalTable: "EventType",
                principalColumn: "EventTypeID");
        }
    }
}
