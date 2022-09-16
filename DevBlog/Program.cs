using DevBlog.Data;
using DevBlog.Models;
using DevBlog.Services;
using DevBlog.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //builder.Services.AddIdentityServer()
    //    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

    builder.Services.AddAuthentication()
        .AddIdentityServerJwt();

    builder.Services.AddControllersWithViews();

    // Add DB contexts 
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("DB"), new MySqlServerVersion(new Version(8, 0, 11)));
    });

    builder.Services.AddDbContext<UserDbContext>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("UserDB"), new MySqlServerVersion(new Version(8, 0, 11)));
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireRole(RoleModel.Admin));
        options.AddPolicy("Visitor", policy => policy.RequireRole(RoleModel.Visitor));
    });

    // Inject dependencies
    //builder.Services.AddScoped<IEmailRepository, EmailRepository>();
    builder.Services.AddScoped<IVoteRepository, VoteRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<IImageRepository, ImageRepository>();
    builder.Services.AddScoped<IPostRepository, PostRepository>();

}

var app = builder.Build();

{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    //app.UseIdentityServer();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    //app.MapRazorPages();

    app.MapFallbackToFile("index.html");
}

app.Run();
