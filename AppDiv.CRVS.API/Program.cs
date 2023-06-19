using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Infrastructure;
using AppDiv.CRVS.Api.Middleware;
using System.Security.Claims;
// using AppDiv.CRVS.Utility.Hub;
using AppDiv.CRVS.Infrastructure.Hub;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddNewtonsoftJson(); ;


// For authentication
var _key = builder.Configuration["Jwt:Key"];
var _issuer = builder.Configuration["Jwt:Issuer"];
var _audience = builder.Configuration["Jwt:Audience"];
var _expirtyMinutes = builder.Configuration["Jwt:ExpiryMinutes"];


// Configuration for token
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = _audience,
        ValidIssuer = _issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))

    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
    // x.Events = new JwtBearerEvents
    // {
    //     OnTokenValidated = context =>
    //     {
    //         // Map additional claims to the ClaimsIdentity object
    //         var identity = context?.Principal?.Identity as ClaimsIdentity;
    //         if (identity != null)
    //         {
    //             var userId = context?.Principal?.FindFirst("userId")?.Value;

    //             if (userId != null)
    //             {
    //                 identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
    //             }
    //             var personId = context?.Principal?.FindFirst("personId")?.Value;
    //             if (personId != null)
    //             {
    //                 identity.AddClaim(new Claim(ClaimTypes.PrimarySid, personId));
    //             }

    //             var roles = context?.Principal?.FindFirst("roles")?.Value;
    //             if (roles != null)
    //             {
    //                 var roleClaims = roles.Split(',').Select(r => new Claim(ClaimTypes.Role, r));
    //                 identity.AddClaims(roleClaims);
    //             }
    //         }

    //         return Task.CompletedTask;
    //     }
    // };

});


// Dependency injection with key
builder.Services.AddSingleton<ITokenGeneratorService>(new TokenGeneratorService(_key, _issuer, _audience, _expirtyMinutes));

// Include Infrastructur/Application Dependency
builder.Services.AddApplication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);


builder.Services.AddSignalR();
builder.Services.AddCors(c =>
{
    c.AddPolicy("CorsPolicy", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{

    // To enable authorization using swagger (Jwt)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {token}\"",
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
                        new string[] {}

                    }
                });

});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MigrateDatabase();
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<CRVSDbContextInitializer>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionMiddleware();
app.UseHttpsRedirection();

// Must be betwwen app.UseRouting() and app.UseEndPoints()
// maintain middleware order


// Added for authentication
// Maintain middleware order
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MessageHub>("/Notification");
});

app.MapControllers();


app.Run();