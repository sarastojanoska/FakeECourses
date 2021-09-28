using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class EnrollStudentsViewModel
    {
        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }
        public Enrollment Enrollment { get; set; }
    }
}
