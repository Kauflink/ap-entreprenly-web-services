using Cortex.Mediator.Commands;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Domain.Services;
using Entreprenly.WebServices.Chatbot.Infrastructure.ExternalServices.WhatsApp;
using Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
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
using Entreprenly.WebServices.Inventory.Application.Acl;
using Entreprenly.WebServices.Inventory.Application.CommandServices;
using Entreprenly.WebServices.Inventory.Application.Internal.CommandServices;
using Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Profiles.Application.CommandServices;
using Entreprenly.WebServices.Profiles.Application.Internal.CommandServices;
using Entreprenly.WebServices.Profiles.Application.Internal.QueryServices;
using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Repositories;
using Entreprenly.WebServices.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Sales.Application.Acl;
using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Application.Internal.CommandServices;
using Entreprenly.WebServices.Sales.Application.Internal.QueryServices;
using Entreprenly.WebServices.Sales.Application.QueryServices;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Sales.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Sales.Interfaces.Acl;
using Entreprenly.WebServices.Subscription.Application.CommandServices;
using Entreprenly.WebServices.Subscription.Application.Internal.CommandServices;
using Entreprenly.WebServices.Subscription.Application.Internal.QueryServices;
using Entreprenly.WebServices.Subscription.Application.QueryServices;
using Entreprenly.WebServices.Subscription.Domain.Repositories;
using Entreprenly.WebServices.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Resources.Shared;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
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

// Chatbot Bounded Context
builder.Services.Configure<WhatsAppBridgeOptions>(builder.Configuration.GetSection("WhatsAppBridge"));
builder.Services.AddHttpClient<IWhatsAppMessagingService, WhatsAppBridgeService>();
builder.Services.AddScoped<IChatbotResponder, RuleBasedChatbotResponder>();
builder.Services.AddScoped<ICatalogProductRepository, CatalogProductRepository>();
builder.Services.AddScoped<ProductReplyComposer>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IChatOrderRepository, ChatOrderRepository>();
builder.Services.AddScoped<IWhatsappSessionRepository, WhatsappSessionRepository>();
builder.Services.AddScoped<IChatbotConversationService, ChatbotConversationService>();
builder.Services.AddScoped<IChatOrderCommandService, ChatOrderCommandService>();
builder.Services.AddScoped<IConversationQueryService, ConversationQueryService>();
builder.Services.AddScoped<IChatMessageQueryService, ChatMessageQueryService>();
builder.Services.AddScoped<IChatOrderQueryService, ChatOrderQueryService>();
builder.Services.AddScoped<IWhatsappSessionQueryService, WhatsappSessionQueryService>();

// Profiles Bounded Context
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();

// Sales Bounded Context
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ICashRegisterRepository, CashRegisterRepository>();
builder.Services.AddScoped<ISaleCommandService, SaleCommandService>();
builder.Services.AddScoped<ISaleQueryService, SaleQueryService>();
builder.Services.AddScoped<ICashRegisterCommandService, CashRegisterCommandService>();
builder.Services.AddScoped<ICashRegisterQueryService, CashRegisterQueryService>();
builder.Services.AddScoped<ISalesContextFacade, SalesContextFacade>();

// Inventory Bounded Context
builder.Services.AddScoped<IUnitProductRepository, UnitProductRepository>();
builder.Services.AddScoped<IWeightProductRepository, WeightProductRepository>();
builder.Services.AddScoped<IUnitLotRepository, UnitLotRepository>();
builder.Services.AddScoped<IWeightLotRepository, WeightLotRepository>();
builder.Services.AddScoped<IUnitProductCommandService, UnitProductCommandService>();
builder.Services.AddScoped<IWeightProductCommandService, WeightProductCommandService>();
builder.Services.AddScoped<IUnitLotCommandService, UnitLotCommandService>();
builder.Services.AddScoped<IWeightLotCommandService, WeightLotCommandService>();
builder.Services.AddScoped<IUnitProductQueryService, UnitProductQueryService>();
builder.Services.AddScoped<IWeightProductQueryService, WeightProductQueryService>();
builder.Services.AddScoped<IUnitLotQueryService, UnitLotQueryService>();
builder.Services.AddScoped<IWeightLotQueryService, WeightLotQueryService>();
builder.Services.AddScoped<ILotQueryService, LotQueryService>();
builder.Services.AddScoped<IStockAlertQueryService, StockAlertQueryService>();
builder.Services.AddScoped<IInventoryContextFacade, InventoryContextFacade>();

// Subscription Bounded Context
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

// Mediator
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        // Seed the system role catalog
        var roleCommandService = services.GetRequiredService<IRoleCommandService>();
        await roleCommandService.Handle(new SeedRolesCommand(), CancellationToken.None);
    }
    catch (Exception exception)
    {
        logger.LogCritical(exception,
            "Database migration or startup seeding failed. The API will keep running so non-database endpoints and Swagger remain available.");
    }
}

// Honour proxy headers (scheme/host) when running behind the Caddy reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseGlobalExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllPolicy");

// Custom token-based request authorization
app.UseRequestAuthorization();

app.UseAuthorization();
app.MapControllers();

app.Run();
