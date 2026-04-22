using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Route_Fare_Management.Application.Season.Queries
{
    public record GetSeasonByIdQuery(Guid Id) : IRequest<SeasonDto>;

}
