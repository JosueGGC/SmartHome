using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Text;

/******************* Builder *************************************************************/
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

//builder.Services.AddDbContext<SmarthomeContext>(options => 
//    options.UseInMemoryDatabase("SensorsList"));
builder.Services.AddDbContext<SmarthomeContext>(options =>
    options.UseMySQL(builder.Configuration["ConnectionStrings:MySql"]));

var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JSON Web Token based security",
};

var securityReq = new OpenApiSecurityRequirement()
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
        new string[] {}
    }
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mi SmartHome API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(securityReq);
});

/**********************  APP *****************************************************/
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

/***************************************************************************************/
app.MapGet("/", [AllowAnonymous] () => "Mi SmartHome API");

app.MapPost("/login", [AllowAnonymous] async (Usuario user, SmarthomeContext db) =>
{
    var userdb = await db.Usuarios.FindAsync(user.nombre);
    if (userdb is null) return Results.NotFound(user.nombre);
    if (userdb.contra != user.contra) return Results.Unauthorized();
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
    var jwtTokenHandler = new JwtSecurityTokenHandler();
    var descriptor = new SecurityTokenDescriptor()
    {
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        Expires = DateTime.UtcNow.AddHours(1)
    };
    var token = jwtTokenHandler.CreateToken(descriptor);
    var jwtToken = jwtTokenHandler.WriteToken(token);
    //return Results.Ok(jwtToken);

    var jwt = "{ \"jwt\": \"" + jwtToken + "\" }";
    return Results.Text(jwt);
});

/************************* CRUD REGISTRO *****************************************************/
app.MapGet("/registro", [Authorize] async (SmarthomeContext db) =>
{
    return await db.Registro.ToListAsync();
});

app.MapGet("/registro/{id}", [Authorize] async (int id, SmarthomeContext db) =>
{
    var sensor = await db.Registro.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(sensor);
});

app.MapPost("/registro", [Authorize] async (Registros s, SmarthomeContext db) =>
{
    s.fec_hora = DateTime.Now;
    db.Registro.Add(s);
    await db.SaveChangesAsync();
    return Results.Created($"/control/{s.id_Registro}", s);
});

app.MapPut("/registro/{id}", [Authorize] async (int id, Registros s, SmarthomeContext db) =>
{
    var sensor = await db.Registro.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    sensor.op_Uno = s.op_Uno;
    sensor.op_Dos = s.op_Dos;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/registro/{id}", [Authorize] async (int id, SmarthomeContext db) =>
{
    var sensor = await db.Registro.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    db.Registro.Remove(sensor);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
/*****************************************************************************************/

/************************* CRUD CONTROL *****************************************************/
app.MapGet("/control", [Authorize] async (SmarthomeContext db) =>
{
    return await db.Control.ToListAsync();
});

app.MapGet("/control/{id}", [Authorize] async (int id, SmarthomeContext db) =>
{
    var sensor = await db.Control.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(sensor);
});

app.MapPost("/control", [Authorize] async (Controles s, SmarthomeContext db) =>
{
    db.Control.Add(s);
    await db.SaveChangesAsync();
    return Results.Created($"/control/{s.id_Control}", s);
});

app.MapPut("/control/{id}", [Authorize] async (int id, Controles s, SmarthomeContext db) =>
{
    var sensor = await db.Control.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    sensor.h_Registro = s.h_Registro;
    sensor.tm_Riego = s.tm_Riego;
    sensor.tm_Ambiente = s.tm_Ambiente;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/control/{id}", [Authorize] async (int id, SmarthomeContext db) =>
{
    var sensor = await db.Control.FindAsync(id);
    if (sensor is null)
    {
        return Results.NotFound();
    }
    db.Control.Remove(sensor);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
/*****************************************************************************************/


app.Run();

class Usuario
{
    [Key]
    public string? nombre { get; set; }
    public string? contra { get; set; }
}

class Registros
{
    [Key]
    public int id_Registro { get; set; }
    public int id_Usuario { get; set; }
    public string? sensor_Uno { get; set; }
    public string? valor_Uno { get; set; }
    public string? op_Uno { get; set; }
    public string? sensor_Dos { get; set; }
    public string? valor_Dos { get; set; }
    public string? op_Dos { get; set; }
    public DateTime fec_hora { get; set; }
}

class Controles
{
    [Key]
    public int id_Control { get; set; }
    public int id_Usuario { get; set; }
    public string? h_Registro { get; set; }
    public string? tm_Riego { get; set; }
    public string? tm_Ambiente { get; set; }
}

class SmarthomeContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Registros> Registro => Set<Registros>();
    public DbSet<Controles> Control => Set<Controles>();
    public SmarthomeContext(DbContextOptions<SmarthomeContext> options) : base(options)
    {
    }
}