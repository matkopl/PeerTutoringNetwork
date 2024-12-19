using BL.Models;

namespace BL.Services;

public interface IReservationService 
{
    void CreateReservation(AppointmentReservation NewReservation);
    void UpdateReservation(int id, AppointmentReservation UpdatedReservation);
    void DeleteReservation(int id);
    AppointmentReservation GetReservation(int id);
}
public class ReservationService : IReservationService
{
    public void CreateReservation(AppointmentReservation NewReservation)
    {
        throw new NotImplementedException();
    }

    public void UpdateReservation(int id, AppointmentReservation UpdatedReservation)
    {
        throw new NotImplementedException();
    }

    public void DeleteReservation(int id)
    {
        throw new NotImplementedException();
    }

    public AppointmentReservation GetReservation(int id)
    {
        throw new NotImplementedException();
    }
}