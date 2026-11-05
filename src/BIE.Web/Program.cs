using BIE.Core;
using BIE.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Registry>(services => new Registry(services.GetRequiredService<IConfiguration>()["DatabasePath"] ?? Path.Combine(builder.Environment.ContentRootPath, "bie.db")));
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/health", () => Results.Ok(new { status = "ready" }));
app.MapGet("/api/cases", (Registry registry) => Results.Ok(registry.All()));
app.MapGet("/api/cases/{id:long}", (long id, Registry registry) => registry.All().Find(c => c.Id == id) is { } entry ? Results.Ok(entry) : Results.NotFound());
app.MapPost("/api/cases", (Entry entry, Registry registry) =>
{
    var name = entry.Name?.Trim();
    if (string.IsNullOrEmpty(name) || name.Length > 60 || entry.Days < 1 || entry.Days > 365 || !new[]
    {
        "Schleimora",
        "Orion",
        "Kepler-186f"
    }.Contains(entry.Planet) || !new[]
    {
        "Tourismus",
        "Diplomatie",
        "Arbeit"
    }.Contains(entry.Purpose))
        return Results.BadRequest(new
        {
            error = "Name, Heimatplanet, Besuchszweck und 1 bis 365 Tage Aufenthalt sind erforderlich."
        });
    var saved = registry.Add(entry with
    {
        Name = name
    });
    return Results.Created($"/api/cases/{saved.Id}", saved);
});
app.Run();
public partial class Program
{
}
