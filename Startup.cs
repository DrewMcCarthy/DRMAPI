using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DRMAPI.ClientComm;
using DRMAPI.Data;
using DRMAPI.Models.Darts;
using DRMAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DRMAPI
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";
        private const string DrewmccarthyComDb = "ConnectionStrings__DRM";
        private const string TriviaDrmDb = "ConnectionStrings__TriviaDRM";

        private const string DartsDrmDb = "ConnectionStrings__DartsDRM";
        private const string DartsJwtSecret = "JwtSecret__DartsDRM";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // CORS configuration
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder =>
                    {
                        builder
                        .WithOrigins("http://drewmccarthy.com", "https://drewmccarthy.com", "https://localhost:3000", "http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });


            #region JWT Configuration
            var dartsKey = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable(DartsJwtSecret));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userName = context.Principal.Identity.Name;
                        var user = userService.GetUserByEmail(userName);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(dartsKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion

            #region DI Services
            services.AddScoped<IUserService, DartsUserService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IClueService, ClueService>();
            services.AddSingleton<DartsService>();
            services.AddSingleton<SharedCodes>();
            #endregion

            services.AddDbContext<DRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(DrewmccarthyComDb)));
            services.AddDbContext<TriviaDRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(TriviaDrmDb)));

            services.AddControllers();
            services.AddSignalR();
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DartsHub>("/dartsHub");
            });
        }
    }
}
