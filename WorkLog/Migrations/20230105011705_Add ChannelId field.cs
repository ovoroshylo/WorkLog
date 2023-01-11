using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkLog.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelIdfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelId",
                table: "WorkLog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "WorkLog");
        }
    }
}
