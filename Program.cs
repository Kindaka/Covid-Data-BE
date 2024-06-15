using ODataCovid.Services;
using ODataCovid.Services.Quartz;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using ODataCovid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using NetTopologySuite.Geometries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton<IConfigManager, ConfigManager>();

builder.Services.AddDbContext<CovidContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
var modelBuilder = new ODataConventionModelBuilder();
//modelBuilder.EntityType<CountryRegion>().CollectionProperty(c => c.covidDailies);
//modelBuilder.EntityType<CovidDaily>();
//modelBuilder.EntityType<CountryRegion>().HasDynamicProperties(c => c.Location = new Point(-122.333056, 47.609722) { SRID = 4326 });
modelBuilder.EntitySet<CountryRegion>("CountryRegions").HasManyBinding(c=>c.CovidDailies, "CovidDailies");
modelBuilder.EntitySet<CovidDaily>("CovidDailies");
modelBuilder.EntitySet<Death>("Death");
modelBuilder.EntitySet<Recovered>("Recovered");
modelBuilder.EntitySet<Confirmed>("Confirmed");
modelBuilder.EntitySet<Active>("Active");

builder.Services.AddControllers().AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddCors(options =>
//    {
//        options.AddDefaultPolicy(
//            builder =>
//            {
//                builder.WithOrigins("*")
//                .AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .AllowCredentials()
//                .Build();
//            });
//    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .Build();
        });
});

builder.Services.AddHttpClient<CovidContractJob>(client =>
{
    client.BaseAddress = new Uri(Configuration.GetSection("CovidServiceUrl").Value);
});
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.AddJobAndTrigger<CovidContractJob>(builder.Configuration);
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

