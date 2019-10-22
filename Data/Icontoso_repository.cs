using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Data
{
    public interface Icontoso_repository
    {
        Student GetAllStudents(int id);
        IEnumerable<Department> GetDepartments();
    }
}
