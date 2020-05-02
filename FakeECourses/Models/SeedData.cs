using FakeECourses.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeECourses.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FakeECoursesContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FakeECoursesContext>>()))
            {
                if (context.Course.Any() || context.Teacher.Any() || context.Student.Any())
                {
                    return;
                }
                context.Teacher.AddRange(
                    new Teacher {/*Id=1,*/FirstName = "Daniel", LastName = "Denkovski" , Degree = "PhD", AcademicRank = "Rocker"},
                    new Teacher {/*Id=2,*/ FirstName = "Pero", LastName = "Latkoski" , Degree = "PhD", AcademicRank="Singer"},
                    new Teacher {/*Id=3,*/ FirstName = "Valentin", LastName = "Rakovik", Degree = "PhD", AcademicRank = "Drummer"},
                    new Teacher {/*Id=4,*/ FirstName = "Vladimir", LastName = "Atanasovski", Degree = "PhD", AcademicRank = "Guitarist"}
                    );
                context.SaveChanges();
                context.Student.AddRange(
                    new Student {/*Id=1,*/ StudentId = "265/2017", FirstName = "Princess", LastName = "Aurora" },
                    new Student {/*Id=2,*/ StudentId = "216/2017", FirstName = "Prince", LastName = "Erik" },
                    new Student {/*Id=3,*/ StudentId = "266/2017", FirstName = "Princess", LastName = "Ariel" },
                    new Student {/*Id=4,*/ StudentId = "261/2017", FirstName = "Prince", LastName = "Philip" }
                    );
                context.SaveChanges();
                context.Course.AddRange(
                    new Course
                    {/*Id=1,*/
                        Title = "Telesoobrakjaen Inzenering",
                        Programme = "TKII",
                        Semester = 6,
                        FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Valentin" && d.LastName == "Rakovik").Id
                    },
                    new Course
                    {/*Id=2,*/
                        Title = "TKM",
                        Credits = 6,
                        Programme = "TKII",
                        FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Vladimir" && d.LastName == "Atanasovski").Id
                    },
                    new Course
                    {/*Id=3,*/
                        Title = "MSAP",
                        Credits = 6,
                        Programme = "TKII",
                        FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Daniel" && d.LastName == "Denkovski").Id,
                        SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Pero" && d.LastName == "Latkoski").Id
                    },
                    new Course
                    {/*Id=4,*/
                        Title = "RSWeb",
                        Credits = 6,
                        Programme = "TKII",
                        FirstTeacherId = context.Teacher.Single(d => d.FirstName == "Daniel" && d.LastName == "Denkovski").Id,
                        SecondTeacherId = context.Teacher.Single(d => d.FirstName == "Pero" && d.LastName == "Latkoski").Id
                    }
                    );
                context.SaveChanges();
                context.Enrollment.AddRange(
                    new Enrollment { CourseId = 1, StudentId = 1 },
                    new Enrollment { CourseId = 1, StudentId = 2 },
                    new Enrollment { CourseId = 1, StudentId = 3 },
                    new Enrollment { CourseId = 2, StudentId = 3 },
                    new Enrollment { CourseId = 2, StudentId = 4 },
                    new Enrollment { CourseId = 3, StudentId = 1 },
                    new Enrollment { CourseId = 3, StudentId = 4 },
                    new Enrollment { CourseId = 4, StudentId = 1 },
                    new Enrollment { CourseId = 4, StudentId = 2 },
                    new Enrollment { CourseId = 4, StudentId = 4 }
                    );
                context.SaveChanges();
            }
        }
    }
}
