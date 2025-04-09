using Microsoft.EntityFrameworkCore;
using PHDFinalVersion.Models;
using System.Text.RegularExpressions;

namespace PHDFinalVersion.AppDBContext

{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<StudentProfileTable> StudentProfile { get; set; }

        public DbSet<OTPValidationRequest> OTPValidation { get; set; }

        public DbSet<StudentsGroup> StudentsGroup { get; set; }

        public DbSet<Models.Group> Group { get; set; }

        public DbSet<Message> Message { get; set; }


        public DbSet<StudentToStudent> StudentToStudent { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OTPValidationRequest>().HasNoKey();

            modelBuilder.Entity<StudentsGroup>().
                HasKey(sc => new { sc.GroupId, sc.StudentId });

            modelBuilder.Entity<StudentsGroup>().
                HasOne(sc=>sc.StudentGroupProfile).
                WithMany(sc=>sc.StudentStudentsGroups).HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentsGroup>().
                HasOne(sc => sc.StudentGroupGroup).
                WithMany(sc => sc.GroupStudentsGroups).HasForeignKey(sc => sc.GroupId);

            modelBuilder.Entity<Message>().
                HasOne(sc => sc.Groups).
                WithMany(sc => sc.MessageGroups).HasForeignKey(sc => sc.GroupId);


            modelBuilder.Entity<Message>().
                HasOne(sc => sc.Receiver).
                WithMany(sc => sc.Message1).
                HasForeignKey(sc => sc.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Message>().
                HasOne(sc => sc.Sender).
                WithMany(sc => sc.Message2).
                HasForeignKey(sc => sc.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentToStudent>().HasKey(sc => new { sc.StudentID1, sc.StudentID2 });

            modelBuilder.Entity<StudentToStudent>().
                HasOne(sc =>sc.Student1).WithMany(sc=> sc.StudentToStudent1).HasForeignKey(sc => sc.StudentID1).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentToStudent>().
                HasOne(sc =>sc.Student2).WithMany(sc => sc.StudentToStudent2).HasForeignKey(sc=> sc.StudentID2).OnDelete(DeleteBehavior.Restrict);
                

        }
    }
}
