using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Route_Fare_Management.Application
{
    public record LoginCommand(
        string Email,
        string Password) : IRequest<AuthResponseDto>;
}
