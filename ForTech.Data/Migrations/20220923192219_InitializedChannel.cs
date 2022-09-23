using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ForTech.Data.Migrations
{
    public partial class InitializedChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfInteractions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channel");
        }
    }
}
