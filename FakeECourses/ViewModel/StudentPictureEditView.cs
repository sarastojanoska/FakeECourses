using FakeECourses.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class StudentPictureEditView
    {
        public Student Student { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
