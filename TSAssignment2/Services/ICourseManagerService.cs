using TSAssignment2.Entities;

namespace TSAssignment2.Services
{
    public interface ICourseManagerService
    {
        public List<Course> GetAllCourses();

        public Course? GetCourseById(int id);

        public int AddCourse(Course subject);

        public void UpdateCourse(Course subject);

        public Student? GetStudentById(int courseId, int studentId);

        public void UpdateConfirmationStatus(int courseId, int studentId, EnrollmentConfirmationStatus status);

        public Course? AddStudentToCourseById(int courseId, Student student);

        public void SendEnrollmentEmailByCourseId(int courseId, string scheme, string host);
    }
}
