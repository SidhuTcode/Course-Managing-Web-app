using System.ComponentModel.DataAnnotations;

namespace TSAssignment2.Entities
{
    public enum EnrollmentConfirmationStatus
    {
        ConfirmationMessageNotSent = 0,
        ConfirmationMessageSent = 1,
        EnrollmentConfirmed = 2,
        EnrollmentDeclined = 3
    }
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Enter a valid Student name")]
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Enter a valid Student Email")]
        [EmailAddress]
        public string? StudentEmail { get; set; }

        public EnrollmentConfirmationStatus Status { get; set; }

        public int CourseId { get; set; }

        public Course? Course { get; set; }
    }
}
