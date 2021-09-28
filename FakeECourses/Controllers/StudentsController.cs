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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FakeECourses.Controllers
{
    public class StudentsController : Controller
    {
        private readonly FakeECoursesContext _context;

        private object webHostEnvironment;
        private IWebHostEnvironment WebHostEnvironment { get; }

        public StudentsController(FakeECoursesContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index(string name,string lastname, string id)
        {
            IQueryable<Student> students = _context.Student.AsQueryable(); 
            IQueryable<string> idQuery = _context.Student.OrderBy(m => m.StudentId).Select(m => m.StudentId).Distinct(); 
            if (!string.IsNullOrEmpty(name)) {
                students = students.Where(s => s.FirstName.Contains(name)); }
            if (!string.IsNullOrEmpty(lastname))
            {
                students = students.Where(s => s.LastName.Contains(lastname));
            }
            if (!string.IsNullOrEmpty(id)) 
            {
                students = students.Where(x => x.StudentId == id); }
            students = students.Include(m => m.Courses).ThenInclude(m => m.Course);
            var studentVM = new StudentViewModel
            { 
                StudentIds = new SelectList(await idQuery.ToListAsync()),
                Students = await students.ToListAsync()}; 
            return View(studentVM);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(m => m.Courses).ThenInclude(m =>m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemester,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = _context.Student.Where(m => m.Id == id).Include(m => m.Courses).First();
            if (student == null)
            {
                return NotFound();
            }
            MyCoursesStudentsEditViewcs mycourses = new MyCoursesStudentsEditViewcs
            {
                Student = student,
               CoursesList = new MultiSelectList(_context.Course.OrderBy(s => s.Title), "Id","Title"),
               SelectedCourses = student.Courses.Select(sa=>sa.CourseId)
            };
            return View(mycourses);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MyCoursesStudentsEditViewcs mycourses)
        {
            if (id != mycourses.Student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mycourses.Student);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> listCourses = mycourses.SelectedCourses; 
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listCourses.Contains(s.CourseId) && s.StudentId == id); 
                    _context.Enrollment.RemoveRange(toBeRemoved); 
                    IEnumerable<int> existCourses = _context.Enrollment.Where(s => listCourses.Contains(s.CourseId) && s.StudentId == id).Select(s => s.CourseId); 
                    IEnumerable<int> newCourses = listCourses.Where(s => !existCourses.Contains(s)); 
                    foreach (int courseId in newCourses) 
                        _context.Enrollment.Add(new Enrollment{ CourseId = courseId, StudentId = id }); 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(mycourses.Student.Id))
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
            return View(mycourses);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(m =>m.Courses).ThenInclude(m=>m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: Students/MyCourses/5
        public async Task<IActionResult> MyCourses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(m => m.Courses).ThenInclude(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
        private string UploadedFile(IFormFile model)
        {
            string uniqueFileName = null;
            if (model != null)
            {
                string uploadsFolder = Path.Combine(WebHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + " _ " + Path.GetFileName(model.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult UploadPic(double? id)
        {
            var vm = new StudentPictureEditView
            {
                Student = null,
                ProfileImage = null
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPic(double? id, IFormFile iffff)
        {
            var vm = new StudentPictureEditView
            {
                Student = await _context.Student.FindAsync(id),
                ProfileImage = iffff
            };
            string uniqueFileName = UploadedFile(vm.ProfileImage);
            vm.Student.ProfilePicture = uniqueFileName;
            _context.Update(vm.Student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

