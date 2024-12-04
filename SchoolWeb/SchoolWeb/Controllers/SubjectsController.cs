using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.SubjectsViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;
using SchoolWeb.Infrastructure;

namespace SchoolWeb.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        // Конструктор, который инициализирует контекст и размер страницы из конфигурации
        public SubjectsController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Subjects
        public async Task<IActionResult> Index(FilterSubjectViewModel subjectFilter, SubjectSortState sortOrder = SubjectSortState.No, int page = 1)
        {
            // Если фильтры не заданы, пытаемся восстановить их из сессии
            if (subjectFilter.SubjectName == null)
            {
                if (HttpContext != null)
                {
                    var sessionSubject = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Subject");
                    if (sessionSubject != null)
                    {
                        subjectFilter = Transformations.DictionaryToObject<FilterSubjectViewModel>(sessionSubject);
                    }
                }
            }

            // Формируем запрос для фильтрации и сортировки
            IQueryable<Subject> subjectQuery = _context.Subjects
                .Include(s => s.Employee); // Включаем информацию о преподавателе

            subjectQuery = Sort_Search(
               subjectQuery,
               sortOrder,
               subjectFilter.SubjectName,
               subjectFilter.EmployeeName
            );

            // Получаем количество записей после фильтрации
            var count = await subjectQuery.CountAsync();

            // Применяем пагинацию
            subjectQuery = subjectQuery.Skip((page - 1) * pageSize).Take(pageSize);

            // Загружаем список предметов
            var subjectsList = await subjectQuery.ToListAsync();

            // Создаем список названий предметов для SelectList
            // Создаем список уникальных названий предметов для SelectList
            var subjectNames = _context.Subjects
                                .Select(s => s.Name)
                                .Distinct()
                                .ToList();


            // Создаем модель для передачи данных в представление
            SubjectViewModel subjectViewModel = new()
            {
                Subjects = subjectsList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SubjectSortViewModel(sortOrder),
                FilterSubjectViewModel = subjectFilter,
                SelectSubjectList = new SelectList(subjectNames)
            };

            return View(subjectViewModel);
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Если ID не задано, возвращаем ошибку
            }

            var subjectItem = await _context.Subjects
                .Include(s => s.Employee) // Включаем информацию о преподавателе
                .SingleOrDefaultAsync(m => m.SubjectId == id);
            if (subjectItem == null)
            {
                return NotFound();
            }

            return View(subjectItem);
        }

        // GET: Subjects/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName");
            return View(); 
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("SubjectId, Name, Description, EmployeeId")] Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Если модель не валидна, возвращаем ошибку
            }
            else
            {
                _context.Add(subject); // Добавляем новый предмет в базу данных
                await _context.SaveChangesAsync(); // Сохраняем изменения
            }

            return RedirectToAction(nameof(Index)); // Перенаправляем на список предметов
        }

        // GET: Subjects/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Если ID не задано, возвращаем ошибку
            }

            var subjectItem = await _context.Subjects.SingleOrDefaultAsync(m => m.SubjectId == id);
            if (subjectItem == null)
            {
                return NotFound(); // Если предмет не найден, возвращаем ошибку
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", subjectItem.EmployeeId);
            return View(subjectItem); // Отображаем форму для редактирования предмета
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("SubjectId, Name, Description, EmployeeId")] Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return NotFound(); // Если ID не совпадает, возвращаем ошибку
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject); // Обновляем информацию о предмете
                    await _context.SaveChangesAsync(); // Сохраняем изменения
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.SubjectId))
                    {
                        return NotFound(); // Если предмет не найден, возвращаем ошибку
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Перенаправляем на список предметов
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName", subject.EmployeeId);
            return View(subject); // Отображаем форму с ошибками, если модель невалидна
        }

        // GET: Subjects/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Если ID не задано, возвращаем ошибку
            }

            var subjectItem = await _context.Subjects
                .Include(s => s.Employee) // Включаем информацию о преподавателе
                .SingleOrDefaultAsync(m => m.SubjectId == id);
            if (subjectItem == null)
            {
                return NotFound(); // Если предмет не найден, возвращаем ошибку
            }

            return View(subjectItem); // Отображаем подтверждение удаления
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectItem = await _context.Subjects.SingleOrDefaultAsync(m => m.SubjectId == id);
            _context.Subjects.Remove(subjectItem); // Удаляем предмет из базы данных
            await _context.SaveChangesAsync(); // Сохраняем изменения
            return RedirectToAction(nameof(Index)); // Перенаправляем на список предметов
        }

        // Проверка, существует ли предмет с заданным ID
        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }

        // Метод для фильтрации и сортировки предметов
        private static IQueryable<Subject> Sort_Search(
         IQueryable<Subject> subjects,
         SubjectSortState sortOrder,
         string subjectName,
         string employeeName)
        {
            // Фильтрация по названию предмета, если передано значение
            if (!string.IsNullOrEmpty(subjectName))
            {
                subjects = subjects.Where(s => s.Name.Contains(subjectName));
            }

            // Фильтрация по имени преподавателя, если передано значение
            if (!string.IsNullOrEmpty(employeeName))
            {
                subjects = subjects.Where(s => s.Employee.FirstName.Contains(employeeName));
            }

            // Сортировка
            switch (sortOrder)
            {
                case SubjectSortState.SubjectNameAsc:
                    subjects = subjects.OrderBy(s => s.Name);
                    break;
                case SubjectSortState.SubjectNameDesc:
                    subjects = subjects.OrderByDescending(s => s.Name);
                    break;
                case SubjectSortState.EmployeeNameAsc:
                    subjects = subjects.OrderBy(s => s.Employee.FirstName);
                    break;
                case SubjectSortState.EmployeeNameDesc:
                    subjects = subjects.OrderByDescending(s => s.Employee.FirstName);
                    break;
                default:
                    break;
            }

            return subjects;
        }
    }
}
