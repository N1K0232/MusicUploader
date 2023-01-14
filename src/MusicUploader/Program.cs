using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using MusicUploader.DataAccessLayer;
using OperationResults.AspNetCore;
using TinyHelpers.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMapperProfiles();
builder.Services.AddValidators();

builder.Services.AddOperationResult();

builder.Services.AddProblemDetails(options =>
{
    options.Map<NotImplementedException>(_ => new StatusCodeProblemDetails(StatusCodes.Status503ServiceUnavailable));
    options.Map<TaskCanceledException>(_ => new StatusCodeProblemDetails(StatusCodes.Status408RequestTimeout));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeSpanTicksConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
})
.AddFluentValidationRulesToSwagger(options =>
{
    options.SetNotNullableIfMinLengthGreaterThenZero = true;
});

builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});

var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddSqlServer<DataContext>(connectionString);
builder.Services.AddScoped<IDataContext>(services => services.GetService<DataContext>());

var app = builder.Build();

app.UseProblemDetails();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();