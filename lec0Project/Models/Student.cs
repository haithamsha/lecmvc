using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lec0Project.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Student Name")]
        public string Name { get; set; }
        public int Age { get; set; }

        // beherviours

        public List<Student> GetAllStudents()
        {
            return new List<Student>
            {
                new Student {Id= 1, Name= "Haitham", Age=30 },
                 new Student {Id= 1, Name= "Weal", Age=20},
                  new Student {Id= 1, Name= "Ahmed", Age=25},
                   new Student {Id= 1, Name= "Loay", Age=22}
            };
        }

    }
}