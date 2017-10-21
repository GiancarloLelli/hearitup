using HIU.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace HIU.Data.Migrations
{
    [DbContext(typeof(MusicRecordContext))]
    [Migration("20160901143534_00-RecordStoreTableCreated")]
    partial class _00RecordStoreTableCreated
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "1.0.0-rtm-21431");
            modelBuilder.Entity("HIU.Data.Model.MusicRecordItem", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Album");
                b.Property<string>("Artist");
                b.Property<string>("BingSearchUrl");
                b.Property<DateTime>("DiscoveredOn");
                b.Property<string>("MicrosoftGuid");
                b.Property<string>("SongTitle");
                b.Property<string>("SpotifyId");
                b.HasKey("Id");
                b.ToTable("Records");
            });
        }
    }
}
