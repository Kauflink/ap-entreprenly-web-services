using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Entreprenly.WebServices.Iam.Application.Acl;
using Entreprenly.WebServices.Iam.Application.CommandServices;
using Entreprenly.WebServices.Iam.Application.Internal.CommandServices;
using Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Iam.Application.Internal.QueryServices;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Domain.Repositories;
using Entreprenly.WebServices.Iam.Infrastructure.Hashing.BCrypt.Services;
using Entreprenly.WebServices.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using Entreprenly.WebServices.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Entreprenly.WebServices.Iam.Infrastructure.Tokens.Jwt.Services;
using Entreprenly.WebServices.Iam.Interfaces.Acl;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Resources.Shared;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using ProblemDetailsFactory = Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// Routing and controllers
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

builder.Services.AddProblemDetails();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Database
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Localization
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services.AddSingleton<IStringLocalizer<CommonMessages>, StringLocalizer<CommonMessages>>();

// Problem details factory
builder.Services.AddSingleton<ProblemDetailsFactory>();

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Entreprenly.WebServices",
            Version = "v1",
            Description = "Entreprenly Web Services API"
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        { [new OpenApiSecuritySchemeReference("bearer", document)] = [] });
    options.EnableAnnotations();
});

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Bounded Context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IRoleCommandService, RoleCommandService>();
builder.Services.AddScoped<IRoleQueryService, RoleQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Mediator
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    // Seed the system role catalog
    var roleCommandService = services.GetRequiredService<IRoleCommandService>();
    await roleCommandService.Handle(new SeedRolesCommand(), CancellationToken.None);
}

app.UseGlobalExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");

// Custom token-based request authorization
app.UseRequestAuthorization();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
