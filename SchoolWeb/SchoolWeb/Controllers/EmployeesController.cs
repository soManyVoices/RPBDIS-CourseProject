using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.EmployeesViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public EmployeesController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Employees
        [SetToSession("Employee")]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 264)]
        public async Task<IActionResult> Index(FilterEmployeeViewModel employeeFilter, EmployeeSortState sortOrder = EmployeeSortState.No, int page = 1)
        {
            if (employeeFilter.EmployeeFirstName == null || employeeFilter.EmployeeLastName == null || employeeFilter.EmployeeMiddleName == null || 
                employeeFilter.PositionName == null)
            {
                if (HttpContext != null)
                {
                    var sessionEmployee = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Employee");
                    if (sessionEmployee != null)
                    {
                        employeeFilter = Transformations.DictionaryToObject<FilterEmployeeViewModel>(sessionEmployee);
                    }
                }
            }

            IQueryable<Employee> employeesQuery = _context.Employees.Include(e => e.Position);

            employeesQuery = SortAndFilter(
                employeesQuery,
                sortOrder,
                employeeFilter.EmployeeFirstName ?? "",
                employeeFilter.EmployeeLastName ?? "",
                employeeFilter.EmployeeMiddleName ?? "",
                employeeFilter.PositionName ?? ""
            );

            var count = await employeesQuery.CountAsync();
            employeesQuery = employeesQuery.Skip((page - 1) * pageSize).Take(pageSize);

            var employeesList = await employeesQuery.ToListAsync();
            var positions = await _context.Positions.ToListAsync();
            var positionNames = positions.Select(p => p.Name).ToList();

            EmployeeViewModel employeeViewModel = new()
            {
                Employees = employeesList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new EmployeeSortViewModel(sortOrder),
                FilterEmployeeViewModel = employeeFilter,
                SelectedPositionTypeList = new SelectList(positionNames)
            };

            return View(employeeViewModel);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .SingleOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("EmployeeId, FirstName, LastName, MiddleName, PositionId")] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.SingleOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "Name", employee.PositionId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId, FirstName, LastName, MiddleName, PositionId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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

            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "Name", employee.PositionId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Position)
                .SingleOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(m => m.EmployeeId == id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        private static IQueryable<Employee> SortAndFilter(
            IQueryable<Employee> employees,
            EmployeeSortState sortOrder,
            string firstName,
            string lastName,
            string middleName,
            string positionName)
        {
            // Фильтрация
            if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName) || !string.IsNullOrWhiteSpace(middleName) || !string.IsNullOrWhiteSpace(positionName))
            {
                employees = employees.Where(e =>
                    (string.IsNullOrWhiteSpace(firstName) || e.FirstName.Contains(firstName)) &&
                    (string.IsNullOrWhiteSpace(lastName) || e.LastName.Contains(lastName)) &&
                    (string.IsNullOrWhiteSpace(middleName) || e.MiddleName.Contains(middleName)) &&
                    (string.IsNullOrWhiteSpace(positionName) || e.Position.Name.Contains(positionName)));
            }

            // Сортировка
            switch (sortOrder)
            {
                case EmployeeSortState.EmployeeFirstNameAsc:
                    employees = employees.OrderBy(e => e.FirstName);
                    break;
                case EmployeeSortState.EmployeeFirstNameDesc:
                    employees = employees.OrderByDescending(e => e.FirstName);
                    break;
                case EmployeeSortState.EmployeeLastNameAsc:
                    employees = employees.OrderBy(e => e.LastName);
                    break;
                case EmployeeSortState.EmployeeLastNameDesc:
                    employees = employees.OrderByDescending(e => e.LastName);
                    break;
                case EmployeeSortState.PositionNameAsc:
                    employees = employees.OrderBy(e => e.Position.Name);
                    break;
                case EmployeeSortState.PositionNameDesc:
                    employees = employees.OrderByDescending(e => e.Position.Name);
                    break;
                default:
                    break;
            }

            return employees;
        }
    }
}
