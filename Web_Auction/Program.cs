using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Web_Auction.Hubs;
using Web_Auction.Middleware;
using Web_Auction.ServiceExtension;

var builder = WebApplication.CreateBuilder(args);

//Logging
builder.Logging.AddSerilogWithPostgresSql(builder.Configuration);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConnectDatabaseUsePostgresSql(builder.Configuration);
builder.Services.AddIdentityFramework();
builder.Services.AddSignalR();
builder.Services.ConfigureRepositoryService();

//Config SendEmail Service
builder.Services.ConfigSendEmailService(builder.Configuration);

//Add Automapper
builder.Services.AddAutoMapper(typeof(Program));

//Add Authentication
builder.Services.AddingAuthentication(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
        //Setting up Swagger (ASP.NET Core) using the Authorization headers (Bearer)
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/hub");

app.MapControllers();

app.Run();