using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)   //for building relations in db when db creates
        {
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
        }
    }
}