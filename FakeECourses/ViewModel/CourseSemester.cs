using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class CourseSemester
    {
        public IList<Course> Courses { get; set; }
        public SelectList Semesters { get; set; }
        public string Coursesemester { get; set; }
        public SelectList Programms { get; set; }
        public string Courseprogramm { get; set; }
        public string SearchString { get; set; }
    }
}
