using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.Export
{
    public class ExportPricesCommandHandler : IRequestHandler<ExportPricesCommand, string>
    {
        private readonly IExportService _exportService;
        private readonly ICurrentUserService _currentUser;

        public ExportPricesCommandHandler(IExportService exportService, ICurrentUserService currentUserService)        
        {
            _exportService = exportService;
            _currentUser = currentUserService;
        }

        public async Task<string> Handle(ExportPricesCommand request, CancellationToken token)
        {
            // Allow operators to only export their own data
            //if (!_currentUser.IsAdmin &&
            //    _currentUser.TourOperatorId != request.TourOperatorId)
            //    throw new ForbiddenAccessException();

            var fileName = await _exportService.ExportPricingToExcelAsync(
                request.TourOperatorId,
                request.SeasonId,
                request.ConnectionId,  
                token);    

            return fileName;
        }
    }
}
