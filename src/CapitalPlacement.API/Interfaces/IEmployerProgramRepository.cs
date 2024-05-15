using CapitalPlacement.API.Models;

namespace CapitalPlacement.API.Interfaces
{
    public interface IEmployerProgramRepository
    {
        Task CreateProgramAsync(EmployerProgram program);
        Task<EmployerProgram?> GetProgramByIdAsync(string id, string employerId);
        Task UpdateProgramAsync(EmployerProgram program);
    }
}
