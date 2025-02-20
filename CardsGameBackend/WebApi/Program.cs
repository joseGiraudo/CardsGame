using System.Text;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Dependency Injection
builder.Services.AddScoped<ITokenDAO, TokenDAO>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IUserDAO, UserDAO>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<ICardDAO, CardDAO>();
builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddScoped<ITournamentDAO, TournamentDAO>();
builder.Services.AddScoped<ITournamentService, TournamentService>();

builder.Services.AddScoped<IGameDAO, GameDAO>();
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddScoped<ITournamentPlayerDAO, TournamentPlayerDAO>();

builder.Services.AddScoped<IPlayerCardDAO, PlayerCardDAO>();
builder.Services.AddScoped<IPlayerCardService, PlayerCardService>();




builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };

        // ver si hace falta agregar algo de la expiracion del token
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agrego la auth para el Jwt
app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors();

app.MapControllers();

app.Run();
