using LinkDev.IKEA.DAL.Persistence.Data;
using LinkDev.IKEA.DAL.Persistence.Repositories.Departments;
using Microsoft.EntityFrameworkCore;

namespace LinkDev.IKEA.PL
{
    public class Program
    {
        // Entry Point
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ApplicationDbContext>();

            builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            #endregion

            var app = builder.Build();

            #region Configure Kestrel Middlewares

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            #endregion

            app.Run();

        }
    }
}
