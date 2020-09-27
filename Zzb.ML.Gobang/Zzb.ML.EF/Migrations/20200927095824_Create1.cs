using Microsoft.EntityFrameworkCore.Migrations;

namespace Zzb.ML.EF.Migrations
{
    public partial class Create1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Target",
                table: "Gobangs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Target",
                table: "Gobangs",
                type: "int",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
