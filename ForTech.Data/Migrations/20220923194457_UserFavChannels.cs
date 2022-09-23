using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForTech.Data.Migrations
{
    public partial class UserFavChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteChannels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteChannels");
        }
    }
}
