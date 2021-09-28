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
    public class TeachersController : Controller
    {
        private readonly FakeECoursesContext _context;
        private object webHostEnvironment;

        private IWebHostEnvironment WebHostEnvironment { get; }

        public TeachersController(FakeECoursesContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string rank, string degree,string name,string lastname)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable(); 
            IQueryable<string> rankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();
            IQueryable<string> degreeQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            if (!string.IsNullOrEmpty(name)) 
            {
                teachers = teachers.Where(s => s.FirstName.Contains(name)); }
            if (!string.IsNullOrEmpty(lastname))
            {
                teachers = teachers.Where(s => s.LastName.Contains(lastname));
            }
            if (!string.IsNullOrEmpty(rank)) 
            { 
                teachers = teachers.Where(x => x.AcademicRank == rank); }
            if (!string.IsNullOrEmpty(degree))
            {
                teachers = teachers.Where(x => x.Degree == degree);
            }
            teachers = teachers.Include(c => c.Courses1).Include(c => c.Courses2);
              var teachernameTM = new TeacherName
              {
                  AcademicRank = new SelectList(await rankQuery.ToListAsync()),
                  Degree = new SelectList(await degreeQuery.ToListAsync()),
                  Teachers = await teachers.ToListAsync()}; 
            return View(teachernameTM);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .Include(m => m.Courses1)
                .Include(m =>m.Courses2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher =_context.Teacher.Where(m => m.Id == id).Include(m => m.Courses1).Include(m => m.Courses2).First();
            if (teacher == null)
            {
                return NotFound();
            }
            MyCoursesTeacherEditView teachercourses = new MyCoursesTeacherEditView
            {
                Teacher = teacher,
                CoursesList1 = new MultiSelectList(_context.Course.OrderBy(s =>s.Title), "Id","Title"),
                CoursesList2 = new MultiSelectList(_context.Course.OrderBy(s => s.Title), "Id", "Title"),
                SelectedCourses1 = teacher.Courses1.Select(sa => sa.Id),
                SelectedCourses2 = teacher.Courses2.Select(sa => sa.Id)
            };
            return View(teachercourses);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MyCoursesTeacherEditView teachercourses)
        {
            if (id != teachercourses.Teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teachercourses.Teacher);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> listCourses1 = teachercourses.SelectedCourses1; 
                    IQueryable<Course> toBeRemoved1 = _context.Course.Where(s => !listCourses1.Contains(s.Id) && s.FirstTeacherId == id);
                    _context.Course.RemoveRange(toBeRemoved1);
                    
                    IEnumerable<int> existCourses1 = _context.Course.Where(s => listCourses1.Contains(s.Id) && s.FirstTeacherId == id).Select(s => s.Id); 
                    IEnumerable<int> newCourses1 = listCourses1.Where(s => !existCourses1.Contains(s)); 
                    foreach (int course1 in newCourses1) _context.Course.Add(new Course
                    { Id = course1, FirstTeacherId = id }); 
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listCourses2 = teachercourses.SelectedCourses2;
                    IQueryable<Course> toBeRemoved2 = _context.Course.Where(s => !listCourses2.Contains(s.Id) && s.SecondTeacherId == id);
                    _context.Course.RemoveRange(toBeRemoved2);

                    IEnumerable<int> existCourses2 = _context.Course.Where(s => listCourses2.Contains(s.Id) && s.SecondTeacherId == id).Select(s => s.Id);
                    IEnumerable<int> newCourses2 = listCourses2.Where(s => !existCourses2.Contains(s));
                    foreach (int course2 in newCourses2) _context.Course.Add(new Course
                    { Id = course2, SecondTeacherId = id });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teachercourses.Teacher.Id))
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
            return View(teachercourses);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.Include(m => m.Courses1).Include(m => m.Courses2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Teachers/My Courses/5
        public async Task<IActionResult> MyCourses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.Include(m => m.Courses1).Include(m => m.Courses2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
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
            var vm = new TeacherPictureEditView
            {
                Teacher = null,
                ProfileImage = null
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPic(double? id, IFormFile iffff)
        {
            var vm = new TeacherPictureEditView
            {
                Teacher = await _context.Teacher.FindAsync(id),
                ProfileImage = iffff
            };
            string uniqueFileName = UploadedFile(vm.ProfileImage);
            vm.Teacher.ProfilePicture = uniqueFileName;
            _context.Update(vm.Teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
