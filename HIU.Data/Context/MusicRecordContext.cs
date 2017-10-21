using HIU.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace HIU.Data.Context
{
    public class MusicRecordContext : DbContext
    {
        public DbSet<MusicRecordItem> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=RecordStore-v1.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicRecordItem>().Property(m => m.Id)
                                                  .IsRequired()
                                                  .ValueGeneratedOnAdd();
        }
    }
}
