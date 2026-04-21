using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public class User : BaseEntity
    {
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public UserRole Role { get; private set; }
        public Guid? TourOperatorId { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Navigation property
        public TourOperator? TourOperator { get; private set; }

        // Required by EF Core
        private User() { }

        public static User CreateAdmin(
            string email, string passwordHash, string firstName, string lastName)
            => new()
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Role = UserRole.Admin
            };

        public static User CreateTourOperatorMember(
            string email, string passwordHash,
            string firstName, string lastName, Guid tourOperatorId)
            => new()
            {
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Role = UserRole.TourOperatorMember,
                TourOperatorId = tourOperatorId
            };

        public void UpdatePassword(string newHash)
        {
            PasswordHash = newHash;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }
    }
}
