using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.ClassesViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.Controllers
{
    public class ClassesController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public ClassesController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Classes
        [SetToSession("Class")]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 264)]
        public async Task<IActionResult> Index(FilterClassViewModel classFilter, ClassSortState sortOrder = ClassSortState.No, int page = 1)
        {
            if (classFilter.ClassName == null || classFilter.YearCreated == null || classFilter.StudentCount == null  ||
                classFilter.ClassTypeName == null)
            {
                if (HttpContext != null)
                {
                    var sessionClass = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Class");
                    if (sessionClass != null)
                    {
                        classFilter = Transformations.DictionaryToObject<FilterClassViewModel>(sessionClass);
                    }
                }
            }

            IQueryable<Class> schoolContext = _context.Classes
                .Include(c => c.ClassType);

            schoolContext = Sort_Search(
               schoolContext,
               sortOrder,
               classFilter.ClassName ?? "",
               classFilter.YearCreated,
               classFilter.StudentCount,
               classFilter.ClassTypeName ?? "" 
           );

            var count = await schoolContext.CountAsync();
            schoolContext = schoolContext.Skip((page - 1) * pageSize).Take(pageSize);

            var classesList = await schoolContext.ToListAsync();
            var classTypes = await _context.ClassTypes.ToListAsync();
            var classTypeNames = classTypes.Select(ct => ct.Name).ToList();

            ClassViewModel classViewModel = new()
            {
                Classes = classesList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new ClassSortViewModel(sortOrder),
                FilterClassViewModel = classFilter,
                SelectedClassTypeList = new SelectList(classTypeNames),
            };
            return View(classViewModel);
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classItem = await _context.Classes
                .Include(c => c.ClassType)
                .SingleOrDefaultAsync(m => m.ClassId == id);
            if (classItem == null)
            {
                return NotFound();
            }

            return View(classItem);
        }

        // GET: Classes/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ClassTypeId"] = new SelectList(_context.ClassTypes, "ClassTypeId", "Name");
            return View();
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ClassId, Name, ClassTeacher, ClassTypeId, StudentCount, YearCreated")] Class classItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Add(classItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Classes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classItem = await _context.Classes.SingleOrDefaultAsync(m => m.ClassId == id);
            if (classItem == null)
            {
                return NotFound();
            }

            ViewData["ClassTypeId"] = new SelectList(_context.ClassTypes, "ClassTypeId", "Name", classItem.ClassTypeId);
            return View(classItem);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId, Name, ClassTeacher, ClassTypeId, StudentCount, YearCreated")] Class classItem)
        {
            if (id != classItem.ClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(classItem.ClassId))
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

            ViewData["ClassTypeId"] = new SelectList(_context.ClassTypes, "ClassTypeId", "Name", classItem.ClassTypeId);
            return View(classItem);
        }

        // GET: Classes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classItem = await _context.Classes
                .Include(c => c.ClassType)
                .SingleOrDefaultAsync(m => m.ClassId == id);
            if (classItem == null)
            {
                return NotFound();
            }

            return View(classItem);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classItem = await _context.Classes.SingleOrDefaultAsync(m => m.ClassId == id);
            _context.Classes.Remove(classItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }

        private static IQueryable<Class> Sort_Search(
     IQueryable<Class> classes,
     ClassSortState sortOrder,
     string className,
     int? yearCreated,
     int? studentCount,
     string classTypeName)  // Добавляем параметр для фильтрации по типу класса
        {
            // Фильтрация
            if (!string.IsNullOrWhiteSpace(className) || yearCreated.HasValue || studentCount.HasValue || !string.IsNullOrWhiteSpace(classTypeName))
            {
                classes = classes.Where(c =>
                    (string.IsNullOrWhiteSpace(className) || c.Name.Contains(className)) &&
                    (!yearCreated.HasValue || yearCreated.Value == 0 || c.YearCreated == yearCreated.Value) &&
                    (!studentCount.HasValue || c.StudentCount == studentCount.Value) &&
                    (string.IsNullOrWhiteSpace(classTypeName) || c.ClassType.Name.Contains(classTypeName)));  // Фильтрация по типу класса
            }

            // Сортировка
            switch (sortOrder)
            {
                case ClassSortState.ClassNameAsc:
                    classes = classes.OrderBy(c => c.Name);
                    break;
                case ClassSortState.ClassNameDesc:
                    classes = classes.OrderByDescending(c => c.Name);
                    break;
                case ClassSortState.YearCreatedAsc:
                    classes = classes.OrderBy(c => c.YearCreated);
                    break;
                case ClassSortState.YearCreatedDesc:
                    classes = classes.OrderByDescending(c => c.YearCreated);
                    break;
                case ClassSortState.StudentCountAsc:
                    classes = classes.OrderBy(c => c.StudentCount);
                    break;
                case ClassSortState.StudentCountDesc:
                    classes = classes.OrderByDescending(c => c.StudentCount);
                    break;
                case ClassSortState.ClassTypeAsc: 
                    classes = classes.OrderBy(c => c.ClassType.Name);
                    break;
                case ClassSortState.ClassTypeDesc:
                    classes = classes.OrderByDescending(c => c.ClassType.Name);
                    break;
                default:
                    break;
            }

            return classes;
        }

    }
}
