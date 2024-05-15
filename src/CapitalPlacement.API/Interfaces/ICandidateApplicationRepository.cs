using CapitalPlacement.API.Models;

namespace CapitalPlacement.API.Interfaces
{
    public interface ICandidateApplicationRepository
    {
        Task AddAsync(CandidateApplication candidateApplication);

        Task<CandidateApplication?> GetByEmailAsync(string email);
    }
}
