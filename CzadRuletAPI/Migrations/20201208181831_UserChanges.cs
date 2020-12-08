using Microsoft.EntityFrameworkCore.Migrations;

namespace CzadRuletAPI.Migrations
{
    public partial class UserChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "user");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "user");
        }
    }
}
