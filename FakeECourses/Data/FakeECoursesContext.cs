using FakeECourses.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.Data
{
    public class FakeECoursesContext : DbContext
    {
        public FakeECoursesContext(DbContextOptions<FakeECoursesContext> options)
               : base(options) { }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Enrollment>()
             .HasOne<Student>(p => p.Student)
             .WithMany(p => p.Courses)
             .HasForeignKey(p => p.StudentId);
            builder.Entity<Enrollment>()
              .HasOne<Course>(p => p.Course)
              .WithMany(p => p.Students)
              .HasForeignKey(p => p.CourseId);
            builder.Entity<Course>()
              .HasOne<Teacher>(p => p.FirstTeacher)
              .WithMany(p => p.Courses1)
             .HasForeignKey(p => p.FirstTeacherId);
            builder.Entity<Course>()
             .HasOne<Teacher>(p => p.SecondTeacher)
             .WithMany(p => p.Courses2)
             .HasForeignKey(p => p.SecondTeacherId);
        }

        public DbSet<FakeECourses.Models.Teacher> Teacher { get; set; }
        public DbSet<FakeECourses.Models.Student> Student { get; set; }
        public DbSet<FakeECourses.Models.Course> Course { get; set; }
        public DbSet<FakeECourses.Models.Enrollment> Enrollment { get; set; }
    }
}
