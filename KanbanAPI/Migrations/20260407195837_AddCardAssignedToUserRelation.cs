using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCardAssignedToUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AssignedToUserId",
                table: "Cards",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_AssignedToUserId",
                table: "Cards",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_AssignedToUserId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_AssignedToUserId",
                table: "Cards");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
