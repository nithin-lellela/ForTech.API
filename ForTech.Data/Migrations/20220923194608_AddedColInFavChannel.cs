using Microsoft.EntityFrameworkCore.Migrations;

namespace ForTech.Data.Migrations
{
    public partial class AddedColInFavChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "FavouriteChannels",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "FavouriteChannels");
        }
    }
}
