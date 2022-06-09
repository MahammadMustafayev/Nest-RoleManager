using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest_BaskEnd_Project.DAL;
using Nest_BaskEnd_Project.Models;
using Nest_BaskEnd_Project.Services;


namespace Nest_BaskEnd_Project
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession(option=> {
                option.IdleTimeout = System.TimeSpan.FromSeconds(120);
            });
            services.AddScoped<LayoutServices>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:Default"]);
            });
            services.AddIdentity<AppUser, IdentityRole>()                
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(usr=> {
                usr.Password.RequiredLength = 8;
                usr.Password.RequireNonAlphanumeric = false;
                usr.Password.RequireDigit = true;
                usr.Password.RequireLowercase = true;
                usr.Password.RequireUppercase = true;
                usr.User.RequireUniqueEmail = true;
                usr.Lockout.AllowedForNewUsers = true;
                usr.Lockout.MaxFailedAccessAttempts = 3;
                usr.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromSeconds(30);
            });
            services.AddHttpContextAccessor();
            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Author/Login";
            });
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
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
