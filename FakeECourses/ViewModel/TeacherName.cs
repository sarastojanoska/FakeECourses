using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class TeacherName
    {
        public IList<Teacher> Teachers { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SearchString { get; set; }
        public SelectList AcademicRank { get; set; }
        public string TeacherRank { get; set; }
        public SelectList Degree { get; set; }
        public string TeacherDegree { get; set; }
    }
}
