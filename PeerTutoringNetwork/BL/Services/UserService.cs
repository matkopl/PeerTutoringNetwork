using BL.Models;

namespace BL.Services;

public interface IUserService 
{
    void CreateUser(User NewUser);
    void UpdateUser(int id, User UpdatedUser);
    void DeleteUser(int id);
    User GetUser(int id);
}
public class UserService : IUserService
{
    public void CreateUser(User NewUser)
    {
        throw new NotImplementedException();
    }

    public void UpdateUser(int id, User UpdatedUser)
    {
        throw new NotImplementedException();
    }

    public void DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public User GetUser(int id)
    {
        throw new NotImplementedException();
    }
}