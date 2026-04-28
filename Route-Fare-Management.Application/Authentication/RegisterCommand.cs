using MediatR;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.Auth;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role//,
    //Guid? TourOperatorId
    ) : IRequest<AuthResponseDto>;
