using HIU.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace HIU.Data.Migrations
{
    [DbContext(typeof(MusicRecordContext))]
    partial class MusicRecordContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.HasAnnotation("ProductVersion", "1.0.0-rtm-21431");
                modelBuilder.Entity("HIU.Model.Repository.MusicRecordItem", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd();

                    // Root object
                    b.Property<string>("MicrosoftId");
                    b.Property<string>("Domain");
                    b.Property<string>("Type");
                    b.Property<string>("Url");
                    b.Property<string>("SecondsOffset");
                    b.Property<DateTime>("DiscoveredOn");
                    b.Property<bool>("IsNull");
                    b.Property<string>("ImageUrl");
                    b.Property<string>("SpotifyId");
                    b.Property<bool>("AddedToSpotify");

                    // Properties
                    b.Property<string>("SongType");
                    b.Property<string>("SongTitle");
                    b.Property<string>("ArtistName");
                    b.Property<string>("ReleaseTitle");
                    b.Property<string>("AudioPreview");

                    b.HasKey("Id");
                    b.ForSqliteToTable("Records");
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
