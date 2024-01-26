using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TSAssignment2.Entities
{
    public class CourseManagerDbContext : DbContext
    {
        public CourseManagerDbContext(DbContextOptions<CourseManagerDbContext> options) : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().Property(student => student.Status).HasConversion<string>().HasMaxLength(64);
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    CourseName = "Programming: Microsoft Web Technologies",
                    Instructor = "Tajesh Sidhu",
                    StartDate = new DateTime(2023, 9, 06),
                    RoomNumber = "3G17"
                }
                );

            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    StudentName = "Tajeshvir Singh",
                    StudentEmail = "tajeshvirsingh8445@conestogac.on.ca",
                    CourseId = 1,
                });

        }
    }

}
