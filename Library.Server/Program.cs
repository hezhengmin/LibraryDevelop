using Library.Server.Authorization;
using Library.Server.Filters;
using Library.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Zheng.Bll;
using Zheng.Bll.Interfaces;
using Zheng.Bll.Services;
using Zheng.Infra.Data.Models;

namespace Library.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelStateValidationFilter>();
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add LibraryContext
            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add HttpContextAccessor for CurrentUser
            builder.Services.AddHttpContextAccessor();
            
            // Register CurrentUser service
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();

            // Configure JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JWT");
            var key = Encoding.UTF8.GetBytes(jwtSettings["KEY"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie("ExternalLogin", options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddGoogle(options =>
            {
                var googleAuth = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuth["ClientId"]!;
                options.ClientSecret = googleAuth["ClientSecret"]!;
                options.SignInScheme = "ExternalLogin";
            })
            .AddFacebook(options =>
            {
                var fbAuth = builder.Configuration.GetSection("Authentication:Facebook");
                options.AppId = fbAuth["AppId"]!;
                options.AppSecret = fbAuth["AppSecret"]!;
                options.SignInScheme = "ExternalLogin";
                options.Scope.Add("email");
            });

            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Register Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ISysMenuService, SysMenuService>();
            builder.Services.AddScoped<Zheng.Bll.Interfaces.IEmailSender, Library.Server.Services.SmtpEmailSender>();
            builder.Services.AddScoped<Zheng.Bll.Interfaces.IRegistrationService, Zheng.Bll.Services.RegistrationService>();
            builder.Services.AddScoped<IExternalLoginService, ExternalLoginService>();
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
