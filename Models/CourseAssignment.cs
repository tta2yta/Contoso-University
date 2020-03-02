using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace University_MS.Models
{
    public class CourseAssignment
    {
        public int InstructorID { get; set; }
        public int CourseID { get; set; }
        public Instructor Instructor { get; set; }
        public Course Cousre { get; set; }
    }
}
