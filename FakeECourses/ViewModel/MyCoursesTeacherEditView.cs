using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class MyCoursesTeacherEditView
    {
        public Teacher Teacher { get; set; }
        public IEnumerable<int> SelectedCourses1 { get; set; }
        public IEnumerable<int> SelectedCourses2 { get; set; }
        public IEnumerable<SelectListItem> CoursesList1 { get; set; }
        public IEnumerable<SelectListItem> CoursesList2 { get; set; }
    }
}
