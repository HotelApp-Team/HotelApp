using System;
using System.Collections.Generic;
using HotelApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Infrastructure.Context;

public partial class HotelAppDbContext : DbContext
{
    public HotelAppDbContext(DbContextOptions<HotelAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelImage> HotelImages { get; set; }

    public virtual DbSet<HotelRoom> HotelRooms { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<RoomView> RoomViews { get; set; }

    public virtual DbSet<RoomAmenity> RoomAmenities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwHotelSummary> VwHotelSummaries { get; set; }

    public virtual DbSet<VwReservationDetail> VwReservationDetails { get; set; }

    public virtual DbSet<VwRoomDetail> VwRoomDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Amenitie__3214EC076EEC5595");

            entity.HasIndex(e => e.Name, "UQ__Amenitie__737584F68DC63E18").IsUnique();

            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Cities");

            entity.HasIndex(e => e.Name, "UQ_Cities_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hotels__3214EC079DF3CACE");

            entity.HasIndex(e => e.CityId, "IX_Hotels_CityId");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.AvgRating).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasDefaultValue("Turkiye");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hotels_Cities");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Hotels)
                .UsingEntity<Dictionary<string, object>>(
                    "HotelAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HotelAmenities_Amenities"),
                    l => l.HasOne<Hotel>().WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HotelAmenities_Hotels"),
                    j =>
                    {
                        j.HasKey("HotelId", "AmenityId").HasName("PK__HotelAme__EE40948F679E84E4");
                        j.ToTable("HotelAmenities");
                    });
        });

        modelBuilder.Entity<HotelImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HotelIma__3214EC0743DC57E5");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelImages)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelImages_Hotels");
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HotelRoo__3214EC07D8ABE90E");

            entity.HasIndex(e => e.HotelId, "IX_HotelRooms_HotelId");

            entity.HasIndex(e => e.RoomTypeId, "IX_HotelRooms_RoomTypeId");

            entity.HasIndex(e => e.RoomViewId, "IX_HotelRooms_RoomViewId");

            // Unique constraint manzara dahil ama NULL-aware: iki filtered index.
            // SQL Server'da default unique index NULL'lari esit saymaz, bu yuzden
            // NULL manzara icin ayri filtered unique index gerekiyor ki "Suite + NULL"
            // ikinci kez girilmesin. NOT NULL manzaralar icin uclu kombinasyon tek.
            entity.HasIndex(e => new { e.HotelId, e.RoomTypeId }, "UQ_HotelRooms_NoView")
                .IsUnique()
                .HasFilter("[RoomViewId] IS NULL");

            entity.HasIndex(e => new { e.HotelId, e.RoomTypeId, e.RoomViewId }, "UQ_HotelRooms_WithView")
                .IsUnique()
                .HasFilter("[RoomViewId] IS NOT NULL");

            entity.Property(e => e.ExtraGuestPricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelRooms_Hotels");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelRooms_RoomTypes");

            // RoomView iliskisi: NULL'a izin verir, RoomView silinirse oda silinmez
            // sadece FK NULL'a duser (SET NULL).
            entity.HasOne(d => d.RoomView).WithMany(p => p.HotelRooms)
                .HasForeignKey(d => d.RoomViewId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HotelRooms_RoomViews");

            entity.HasMany(d => d.RoomAmenities).WithMany(p => p.HotelRooms)
                .UsingEntity<Dictionary<string, object>>(
                    "HotelRoomAmenity",
                    r => r.HasOne<RoomAmenity>().WithMany()
                        .HasForeignKey("RoomAmenityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HotelRoomAmenities_RoomAmenities"),
                    l => l.HasOne<HotelRoom>().WithMany()
                        .HasForeignKey("HotelRoomId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_HotelRoomAmenities_HotelRooms"),
                    j =>
                    {
                        j.HasKey("HotelRoomId", "RoomAmenityId")
                            .HasName("PK_HotelRoomAmenities");
                        j.ToTable("HotelRoomAmenities");
                    });
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC07AC4EE53A");

            entity.HasIndex(e => e.Level, "IX_Logs_Level");

            entity.HasIndex(e => e.Timestamp, "IX_Logs_Timestamp").IsDescending();

            entity.HasIndex(e => e.UserId, "IX_Logs_UserId");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.Controller).HasMaxLength(100);
            entity.Property(e => e.ExceptionDetail).HasMaxLength(4000);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Message).HasMaxLength(2000);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserEmail).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Logs_User");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC0746F46965");

            entity.HasIndex(e => new { e.CheckInDate, e.CheckOutDate }, "IX_Reservations_Dates");

            entity.HasIndex(e => e.HotelRoomId, "IX_Reservations_HotelRoomId");

            entity.HasIndex(e => e.UserId, "IX_Reservations_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GuestCount).HasDefaultValue(1);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.HotelRoom).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.HotelRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_HotelRoomId");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Users");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC077A64544E");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateHotelRating"));

            entity.HasIndex(e => e.HotelId, "IX_Reviews_HotelId");

            entity.HasIndex(e => new { e.UserId, e.HotelId }, "UQ_Reviews_UserHotel").IsUnique();

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsApproved).HasDefaultValue(false);

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Hotels");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0782282B49");

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasIndex(e => e.HotelRoomId, "IX_RoomImages_HotelRoomId");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.HotelRoom).WithMany(p => p.RoomImages)
                .HasForeignKey(d => d.HotelRoomId)
                .HasConstraintName("FK_RoomImages_HotelRooms");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomType__3214EC07DF186721");

            entity.HasIndex(e => e.Name, "UQ_RoomTypes_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MaxGuests).HasDefaultValue(2);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RoomAmenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RoomAmenities");

            entity.HasIndex(e => e.Name, "UQ_RoomAmenities_Name").IsUnique();

            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RoomView>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RoomViews");

            entity.HasIndex(e => e.Name, "UQ_RoomViews_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC075FC12906");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.PasswordResetToken, "IX_Users_PasswordResetToken").HasFilter("([PasswordResetToken] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                    });
        });

        modelBuilder.Entity<VwHotelSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_HotelSummary");

            entity.Property(e => e.AvgRating).HasColumnType("decimal(38, 6)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.MinPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<VwReservationDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ReservationDetails");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.HotelName).HasMaxLength(200);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MainImageUrl).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RoomTypeName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<VwRoomDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_RoomDetails");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ExtraGuestPricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.HotelName).HasMaxLength(200);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RoomTypeName).HasMaxLength(100);
            entity.Property(e => e.RoomViewName).HasMaxLength(50);
            entity.Property(e => e.RoomViewIcon).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
