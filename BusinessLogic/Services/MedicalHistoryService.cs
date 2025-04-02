using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class MedicalHistoryService : BaseService<MedicalHistory, int, MedicalHistoryViewModel>, IMedicalHistoryService
{
    public MedicalHistoryService(IBaseRepository<MedicalHistory, int, MedicalHistoryViewModel> repository) : base(repository)
    {
    }

    public async Task AddMedicalHistoryAsync(MedicalHistory medicalHistory)
    {
        _repository.AddAsync(medicalHistory);
        await _repository.SaveChangesAsync("system", false);
    }
}