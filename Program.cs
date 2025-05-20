using Microsoft.OpenApi.Models;
using backend.Repositories;
using backend.Services;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<TagRepository>();       
builder.Services.AddScoped<NoteTagRepository>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<AuthService>();
//Swagger with JWT authentication
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Notes API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",

        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    opt.AddSecurityDefinition("Bearer", securityScheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new string[] {}
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "12345678901234567890123456789012";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Solo para desarrollo
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

// This is just for debugging purposes, you can remove it later
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Cadena de conexión: " + connection); // Solo para depuración

//Add DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS policy that allows requests from http://localhost:5173 (frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("https://notes-app-pi0t.onrender.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});



var app = builder.Build();
// Apply migrations at startup
// Debugging purposes, remove it later
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
