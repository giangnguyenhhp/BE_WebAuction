using System.Text;
using Contracts.Identity;
using Contracts.Models;
using EmailService.SendMailServices;
using Entities;
using Entities.Identity.Models;
using LoggingWithSerilog.Sinks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Repository.Identity;
using Repository.Models;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace Web_Auction.ServiceExtension;

public static class CustomConfigService
{
    /// <summary>
    /// Config CORS
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    /// <summary>
    /// Đăng kí dịch vụ cho SendEmail
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurationManager"></param>
    public static void ConfigSendEmailService(this IServiceCollection services,ConfigurationManager configurationManager)
    {

        
        services.Configure<EmailConfiguration>(configurationManager.GetSection("EmailConfiguration"));
        

        services.AddScoped<IEmailSender, EmailSender>();

        services.Configure<FormOptions>(op =>
        {
            op.ValueLengthLimit = int.MaxValue;
            op.MultipartBodyLengthLimit = int.MaxValue;
            op.MemoryBufferThreshold = int.MaxValue;
        });
    }

    /// <summary>
    /// Đăng kí dịch vụ cho Repository
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureRepositoryService(this IServiceCollection services)
    {
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthenticateRepository,AuthenticateRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }

    /// <summary>
    /// Add Serilog
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configurationManager"></param>
    public static void AddSerilog(this ILoggingBuilder builder, ConfigurationManager configurationManager)
    {
        var logger = new LoggerConfiguration().ReadFrom.Configuration(configurationManager)
            .WriteTo.CustomSink()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .CreateLogger();

        Log.Logger = logger;
        builder.ClearProviders();
        builder.AddSerilog(logger);
    }
    /// <summary>
    /// Add Serilog with auto save in Database Postgres SQL
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configurationManager"></param>
    public static void AddSerilogWithPostgresSql(this ILoggingBuilder builder,
        ConfigurationManager configurationManager)
    {
        var connectionString = configurationManager.GetConnectionString("DefaultConnection");

        const string tableName = "Logs";

        IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>()
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            {
                "machine_name",
                new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "1")
            }
        };

        if (connectionString == null) return;
        var logger = new LoggerConfiguration().ReadFrom.Configuration(configurationManager)
            .WriteTo.PostgreSQL(connectionString, tableName, columnOptions,
                needAutoCreateTable: true,
                schemaName: "public",
                useCopy: true,
                queueLimit: 3000,
                batchSizeLimit: 40,
                period: new TimeSpan(0, 0, 10),
                formatProvider: null)
            .WriteTo.CustomSink()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()
            .CreateLogger();
        
        Log.Logger = logger;
        builder.ClearProviders();
        builder.AddSerilog(logger);
    }

    /// <summary>
    /// Connect Database with Postgres SQL
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void ConnectDatabaseUsePostgresSql(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MasterDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly("Web_Auction")));
    }

    /// <summary>
    /// Đăng kí Identity Framework cho ứng dụng
    /// </summary>
    /// <param name="services"></param>
    public static void AddIdentityFramework(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = false;
                options.SignIn.RequireConfirmedEmail = true;
                // options.SignIn.RequireConfirmedPhoneNumber = true;
            }).AddEntityFrameworkStores<MasterDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            opt.TokenLifespan = TimeSpan.FromHours(3);
        });
    }

    public static void AddingAuthentication(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //Adding JwtBearer
            .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? string.Empty)),
                    };
                });
    }
}