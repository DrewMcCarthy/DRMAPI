using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DRMAPI.Data;
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

namespace DRMAPI
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";
        private const string DrewmccarthyComDb = "ConnectionStrings__DRM";
        private const string TriviaDrmDb = "ConnectionStrings__TriviaDRM";
        private const string GroceryDrmDb = "ConnectionStrings__GroceryDRM";
        private const string GroceryJwtSecret = "JwtSecret__GroceryDRM";

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
                        /* WithOrigins("http://drewmccarthy.com", "https://drewmccarthy.com",
                            "http://www.drewmccarthy.com","https://www.drewmccarthy.com"
                            )*/
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });


            // JWT Configuration
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


            services.AddScoped<IUserService, DartsUserService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IClueService, ClueService>();
            services.AddScoped<IGroceryListService, GroceryListService>();
            

            services.AddDbContext<DRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(DrewmccarthyComDb)));
            services.AddDbContext<TriviaDRMContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(TriviaDrmDb)));
            services.AddDbContext<GroceryContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(GroceryDrmDb)));
            services.AddDbContext<GroceryContext>(options =>
                options.UseNpgsql(Environment.GetEnvironmentVariable(DartsDrmDb)));
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
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
