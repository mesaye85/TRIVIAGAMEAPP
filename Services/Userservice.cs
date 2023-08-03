using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using TriviaGameApp.Models;
namespace TriviaGameApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("TriviaGame");
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task UpdateAsync(string id, User userIn)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _users.ReplaceOneAsync(filter, userIn);
        }

        public async Task RemoveAsync(User userIn)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userIn.Id);
            await _users.DeleteOneAsync(filter);
        }

        public async Task RemoveAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _users.DeleteOneAsync(filter);
        }
    }
}
