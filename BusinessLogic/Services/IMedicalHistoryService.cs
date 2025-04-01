using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IMedicalHistoryService : IBaseService<MedicalHistory, int, MedicalHistoryViewModel>
{
    Task AddMedicalHistoryAsync(MedicalHistory medicalHistory);

}