using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using TSAssignment2.Entities;

namespace TSAssignment2.Services
{
    public class CourseManagerService : ICourseManagerService
    {
        private readonly CourseManagerDbContext _courseManagerDbContext;
        private readonly IConfiguration _configuration;

        public CourseManagerService(CourseManagerDbContext courseManagerDbContext, IConfiguration configuration)
        {
            _courseManagerDbContext = courseManagerDbContext;
            _configuration = configuration;
        }


        public List<Course> GetAllCourses()
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).OrderByDescending(e => e.StartDate).ToList();
        }

        public Course? GetCourseById(int id)
        {
            return _courseManagerDbContext.Courses.Include(e => e.Students).FirstOrDefault(e => e.CourseId == id);
        }

        public int AddCourse(Course party)
        {
            _courseManagerDbContext.Courses.Add(party);
            _courseManagerDbContext.SaveChanges();

            return party.CourseId;
        }

        public void UpdateCourse(Course party)
        {
            _courseManagerDbContext.Courses.Update(party);
            _courseManagerDbContext.SaveChanges();

        }
        public Student? GetStudentById(int courseId, int studentId)
        {
            return _courseManagerDbContext.Students
                .Include(g => g.Course)
                .FirstOrDefault(g => g.CourseId == courseId && g.StudentId == studentId);
        }

        public void UpdateConfirmationStatus(int courseId, int studentId, EnrollmentConfirmationStatus status)
        {
            var student = GetStudentById(courseId, studentId);

            if (student == null)
            {
                return;
            }

            student.Status = status;
            _courseManagerDbContext.SaveChanges();
        }
        public Course? AddStudentToCourseById(int courseId, Student student)
        {
            var subject = GetCourseById(courseId);

            if (subject == null) { return null; }

            subject.Students?.Add(student);
            _courseManagerDbContext.SaveChanges();

            return subject;
        }

        public void SendEnrollmentEmailByCourseId(int courseId, string scheme, string instructor)
        {
            var subject = GetCourseById(courseId);

            if (subject == null) return;

            var students = subject.Students.Where(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent).ToList();

            try
            {
                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = _configuration["SmtpSettings:Port"];
                var fromAddress = _configuration["SmtpSettings:FromAddress"];
                var fromPassword = _configuration["SmtpSettings:FromPassword"];

                using var smtpClient = new SmtpClient(smtpHost);
                smtpClient.Port = string.IsNullOrEmpty(smtpPort) ? 587 : Convert.ToInt32(smtpPort);
                smtpClient.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtpClient.EnableSsl = true;

                foreach (var student in students)
                {

                    var responseUrl = $"{scheme}://{instructor}/courses/{courseId}/enroll/{student.StudentId}";
                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(fromAddress),
                        Subject = $"{{Action Required}} Confirm \"{student?.Course?.CourseName}\" Enrollment",
                        Body = CreateBoody(student, responseUrl),
                        IsBodyHtml = true
                    };
                    if (student.StudentEmail == null) { return; }
                    mailMessage.To.Add(student.StudentEmail);

                    smtpClient.Send(mailMessage);
                    student.Status = EnrollmentConfirmationStatus.ConfirmationMessageSent;
                }

                _courseManagerDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private string CreateBoody(Student student, string responseUrl)
        {
            return $@"
            <h1>Hello {student.StudentName}:</h1>

             <p> 
                Your request to enroll in the course {student.Course.CourseName}.
                in room {student.Course.RoomNumber}
                starting {student.Course.StartDate:d}
                 with instructor {student.Course.Instructor}
             </p>

             <p>we are pleased to have you in the course so if you could
                <a href={responseUrl}> confirm your enrollment</a> as soon as possible that would be appreciated!
             </p>
                
            <p> Sincerly. </p>

            <p> The Course Manager App </p> ";
        }
    }
}
