using LinkDev.IKEA.BLL.Common.Services.Attachments;
using LinkDev.IKEA.BLL.Services.Departments;
using LinkDev.IKEA.BLL.Services.Employees;
using LinkDev.IKEA.DAL.Entities.Identity;
using LinkDev.IKEA.DAL.Persistence.Data;
using LinkDev.IKEA.DAL.Persistence.Repositories.Departments;
using LinkDev.IKEA.DAL.Persistence.Repositories.Employees;
using LinkDev.IKEA.DAL.Persistence.UnitOfWork;
using LinkDev.IKEA.PL.Mapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Reflection;

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
                optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddTransient<IAttachmentService, AttachmentService>();

            //builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            //builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 5;
				options.Password.RequireDigit = true;
				options.Password.RequireNonAlphanumeric = true; // #%$
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;
				options.Password.RequiredUniqueChars = 1;

				options.User.RequireUniqueEmail = true;
				//options.User.AllowedUserNameCharacters = "asdmad;asdmas;1fdff";

				options.Lockout.AllowedForNewUsers = true;
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(5);
			})
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
				options.LogoutPath = "/Account/SignIn";
				options.AccessDeniedPath = "/Home/Error";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            // builder.Services.AddAuthentication();
            // builder.Services.AddAuthentication("Identity.Application");
            // builder.Services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = "Identity.Application";
            //     options.DefaultChallengeScheme = "Identity.Application";
            // }).AddCookie("Hamada", ".AspNetCore.Hamada", options =>
            // {
            // 	options.LoginPath = "/Account/SignIn";
            // 	options.LogoutPath = "/Account/SignIn";
            // 	options.AccessDeniedPath = "/Home/Error";
            // 	options.ExpireTimeSpan = TimeSpan.FromDays(10);
            // });

            #region Google Authentication

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";
            });

            #endregion

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            #endregion

            app.Run();

        }
    }
}
