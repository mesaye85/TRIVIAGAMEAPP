using System;
using System.Threading.Tasks;
using TriviaGameApp.Models;
using Microsoft.EntityFrameworkCore;

namespace TriviaGameApp.Services
{
    public class UserService
    {
        private readonly TriviaDBContext _context;

        public UserService(TriviaDBContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(string id, User userIn)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Email = userIn.Email;
                user.Password = userIn.Password;
                // Set other properties here as needed
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(User userIn)
        {
            _context.Users.Remove(userIn);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
