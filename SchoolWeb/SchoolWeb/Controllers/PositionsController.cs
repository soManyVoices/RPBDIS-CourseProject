using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolWeb.DataLayer;
using SchoolWeb.DataLayer.Data;
using SchoolWeb.DataLayer.Models;
using SchoolWeb.Infrastructure.Filters;
using SchoolWeb.Infrastructure;
using SchoolWeb.ViewModels;
using SchoolWeb.ViewModels.PositionsViewModel;
using SchoolWeb.ViewModels.SortStates;
using SchoolWeb.ViewModels.SortViewModels;

namespace SchoolWeb.Controllers
{
    public class PositionsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly int pageSize = 10;

        public PositionsController(SchoolContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Positions
        [SetToSession("Position")]
        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 264)]
        public async Task<IActionResult> Index(FilterPositionViewModel filter, PositionSortState sortOrder = PositionSortState.No, int page = 1)
        {
            if (filter.PositionName == null)
            {
                if (HttpContext != null)
                {
                    var sessionPosition = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Position");
                    if (sessionPosition != null)
                    {
                        filter = Transformations.DictionaryToObject<FilterPositionViewModel>(sessionPosition);
                    }
                }
            }

            IQueryable<Position> positions = _context.Positions;

            positions = Sort_Search(
               positions,
               sortOrder,
               filter.PositionName ?? ""
           );

            var count = await positions.CountAsync();
            positions = positions.Skip((page - 1) * pageSize).Take(pageSize);

            var positionList = await positions.ToListAsync();
            var allPositionNames = _context.Positions.Select(p => p.Name).Distinct().ToList();

            PositionViewModel viewModel = new()
            {
                Positions = positionList,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new PositionSortViewModel(sortOrder),
                FilterPositionViewModel = filter,
                SelectedPositionNameList = new SelectList(allPositionNames),
            };
            return View(viewModel);
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .SingleOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // GET: Positions/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Positions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("PositionId, Name, Description, Salary")] Position position)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Add(position);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Positions/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.SingleOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }
            return View(position);
        }

        // POST: Positions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PositionId, Name, Description, Salary")] Position position)
        {
            if (id != position.PositionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(position);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(position.PositionId))
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
            return View(position);
        }

        // GET: Positions/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .SingleOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Positions.SingleOrDefaultAsync(m => m.PositionId == id);
            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.PositionId == id);
        }

        private static IQueryable<Position> Sort_Search(
            IQueryable<Position> positions,
            PositionSortState sortOrder,
            string positionName)
        {
            Console.WriteLine($"Filtering by position name: {positionName}");
            // Фильтрация только по имени должности
            if (!string.IsNullOrWhiteSpace(positionName))
            {
                positions = positions.Where(p => p.Name.Contains(positionName));
            }

            // Сортировка
            switch (sortOrder)
            {
                case PositionSortState.PositionNameAsc:
                    positions = positions.OrderBy(p => p.Name);
                    break;
                case PositionSortState.PositionNameDesc:
                    positions = positions.OrderByDescending(p => p.Name);
                    break;
                default:
                    break;
            }

            return positions;
        }
    }
}
