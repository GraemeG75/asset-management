using System.Reflection;
using System.Text;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace AssetManagement.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Controllers & OpenAPI
            services.AddControllers();
            services.AddOpenApi();

            // Configure Swagger / OpenAPI Documentation
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Asset Management API",
                    Version = "v1",
                    Description = "API endpoints for Asset Management system."
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token"
                });

                options.AddSecurityRequirement((doc) => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        new List<string>()
                    }
                });

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // Add EF Core In-Memory Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("AssetManagementDb"));

            // Register HttpContextAccessor, SiteContext & UserContext
            services.AddHttpContextAccessor();
            services.AddScoped<ISiteContext, SiteContext>();
            services.AddScoped<IUserContext, UserContext>();

            // Register Services from Infrastructure
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            // Register Dapper Services using connection string from appsettings.json
            string connectionString = Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection string is not configured in appsettings.json.");

            services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(connectionString));
            services.AddScoped<IMetadataRepository, DapperMetadataRepository>();

            // Register Generic Form Service & Form Type Strategy Handlers
            services.AddScoped<IFormTypeHandler, AssetManagement.Infrastructure.Services.FormHandlers.StandardFormHandler>();
            services.AddScoped<IFormTypeHandler, AssetManagement.Infrastructure.Services.FormHandlers.DetailFormHandler>();
            services.AddScoped<IFormTypeHandler, AssetManagement.Infrastructure.Services.FormHandlers.GridFormHandler>();
            services.AddScoped<IFormTypeHandler, AssetManagement.Infrastructure.Services.FormHandlers.SearchFormHandler>();
            services.AddScoped<IFormTypeHandler, AssetManagement.Infrastructure.Services.FormHandlers.WidgetFormHandler>();
            services.AddScoped<AssetManagement.Infrastructure.Services.FormHandlers.FormHandlerFactory>();
            services.AddScoped<IMapperService, MapperService>();
            services.AddScoped<IGenericFormService, GenericFormService>();

            // Configure JWT Authentication
            string secretKey = Configuration["JwtSettings:Secret"] 
                ?? "SuperSecretKeyForAssetManagementJwtSigning2026!#$";
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            // Configure CORS for Angular Frontend
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // Seed initial database demo records
            using (IServiceScope scope = app.Services.CreateScope())
            {
                AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            // Configure HTTP pipeline
            if (env.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Asset Management API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseCors("AllowAngularFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
