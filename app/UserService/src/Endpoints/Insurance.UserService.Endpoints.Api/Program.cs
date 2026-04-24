using Elastic.Apm.NetCoreAll;
using Insurance.CacheService.Infra.CallerService.Models;
using Insurance.Infra.InternalServices.UserApiCaller.Models;
using Insurance.PanelPayamakService.Infra.KavehNegarProvider;
using Insurance.UserService.AppCore.AppServices.AAA.Services;
using Insurance.UserService.AppCore.AppServices.Permissions.Services;
using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Endpoints.Api.Extensions;
using Insurance.UserService.Endpoints.Api.Extensions.Db;
using Insurance.UserService.Endpoints.Api.Extensions.Swaggers.Extentions;
using Insurance.UserService.Endpoints.Api.Middlewares;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Add this using
using Microsoft.IdentityModel.Tokens;
using OysterFx.Endpoints.Api.Extensions.DI;
using OysterFx.Endpoints.Api.ModelBindings;
using OysterFx.Infra.Auth.JwtServices;
using OysterFx.Infra.Auth.UserServices.Policies;
using OysterFx.Infra.Auth.UserServices.Policies.Abstractions;
using Suspect.TaskPro.AppCore.Shared.AAA.Services;
using System.Diagnostics;
using System.Text;
using AuthenticationService = Insurance.UserService.AppCore.AppServices.AAA.Services.AuthenticationService;
using IAuthenticationService = Insurance.UserService.AppCore.Shared.AAA.Services.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add Elastic APM
builder.Services.AddAllElasticApm();

builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.UserService");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNonValidatingValidator();
builder.Services.AddSwaggerDocumentation("Insurance User Service API", "v1");

// builder.Services.AddOpenApi();
builder.Services.AddUserServiceCommandPersistenceServices();
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddJwtUserInfoService();
builder.Services.AddKavehNegarPayamk(options =>
{
    options.ServicePath = builder.Configuration["PanelPayamak:Providers:KavehNegar:ServicePath"];
});

builder.Services.AddDbContext<InsuranceUserServiceDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Account, AppRole>(options =>
{
    // ??????? Password
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // ??????? Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // ??????? User
    options.User.RequireUniqueEmail = false;

    // ??????? SignIn
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<InsuranceUserServiceDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<Insurance.UserService.AppCore.Shared.AAA.Services.IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<ILoginCompletionService, LoginCompletionService>();
builder.Services.Configure<LegacyOtpFallbackOptions>(builder.Configuration.GetSection(LegacyOtpFallbackOptions.SectionName));
builder.Services.AddScoped<UserOtpService>();
builder.Services.AddScoped<ILegacyUserOtpService, LegacyUserOtpService>();
builder.Services.AddScoped<IUserOtpService, HybridUserOtpService>();
builder.Services.AddCacheServices(builder.Configuration);

// ????? ???? JwtSettings ?? ???????
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ????? ???? HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ????? ???? ????????
builder.Services.AddScoped<ITokenService, JwtTokenService>(); // Duplicate registration, you can remove one
builder.Services.AddScoped<IUserContextService, UserContextService>(); // Duplicate registration, you can remove one
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ??????? ????? ???? ?? JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "")),
        ClockSkew = TimeSpan.Zero
    };

    // ????? ???? ???????? ???? ??????? ?? SignalR ?? WebSockets
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddRateLimitServices(builder.Configuration);

builder.Services.AddScoped<IPermissionPolicyService, PermissionPolicyService>();
builder.Services.AddAutoPermissionPolicies(options =>
{
    options.EnablePeriodicUpdate = true;
    options.UpdateIntervalMinutes = 30;
    options.PolicyPrefix = "Permission_";
});

var app = builder.Build();
app.UseMiddleware<ApiKeyMiddleware>();
await app.Services.UsePermissionPoliciesAsync();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<InsuranceUserServiceDbContext>();

    try
    {
        Console.WriteLine("Applying UserService database migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("UserService database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying database migrations: {ex.Message}");
    }
}

// Configure W3C Activity format for distributed tracing
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddleware("Insurance UserService API v1");
}

// Use Elastic APM
app.UseAllElasticApm(builder.Configuration);

app.UseHttpsRedirection();
app.UseTokenProcessing();

// Don't forget to add Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();





