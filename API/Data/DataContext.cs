using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    public class DataContext : //we needed to specify all that types coz default is string, and we needed to override them by int
    IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, 
    AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,  IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // public DbSet<AppUser> Users { get; set; }    with IdentityDbContext dont need to create table Users like that, it will be handled by Identity
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)   //for building relations in db when db creates
        {

            builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

            base.OnModelCreating(builder);
            builder.Entity<UserLike>()
            .HasKey(k => new {k.SourceUserId, k.LikedUserId});  //creates primary key for UserLike table

            builder.Entity<UserLike>()  //we set relation that source user can like many users
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()  //we set relation that likedUser can be liked by many users
            .HasOne(s => s.LikedUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()   //Recipient has many message received
            .HasOne(user => user.Recipient)
            .WithMany(message => message.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);  //we dont want to delete message when only 1 person deleted message

            builder.Entity<Message>()   //sender has many messages sent
            .HasOne(user => user.Sender)
            .WithMany(message => message.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);     //we dont want to delete message when only 1 person deleted message

            builder.ApplyUtcDateTimeConverter();
        }
    }

    public static class UtcDateAnnotation
{
    private const string IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true) =>
        builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static bool IsUtc(this IMutableProperty property) =>
        ((bool?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc())
                {
                    continue;
                }

                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcConverter);
                }
            }
        }
    }
}
}