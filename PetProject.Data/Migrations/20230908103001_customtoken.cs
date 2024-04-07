using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetProject.Data.Migrations
{
    public partial class customtoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8acea70-9d7b-4a58-974e-de559364113e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d4c2e5ea-1fd5-4432-b431-b14821b124c6");

            migrationBuilder.CreateTable(
                name: "ConfirmEmailTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmEmailTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmEmailTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0ec1b1a1-8737-445b-9178-c0b8900d4b02", "2", "USER", "USER" },
                    { "a068f74e-3cbb-4571-a7de-56783d8f4db6", "1", "ADMIN", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmEmailTokens_UserId",
                table: "ConfirmEmailTokens",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmEmailTokens");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ec1b1a1-8737-445b-9178-c0b8900d4b02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a068f74e-3cbb-4571-a7de-56783d8f4db6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b8acea70-9d7b-4a58-974e-de559364113e", "2", "USER", "USER" },
                    { "d4c2e5ea-1fd5-4432-b431-b14821b124c6", "1", "ADMIN", "ADMIN" }
                });
        }
    }
}
