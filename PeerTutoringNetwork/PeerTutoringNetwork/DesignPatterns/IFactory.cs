using BL.Models;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.DesignPatterns
{
    public interface IFactory<TModel, TViewModel>
    {
        TViewModel CreateVM(TModel value);
        TModel CreateModel(TViewModel value);
    }

    public class SubjectFactory : IFactory<Subject, SubjectVM>
    {
        public Subject CreateModel(SubjectVM value)
        {
            return new Subject
            {
                SubjectId = value.SubjectId,
                SubjectName = value.SubjectName,
                Description = value.Description,
                CreatedByUserId = value.CreatedByUserId
            };
        }

        public SubjectVM CreateVM(Subject value)
        {
            SubjectVM subjectVM = new SubjectVM
            {
                SubjectId = value.SubjectId,
                SubjectName = value.SubjectName,
                CreatedByUserId = value.CreatedByUserId,
                Description = value.Description,
                CreatedByUsername = value.CreatedByUser?.Username
            };

            return subjectVM;
        }
    }

    public class ReservationFactory : IFactory<AppointmentReservation, ReservationVM>
    {
        public ReservationVM CreateVM(AppointmentReservation model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return new ReservationVM
            {
                ReservationId = model.ReservationId,
                AppointmentId = model.AppointmentId,
                StudentId = model.Student.UserId,
                StudentName = model.Student?.Username ?? string.Empty, 
                MentorUsername = model.Appointment?.Mentor?.Username ?? string.Empty,
                SubjectName = model.Appointment?.Subject?.SubjectName ?? string.Empty,
                AppointmentDetails = $"Student: {model.Student?.FirstName} - {model.Student?.LastName}" ?? string.Empty, 
                ReservationTime = model.ReservationTime ?? DateTime.Now,
                AppointmentDate = model.Appointment?.AppointmentDate ?? DateTime.Now,
                IsReservation = true
            };
        }

        public AppointmentReservation CreateModel(ReservationVM viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new AppointmentReservation
            {
                ReservationId = viewModel.ReservationId,
                AppointmentId = viewModel.AppointmentId,
                StudentId = viewModel.StudentId, 
                ReservationTime = viewModel.ReservationTime
            };
        }
    }
}
