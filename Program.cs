using ATS_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom Services
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<ResumeParserService>();
builder.Services.AddScoped<JobDescriptionParserService>();
builder.Services.AddScoped<AtsAnalysisService>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<ResumeRewriteService>();

// HttpClient
builder.Services.AddHttpClient();

// CORS
var frontendUrl =
    builder.Configuration["FRONTEND_URL"]
    ?? builder.Configuration["FrontendUrl"]
    ?? "http://localhost:3000";

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy",
        policy =>
        {
            policy.WithOrigins(frontendUrl)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Swagger only in Development
   app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();