using FakeECourses.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class StudentViewModel
    {
        public IList<Student> Students { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SelectList StudentIds { get; set; }
        public string studentIds { get; set; }
    }
}
