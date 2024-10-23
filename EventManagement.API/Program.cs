using EventManagement.API.Middlewares;
using EventManagement.Application;
using EventManagement.Application.Attributes;
using EventManagement.Application.Interfaces;
using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Application.Use_Cases.ImageUseCases;
using EventManagement.Application.Use_Cases.ParticipantUseCases;
using EventManagement.Application.Use_Cases.UserUseCases;
using EventManagement.Application.Validation;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.EventManagement.Infrastructure.Email;
using EventManagement.Infrastructure;
using EventManagement.Infrastructure.Notifications;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
});

builder.Services.AddValidatorsFromAssemblyContaining<UserRequestDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EventRequestDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ParticipantRequestDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterParticipantToEventRequestDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestDTOValidator>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<GetAllEventsUseCase>();
builder.Services.AddScoped<GetEventByIdUseCase>();
builder.Services.AddScoped<GetEventByNameUseCase>();
builder.Services.AddScoped<AddEventUseCase>();
builder.Services.AddScoped<UpdateEventUseCase>();
builder.Services.AddScoped<DeleteEventUseCase>();
builder.Services.AddScoped<GetEventsByCriteriaUseCase>();
builder.Services.AddScoped<GetPagedEventsUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();

builder.Services.AddScoped<GetParticipantByIdUseCase>();
builder.Services.AddScoped<GetParticipantsByEventIdUseCase>();
builder.Services.AddScoped<RegisterParticipantToEventUseCase>();
builder.Services.AddScoped<RemoveParticipantFromEventUseCase>();

builder.Services.AddScoped<AddImageUseCase>();

builder.Services.AddScoped<GetUserByIdUseCase>();
builder.Services.AddScoped<GetUserByUsernameUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<ValidateCredentialsUseCase>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "¬ведите JWT токен: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
