using BL.Models;
using PeerTutoringNetwork.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectVM>> GetAllSubjectsAsync();
        Task<bool> CreateSubjectAsync(SubjectVM subjectVM);
        Task<SubjectVM?> GetSubjectByIdAsync(int id);
        Task<bool> UpdateSubjectAsync(SubjectVM subjectVM);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
