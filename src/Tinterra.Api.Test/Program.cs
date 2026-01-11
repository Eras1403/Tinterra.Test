using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Tinterra.Api.Test.Authorization;
using Tinterra.Api.Test.Extensions;
using Tinterra.Api.Test.Services;
using Tinterra.Application.Interfaces;
using Tinterra.Application.Services;
using Tinterra.Infrastructure.Identity.Services;
using Tinterra.Infrastructure.Persistence.SqlServer;
using Tinterra.Infrastructure.Persistence.SqlServer.Repositories;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<SqlServerDb>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServer")
        ?? Environment.GetEnvironmentVariable("TINTERRA_SQL_CONNECTION")
        ?? "Server=localhost;Database=Tinterra.Test;Trusted_Connection=True;TrustServerCertificate=True";

    options.UseSqlServer(connectionString, sql =>
    {
        sql.MigrationsAssembly("Tinterra.Infrastructure.Persistence.DesignTime");
    });
});

builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<IAllowedTenantRepository, AllowedTenantRepository>();
builder.Services.AddScoped<ISecurityAdminRepository, SecurityAdminRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

builder.Services.AddScoped<ConfigurationService>();
builder.Services.AddScoped<SecurityAdminService>();

builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddScoped<IGroupResolver, GraphGroupResolver>();
builder.Services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Config.View", policy => policy.Requirements.Add(new PermissionRequirement("Config.View")));
    options.AddPolicy("Config.Edit", policy => policy.Requirements.Add(new PermissionRequirement("Config.Edit")));
    options.AddPolicy("Config.Publish", policy => policy.Requirements.Add(new PermissionRequirement("Config.Publish")));
    options.AddPolicy("Config.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Config.Delete")));
    options.AddPolicy("Security.Admin", policy => policy.Requirements.Add(new PermissionRequirement("Security.Admin")));
});

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ConfigurationAuthorizationHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tinterra.Api.Test", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

await BootstrapService.EnsureBootstrapAsync(app.Services, app.Configuration, app.Lifetime.ApplicationStopping);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<TenantAllowlistMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
