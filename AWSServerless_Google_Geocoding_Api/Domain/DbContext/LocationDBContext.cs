using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AWSServerless_Google_Geocoding_Api.Domain
{
    public partial class LocationDBContext : DbContext
    {
        public LocationDBContext()
        {
        }

        public LocationDBContext(DbContextOptions<LocationDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Map> Map { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.88.48;Initial Catalog=LocationDB;User ID=bigdata;Password=zxcvasdf;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Map>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreateHost).HasDefaultValueSql("(suser_sname())");

                entity.Property(e => e.CreateUserId).HasDefaultValueSql("(user_id())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyUser).HasDefaultValueSql("(suser_sname())");

                entity.Property(e => e.ModifyUserId).HasDefaultValueSql("(user_id())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Map)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Map_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
