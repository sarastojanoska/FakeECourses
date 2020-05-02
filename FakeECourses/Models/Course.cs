using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public int Credits { get; set; }
        public int Semester { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [StringLength(100)]
        public string Programme { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [StringLength(25)]
        public string EducationLevel { get; set; }

        [Display(Name = "Teacher 1")]
        public int? FirstTeacherId { get; set; }
        [Display(Name = "Teacher 1")]
        public Teacher FirstTeacher { get; set; }


        [Display(Name = "Teacher 2")]
        public int? SecondTeacherId { get; set; }
        [Display(Name = "Teacher 2")]
        public Teacher SecondTeacher { get; set; }
        public ICollection<Enrollment> Students { get; set; }
    }
}
