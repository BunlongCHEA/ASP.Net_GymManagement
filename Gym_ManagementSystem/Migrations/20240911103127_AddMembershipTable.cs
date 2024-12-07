using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscribedMonth = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(20,2)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(20,2)", nullable: false),
                    RenewalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiredDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_AspNetUsers_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Memberships_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_CreatedByUserID",
                table: "Memberships",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberId",
                table: "Memberships",
                column: "MemberId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Memberships");
        }
    }
}
