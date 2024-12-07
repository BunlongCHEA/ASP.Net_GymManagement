using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Gym_ManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MemberViewModel> Members { get; set; }
        public DbSet<TrainerViewModel> Trainers { get; set; }
        public DbSet<ClassViewModel> Classes { get; set; }
        public DbSet<ClassScheduleViewModel> ClassSchedules { get; set; }
        public DbSet<WorkoutViewModel> Workouts { get; set; }
        public DbSet<MembershipViewModel> Memberships { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // Intentionally left blank
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the one-to-many relationship ApplicationUser & MemberViewModel
            builder.Entity<MemberViewModel>()
                .HasOne(s => s.User)
                .WithMany(m => m.Member)
                .HasForeignKey(s => s.CreatedByUserID);

            // Configure the one-to-many relationship ApplicationUser & TrainerViewModel
            builder.Entity<TrainerViewModel>()
                .HasOne(s => s.User)
                .WithMany(m => m.Trainers)
                .HasForeignKey(s => s.CreatedByUserID);

            // Configure the one-to-many relationship ApplicationUser & ClassViewModel
            builder.Entity<ClassViewModel>()
                .HasOne(s => s.User)
                .WithMany(m => m.Classes)
                .HasForeignKey(s => s.CreatedByUserID);

            // Configure the one-to-many relationship ClassViewModel & ClassScheduleViewModel
            builder.Entity<ClassScheduleViewModel>()
                .HasOne(s => s.Class)
                .WithMany(m => m.ClassSchedules)
                .HasForeignKey(s => s.ClassId);

            // Configure the one-to-many relationship ApplicationUser & WorkoutViewModel
            builder.Entity<WorkoutViewModel>()
                .HasOne(s => s.User)
                .WithMany(m => m.Workouts)
                .HasForeignKey(s => s.CreatedByUserID);

            // Configure the one-to-many relationship ClassViewModel & WorkoutViewModel
            builder.Entity<WorkoutViewModel>()
                .HasOne(s => s.Class)
                .WithMany(m => m.Workouts)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Configure the one-to-many relationship TrainerViewModel & WorkoutViewModel
            builder.Entity<WorkoutViewModel>()
                .HasOne(s => s.Trainer)
                .WithMany(m => m.Workouts)
                .HasForeignKey(s => s.TrainerId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            // Configure the one-to-many relationship MemberViewModel & WorkoutViewModel
            builder.Entity<WorkoutViewModel>()
                .HasOne(s => s.Member)
                .WithMany(m => m.Workouts)
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Restrict); // Disable cascading delete

            //// Configure the one-to-many relationship MemberViewModel & MembershipViewModel
            //builder.Entity<MembershipViewModel>()
            //    .HasOne(s => s.Member)
            //    .WithMany(m => m.Memberships)
            //    .HasForeignKey(s => s.MemberId);

            // Configure the one-to-one relationship MemberViewModel & MembershipViewModel
            builder.Entity<MembershipViewModel>()
                .HasOne(m => m.Member) // One Member has one Membership
                .WithOne(membership => membership.Memberships) // One Membership belongs to one Member
                .HasForeignKey<MembershipViewModel>(membership => membership.MemberId) // Membership's foreign key
                .OnDelete(DeleteBehavior.Cascade); // Delete behavior

            // Configure the one-to-many relationship ApplicationUser & MembershipViewModel
            builder.Entity<MembershipViewModel>()
                .HasOne(s => s.User)
                .WithMany(m => m.Memberships)
                .HasForeignKey(s => s.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
