using Microsoft.EntityFrameworkCore.Migrations;

namespace DaewooLMS.Migrations
{
    public partial class qastatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "QuizAttempts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "QuizAttempts");
        }
    }
}
