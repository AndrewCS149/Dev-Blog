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
using Dev_Blog.State;
using System;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.Consul;

namespace Dev_Blog
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public async void ConfigureServices(IServiceCollection services)
        {
            var authMethod = new TokenAuthMethodInfo("");

            var vaultClientSettings = new VaultClientSettings("", authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "devblog", mountPoint: "ADMIN_EMAIL");

            //Secret<ConsulCredentials> consulCreds = await vaultClient.V1.Secrets.Consul.GetCredentialsAsync("devblog");
            //string consulToken = consulCreds.Data.Token;

            services.AddMvc();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddBlazoredModal();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(Environment.GetEnvironmentVariable("DEVBLOG_DB_CON_STR"));
            });

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseMySql(Environment.GetEnvironmentVariable("DEVBLOG_USER_DB_CON_STR"));
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
            services.AddScoped<AppState>();
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