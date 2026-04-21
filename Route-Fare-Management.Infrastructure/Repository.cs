using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Route_Fare_Management.Domain;
using MediatR;
using System.Threading;
using Route_Fare_Management.Application.Interfaces;

namespace Route_Fare_Management.Infrastructure
{
    public class Repository : IRepository
    {   
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a user by searching his email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User> GetUserAsync(string email, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                   u => u.Email == email.ToLowerInvariant() && u.IsActive,
                   token);
            return user;
        }

        public async Task<int> AddUserAsync(User user, CancellationToken token)
        {
            _context.Users.Add(user);
           return await _context.SaveChangesAsync(token);
        }
    }
}
