using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Swan.AspNetCore;
using Unosquare.Tubular.AspNetCoreSample.Models;

namespace Unosquare.Tubular.AspNetCoreSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            TokenOptions = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YOUMAYCHANGEMETOANOTHER")),

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "TubularSample",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "Unosquare",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };
        }

        public IConfiguration Configuration { get; }
        private TokenValidationParameters TokenOptions { get; }


        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SampleDbContext>(
                options => options.UseSqlServer(Configuration["ConnectionString"]));

            services.AddBearerTokenAuthentication(TokenOptions);

            // Add framework services.
            services.AddControllers()
                // Change the JSON contract resolver to DefaultContractResolver to avoid issues with camel case property
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app)
        {
            app.UseFallback();
            app.UseJsonExceptionHandler();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseBearerTokenAuthentication(TokenOptions, (provider, userName, password, grantType, clientId) =>
            {
                // TODO: Replace with your implementation
                if (userName != "Admin" || password != "pass.word") return Task.FromResult<ClaimsIdentity>(null);

                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userName));

                return Task.FromResult(identity);
            });

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });          
        }
    }
}