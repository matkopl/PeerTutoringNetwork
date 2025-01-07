using BL.Interfaces;
using BL.Models;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly PeerTutoringNetworkContext _context;

        public SubjectService(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectVM>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .Select(s => new SubjectVM
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    Description = s.Description
                }).ToListAsync();
        }

        public async Task<bool> CreateSubjectAsync(SubjectVM subjectVM)
        {
            const int DefaultUserId = 15;

            var subject = new Subject
            {
                SubjectName = subjectVM.SubjectName,
                Description = subjectVM.Description,
                CreatedByUserId = DefaultUserId 
            };

            _context.Subjects.Add(subject);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<SubjectVM?> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id);

            if (subject == null) return null;

            return new SubjectVM
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                Description = subject.Description
            };
        }

        public async Task<bool> UpdateSubjectAsync(SubjectVM subjectVM)
        {
            var subject = await _context.Subjects.FindAsync(subjectVM.SubjectId);
            if (subject == null) return false;

            subject.SubjectName = subjectVM.SubjectName;
            subject.Description = subjectVM.Description;

            _context.Subjects.Update(subject);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            _context.Subjects.Remove(subject);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
