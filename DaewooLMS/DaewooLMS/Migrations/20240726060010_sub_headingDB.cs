using Microsoft.EntityFrameworkCore.Migrations;

namespace DaewooLMS.Migrations
{
    public partial class sub_headingDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmpID",
                table: "NewHrPolicies",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "PolicyDetail",
                table: "AddPolicies",
                type: "varchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PolicySubHeadings",
                columns: table => new
                {
                    SubHeadingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Main_HeadingID = table.Column<int>(nullable: false),
                    SubHeading_Title = table.Column<string>(type: "varchar(150)", nullable: true),
                    Sub_Detail = table.Column<string>(type: "varchar(MAX)", nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicySubHeadings", x => x.SubHeadingID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicySubHeadings");

            migrationBuilder.DropColumn(
                name: "EmpID",
                table: "NewHrPolicies");

            migrationBuilder.AlterColumn<string>(
                name: "PolicyDetail",
                table: "AddPolicies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)",
                oldNullable: true);
        }
    }
}
