using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using forum.Models;
using Microsoft.AspNetCore.Identity;

namespace forum
{
    public class Startup
    {
        public IConfiguration Configuration {get; private set;}

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddIdentity<User, IdentityRole>(o => {
                // password settings
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 0;

                // email settings
                o.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();
                
            services.AddDbContext<Context>(options => options.UseNpgsql(Configuration["DBInfo:ConnectionString"]));
            services.AddMvc();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.UseIdentity();
            app.UseCookieAuthentication( new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "Cookie",
                LoginPath = new PathString("/home"),
                AccessDeniedPath = new PathString("/home"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });
            InitializeRoles(app.ApplicationServices).Wait();
            app.UseMvc();
        }

        private async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            // Array of Roles to create
            string[] RolesToCreate = new string[] {"basic", "moderator", "admin"};
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach( string role in RolesToCreate )
            {
                // If a Role doesn't already exist, create it
                if( !await roleManager.RoleExistsAsync(role) )
                {
                    await roleManager.CreateAsync( new IdentityRole(role) );
                }
            }
        }
    }
}
