using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_MS.Data;
using University_MS.Models;
using University_MS.Models.SchoolViewModels;

namespace University_MS.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly Icontoso_repository _repository;

        public StudentsController(SchoolContext context, Icontoso_repository _rep)
        {
            _context = context;
            _repository = _rep;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string SearchString, string currentFilter,
            int? pageNumber)
        {
            
            ViewData["CurrentSort"] = "sortOrder";
            ViewData["NameSortparm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if(SearchString != null)
            {
                pageNumber = 1;

            }
            else
            {
                SearchString = currentFilter;
            }
            ViewData["CurrentFilter"] = SearchString;
            var students = from s in _context.Students select s;
            if (!String.IsNullOrEmpty(SearchString))
            {
                students = students.Where(s => s.LastName.Contains(SearchString)
                  || s.FirstMidName.Contains(SearchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;

                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;

            //return View(await students.ToListAsync());
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(),
                pageNumber ?? 1, pageSize));

            
            
        }

        // GET: Students/Details/5
        public  IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var student = await _context.Students
            //   .FirstOrDefaultAsync(m => m.ID == id);
            //var student = await _context.Students.Include(s => s.Enrollments).
              // ThenInclude(e => e.Course).AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
              /*
            var student = await _context.Students.Include(s => s.Enrollments).
              ThenInclude(s => s.Course).ThenInclude(s=>s.Department).
              Include(s => s.Enrollments).
              ThenInclude(s => s.Course).ThenInclude(s=>s.CourseAssignmets).ThenInclude(s=>s.Instructor).
              AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
              */
            
            var cour_inst = new List< Student_teacher>();
            
         /* var   cour_instt = await _context.Students.Where(i => i.ID == id).
                 Include(i => i.Enrollments).ThenInclude(i => i.Course).ThenInclude(i => i.CourseAssignmets).ThenInclude(i => i.Instructor).
                 AsNoTracking().ToListAsync();*/

          /*  var result_linq = from s in _context.Students
                              from i in _context.Enrollments
                              from c in _context.Courses
                              from cs in _context.CourseAssignments
                              from ins in _context.Instructors
                              where s.ID == id
                              select new
                              {
                                  CourseID = c.CourseID,
                                  Instr_fullname = ins.FullName

                              };*/
                 
            var cour_instc =  _context.Students.Where(s => s.ID == id)
                                    .Select(s => new
                                    {
                                        //Student = s,
                                        //Enrollment = s.Enrollments,
                                        CourseID = s.Enrollments.Select(c => c.Course.CourseID).ToList(),
                                        Instr_fullname = (s.Enrollments.Select(c => c.Course.CourseAssignmets.Select(i=>i.Instructor.FullName).First())).ToList()
                                    }).ToList();
            
            foreach( var r in cour_instc)
            {
                cour_inst.Add(
                     new Student_teacher
                     {
                         CoursID =  r.CourseID,
                         Instructor_Name =  r.Instr_fullname

                     });

            }
            
/*
            Student students  = cour_inst.students.Where(i => i.ID == id.Value).Single();
            //  cour_inst.Instructors = students.Enrollments.Select(s => s.Course.CourseAssignmets.Select(i => i.Instructor));
            cour_inst.Instructors = students.Enrollments.Select(s => s.Course.CourseAssignmets.Select(i => i.Instructor));
            */
            ViewData["result"] = cour_inst;
           int stud_id=  id ?? default(int);
            var student =  _repository.GetAllStudents(stud_id);
                if (student == null)
            {
                return NotFound();
            }

            return View( student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            populatecourses();
            return View();
        }

        public void populatecourses()
        {
            var allCourses = new List<Course>(_context.Courses);
            ViewData["courses"] = allCourses;
        }


        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(" ID, LastName,FirstMidName,EnrollmentDate, Acc_Year, Semi_Id")] Student student, string[] selecteedcourses)
        {
            var stud_id = await _context.Students.Select(d=> d.ID).ToListAsync();
            foreach(var id in stud_id)
            {
                if(id==student.ID)
                {
                    ModelState.AddModelError("", "Id alraedy Exists " +
                       "Try again with new Id, and if the problem persists, " +
                       "see your system administrator.");
                    populatecourses();
                    return View(student);

                }
            }

            if (selecteedcourses != null)
            {
                student.Enrollments = new List<Enrollment>();
                foreach (var course in selecteedcourses)
                {
                    var stud_course = new Enrollment { StudentID = student.ID, CourseID = int.Parse(course) };
                    student.Enrollments.Add(stud_course);
                    //Stu
                }
            }






            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            populatecourses();
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m=>m.ID==id);
                
            if (student == null)
            {
                return NotFound();
            }

            List<string> acc_year = new List<string> { "17/18", "18/19", "19/20" };
            PopulateAssignedCourseData(student);
            ViewData["acc_year"] = student.Acc_Year.ToString();
            ViewData["acc_year_list"] = acc_year;
            ViewBag.Profile_Id = new SelectList(acc_year, "Id", "Name", student.Acc_Year.ToString());
            return View(student);
        }

        private void PopulateAssignedCourseData(Student student)
        {
            var allCourses = _context.Courses;
            var studentCourses = new HashSet<int>(student.Enrollments
                .Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(
                    new AssignedCourseData
                    {
                        CourseID = course.CourseID,
                        Title = course.Title,
                        Assigned = studentCourses.Contains(course.CourseID)

                    });


            }
            ViewData["Courses"] = viewModel;
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentToUpdate = await _context.Students
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if ( await TryUpdateModelAsync<Student>(studentToUpdate,
                "",s => s.LastName, s=> s.FirstMidName, s=> s.EnrollmentDate, s=>s.Semi_Id, ac=>ac.Acc_Year))
            {
                UpdateStudentCourses(selectedCourses, studentToUpdate);
                try
                {
                    
                 int x=   await _context.SaveChangesAsync();
                    int y = 9;
                    string kk = "";
                    
                }
                catch (DbUpdateException)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                
                return RedirectToAction(nameof(Index));
            }
            UpdateStudentCourses(selectedCourses, studentToUpdate);
            PopulateAssignedCourseData(studentToUpdate);
            return View(studentToUpdate);
        }

        private void UpdateStudentCourses(string[] selectedCourses, Student studentToUpdate)
        {
            if (selectedCourses == null)
            {
                studentToUpdate.Enrollments = new List<Enrollment>();
                return;
            }
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var studentCourses = new HashSet<int>(studentToUpdate.Enrollments
                .Select(c => c.Course.CourseID));

            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!studentCourses.Contains(course.CourseID))
                    {
                        studentToUpdate.Enrollments.Add(new Enrollment
                        { StudentID = studentToUpdate.ID, CourseID = course.CourseID });
                    }
                }
                else
                {
                    if (studentCourses.Contains(course.CourseID))
                    {
                        Enrollment courseToRemove = studentToUpdate.Enrollments
                            .FirstOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            if(saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists" +
                       "see your system administrator";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if(student==null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DbUpdateException /* ex */)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            /*
             The create-and-attach approach to HttpPost Delete
            try
            {
                Student studentToDelete = new Student() { ID = id };
                _context.Entry(studentToDelete).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException  ex )
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
            */
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
