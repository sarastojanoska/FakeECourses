using FakeECourses.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.ViewModel
{
    public class TeacherPictureEditView
    {
        public Teacher Teacher { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
