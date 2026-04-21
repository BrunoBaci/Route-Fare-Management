using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.Interfaces
{
    public interface IExportService
    {
        Task<string> ExportPricingToExcelAsync(
            Guid tourOperatorId,
            Guid seasonId,
            string connectionId,
            CancellationToken cancellationToken = default);
    }
}
