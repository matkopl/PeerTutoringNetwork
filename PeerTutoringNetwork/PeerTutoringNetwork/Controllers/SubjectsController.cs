using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BL.Models;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;

        public SubjectsController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.CreatedByUser)
                .Select(s => new SubjectVM
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description,
                    CreatedByUsername = s.CreatedByUser.Username,
                    CreatedByUserId = s.CreatedByUserId
                }).ToListAsync();

            return View(subjects);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectName,Description,CreatedByUserId")] SubjectVM subjectVM)
        {
            if (ModelState.IsValid)
            {
                var subject = new Subject
                {
                    SubjectName = subjectVM.SubjectName,
                    Description = subjectVM.Description,
                    CreatedByUserId = subjectVM.CreatedByUserId
                };

                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "UserId", "Username", subjectVM.CreatedByUserId);
            return View(subjectVM);
        }

        // GET: Subjects/EditAction/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();

            var subjectVM = new SubjectVM
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                CreatedByUserId = subject.CreatedByUserId
            };

            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "UserId", "Username", subject.CreatedByUserId);
            return View(subjectVM);
        }

        // POST: Subjects/EditAction/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubjectId,SubjectName,Description,CreatedByUserId")] SubjectVM subjectVM)
        {
            if (id != subjectVM.SubjectId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var subject = new Subject
                    {
                        SubjectId = subjectVM.SubjectId,
                        SubjectName = subjectVM.SubjectName,
                        Description = subjectVM.Description,
                        CreatedByUserId = subjectVM.CreatedByUserId
                    };

                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subjectVM.SubjectId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "UserId", "Username", subjectVM.CreatedByUserId);
            return View(subjectVM);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _context.Subjects
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null) return NotFound();

            var subjectVM = new SubjectVM
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                CreatedByUsername = subject.CreatedByUser.Username
            };

            return View(subjectVM);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.CreatedByUser)
                .FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null)
            {
                return NotFound();
            }

            var subjectVM = new SubjectVM
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                CreatedByUserId = subject.CreatedByUserId,
                CreatedByUsername = subject.CreatedByUser.Username
            };

            return View(subjectVM);
        }
    }
}
