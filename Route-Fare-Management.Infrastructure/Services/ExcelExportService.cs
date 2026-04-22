using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;
using Microsoft.EntityFrameworkCore;

namespace Route_Fare_Management.Infrastructure.Services
{

    public sealed class ExcelExportService : IExportService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notify;
        private readonly ILogger<ExcelExportService> _logger;
        private const string ExportsFolder = "exports";

        public ExcelExportService(
            AppDbContext context,
            INotificationService notify,
            ILogger<ExcelExportService> logger)
        {
            _context = context;
            _notify = notify;
            _logger = logger;
            Directory.CreateDirectory(ExportsFolder);
        }

        public async Task<string> ExportPricingToExcelAsync(
            Guid tourOperatorId,
            Guid seasonId,
            string connectionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _notify.SendProgressAsync(connectionId, 5,
                    "Loading operator and season data…", cancellationToken);

                var op = await _context.TourOperators
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == tourOperatorId, cancellationToken)
                    ?? throw new InvalidOperationException(
                        $"Tour operator '{tourOperatorId}' not found.");

                var season = await _context.Seasons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == seasonId, cancellationToken)
                    ?? throw new InvalidOperationException(
                        $"Season '{seasonId}' not found.");

                await _notify.SendProgressAsync(connectionId, 20,
                    "Loading pricing entries…", cancellationToken);

                // Load all pricing entries for this operator+season, ordered for both sheets
                var entries = await _context.PricingEntries
                    .Include(p => p.TourOperatorRoute)
                        .ThenInclude(tor => tor.Route)
                    .AsNoTracking()
                    .Where(p =>
                        p.TourOperatorRoute.TourOperatorId == tourOperatorId &&
                        p.TourOperatorRoute.SeasonId == seasonId)
                    .OrderBy(p => p.TourOperatorRoute.Route.Origin)
                    .ThenBy(p => p.Date)
                    .ToListAsync(cancellationToken);

                await _notify.SendProgressAsync(connectionId, 45,
                    $"Generating workbook ({entries.Count} rows)…", cancellationToken);

                using var wb = new XLWorkbook();

                BuildSummarySheet(wb, op, season, entries);

                await _notify.SendProgressAsync(connectionId, 70,
                    "Building detail sheet…", cancellationToken);

                BuildDetailSheet(wb, op, season, entries);

                await _notify.SendProgressAsync(connectionId, 88,
                    "Saving file…", cancellationToken);

                var fileName = $"pricing_{op.Code}_{season.Type}_{season.Year}" +
                               $"_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(ExportsFolder, fileName);
                wb.SaveAs(filePath);

                var downloadUrl = $"/api/export/download/{fileName}";

                await _notify.SendProgressAsync(connectionId, 100,
                    "Export complete!", cancellationToken);
                await _notify.SendCompletedAsync(connectionId, downloadUrl,
                    cancellationToken);

                _logger.LogInformation(
                    "Export complete | operator={OperatorId} season={SeasonId} file={File}",
                    tourOperatorId, seasonId, fileName);

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Export failed | operator={OperatorId} season={SeasonId}",
                    tourOperatorId, seasonId);
                await _notify.SendErrorAsync(connectionId, ex.Message, cancellationToken);
                throw;
            }
        }

        // Sheet 1

        private static void BuildSummarySheet(
            IXLWorkbook wb,
            TourOperator op,
            Season season,
            List<PricingEntry> entries)
        {
            var ws = wb.Worksheets.Add("Summary");

            // Title
            var titleCell = ws.Cell("A1");
            titleCell.Value = $"{op.Name} — {season.Type} {season.Year} Pricing Summary";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 14;
            titleCell.Style.Font.FontColor = XLColor.DarkBlue;
            ws.Range("A1:H1").Merge();

            // Subtitle
            ws.Cell("A2").Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Range("A2:H2").Merge();

            // Column headers
            string[] headers =
            {
            "Route", "Total Days",
            "Avg Economy Price", "Avg Business Price",
            "Total Economy Seats", "Total Business Seats",
            "Projected Economy Revenue", "Projected Business Revenue"
        };

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A5F");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            }

            // Group and aggregate by route
            var summary = entries
                .GroupBy(e => e.TourOperatorRoute.Route)
                .Select(g => new
                {
                    RouteName = $"{g.Key.Origin} → {g.Key.Destination}",
                    TotalDays = g.Count(),
                    AvgEconomy = g.Where(e => e.EconomyPrice.HasValue)
                                     .Select(e => e.EconomyPrice!.Value)
                                     .DefaultIfEmpty(0m).Average(),
                    AvgBusiness = g.Where(e => e.BusinessPrice.HasValue)
                                     .Select(e => e.BusinessPrice!.Value)
                                     .DefaultIfEmpty(0m).Average(),
                    TotalEcoSeats = g.Sum(e => e.EconomySeats ?? 0),
                    TotalBizSeats = g.Sum(e => e.BusinessSeats ?? 0),
                    EcoRevenue = g.Sum(e =>
                        (e.EconomyPrice ?? 0m) * (e.EconomySeats ?? 0)),
                    BizRevenue = g.Sum(e =>
                        (e.BusinessPrice ?? 0m) * (e.BusinessSeats ?? 0))
                })
                .ToList();

            for (var r = 0; r < summary.Count; r++)
            {
                var row = r + 5;
                var s = summary[r];

                ws.Cell(row, 1).Value = s.RouteName;
                ws.Cell(row, 2).Value = s.TotalDays;
                ws.Cell(row, 3).Value = Math.Round(s.AvgEconomy, 2);
                ws.Cell(row, 4).Value = Math.Round(s.AvgBusiness, 2);
                ws.Cell(row, 5).Value = s.TotalEcoSeats;
                ws.Cell(row, 6).Value = s.TotalBizSeats;
                ws.Cell(row, 7).Value = Math.Round(s.EcoRevenue, 2);
                ws.Cell(row, 8).Value = Math.Round(s.BizRevenue, 2);

                const string currencyFormat = "#,##0.00";
                ws.Cell(row, 3).Style.NumberFormat.Format = currencyFormat;
                ws.Cell(row, 4).Style.NumberFormat.Format = currencyFormat;
                ws.Cell(row, 7).Style.NumberFormat.Format = currencyFormat;
                ws.Cell(row, 8).Style.NumberFormat.Format = currencyFormat;

                if (r % 2 == 0)
                    ws.Range(row, 1, row, 8).Style
                        .Fill.BackgroundColor = XLColor.FromHtml("#F0F4FA");
            }

            // Totals row
            if (summary.Count > 0)
            {
                var totRow = summary.Count + 5;
                ws.Cell(totRow, 1).Value = "TOTAL";
                ws.Cell(totRow, 5).Value = summary.Sum(x => x.TotalEcoSeats);
                ws.Cell(totRow, 6).Value = summary.Sum(x => x.TotalBizSeats);
                ws.Cell(totRow, 7).Value = Math.Round(summary.Sum(x => x.EcoRevenue), 2);
                ws.Cell(totRow, 8).Value = Math.Round(summary.Sum(x => x.BizRevenue), 2);
                ws.Cell(totRow, 7).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(totRow, 8).Style.NumberFormat.Format = "#,##0.00";
                ws.Range(totRow, 1, totRow, 8).Style.Font.Bold = true;
                ws.Range(totRow, 1, totRow, 8).Style
                    .Fill.BackgroundColor = XLColor.FromHtml("#D6E4F0");
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(4);
        }

        // Sheet 2: 
        private static void BuildDetailSheet(
            IXLWorkbook wb,
            TourOperator op,
            Season season,
            List<PricingEntry> entries)
        {
            var ws = wb.Worksheets.Add("Detailed");

            var titleCell = ws.Cell("A1");
            titleCell.Value = $"{op.Name} — {season.Type} {season.Year} Daily Pricing";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 14;
            titleCell.Style.Font.FontColor = XLColor.DarkBlue;
            ws.Range("A1:I1").Merge();

            ws.Cell("A2").Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Range("A2:I2").Merge();

            string[] headers =
            {
            "Date", "Day of Week", "Route",
            "Economy Price",   "Economy Seats",
            "Business Price",  "Business Seats",
            "First Class Price", "First Class Seats"
        };

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E6DA4");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            }

            for (var r = 0; r < entries.Count; r++)
            {
                var row = r + 5;
                var entry = entries[r];
                var route = entry.TourOperatorRoute.Route;

                ws.Cell(row, 1).Value = entry.Date.ToString("yyyy-MM-dd");
                ws.Cell(row, 2).Value = entry.DayOfWeek.ToString();
                ws.Cell(row, 3).Value = $"{route.Origin} → {route.Destination}";

                // Only set cell value if the data exists (leave empty for null)
                if (entry.EconomyPrice.HasValue)
                    ws.Cell(row, 4).Value = (double)entry.EconomyPrice.Value;
                if (entry.EconomySeats.HasValue)
                    ws.Cell(row, 5).Value = entry.EconomySeats.Value;
                if (entry.BusinessPrice.HasValue)
                    ws.Cell(row, 6).Value = (double)entry.BusinessPrice.Value;
                if (entry.BusinessSeats.HasValue)
                    ws.Cell(row, 7).Value = entry.BusinessSeats.Value;
                if (entry.FirstClassPrice.HasValue)
                    ws.Cell(row, 8).Value = (double)entry.FirstClassPrice.Value;
                if (entry.FirstClassSeats.HasValue)
                    ws.Cell(row, 9).Value = entry.FirstClassSeats.Value;

                // Currency format on price columns
                foreach (var col in new[] { 4, 6, 8 })
                    ws.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";

                // Zebra striping
                if (r % 2 == 0)
                    ws.Range(row, 1, row, 9).Style
                        .Fill.BackgroundColor = XLColor.FromHtml("#F7F9FC");

                // Weekend highlight
                if (entry.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    ws.Cell(row, 2).Style.Font.FontColor = XLColor.DarkRed;
                    ws.Cell(row, 2).Style.Font.Bold = true;
                }
            }

            ws.Column(1).Width = 13;
            ws.Column(2).Width = 14;
            ws.Column(3).Width = 30;
            ws.Columns(4, 9).AdjustToContents();
            ws.SheetView.FreezeRows(4);
            ws.Range(4, 1, 4, 9).SetAutoFilter();
        }
    }
}
