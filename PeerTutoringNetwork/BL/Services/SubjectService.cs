using BL.Models;

namespace BL.Services;

public interface ISubjectService 
{
    void CreateSubject(Subject NewSubject);
    void UpdateSubject(int id, Subject UpdatedSubject);
    void DeleteSubject(int id);
    Subject GetSubject(int id);
}
public class SubjectService : ISubjectService
{
    public void CreateSubject(Subject NewSubject)
    {
        throw new NotImplementedException();
    }

    public void UpdateSubject(int id, Subject UpdatedSubject)
    {
        throw new NotImplementedException();
    }

    public void DeleteSubject(int id)
    {
        throw new NotImplementedException();
    }

    public Subject GetSubject(int id)
    {
        throw new NotImplementedException();
    }
}