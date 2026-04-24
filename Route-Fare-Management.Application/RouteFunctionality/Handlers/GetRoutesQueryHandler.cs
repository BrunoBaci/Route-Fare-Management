using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.RouteFunctionality.Queries;

namespace Route_Fare_Management.Application.RouteFunctionality.Handlers
{
    public class GetRoutesQueryHandler
        : IRequestHandler<GetRoutesQuery, List<RouteDto>>
    {
        private readonly IRepository _repository;

        public GetRoutesQueryHandler(IRepository repository)
            => _repository = repository;

        public async Task<List<RouteDto>> Handle(
            GetRoutesQuery request, CancellationToken cancellationToken)
        {
            var routes = await _repository.GetRoutesAsync(request.Search, cancellationToken);

            return routes.Select(r => r.ToDto()).ToList(); 
        }
            
        }
}
