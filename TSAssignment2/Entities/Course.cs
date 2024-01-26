using System.ComponentModel.DataAnnotations;

namespace TSAssignment2.Entities
{

    public class Course
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is missing.")]
        public string? CourseName { get; set; }

        [Required(ErrorMessage = "Instructor name is missing.")]
        public string? Instructor { get; set; }

        [Required(ErrorMessage = "StartDate is missing.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "RoomNumber is missing.")]
        [RegularExpression(@"^\d[A-Z]\d{2}$", ErrorMessage = "Room number must be in the format: 1X11")]
        public string? RoomNumber { get; set; }

        public List<Student>? Students { get; set; }
    }
}
