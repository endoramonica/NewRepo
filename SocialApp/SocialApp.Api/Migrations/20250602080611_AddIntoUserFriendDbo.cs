
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIntoUserFriendDbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserFriends",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserFriends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId_IsActive",
                table: "Follows",
                columns: new[] { "FollowerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId_IsActive",
                table: "Follows",
                columns: new[] { "FollowingId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Follows_FollowerId_IsActive",
                table: "Follows");

            migrationBuilder.DropIndex(
                name: "IX_Follows_FollowingId_IsActive",
                table: "Follows");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserFriends");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserFriends");
        }
    }
}