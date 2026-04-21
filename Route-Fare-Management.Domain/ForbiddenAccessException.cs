using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public sealed class ForbiddenAccessException : DomainException
    {
        public ForbiddenAccessException()
            : base("You do not have permission to perform this action.") { }
    }
}
