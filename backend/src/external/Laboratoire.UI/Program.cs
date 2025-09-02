using Laboratoire.Application.Utils;
using Laboratoire.Domain.Utils;
using Laboratoire.Infrastructure.DbContext;
using Laboratoire.Infrastructure.Extensions;
using Laboratoire.Web.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
string? connectionString = environment == "Development"
? builder.Configuration.GetConnectionString("DefaultConnectionDev")
: builder.Configuration.GetConnectionString("DefaultConnectionProd");

// Define as colunas da tabela de log
var columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter() },
    { "message_template", new MessageTemplateColumnWriter() },
    { "level", new LevelColumnWriter() },
    { "time_stamp", new TimestampColumnWriter() },
    { "exception", new ExceptionColumnWriter() },
    { "log_event", new LogEventSerializedColumnWriter() },
    { "user_id", new SinglePropertyColumnWriter("AuthenticatedUser")},
    { "thread_id", new SinglePropertyColumnWriter("ThreadId") }
};

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.PostgreSQL(
        connectionString: connectionString,
        tableName: "logs",
        columnOptions: columnWriters,
        needAutoCreateTable: true,
        restrictedToMinimumLevel: LogEventLevel.Warning
    )
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddSingleton<Token>();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddUtils();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.Workers, policy =>
        policy.RequireRole("admin", "recepção", "químico"));

    options.AddPolicy(Policy.All, policy =>
        policy.RequireRole("admin", "recepção", "químico", "projetista", "cliente"));
});

builder.Services.AddCors((options) =>
    {
        options.AddPolicy("DevCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        options.AddPolicy("ProdCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("https://www.labsolo.com.br", "https://labsolo.com.br")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    });


builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseRouting();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseAuthentication();
app.UseMiddleware<CatchMiddleware>();
app.UseMiddleware<LogUserEnricherMiddleware>();
app.UseAuthorization();

app.MapControllers();

// app.Run("http://0.0.0.0:5000");

app.Run();
