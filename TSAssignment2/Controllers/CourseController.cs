using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSAssignment2.Entities;
using TSAssignment2.Models;
using TSAssignment2.Services;

namespace TSAssignment2.Controllers
{
    public class CourseController : AbstractBaseController
    {
        private readonly ICourseManagerService _courseManagerService;

        public CourseController(ICourseManagerService courseManagerService)
        {
            _courseManagerService = courseManagerService;
        }

        [HttpGet("/courses")]
        public IActionResult List()
        {
            SetWelcome();
            var coursesViewModel = new CoursesViewModel()
            {
                Courses = _courseManagerService.GetAllCourses(),
            };

            return View(coursesViewModel);
        }

        [HttpGet("/courses/{id:int}")]
        public IActionResult Manage(int id)
        {
            SetWelcome();

            var subject = _courseManagerService.GetCourseById(id);

            var manageCourseViewModel = new ManageCourseViewModel()
            {
                Course = subject,
                Student = new Entities.Student(),
                CountConfirmationMessageNotSent = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent),
                CountConfirmationMessageSent = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent),
                CountEnrollmentConfirmed = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed),
                CountEnrollmentDeclined = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentDeclined),

            };
            return View(manageCourseViewModel);
        }
        [HttpGet("/courses/{courseId:int}/enroll/{studentId:int}")]
        public IActionResult Enroll(int courseId, int studentId)
        {
            SetWelcome();
            var student = _courseManagerService.GetStudentById(courseId, studentId);

            if (student == null)
            {
                return NotFound();
            }
            var enrollStudentViewModel = new EnrollStudentViewModel()
            {
                Student = student
            };
            return View(enrollStudentViewModel);
        }
        [HttpPost("/courses/{courseId:int}/enroll/{studentId:int}")]
        public IActionResult Enroll(int courseId, int studentId, EnrollStudentViewModel enrollStudentViewModel)
        {
            SetWelcome();
            var student = _courseManagerService.GetStudentById(courseId, studentId);
            if (student == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var status = enrollStudentViewModel.Response == "Yes"
                    ? EnrollmentConfirmationStatus.EnrollmentConfirmed
                    : EnrollmentConfirmationStatus.EnrollmentDeclined;
                _courseManagerService.UpdateConfirmationStatus(courseId, studentId, status);

                return RedirectToAction("ThankYou", new { response = enrollStudentViewModel.Response });

            }
            else
            {
                enrollStudentViewModel.Student = student;
                return View(enrollStudentViewModel);
            }
        }
        [HttpGet]
        public IActionResult Add()
        {
            SetWelcome();

            var courseViewModel = new CourseViewModel()
            {
                ActiveCourse = new Course()
            };
            return View(courseViewModel);
        }
        [HttpPost]
        public IActionResult Add(CourseViewModel courseViewModel)
        {
            SetWelcome();
            if (!ModelState.IsValid)
            {
                return View(courseViewModel);
            }
            _courseManagerService.AddCourse(courseViewModel.ActiveCourse);

            TempData["Notify"] = $"{courseViewModel.ActiveCourse.CourseName} added successfully!";
            TempData["className"] = "Success";

            return RedirectToAction("List", new { id = courseViewModel.ActiveCourse.CourseId });
        }

        [HttpGet("/courses/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            SetWelcome();
            var subject = _courseManagerService.GetCourseById(id);

            if (subject == null)
            {
                return NotFound();
            }
            var courseViewModel = new CourseViewModel()
            {
                ActiveCourse = _courseManagerService.GetCourseById(id)
            };
            return View(courseViewModel);
        }

        [HttpPost("/courses/{id:int}/edit")]

        public IActionResult Edit(CourseViewModel courseViewModel, int id)
        {
            SetWelcome();
            if (!ModelState.IsValid)
            {
                return View(courseViewModel);
            }
            _courseManagerService.UpdateCourse(courseViewModel.ActiveCourse);

            TempData["notify"] = $"{courseViewModel.ActiveCourse.CourseName} Updated successfully";
            TempData["className"] = "info";

            return RedirectToAction("Manage", new { id });
        }
        [HttpGet("/thank-you/{response}")]
        public IActionResult ThankYou(string response)
        {
            SetWelcome();
            return View("ThankYou", response);
        }

        [HttpPost("/courses/{courseId:int}/add-student")]
        public IActionResult AddStudent(int courseId, ManageCourseViewModel manageCourseViewModel)
        {
            SetWelcome();

            Course? subject;
            if (ModelState.IsValid)
            {
                subject = _courseManagerService.AddStudentToCourseById(courseId, manageCourseViewModel.Student);

                if (subject == null) { return NotFound(); }

                TempData["notify"] = $"{manageCourseViewModel.Student.StudentName} added to student list ";
                TempData["className"] = "success";

                return RedirectToAction("Manage", new { id = courseId });
            }
            else
            {
                subject = _courseManagerService.GetCourseById(courseId);

                if (subject == null) { return NotFound(); }

                manageCourseViewModel.Course = subject;
                manageCourseViewModel.CountConfirmationMessageNotSent = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageNotSent);
                manageCourseViewModel.CountConfirmationMessageSent = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.ConfirmationMessageSent);
                manageCourseViewModel.CountEnrollmentConfirmed = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentConfirmed);
                manageCourseViewModel.CountEnrollmentDeclined = subject.Students.Count(g => g.Status == EnrollmentConfirmationStatus.EnrollmentDeclined);
                return View("Manage", manageCourseViewModel);
            }
        }

        [HttpPost("courses/{courseId:int}/enroll")]
        public IActionResult SendInvitation(int courseId)
        {
            _courseManagerService.SendEnrollmentEmailByCourseId(courseId, Request.Scheme, Request.Host.ToString());
            return RedirectToAction("Manage", new { id = courseId });
        }
    }


}
