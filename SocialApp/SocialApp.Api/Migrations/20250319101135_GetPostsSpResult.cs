using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class GetPostsSpResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // generate for PostDto - which we are using as a return type of our GetPosts stored Procedure
            /*migrationBuilder.CreateTable(
                name: "PostDto",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLiked = table.Column<bool>(type: "bit", nullable: false),
                    IsBookmarked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });*/

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostDto");
        }
    }
}
