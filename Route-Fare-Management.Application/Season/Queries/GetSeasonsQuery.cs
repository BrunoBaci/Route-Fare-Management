using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Route_Fare_Management.Application.Season.Queries
{
    public record GetSeasonsQuery(int? Year = null) : IRequest<List<SeasonDto>>;

}
