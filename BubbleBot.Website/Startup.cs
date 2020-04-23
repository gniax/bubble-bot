using BubbleBot.Website.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace BubbleBot.Website
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
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddDbContext<PanelDbContext>(options => options.UseMySql(Configuration.GetConnectionString("PanelConnection")));
            services.AddMvcCore();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Panel/Login/";
                    options.ExpireTimeSpan = new System.TimeSpan(0, 20, 0);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            loggerFactory.AddFile("Logs/myapp-{Date}.txt");
            app.UseStatusCodePages();
            app.UseSession();
            app.UseAuthentication();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            //var options = new RewriteOptions()
            //    .AddRedirectToHttps();
            //app.UseRewriter(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
