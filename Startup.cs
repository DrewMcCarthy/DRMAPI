using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DRMAPI.Data;
using DRMAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DRMAPI
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";
        private const string DrewmccarthyComDb = "ConnectionStrings__DRM";
        private const string TriviaDrmDb = "ConnectionStrings__TriviaDRM";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder =>
                    {
                        /* WithOrigins("http://drewmccarthy.com", "https://drewmccarthy.com",
                            "http://www.drewmccarthy.com","https://www.drewmccarthy.com"
                            )*/
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IClueService, ClueService>();

            services.AddDbContext<DRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(DrewmccarthyComDb)));
            services.AddDbContext<TriviaDRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(TriviaDrmDb)));
            services.AddControllers();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string domain = Configuration.GetValue<string>("Kestrel:EndPoints:HttpsDefaultCert:Url");

            Uri rootUri = new Uri(domain + "/api/");
            string path = rootUri.AbsolutePath;
            if (path != "/")
            {
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(path);
                    return next.Invoke();
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(CorsPolicy);
            // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
