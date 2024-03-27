using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Controllers service
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// prevent Object cycle
// Need to install NewtonSoftJson through NuGet
// NewtonSoft.Json by James Newton-King and
// MVC.NewtonSoftJson by Microsoft
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

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

// Add Identity services to the container.
// Identity is used to manage users and roles
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    // Password options
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
// Add UserStore and RoleStore
.AddEntityFrameworkStores<ApplicationDBContext>();

// Add Authentication services to the container
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    options => {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            //ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
        };
    }
);

// Add Interface StockRepository and StockRepository services 
builder.Services.AddScoped<IStockRepository, StockRepository>();
// Add Interface CommentRepository and CommentRepository services
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
// Add Interface TokenService and TokenService services
builder.Services.AddScoped<ITokenService, TokenService>();
// Add Interface PortfolioRepository and PortfolioRepository services
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
// Add Interface FMPService and FMPService services
builder.Services.AddScoped<IFMPService, FMPService>();
builder.Services.AddHttpClient<IFMPService, FMPService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    //.WithOrigins("https://localhost)")
    .SetIsOriginAllowed(origin => true));

// JWT Authentication
app.UseAuthentication();
app.UseAuthorization();

// add MappControllers
app.MapControllers();

app.Run();

