using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();

var connectionString = System.Environment.GetEnvironmentVariable("AZURE");
Console.WriteLine("HERE IS THE PASSWORD");
Console.WriteLine(connectionString);
Console.WriteLine("----------");

builder.Services.AddDbContextPool<MiniTwitContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is a super secret key that needs to be in appsettings")),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

builder.Services.AddCors(options =>
    options.AddPolicy("localhost", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    })
);


// Setup serrlog and set the sink to elasticSearch 
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://157.245.27.14:9200")){
             AutoRegisterTemplate = true,
             AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
             CustomFormatter = new ElasticsearchJsonFormatter()
             
    }).CreateLogger();

builder.Host.UseSerilog(Log.Logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("localhost");
}

app.UseCors("localhost");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<MiniTwitContext>();
    context.Database.EnsureCreated();

}

app.UseRouting();
app.UseHttpMetrics();
app.UseMetricServer();

app.UseEndpoints(endpoints =>
    endpoints.MapMetrics()
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
