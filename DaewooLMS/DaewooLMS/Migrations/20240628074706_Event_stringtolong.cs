using Microsoft.EntityFrameworkCore.Migrations;

namespace DaewooLMS.Migrations
{
    public partial class Event_stringtolong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Authorize_Person",
                table: "EventLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Authorize_Person",
                table: "EventLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
