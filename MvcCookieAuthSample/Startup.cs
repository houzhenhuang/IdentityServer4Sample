using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4;
using MvcCookieAuthSample.Services;
using MvcCookieAuthSample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;
using IdentityServer4.Services;
using IdentityServer4.EntityFramework;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace MvcCookieAuthSample
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

            const string connectionString = "Server =.; Database =IdentityServer4.Quickstart.EntityFramework-2.0.0; uid = sa; pwd = 123456";

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext
            services.AddIdentityServer()
               .AddDeveloperSigningCredential()
              //.AddInMemoryIdentityResources(Config.GetIdentityResources())
              //.AddInMemoryApiResources(Config.GetApiResources())
              //.AddInMemoryClients(Config.GetClients())
              .AddConfigurationStore(options =>
                  {
                      options.ConfigureDbContext = builder =>
                          {
                              builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                          };
                  })
               .AddOperationalStore(options =>
               {
                   options.ConfigureDbContext = builder =>
                   {
                       builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                   };
               })

              //.AddTestUsers(Config.GetTestUsers())
              .AddAspNetIdentity<ApplicationUser>()
              .Services.AddScoped<IProfileService, ProfileService>()
               ;

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            });



            services.AddScoped<ConsentService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            InitData(app);
            app.UseIdentityServer();
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            // app.UseCookiePolicy();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void InitData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var configuration = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configuration.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configuration.Clients.Add(client.ToEntity());
                    }
                }

                if (!configuration.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        configuration.ApiResources.Add(api.ToEntity());
                    }
                }

                if (!configuration.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configuration.IdentityResources.Add(identity.ToEntity());
                    }
                }

                configuration.SaveChanges();
            }
        }
    }
}
