using Microsoft.EntityFrameworkCore.Migrations;

namespace Zzb.ML.EF.Migrations
{
    public partial class Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonteCarloTrees",
                columns: table => new
                {
                    MonteCarloTreeId = table.Column<long>(nullable: false),
                    ParentTreeId = table.Column<long>(nullable: true),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Count = table.Column<long>(nullable: false),
                    Win = table.Column<long>(nullable: false),
                    IsBlack = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonteCarloTrees", x => x.MonteCarloTreeId);
                    table.ForeignKey(
                        name: "FK_MonteCarloTrees_MonteCarloTrees_ParentTreeId",
                        column: x => x.ParentTreeId,
                        principalTable: "MonteCarloTrees",
                        principalColumn: "MonteCarloTreeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonteCarloTrees_ParentTreeId",
                table: "MonteCarloTrees",
                column: "ParentTreeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonteCarloTrees");
        }
    }
}
