using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using BL.Models;
using BL.Repositories;
using BL.lnterfaces;
using BL.Services;
using BL.Hubs;
using PeerTutoringNetwork.DesignPatterns;
using PeerTutoringNetwork.Viewmodels;
using PeerTutoringNetwork.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRepository<Subject>, SubjectRepository>();
builder.Services.AddScoped<IFactory<Subject, SubjectVM>, SubjectFactory>();
builder.Services.AddScoped<IUtils, Utils>();
builder.Services.AddScoped<IRepository<AppointmentReservation>, ReservationRepository>();
builder.Services.AddScoped<IFactory<AppointmentReservation, ReservationVM>, ReservationFactory>();
builder.Services.AddScoped<DashboardFacade>();

builder.Services.AddSingleton<ISubject, ReservationNotifier>();
builder.Services.AddSingleton<IObserver, ReservationLogger>();

builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1",
        new OpenApiInfo { Title = "RWA Web API", Version = "v1" });

    option.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter valid JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
                new List<string>()
            }
        });
});
builder.Services.AddDbContext<PeerTutoringNetworkContext>(options => {
    options.UseSqlServer("Name=ConnectionStrings:PeerTutoringNetworkConnStr");
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();// Add the repository to the services
builder.Services.AddScoped<ReviewService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;

    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Configure JWT security services
var secureKey = builder.Configuration["JWT:SecureKey"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        var Key = Encoding.UTF8.GetBytes(secureKey);
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            NameClaimType = "userId",
        };
    });


var app = builder.Build();

var reservationNotifier = app.Services.GetService<ISubject>();
var reservationLogger = app.Services.GetService<IObserver>();

reservationNotifier?.Attach(reservationLogger);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MentorDashboard}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Login.html");
        return;
    }
    await next();
});

app.MapRazorPages();

app.Run();
