using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zzb.ML.EF.Migrations
{
    public partial class Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gobangs",
                columns: table => new
                {
                    GobangId = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    IsEnable = table.Column<bool>(nullable: false),
                    Map = table.Column<string>(nullable: true),
                    IsBlack = table.Column<bool>(nullable: false),
                    Target = table.Column<int>(nullable: false),
                    IsWin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gobangs", x => x.GobangId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gobangs");
        }
    }
}
