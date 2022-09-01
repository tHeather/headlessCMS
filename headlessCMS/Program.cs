using System.Data.SqlClient;
using headlessCMS.Models.Erros;
using headlessCMS.Services;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SqlConnection>( 
    c => new SqlConnection( builder.Configuration.GetConnectionString("SqlConnection")) 
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

builder.Services.AddScoped<ISqlApiService, SqlApiService>();
builder.Services.AddScoped<ISqlCmsService, SqlCmsService>();
builder.Services.AddScoped<CollectionMetadataService>();
builder.Services.AddScoped<CollectionDataService>();

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
