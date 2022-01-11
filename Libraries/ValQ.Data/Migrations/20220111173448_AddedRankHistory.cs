using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ValQ.Data.Migrations
{
    public partial class AddedRankHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRankHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldRankId = table.Column<int>(type: "int", nullable: false),
                    NewRankId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRankHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRankHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRankHistory_Rank_NewRankId",
                        column: x => x.NewRankId,
                        principalTable: "Rank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRankHistory_Rank_OldRankId",
                        column: x => x.OldRankId,
                        principalTable: "Rank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRankHistory_NewRankId",
                table: "UserRankHistory",
                column: "NewRankId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRankHistory_OldRankId",
                table: "UserRankHistory",
                column: "OldRankId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRankHistory_UserId",
                table: "UserRankHistory",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRankHistory");
        }
    }
}
