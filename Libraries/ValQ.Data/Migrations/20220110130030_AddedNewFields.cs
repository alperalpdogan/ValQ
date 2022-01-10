using Microsoft.EntityFrameworkCore.Migrations;

namespace ValQ.Data.Migrations
{
    public partial class AddedNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "NickName");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfCorrectAnswers",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfIncorrectAnswers",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCorrectAnswers",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "NumberOfIncorrectAnswers",
                table: "Match");

            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
