using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class MessagesEntityAdded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ReciepentId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReciepentId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReciepentDeleted",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ReciepentUsername",
                table: "Messages",
                newName: "RecipientId");

            migrationBuilder.RenameColumn(
                name: "ReciepentId",
                table: "Messages",
                newName: "RecipientDeleted");

            migrationBuilder.AlterColumn<string>(
                name: "SenderUsername",
                table: "Messages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "RecipientUsername",
                table: "Messages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RecipientUsername",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Messages",
                newName: "ReciepentUsername");

            migrationBuilder.RenameColumn(
                name: "RecipientDeleted",
                table: "Messages",
                newName: "ReciepentId");

            migrationBuilder.AlterColumn<int>(
                name: "SenderUsername",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReciepentDeleted",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReciepentId",
                table: "Messages",
                column: "ReciepentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ReciepentId",
                table: "Messages",
                column: "ReciepentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
