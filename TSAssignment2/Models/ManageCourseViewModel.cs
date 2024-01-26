using TSAssignment2.Entities;

namespace TSAssignment2.Models
{
    public class ManageCourseViewModel
    {
        public Course? Course { get; set; }

        public Student? Student { get; set; }

        public int ConfirmationMessageNotSent { get; set; }

        public int ConfirmationMessageSent { get; set; }

        public int CountConfirmationMessageNotSent { get; set; }

        public int CountConfirmationMessageSent { get; set; }

        public int CountEnrollmentConfirmed { get; set; }

        public int CountEnrollmentDeclined { get; set; }
    }
}
