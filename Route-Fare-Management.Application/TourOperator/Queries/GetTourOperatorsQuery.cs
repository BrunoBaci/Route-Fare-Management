using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.TourOperator.DTOs;

namespace Route_Fare_Management.Application.TourOperator.Queries
{
    public sealed record GetTourOperatorsQuery(
        bool? ActiveOnly = true) : IRequest<List<TourOperatorDto>>;

}
