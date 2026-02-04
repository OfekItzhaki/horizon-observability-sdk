using Horizon.Platform.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Add Horizon Observability SDK
builder.Services.AddHorizonObservability(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Map Health Checks
app.MapHorizonHealthChecks();

// 3. Enable Observability Middleware
app.UseHorizonObservability();

app.UseAuthorization();

app.MapControllers();

app.Run();
