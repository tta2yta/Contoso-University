using University_MS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace University_MS.Data
{
    public interface Icontoso_repository
    {
        Student GetAllStudents(int id);
        IEnumerable<Department> GetDepartments();
    }
}
