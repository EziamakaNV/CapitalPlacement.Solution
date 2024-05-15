using CapitalPlacement.API.Interfaces;
using CapitalPlacement.API.Models;
using Microsoft.Azure.Cosmos;

namespace CapitalPlacement.API.Persistence
{
    public class EmployerProgramRepository : IEmployerProgramRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger<EmployerProgramRepository> _logger;

        public EmployerProgramRepository(CosmosClient cosmosClient, string databaseName, string containerName,
            ILogger<EmployerProgramRepository> logger)
        {
            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            _container = _cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }
        public async Task CreateProgramAsync(EmployerProgram program)
        {
            await _container.UpsertItemAsync(program, new PartitionKey(program.employerid));
        }

        public async Task<EmployerProgram?> GetProgramByIdAsync(string id, string employerId)
        {
            try
            {
                ItemResponse<EmployerProgram> response = await _container.ReadItemAsync<EmployerProgram>(id: id,
                     partitionKey: new PartitionKey(employerId));
                return response.Resource;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching the employer program: {message}", ex.Message);
                return null;
            }
        }

        public async Task UpdateProgramAsync(EmployerProgram program, string employerId)
        {
            await _container.UpsertItemAsync(program, new PartitionKey(employerId));
        }
    }
}
