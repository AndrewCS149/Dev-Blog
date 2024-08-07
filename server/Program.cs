using devblog.Data;
using devblog.Interfaces;
using devblog.Models;
using devblog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Discord.WebSocket;

namespace devblog
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // --------------------- CORS POLICY ------------------------------
            builder.Services.AddCors(options =>
            {
                var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
                options.AddPolicy("AllowSpecificOrigin", b => b.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            // --------------------- SWAGGER ----------------------------------
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevBlog", Version = "v1" });
            });

            builder.Services.AddControllersWithViews();

            // ----------------------- DATABASES -------------------------------
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(builder.Configuration.GetConnectionString("devblogdb"), new MySqlServerVersion(new Version(8, 0, 11)));
            });

            builder.Services.AddDbContext<UserDbContext>(options =>
            {
                options.UseMySql(builder.Configuration.GetConnectionString("devbloguserdb"), new MySqlServerVersion(new Version(8, 0, 11)));
            });

            builder.Services.AddSingleton<DiscordSocketClient>();
            builder.Services.AddScoped<IYtVideoService, YtVideoService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IVoteService, VoteService>();
            builder.Services.AddScoped<IImgService, ImgService>();
            builder.Services.AddScoped<IUsernameService, UsernameService>();

            // ----------------------- IDENTITY -------------------------------
            builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

            // password requirements
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%&";
            });

            // ----------------------- AUTHORIZATION ----------------------------
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration.GetValue<string>("Issuer"),
                    ValidAudience = builder.Configuration.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Key"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(Role.Admin));
                options.AddPolicy("Visitor", policy => policy.RequireRole(Role.Visitor));
            });

            var app = builder.Build();

            // ------------------- MIDDLEWARE -------------------------------
            app.UseCors("AllowSpecificOrigin");


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ----------------------- ROUTING -------------------------------
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
