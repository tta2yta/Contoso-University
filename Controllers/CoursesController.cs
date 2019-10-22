using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            /*
            var schoolContext = _context.Courses.Include(c => c.Department);
            return View(await schoolContext.ToListAsync());
            */
            var courses = _context.Courses.Include(c => c.Department).AsNoTracking();
            return View(await courses.ToListAsync());

        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, int? courseID)
        {
            
            if (id == null)
            {
                return NotFound();
            }
            var viewmodel_course = new course_stud_teacher();
            viewmodel_course.course = await _context.Courses
               .Include(c => c.Department)
               .Include(cs => cs.CourseAssignmets).ThenInclude(i => i.Instructor)
               .Include(e=>e.Enrollments).ThenInclude(s=>s.Student)
               .ToListAsync();
                
            if (viewmodel_course == null)
            {
                return NotFound();
            }
            Course course = viewmodel_course.course.Where(c => c.CourseID == id).Single();
            viewmodel_course.single_course = course;
            if (courseID != null)
            {
                viewmodel_course.instructors = course.CourseAssignmets.Select(i => i.Instructor);
                viewmodel_course.students = course.Enrollments.Select(s => s.Student);
            }
            return View(viewmodel_course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            PopulateDepartmentsDropDownList();
            return View();
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID"
                , "Name", selectedDepartment);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits,DepartmentID")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", course.DepartmentID);
            PopulateDepartmentsDropDownList();
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var course = await _context.Courses.FindAsync(id);

            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", course.DepartmentID);
            PopulateDepartmentsDropDownList();
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       // [HttpPost]
        //[ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Credits,DepartmentID")] Course course)
        // {
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
            {

                if (id ==null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseID == id);
            if( await TryUpdateModelAsync<Course>(courseToUpdate,"",
                c=> c.Credits, c=> c.DepartmentID, c=>c.Title))
            { 
                try
                {
                   // _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes." +
                        "Try again, and if the problem persists," +
                        "see your system administrator");
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", course.DepartmentID);
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department).AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}
