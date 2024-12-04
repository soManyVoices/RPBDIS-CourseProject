using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.ClassTypesViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.Controllers
{
    public class ClassTypesController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public ClassTypesController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: ClassTypes
        [SetToSession("ClassType")]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 264)]
        public async Task<IActionResult> Index(FilterClassTypeViewModel filter, ClassTypeSortState sortOrder = ClassTypeSortState.No, int page = 1)
        {
            if (filter.ClassTypeName == null)
            {
                if (HttpContext != null)
                {
                    var sessionClassType = Infrastructure.SessionExtensions.Get(HttpContext.Session, "ClassType");
                    if (sessionClassType != null)
                    {
                        filter = Transformations.DictionaryToObject<FilterClassTypeViewModel>(sessionClassType);
                    }
                }
            }

            IQueryable<ClassType> classTypes = _context.ClassTypes;

            // Применяем фильтрацию только по имени типа класса
            classTypes = Sort_Search(
               classTypes,
               sortOrder,
               filter.ClassTypeName ?? ""
           );

            var count = await classTypes.CountAsync();
            classTypes = classTypes.Skip((page - 1) * pageSize).Take(pageSize);

            var classTypeList = await classTypes.ToListAsync();
            var allClassTypeNames = _context.ClassTypes.Select(ct => ct.Name).Distinct().ToList(); // Сохраняем полный список типов классов

            ClassTypeViewModel viewModel = new()
            {
                ClassTypes = classTypeList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new ClassTypeSortViewModel(sortOrder),
                FilterClassTypeViewModel = filter,
                SelectedClassTypeList = new SelectList(allClassTypeNames), // Передаем полный список типов классов
            };
            return View(viewModel);
        }

        // GET: ClassTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classType = await _context.ClassTypes
                .SingleOrDefaultAsync(m => m.ClassTypeId == id);
            if (classType == null)
            {
                return NotFound();
            }

            return View(classType);
        }

        // GET: ClassTypes/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ClassTypeId, Name, Description")] ClassType classType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classType);
        }

        // GET: ClassTypes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classType = await _context.ClassTypes.SingleOrDefaultAsync(m => m.ClassTypeId == id);
            if (classType == null)
            {
                return NotFound();
            }
            return View(classType);
        }

        // POST: ClassTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ClassTypeId, Name, Description")] ClassType classType)
        {
            if (id != classType.ClassTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassTypeExists(classType.ClassTypeId))
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
            return View(classType);
        }

        // GET: ClassTypes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classType = await _context.ClassTypes
                .SingleOrDefaultAsync(m => m.ClassTypeId == id);
            if (classType == null)
            {
                return NotFound();
            }

            return View(classType);
        }

        // POST: ClassTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classType = await _context.ClassTypes.SingleOrDefaultAsync(m => m.ClassTypeId == id);
            _context.ClassTypes.Remove(classType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassTypeExists(int id)
        {
            return _context.ClassTypes.Any(e => e.ClassTypeId == id);
        }

        private static IQueryable<ClassType> Sort_Search(
            IQueryable<ClassType> classTypes,
            ClassTypeSortState sortOrder,
            string classTypeName)
        {
            // Фильтрация только по имени типа класса
            if (!string.IsNullOrWhiteSpace(classTypeName))
            {
                classTypes = classTypes.Where(c => c.Name.Contains(classTypeName));
            }

            // Сортировка
            switch (sortOrder)
            {
                case ClassTypeSortState.ClassTypeNameAsc:
                    classTypes = classTypes.OrderBy(c => c.Name);
                    break;
                case ClassTypeSortState.ClassTypeNameDesc:
                    classTypes = classTypes.OrderByDescending(c => c.Name);
                    break;
                default:
                    break;
            }

            return classTypes;
        }
    }
}
