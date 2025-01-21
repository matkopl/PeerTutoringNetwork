using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;
using PeerTutoringNetwork.DesignPatterns;

namespace PeerTutoringNetwork.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly IRepository<Subject> _subjectRepository;
        private readonly IFactory<Subject, SubjectVM> _subjectFactory;
        private readonly IUtils _utils;

        public SubjectsController(
            IRepository<Subject> subjectRepository,
            IFactory<Subject, SubjectVM> subjectFactory,
            IUtils utils)
        {
            _subjectRepository = subjectRepository;
            _subjectFactory = subjectFactory;
            _utils = utils;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            var subjectsVM = subjects.Select(s => _subjectFactory.CreateVM(s)).ToList();
            return View(subjectsVM);
        }

        // GET: Subjects/Create
        public async Task<IActionResult> Create()
        {
            var users = await _utils.GetUsersAsync();
            ViewData["CreatedByUserId"] = new SelectList(users, "UserId", "Username");
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectName,Description,CreatedByUserId")] SubjectVM subjectVM)
        {
            if (!ModelState.IsValid)
            {
                var users = await _utils.GetUsersAsync();
                ViewData["CreatedByUserId"] = new SelectList(users, "UserId", "Username", subjectVM.CreatedByUserId);
                return View(subjectVM);
            }

            var subject = _subjectFactory.CreateModel(subjectVM);
            await _subjectRepository.AddAsync(subject);
            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            var subjectVM = _subjectFactory.CreateVM(subject);
            var users = await _utils.GetUsersAsync();
            ViewData["CreatedByUserId"] = new SelectList(users, "UserId", "Username", subjectVM.CreatedByUserId);
            return View(subjectVM);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubjectId,SubjectName,Description,CreatedByUserId")] SubjectVM subjectVM)
        {
            if (id != subjectVM.SubjectId)
                return NotFound(); 

            var existingSubject = await _subjectRepository.GetByIdAsync(id);
            if (existingSubject == null)
                return NotFound(); 

            if (!ModelState.IsValid)
            {
                var users = await _utils.GetUsersAsync();
                ViewData["CreatedByUserId"] = new SelectList(users, "UserId", "Username", subjectVM.CreatedByUserId);
                return View(subjectVM);
            }

            var subject = _subjectFactory.CreateModel(subjectVM);
            await _subjectRepository.UpdateAsync(subject);
            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            var subjectVM = _subjectFactory.CreateVM(subject);
            return View(subjectVM);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _subjectRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            var subjectVM = _subjectFactory.CreateVM(subject);
            return View(subjectVM);
        }
    }
}
