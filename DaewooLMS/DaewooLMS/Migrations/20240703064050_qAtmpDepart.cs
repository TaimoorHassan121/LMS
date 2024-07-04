using Microsoft.EntityFrameworkCore.Migrations;

namespace DaewooLMS.Migrations
{
    public partial class qAtmpDepart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emp_Department",
                table: "QuizAttempts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emp_Department",
                table: "QuizAttempts");
        }
    }
}
