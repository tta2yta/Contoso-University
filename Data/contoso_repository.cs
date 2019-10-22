﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class contoso_repository : Icontoso_repository
    {

        private readonly SchoolContext _context;


        public contoso_repository(SchoolContext context)
        {
            _context = context;
        }
        public   Student GetAllStudents( int id)
        {
            var student =    _context.Students.Include(s => s.Enrollments).
             ThenInclude(s => s.Course).ThenInclude(s => s.Department).
             Include(s => s.Enrollments).
             ThenInclude(s => s.Course).ThenInclude(s => s.CourseAssignmets).ThenInclude(s => s.Instructor).
             FirstOrDefault(m => m.ID == id);
             return student;
        }

        public IEnumerable<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
