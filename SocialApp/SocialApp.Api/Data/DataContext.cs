using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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


        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder)
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
                 .OnDelete(DeleteBehavior.Restrict); // Sửa từ Cascade -> Restrict

                e.HasOne(b => b.Post)
                 .WithMany()
                 .HasForeignKey(b => b.PostId)
                 .OnDelete(DeleteBehavior.Cascade); // Giữ lại Cascade ở Post
            });

            modelBuilder.Entity<Likes>(e =>
            {
                e.HasKey(b => new { b.PostId, b.UserId });

                e.HasOne(b => b.User)
                 .WithMany()
                 .HasForeignKey(b => b.UserId)
                 .OnDelete(DeleteBehavior.Restrict); // Sửa từ Cascade -> Restrict

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
            // Cấu hình bảng Follow
            modelBuilder.Entity<Follow>(e =>
            {
                e.HasKey(f => f.Id);
                e.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique(); // Đảm bảo không có bản ghi trùng
                e.HasOne(f => f.Follower)
                 .WithMany()
                 .HasForeignKey(f => f.FollowerId)
                 .OnDelete(DeleteBehavior.Restrict); // Không xóa User nếu có Follow
                e.HasOne(f => f.Following)
                 .WithMany()
                 .HasForeignKey(f => f.FollowingId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }


    }


}
