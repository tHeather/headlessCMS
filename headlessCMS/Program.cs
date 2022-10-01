using headlessCMS.Models.Erros;
using headlessCMS.Services;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(
    c => new SqlConnection(builder.Configuration.GetConnectionString("SqlConnection"))
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(new ValidationErrorsResponse(context.ModelState));
    };
}).AddNewtonsoftJson();

builder.Services.AddScoped<ISqlDataService, SqlDataService>();
builder.Services.AddScoped<ISqlCmsService, SqlCmsService>();
builder.Services.AddScoped<CollectionMetadataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
