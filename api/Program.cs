using api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a database context to the container.
// Once you have a DbContext, you can inject it into your services.
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    // use SqlServer as the database provider 
    // connection string is stored in appsettings.json
    // open Ternminal and type "dotnet ef migrations add Init" to create the database
    // Migrations folder is generated and created when you run "dotnet ef migrations add Init" 
    // and it in the project api\Migrations
    // type "dotnet ef database update" to update the database
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
} );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

