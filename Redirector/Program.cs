using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Redirector;

var builder = WebApplication.CreateBuilder(args);

// logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpContextAccessor();

// storage repo
builder.Services.Configure<FileStorageRepositoryOptions>(builder.Configuration.GetSection("FileStorage"));
// builder.Services.AddSingleton<FileStorageRepositoryCache>();
// builder.Services.AddScoped<IStorageRepository, FileStorageRepository>();
builder.Services.AddScoped<IStorageRepository, DbStorageRepository>();
builder.Services.AddDbContext<RedirectsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// redirects repo
builder.Services.AddSingleton<IKeyedServiceProviderWrapper>(provider => new KeyedServiceProviderWrapper(provider));
builder.Services.AddKeyedSingleton<IRedirectRuleDeserializer, AndRuleDeserializer>("And");
builder.Services.AddKeyedSingleton<IRedirectRuleDeserializer, OrRuleDeserializer>("Or");
builder.Services.AddKeyedSingleton<IRedirectRuleDeserializer, PredicatesRuleDeserializer>("Predicates");

builder.Services.Configure<PredicateFactoryOptions>(options =>
{
    options.Operations.Add("EqualsTo", typeof(EqualsBoolExpression<>));
    options.Operations.Add("GreaterThan", typeof(GreaterThanBoolExpression<>));
    options.Operations.Add("LessThan", typeof(LessThanBoolExpression<>));
    options.Operations.Add("IsIn", typeof(IsInBoolExpression<>));
});
builder.Services.AddSingleton<IValidateOptions<PredicateFactoryOptions>, PredicateFactoryOptionsValidator>();

builder.Services.AddSingleton(provider =>
{
    var options = new JsonSerializerOptions();
    options.Converters.Add(new ObjectConverter());
    options.Converters.Add(new RedirectRuleConverter(new KeyedServiceProviderWrapper(provider)));
    options.Converters.Add(new SmartLinkDescriptionConverter());
    return options;
});

builder.Services.AddSingleton<ISmartLinkDescrptionValidator, SmartLinkDescrptionValidator>();

builder.Services.AddScoped<IRedirectsRepository, RedirectsRepository>();
builder.Services.AddScoped<IStatableSmartLinkRepository, StatableSmartLinkRepository>();
builder.Services.AddScoped<ISmartLink, SmartLink>();
builder.Services.AddSingleton<IPredicateFactory, PredicateFactory>();
builder.Services.AddScoped<IRedirectService, RedirectService>();

builder.Services.AddRedirectSubjectsFromAssemblies(builder.Configuration);

builder.Services.AddScoped<ISmartLinkEditorService, SmartLinkEditorService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<RedirectsDbContext>();
    
    dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
    
    var sourceFilePath = scope.ServiceProvider.GetRequiredService<IOptions<FileStorageRepositoryOptions>>().Value.FilePath;
    if (sourceFilePath is not null)
        DbInitializer.Seed(dbContext, sourceFilePath);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapSmartlinksEditorEndpoint();

app.UseMiddleware<NotFoundMiddleware>();
app.UseMiddleware<RedirectMiddleware>();
app.UseHttpsRedirection();

app.Run();

public partial class Program { }