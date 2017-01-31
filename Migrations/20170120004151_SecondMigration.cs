using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace forum.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CommentorId1",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId1",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Cateogries_AspNetUsers_ModeratorId1",
                table: "User_Cateogries");

            migrationBuilder.DropIndex(
                name: "IX_User_Cateogries_ModeratorId1",
                table: "User_Cateogries");

            migrationBuilder.DropIndex(
                name: "IX_Topics_CreatorId1",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentorId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ModeratorId1",
                table: "User_Cateogries");

            migrationBuilder.DropColumn(
                name: "CreatorId1",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "CommentorId1",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "ModeratorId",
                table: "User_Cateogries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Cateogries_ModeratorId",
                table: "User_Cateogries",
                column: "ModeratorId");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Topics",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CreatorId",
                table: "Topics",
                column: "CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "CommentorId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentorId",
                table: "Comments",
                column: "CommentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CommentorId",
                table: "Comments",
                column: "CommentorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId",
                table: "Topics",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Cateogries_AspNetUsers_ModeratorId",
                table: "User_Cateogries",
                column: "ModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CommentorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Cateogries_AspNetUsers_ModeratorId",
                table: "User_Cateogries");

            migrationBuilder.DropIndex(
                name: "IX_User_Cateogries_ModeratorId",
                table: "User_Cateogries");

            migrationBuilder.DropIndex(
                name: "IX_Topics_CreatorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentorId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "ModeratorId1",
                table: "User_Cateogries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorId1",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentorId1",
                table: "Comments",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModeratorId",
                table: "User_Cateogries",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_User_Cateogries_ModeratorId1",
                table: "User_Cateogries",
                column: "ModeratorId1");

            migrationBuilder.AlterColumn<int>(
                name: "CreatorId",
                table: "Topics",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CreatorId1",
                table: "Topics",
                column: "CreatorId1");

            migrationBuilder.AlterColumn<int>(
                name: "CommentorId",
                table: "Comments",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentorId1",
                table: "Comments",
                column: "CommentorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CommentorId1",
                table: "Comments",
                column: "CommentorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId1",
                table: "Topics",
                column: "CreatorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Cateogries_AspNetUsers_ModeratorId1",
                table: "User_Cateogries",
                column: "ModeratorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
