using System.Data.SqlClient;
using headlessCMS.Services;
using headlessCMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SqlConnection>( 
    c => new SqlConnection( builder.Configuration.GetConnectionString("SqlConnection")) 
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<ICollectionMetadataService, CollectionMetadataService>();
builder.Services.AddScoped<ICollectionDataService, CollectionDataService>();
builder.Services.AddScoped<ISqlService, SqlService>();

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
