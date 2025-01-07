using BL.Interfaces;
using BL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PeerTutoringNetwork.Viewmodels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerTutoringNetwork.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            return View(subjects);
        }

        // GET: Subjects/Create
        public async Task<IActionResult> Create()
        {
            // Hardcoded users until UserService is implemented
            var users = new List<User>
            {
                new User { UserId = 15, Username = "MainUser" }
            };

            ViewBag.Users = new SelectList(users, "UserId", "Username");
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectVM subjectVM)
        {
            if (ModelState.IsValid)
            {
                // Set CreatedByUserId to hardcoded UserId (15)
                subjectVM.CreatedByUserId = 15;

                var success = await _subjectService.CreateSubjectAsync(subjectVM);
                if (success) return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown in case of failure
            var users = new List<User>
            {
                new User { UserId = 15, Username = "MainUser" }
            };

            ViewBag.Users = new SelectList(users, "UserId", "Username", subjectVM.CreatedByUserId);
            return View(subjectVM);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectService.GetSubjectByIdAsync(id.Value);
            if (subject == null) return NotFound();

            // Hardcoded users for the dropdown
            var users = new List<User>
            {
                new User { UserId = 15, Username = "MainUser" }
            };

            ViewBag.Users = new SelectList(users, "UserId", "Username", subject.CreatedByUserId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectVM subjectVM)
        {
            if (id != subjectVM.SubjectId) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _subjectService.UpdateSubjectAsync(subjectVM);
                if (success) return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown in case of failure
            var users = new List<User>
            {
                new User { UserId = 15, Username = "MainUser" }
            };

            ViewBag.Users = new SelectList(users, "UserId", "Username", subjectVM.CreatedByUserId);
            return View(subjectVM);
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectService.GetSubjectByIdAsync(id.Value);
            if (subject == null) return NotFound();

            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _subjectService.DeleteSubjectAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectService.GetSubjectByIdAsync(id.Value);
            if (subject == null) return NotFound();

            return View(subject);
        }
    }
}
