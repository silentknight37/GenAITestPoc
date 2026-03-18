using LogisticsPro_API;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Handler;
using Microsoft.EntityFrameworkCore;
using MediatR;
using YoutubeShareManager.Handler;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureService(builder.Services);
ConfigureApp(builder);


void ConfigureApp(WebApplicationBuilder builder)
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors(ConfigurationValues.CORS_DOMAIN);
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

void ConfigureService(IServiceCollection services)
{
    services.AddScoped<JwtService>();
    services.AddAuthentication("JWTAuth").AddJwtBearer("JWTAuth", o =>
    {
        var keyBytes = Encoding.UTF8.GetBytes(ConfigurationValues.SECURITY_KEY);
        var key = new SymmetricSecurityKey(keyBytes);
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = ConfigurationValues.API_URL,
            ValidAudience = ConfigurationValues.API_URL,
            IssuerSigningKey = key
        };
    });
    services.AddCors(options =>
    {
        options.AddPolicy(ConfigurationValues.CORS_DOMAIN,
            policy => policy.WithOrigins(ConfigurationValues.CORS_ORIGINS).AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    });

    services.AddEntityFrameworkSqlServer()
        .AddDbContextPool<DB_LogisticsproContext>(options => options.UseSqlServer(ConfigurationValues.DATABASE_CONNECTION_STRING));
    DB_LogisticsproContext.ConnectionString = ConfigurationValues.DATABASE_CONNECTION_STRING;
    services.AddDbContext<DB_LogisticsproContext>(options => options.UseSqlServer(ConfigurationValues.DATABASE_CONNECTION_STRING));

    services.AddMediatR(x=>x.RegisterServicesFromAssemblies(typeof(SaveCustomerCommandHandler).Assembly));
    services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(CustomerEventQueryHandler).Assembly));
    services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = int.MaxValue;
    });
    services.Configure<FormOptions>(options =>
    {
        options.ValueLengthLimit = int.MaxValue;
        options.MultipartBodyLengthLimit = int.MaxValue;
    });
}