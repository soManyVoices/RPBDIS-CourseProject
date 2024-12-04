using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure;
using SchoolWeb.ViewModels.StudentViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.ViewModels;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchoolWeb.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public StudentsController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Students
        [SetToSession("Student")]
        [Authorize]
        public async Task<IActionResult> Index(FilterStudentViewModel studentFilter, StudentSortState sortOrder = StudentSortState.No, int page = 1, DateOnly? dateOfBirth = null)
        {
            // Установка фильтров из сессии, если они не заданы
            if (studentFilter.ClassName == null || studentFilter.SubjectName == null || studentFilter.DateOfBirth == null)
            {
                if (HttpContext != null)
                {
                    var sessionStudent = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Student");
                    if (sessionStudent != null)
                    {
                        studentFilter = Transformations.DictionaryToObject<FilterStudentViewModel>(sessionStudent);
                    }
                }
            }

            IQueryable<Student> studentsQuery = _context.Students
            .Include(s => s.Class)
                .ThenInclude(c => c.Schedules)
                    .ThenInclude(sch => sch.Subject)
            .AsQueryable();

            // Логирование или отладка

            studentsQuery = Sort_Search(
                studentsQuery,
                sortOrder,
                studentFilter.ClassName ?? "",
                studentFilter.SubjectName ?? "",
                studentFilter.DateOfBirth
            );  

            var count = await studentsQuery.CountAsync();
            studentsQuery = studentsQuery.Skip((page - 1) * pageSize).Take(pageSize);

            var studentsList = await studentsQuery.ToListAsync();
            Console.WriteLine($"Загружено студентов: {studentsList.Count}");
            var classes = await _context.Classes.ToListAsync();
            var subjects = await _context.Subjects.ToListAsync();
            var subjectName = subjects.Select(s => s.Name).ToList();
            var className = classes.Select(c => c.Name).ToList();

            StudentViewModel studentViewModel = new()
            {
                Students = studentsList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new StudentSortViewModel(sortOrder),
                FilterStudentViewModel = studentFilter,
                SelectedClassNameList = new SelectList(className),
                SelectedSubjectNameList = new SelectList(subjectName)
            };

            return View(studentViewModel);
        }


        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .SingleOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", student.ClassId);
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.SingleOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", student.ClassId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", student.ClassId);
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
                .SingleOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.SingleOrDefaultAsync(s => s.StudentId == id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        private static IQueryable<Student> Sort_Search(
      IQueryable<Student> students,
      StudentSortState sortOrder,
      string className,
      string subjectName,
      DateOnly? dateOfBirth)
        {
            // Фильтрация по названию класса, если передано значение
            if (!string.IsNullOrEmpty(className))
            {
                students = students.Include(s => s.Class)
                                   .Where(s => s.Class.Name.Contains(className));
            }

            // Фильтрация по названию предмета, если передано значение
            if (!string.IsNullOrEmpty(subjectName))
            {
                students = students.Include(s => s.Class)
                                   .ThenInclude(c => c.Schedules)
                                   .ThenInclude(sch => sch.Subject)
                                   .Where(s => s.Class.Schedules.Any(sch => sch.Subject.Name.Contains(subjectName)));
            }

            if (dateOfBirth.HasValue && dateOfBirth != DateOnly.MinValue)
            {
                students = students.Where(s => s.DateOfBirth == dateOfBirth.Value);
            }
            else
            {
                Console.WriteLine("Дата рождения не указана или установлена по умолчанию.");
            }



            // Сортировка
            switch (sortOrder)
            {
                case StudentSortState.ClassNameAsc:
                    students = students.OrderBy(s => s.Class.Name);
                    break;
                case StudentSortState.ClassNameDesc:
                    students = students.OrderByDescending(s => s.Class.Name);
                    break;
                case StudentSortState.DateOfBirthAsc:
                    students = students.OrderBy(s => s.DateOfBirth);
                    break;
                case StudentSortState.DateOfBirthDesc:
                    students = students.OrderByDescending(s => s.DateOfBirth);
                    break;
                case StudentSortState.SubjectNameAsc:
                    students = students.OrderBy(s => s.Class.Schedules.FirstOrDefault().Subject.Name);
                    break;
                case StudentSortState.SubjectNameDesc:
                    students = students.OrderByDescending(s => s.Class.Schedules.FirstOrDefault().Subject.Name);
                    break;
                default:
                    break;
            }

            return students;
        }
    }
}
