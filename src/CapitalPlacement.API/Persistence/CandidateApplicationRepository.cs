using CapitalPlacement.API.Interfaces;
using CapitalPlacement.API.Models;
using Microsoft.Azure.Cosmos;

namespace CapitalPlacement.API.Persistence
{
    public class CandidateApplicationRepository : ICandidateApplicationRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger<CandidateApplicationRepository> _logger;
        public CandidateApplicationRepository(CosmosClient cosmosClient, string databaseName, string containerName,
            ILogger<CandidateApplicationRepository> logger)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _container = _cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }

        public async Task AddAsync(CandidateApplication candidateApplication)
        {
            await _container.UpsertItemAsync(candidateApplication, new PartitionKey(candidateApplication.programid));
        }

        public async Task<CandidateApplication?> GetByEmailAsync(string email)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.Email = @Email")
            .WithParameter("@Email", email);
            var iterator = _container.GetItemQueryIterator<CandidateApplication>(query);
            var candidateApplications = await iterator.ReadNextAsync();

            return candidateApplications.FirstOrDefault();
        }
    }
}
