using Microsoft.EntityFrameworkCore.Migrations;

namespace DaewooLMS.Migrations
{
    public partial class NameToid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emp_Name",
                table: "QuizAttempts");

            migrationBuilder.AddColumn<long>(
                name: "Emp_ID",
                table: "QuizAttempts",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emp_ID",
                table: "QuizAttempts");

            migrationBuilder.AddColumn<string>(
                name: "Emp_Name",
                table: "QuizAttempts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
