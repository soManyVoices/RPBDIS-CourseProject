using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.SchedulesViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public SchedulesController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Schedules
        [SetToSession("Schedule")]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 264)]
        public async Task<IActionResult> Index(FilterScheduleViewModel scheduleFilter, ScheduleSortState sortOrder = ScheduleSortState.No, int page = 1)
        {
            if (scheduleFilter.ClassName == null || scheduleFilter.SubjectName == null)
            {
                if (HttpContext != null)
                {
                    var sessionSchedule = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Schedule");
                    if (sessionSchedule != null)
                    {
                        scheduleFilter = Transformations.DictionaryToObject<FilterScheduleViewModel>(sessionSchedule);
                    }
                }
            }

            // Формируем запрос для фильтрации и сортировки
            IQueryable<Schedule> scheduleQuery = _context.Schedules
                .Include(s => s.Class) // Включаем класс
                .Include(s => s.Subject);

            scheduleQuery = Sort_Search(
               scheduleQuery,
               sortOrder,
               scheduleFilter.ClassName ?? "",
               scheduleFilter.SubjectName ?? ""
            );

            // Получаем количество записей после фильтрации
            var count = await scheduleQuery.CountAsync();

            // Применяем пагинацию
            scheduleQuery = scheduleQuery.Skip((page - 1) * pageSize).Take(pageSize);

            // Загружаем данные расписания
            var schedulesList = await scheduleQuery.ToListAsync();

            // Получаем все классы и предметы из базы данных
            var classes = await _context.Classes.ToListAsync();
            var subjects = await _context.Subjects.ToListAsync();

            // Формируем списки для выпадающих списков
            var classNames = classes.Select(c => c.Name).ToList();
            var subjectNames = subjects.Select(s => s.Name).ToList();

            // Создаем модель для передачи данных в представление
            ScheduleViewModel scheduleViewModel = new()
            {
                Schedules = schedulesList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new ScheduleSortViewModel(sortOrder),
                FilterScheduleViewModel = scheduleFilter,
                SelectedClassNameList = new SelectList(classNames),
                SelectedSubjectNameList = new SelectList(subjectNames),
            };

            return View(scheduleViewModel);
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleItem = await _context.Schedules
                .Include(s => s.Class)
                .Include(s => s.Subject)
                .SingleOrDefaultAsync(m => m.ScheduleId == id);
            if (scheduleItem == null)
            {
                return NotFound();
            }

            return View(scheduleItem);
        }

        // GET: Schedules/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name");
            return View();
        }

        // POST: Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("ScheduleId, Date, DayOfWeek, ClassId, SubjectId, StartTime, EndTime")] Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Schedules/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleItem = await _context.Schedules.SingleOrDefaultAsync(m => m.ScheduleId == id);
            if (scheduleItem == null)
            {
                return NotFound();
            }

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", scheduleItem.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name", scheduleItem.SubjectId);
            return View(scheduleItem);
        }

        // POST: Schedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId, Date, DayOfWeek, ClassId, SubjectId, StartTime, EndTime")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
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

            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", schedule.ClassId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "Name", schedule.SubjectId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleItem = await _context.Schedules
                .Include(s => s.Class)
                .Include(s => s.Subject)
                .SingleOrDefaultAsync(m => m.ScheduleId == id);
            if (scheduleItem == null)
            {
                return NotFound();
            }

            return View(scheduleItem);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleItem = await _context.Schedules.SingleOrDefaultAsync(m => m.ScheduleId == id);
            _context.Schedules.Remove(scheduleItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }

        private static IQueryable<Schedule> Sort_Search(
     IQueryable<Schedule> schedules,
     ScheduleSortState sortOrder,
     string className,
     string subjectName)
        {
            // Фильтрация по названию класса, если передано значение
            if (!string.IsNullOrEmpty(className))
            {
                schedules = schedules.Where(s => s.Class.Name.Contains(className));
            }

            // Фильтрация по названию предмета, если передано значение
            if (!string.IsNullOrEmpty(subjectName))
            {
                schedules = schedules.Where(s => s.Subject.Name.Contains(subjectName));
            }

            // Сортировка
            switch (sortOrder)
            {
                case ScheduleSortState.ClassNameAsc:
                    schedules = schedules.OrderBy(s => s.Class.Name);
                    break;
                case ScheduleSortState.ClassNameDesc:
                    schedules = schedules.OrderByDescending(s => s.Class.Name);
                    break;
                case ScheduleSortState.SubjectNameAsc:
                    schedules = schedules.OrderBy(s => s.Subject.Name);
                    break;
                case ScheduleSortState.SubjectNameDesc:
                    schedules = schedules.OrderByDescending(s => s.Subject.Name);
                    break;
                default:
                    break;
            }

            return schedules;
        }
    }
}
