using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.Season.Commands
{
    public record CreateSeasonCommand(
        int Year,
        SeasonType Type) : IRequest<SeasonDto>;

}
