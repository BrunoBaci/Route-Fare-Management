using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.TourOperator.Commands
{
    public sealed record CreateTourOperatorCommand(
        string Name,
        string Code,
        List<BookingClass> SupportedBookingClasses) : IRequest<TourOperatorDto>;

}
