using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel_YYYYMMDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoHash",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "photoUrl",
                table: "Users",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "photoPath",
                table: "Users",
                newName: "PhotoPath");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "Text");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHarsh",
                table: "Users",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Physical path of image",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .OldAnnotation("Relational:ColumnOrder", 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Users",
                newName: "photoUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Users",
                newName: "photoPath");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHarsh",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Physical path of image");

            migrationBuilder.AddColumn<string>(
                name: "PhotoHash",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Posts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 1);
        }
    }
}
