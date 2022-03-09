using Dev_Blog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dev_Blog.Models;
using Dev_Blog.Interfaces;
using Dev_Blog.Services;
using Blazored.Modal;
using System;
using Microsoft.Extensions.Configuration;

namespace Dev_Blog
{

    public class Startup
    {
        //private IConfiguration _config;

        //public Startup(IConfiguration config)
        //{
        //    _config = config;
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddBlazoredModal();

            services.AddDbContext<AppDbContext>(options =>
            {
                //options.UseMySql(_config.GetConnectionString("DB"), new MySqlServerVersion(new Version(8, 0, 11)));
                options.UseMySql(Environment.GetEnvironmentVariable("DBSTRING"), new MySqlServerVersion(new Version(8, 0, 11)));
            });

            services.AddDbContext<UserDbContext>(options =>
            {
                //options.UseMySql(_config.GetConnectionString("UserDB"), new MySqlServerVersion(new Version(8, 0, 11)));
                options.UseMySql(Environment.GetEnvironmentVariable("USERDBSTRING"), new MySqlServerVersion(new Version(8, 0, 11)));
            });

            services.AddIdentity<UserModel, IdentityRole>()
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(RoleModel.Admin));
                options.AddPolicy("Visitor", policy => policy.RequireRole(RoleModel.Visitor));
            });

            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}