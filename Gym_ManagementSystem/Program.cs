using Gym_ManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_ManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add ApplicationDbContext and Identity Services
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Add IdentityRole
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add Password options configurationz
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password setting
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // User setting
                options.User.RequireUniqueEmail = true;
            });

            // Configure Model
            //builder.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.SuppressModelStateInvalidFilter = true;
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            //app.UseAuthentication();

            app.MapControllerRoute(
                name: "Account",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=MemberForm}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
