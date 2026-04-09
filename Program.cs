var builder = WebApplication.CreateBuilder(args);

// ── Register services ──────────────────────────────

// This tells ASP.NET to scan for Controller classes and use them
builder.Services.AddControllers();

// OpenAPI/Swagger so you can test endpoints in the browser
builder.Services.AddOpenApi();

// CORS = Cross-Origin Resource Sharing
// Without this, Angular (running on localhost:4200) can't call our API (localhost:5xxx).
// Browsers block cross-origin requests by default for security.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular's default dev server
              .AllowAnyHeader()                     // Allow JSON content-type etc.
              .AllowAnyMethod();                    // Allow GET, POST, etc.
    });
});

var app = builder.Build();

// ── Configure the HTTP pipeline ────────────────────

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS - must be before MapControllers
app.UseCors("AllowAngular");

// This tells ASP.NET to route requests to our controller endpoints
app.MapControllers();

app.Run();
