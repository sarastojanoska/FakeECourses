using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class MyCoursesStudentsEditViewcs
    {
        public Student Student { get; set; }
        public IEnumerable<int> SelectedCourses { get; set; }
        public IEnumerable<SelectListItem> CoursesList { get; set; }
    }
}
