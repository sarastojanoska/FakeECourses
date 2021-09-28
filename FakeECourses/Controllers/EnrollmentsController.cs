using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FakeECourses.Data;
using FakeECourses.Models;
using FakeECourses.ViewModel;
using System.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FakeECourses.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly FakeECoursesContext _context;
        private IWebHostEnvironment WebHostEnvironment { get; }

        public EnrollmentsController(FakeECoursesContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            var fakeECoursesContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
            return View(await fakeECoursesContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title");
            ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,ProjectPoints,SeminalPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,ProjectPoints,SeminalPoints,AdditionalPoints")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [Bind("FinishDate")] Enrollment enrollment)
        {
             enrollment = await _context.Enrollment.FindAsync(id);
              _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Enrollments/EnrollStudent/5
        public async Task<IActionResult> ListAllCourses()
        {
            return View(await _context.Course.ToListAsync());
        }
        public async Task<IActionResult> EnrollStudents(int? id)
        {
            var course = await _context.Course.Where(m => m.Id == id).Include(m => m.Students).FirstOrDefaultAsync();

            var viewmodel = new EnrollStudentsViewModel
            {
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId)
                
            };
            ViewData["CourseName"] = _context.Course.Where(c => c.Id == id).Select(c => c.Title).FirstOrDefault();
            ViewData["chosenId"] = id;
            return View(viewmodel);
        }

        // POST: Enrollments/EnrollStudent/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudents(int id, EnrollStudentsViewModel viewmodel)
        {
            if (id != viewmodel.Enrollment.CourseId)
            {
                return NotFound();
            }
            IEnumerable<int> listStudents = viewmodel.SelectedStudents;
            IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
            IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
            foreach (int sId in newStudents)
            {
                _context.Enrollment.Add(new Enrollment { StudentId = sId, CourseId = id, Year = viewmodel.Enrollment.Year, Semester = viewmodel.Enrollment.Semester });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Enrollments/EnrollStudent/5
        public async Task<IActionResult> UnenrollStudents(int? id)
        {
            var course = await _context.Course.Where(m => m.Id == id).Include(m => m.Students).FirstOrDefaultAsync();

            var viewmodel = new EnrollStudentsViewModel
            {
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId)

            };
            ViewData["CourseName"] = _context.Course.Where(c => c.Id == id).Select(c => c.Title).FirstOrDefault();
            ViewData["chosenId"] = id;

            return View(viewmodel);
        }

        // POST: Enrollments/EnrollStudent/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnEnrollStudents(int id,EnrollStudentsViewModel viewmodel)
        {
            if (id != viewmodel.Enrollment.CourseId)
            {
                return NotFound();
            }
            IEnumerable<int> listStudents = viewmodel.SelectedStudents;
            IEnumerable<Enrollment> existStudents = _context.Enrollment.Where((s => listStudents.Contains(s.StudentId) && s.CourseId == id));
            foreach (var enr in existStudents)
            {
                _context.Enrollment.Update(viewmodel.Enrollment);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MyStudentsByCourse(int id, int? yearInt = 0)
        {
            var course = _context.Course.Where(l => l.Id == id).FirstOrDefault();
            
            var enrollments = _context.Enrollment.Where(s => s.CourseId == id);
          
            enrollments = enrollments.Include(s => s.Student);
           
            enrollments = enrollments.Where(s => s.Year == yearInt);
           
            
            return View(enrollments);
        }
        public async Task<IActionResult> editMyStudentsByCourse (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.Include(s => s.Course).Include(s => s.Student).FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }
           
            return View(enrollment);
        }
        [HttpPost, ActionName("editMyStudentsByCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editMyStudentsByCourse(int id, [Bind("Grade,ExamPoints,ProjectPoints,SeminalPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            //var enrollments = _context.Enrollment.Where(s => s.StudentId == id);
            if (id == null)
            {
                return NotFound();
            }
            int crsId = 1;
            string crs = null;
            if (TempData["selectedCourse"] != null)
            {
                crs = TempData["selectedCourse"].ToString();
                crsId = Int32.Parse(crs);
            }
            var enrollmentToUpdate = await _context.Enrollment.FirstOrDefaultAsync(s => s.Id == id);
            await TryUpdateModelAsync<Enrollment>(
               enrollmentToUpdate,
             "", s => s.ExamPoints, s => s.SeminalPoints, s => s.ProjectPoints, s => s.AdditionalPoints,
            s => s.Grade, s => s.FinishDate);
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("MyStudentsByCourse", "Enrollments", new { id = crsId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName", enrollment.StudentId);
            return View(enrollmentToUpdate);
        }
        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "seminals");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<IActionResult> MyStatusByCourse(int id)
        {
            var student = _context.Student.Where(s => s.Id == id);
            var enrollment = _context.Enrollment.Where(s => s.StudentId == id);
              enrollment = enrollment.Include(c => c.Course);
            
            return View(enrollment);
        }
        public async Task<IActionResult> editMyStatusByCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.Include(s => s.Course).Include(s => s.Student).FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }
        [HttpPost, ActionName("editMyStatusByCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editMyStatusByCourse(int id, IFormFile semUrl)
        {
            //var enrollments = _context.Enrollment.Where(s => s.StudentId == id);
            if (id == null)
            {
                return NotFound();
            }
            int stId = 1;
            string st = null;
            if (TempData["selectedStudent"] != null)
            {
                st = TempData["selectedStudent"].ToString();
                stId = Int32.Parse(st);
            }

            var enrollmentToUpdate = await _context.Enrollment.FirstOrDefaultAsync(s => s.Id == id);
            enrollmentToUpdate.SeminalUrl = UploadedFile(semUrl);
            await TryUpdateModelAsync<Enrollment>(
                enrollmentToUpdate,
                "", s => s.ProjectUrl);
            if (ModelState.IsValid)
            {
                try
                {
                   // _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("MyStatusByCourse","Enrollments",new { id = stId});
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
           // ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            //ViewData["StudentId"] = new SelectList(_context.Set<Student>(), "Id", "FullName", enrollment.StudentId);
            return View(enrollmentToUpdate);
        }
        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
