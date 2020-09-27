using Microsoft.EntityFrameworkCore.Migrations;

namespace Zzb.ML.EF.Migrations
{
    public partial class Create1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name1",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name2",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name3",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name4",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name5",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name6",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name7",
                table: "Tests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name8",
                table: "Tests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name1",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name2",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name3",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name4",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name5",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name6",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name7",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Name8",
                table: "Tests");
        }
    }
}
