
using Microsoft.EntityFrameworkCore;
using Propeller.DALC.Interfaces;
using Propeller.DALC.Sqlite;
using Propeller.DALC.Repositories;
using NLog.Web;
using NLog;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Globalization;
using Propeller.API.Providers;
using System.Security.Claims;
// using Propeller.Entities.DbContexts;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

// Switch log provider
// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.
// TODO: Add proper options to limit pointsof error
builder.Services
    .AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hook up the DbContext
builder.Services.AddDbContext<PropellerDbContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CustomersSQLiteConnString"]));

// Inject Repos
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<IContactsRepository, ContactsRepository>();
builder.Services.AddScoped<ICustomerStatusRepository, CustomerStatusRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

// Inject Automapper
builder.Services.AddAutoMapper(typeof(Propeller.Mappers.CustomerProfile));
builder.Services.AddAutoMapper(typeof(Propeller.Mappers.NoteProfile));
builder.Services.AddAutoMapper(typeof(Propeller.Mappers.ContactProfile));
builder.Services.AddAutoMapper(typeof(Propeller.Mappers.UserProfile));

// Localization
// NOTE: This is pretty interesting, there seems to be different approaches to how to handle the Resources files
// I usually created an empty class and added the Resource files under it, but it seems I can also set the
// resources path here and that works too, even more, I can also fully qualify te empty class
// ie Namespace.Resources and it'll allow the injector to resolve the path on it's own too (approach taken for this POC)
// builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddLocalization();

// TODO: Whats the dif? LocalizationOptions locOptions = new RequestLocalizationOptions();
RequestLocalizationOptions locOptions = new RequestLocalizationOptions();

var supportedCultures = new[]
{
    new CultureInfo("en-NZ"),
    new CultureInfo("es-MX"),
    new CultureInfo("fr-FR")
};

locOptions.SupportedCultures = supportedCultures;
locOptions.SupportedUICultures = supportedCultures; // ???
locOptions.SetDefaultCulture("en-NZ");
locOptions.ApplyCurrentCultureToResponseHeaders = true;

// Attach custom provider to read locale from Token
// TODO: Investigate what happens if I clear all providers and try to read locale from header
locOptions.RequestCultureProviders.Clear();
locOptions.RequestCultureProviders.Add(new TokenBasedRequestCultureProvider());

// Attach Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        options => options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:Secret"]))
        }
    );

// Add ABAC
builder.Services.AddAuthorization(ops =>
{
    ops.AddPolicy("IsNZLUser", pol =>
        {
            pol.RequireAuthenticatedUser();
            pol.RequireClaim(ClaimTypes.Country, "NZL");
        });
});

var app = builder.Build();

app.UseRequestLocalization(locOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
