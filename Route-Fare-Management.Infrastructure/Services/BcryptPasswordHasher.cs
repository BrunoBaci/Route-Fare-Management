using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Route_Fare_Management.Application.Interfaces;

namespace Route_Fare_Management.Infrastructure.Services
{
    /// <summary>
    /// BCrypt implementation of IPasswordHasher.
    /// Work factor 12 provides good security while keeping login latency acceptable.
    /// </summary>
    public sealed class BcryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
