using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public sealed class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object key)
            : base($"'{entityName}' with key '{key}' was not found.") { }
    }
}
