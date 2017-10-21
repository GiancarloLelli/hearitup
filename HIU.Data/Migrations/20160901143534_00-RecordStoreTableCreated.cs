using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace HIU.Data.Migrations
{
    partial class _00RecordStoreTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            try
            {
                migrationBuilder.CreateTable(name: "HIU.Model.Repository.MusicRecordItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                    IsNull = table.Column<bool>(nullable: true),
                    MicrosoftId = table.Column<string>(nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    SecondsOffset = table.Column<string>(nullable: true),
                    DiscoveredOn = table.Column<DateTime>(nullable: true),
                    SongType = table.Column<string>(nullable: true),
                    SongTitle = table.Column<string>(nullable: true),
                    ArtistName = table.Column<string>(nullable: true),
                    ReleaseTitle = table.Column<string>(nullable: true),
                    AudioPreview = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    SpotifyId = table.Column<string>(nullable: true),
                    AddedToSpotify = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Record", x => x.Id);
                });

                migrationBuilder.RenameTable(name: "HIU.Model.Repository.MusicRecordItem", newName: "Records");
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "Records", newName: "HIU.Model.Repository.MusicRecordItem");
            migrationBuilder.DropTable(name: "HIU.Model.Repository.MusicRecordItem");
        }
    }
}
