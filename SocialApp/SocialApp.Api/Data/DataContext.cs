using Microsoft.EntityFrameworkCore;
using SocialApp.Api.Data.Entities;

using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Bookmarks> Bookmarks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<Message> Message { get; set; } // Sửa tên DbSet từ Messages thành Message cho đúng chuẩn

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostDto>().HasNoKey();

            modelBuilder.Entity<Bookmarks>(e =>
            {
                e.HasKey(b => new { b.PostId, b.UserId });

                e.HasOne(b => b.User)
                 .WithMany()
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Post)
                 .WithMany()
                 .HasForeignKey(b => b.PostId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Likes>(e =>
            {
                e.HasKey(b => new { b.PostId, b.UserId });

                e.HasOne(b => b.User)
                 .WithMany()
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Post)
                 .WithMany()
                 .HasForeignKey(b => b.PostId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(c => c.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(c => c.Post)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>(e =>
            {
                e.HasKey(f => f.Id);
                e.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();
                e.HasOne(f => f.Follower)
                 .WithMany()
                 .HasForeignKey(f => f.FollowerId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(f => f.Following)
                 .WithMany()
                 .HasForeignKey(f => f.FollowingId)
                 .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.IsActive })
                .HasDatabaseName("IX_Follows_FollowerId_IsActive");
                modelBuilder.Entity<Follow>()
                    .HasIndex(f => new { f.FollowingId, f.IsActive })
                    .HasDatabaseName("IX_Follows_FollowingId_IsActive");
            });

            // Cấu hình bảng UserFriend
            modelBuilder.Entity<UserFriend>(e =>
            {
                e.HasKey(uf => new { uf.UserId, uf.FriendId }); // Khóa chính composite
                e.HasIndex(uf => new { uf.UserId, uf.FriendId }).IsUnique(); // Đảm bảo không trùng lặp
                e.HasOne(uf => uf.User)
                 .WithMany()
                 .HasForeignKey(uf => uf.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(uf => uf.Friend)
                 .WithMany()
                 .HasForeignKey(uf => uf.FriendId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình bảng Message
            modelBuilder.Entity<Message>(e =>
            {
                e.HasKey(m => m.Id); // Khóa chính
                e.Property(m => m.Content).IsRequired(); // Content không được null
                e.Property(m => m.SendDateTime).IsRequired(); // SendDateTime không được null
                e.HasOne(m => m.FromUser)
                 .WithMany()
                 .HasForeignKey(m => m.FromUserId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.ToUser)
                 .WithMany()
                 .HasForeignKey(m => m.ToUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}