using LeaderboardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
// Name: Kristiin Tribbeck
// Student ID: 30045325
// Date:16/11/2022
namespace LeaderboardAPI.Services
{
    public class LeaderboardService
    {
        private readonly IMongoCollection<Player> _statsCollection;

        public LeaderboardService(
            IOptions<LeaderboardDatabaseSettings> leaderboardDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                leaderboardDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                leaderboardDatabaseSettings.Value.DatabaseName);

            _statsCollection = mongoDatabase.GetCollection<Player>(
                leaderboardDatabaseSettings.Value.LeaderboardCollectionName);
        }
        public async Task<List<Player>> GetAsync() =>
            await _statsCollection.Find(_ => true).ToListAsync();

        public async Task<Player?> GetAsync(string id) =>
            await _statsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Player newPlayer) =>
            await _statsCollection.InsertOneAsync(newPlayer);

        public async Task UpdateAsync(string id, Player updatedPlayer) =>
            await _statsCollection.ReplaceOneAsync(x => x.Id == id, updatedPlayer);

        public async Task RemoveAsync(string id) =>
            await _statsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
