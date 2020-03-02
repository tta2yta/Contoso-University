using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace University_MS.Models
{
    public class course_stud_teacher
    {
        public IEnumerable<Student> students { get; set; }
        public IEnumerable<Instructor> instructors { get; set; }
        public IEnumerable<Course> course { get; set; }
        public Course single_course { get; set; }
    }
}
