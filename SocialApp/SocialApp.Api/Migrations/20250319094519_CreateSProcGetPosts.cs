﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateSProcGetPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROC CreateSProcGetPosts
                    (
                        @StartIndex INT,
                        @PageSize INT,
                        @CurrentUserId UNIQUEIDENTIFIER
                    )
                    AS
                    BEGIN
                        SELECT  p.Id AS PostId,
                                p.UserId,
                                u.[Name] AS UserName,
                                u.PhotoUrl AS UserPhotoUrl,
                                p.Content,
                                p.PhotoUrl,
                                p.PostedOn,
                                p.ModifiedOn,
                                CASE WHEN l.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsLiked,
                                CASE WHEN b.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsBookmarked
                        FROM    Posts p
                        INNER JOIN Users u ON p.UserId = u.Id
                        LEFT JOIN Likes l ON p.Id = l.PostId AND l.UserId = @CurrentUserId
                        LEFT JOIN Bookmarks b ON p.Id = b.PostId AND b.UserId = @CurrentUserId
                       
                        ORDER BY COALESCE(p.ModifiedOn, p.PostedOn) DESC
                        OFFSET  @StartIndex ROWS
                        FETCH NEXT @PageSize ROWS ONLY
                    END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPosts;");
        }
    }
}
