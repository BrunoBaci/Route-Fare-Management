using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine(" Route Fare Progress Listener \n");

//input jobID
Console.Write("Enter JobId: ");
var jobId = Console.ReadLine();


var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7112/hubs/export")
    .WithAutomaticReconnect()
    .Build();


connection.On<string>("ReceiveConnectionId", id =>
{
    Console.WriteLine($"Server gave ConnectionId: {id}");
    jobId = id;
});

connection.On<int, string>("ProgressUpdate", (progress, message) =>
{
    var bar = ProgressBar(progress);

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($" {bar} {progress,3}%  {message}");
    Console.ResetColor();
});


connection.On<string>("ExportComplete", fileUrl =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine();
    Console.WriteLine(" Export complete!");
    Console.WriteLine($" File: {fileUrl}");
    Console.ResetColor();
});


connection.On<string>("ExportError", error =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    Console.WriteLine($" Export failed: {error}");
    Console.ResetColor();
});

try
{
    await connection.StartAsync();
    Console.WriteLine(" Connected to SignalR hub");

    await connection.InvokeAsync("JoinExportJob", jobId);
    Console.WriteLine($" Joined job: {jobId}");

}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($" Connection failed: {ex.Message}");
    Console.ResetColor();
    return;
}

// ── Keep alive ────────────────────────────────────────────────
await Task.Delay(-1);

// ── Helper ────────────────────────────────────────────────────
static string ProgressBar(int progress)
{
    const int width = 20;
    var filled = (int)Math.Round(progress / 100.0 * width);

    return $"[{new string('█', filled)}{new string('░', width - filled)}]";
}