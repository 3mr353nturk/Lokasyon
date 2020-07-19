using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSServerless_Google_Geocoding_Api.Domain;
using AWSServerless_Google_Geocoding_Api.Domain.Helpers;
using AWSServerless_Google_Geocoding_Api.Domain.Model;
using AWSServerless_Google_Geocoding_Api.Domain.Repositories;
using AWSServerless_Google_Geocoding_Api.Domain.Security;
using AWSServerless_Google_Geocoding_Api.Domain.Services;
using AWSServerless_Google_Geocoding_Api.Domain.UnitOfWork;
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
using Microsoft.Extensions.Options;

namespace AWSServerless_Google_Geocoding_Api
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddLogging();
            services.AddSingleton<IAwsS3HelperService, AwsS3HelperService>();
            services.Configure<AwsS3BucketOptions>(Configuration.GetSection(nameof(AwsS3BucketOptions)))
                .AddSingleton(x => x.GetRequiredService<IOptions<AwsS3BucketOptions>>().Value);

            services.AddHttpContextAccessor();
            //services.AddScoped<IGeocoder, Address>();

            //services.AddCors(opts =>
            //{
            //    opts.AddDefaultPolicy(builder =>
            //    {
            //        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //    });
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("testPolicy", builder =>
                 {
                     builder.WithOrigins("http://localhost:53607", "https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod", "http://localhost:53583", "http://geo.tezmaksan.net").AllowAnyMethod().AllowAnyHeader();
                 });
            });


            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtbeareroptions =>
            {
                jwtbeareroptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    IssuerSigningKey = SignHandler.GetSecurityKey(tokenOptions.SecurityKey),
                    ClockSkew = TimeSpan.Zero
                };
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("testPolicy", builder =>
            //     {
            //         builder.WithOrigins("http://localhost:53607", "https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod", "http://localhost:53583").AllowAnyHeader().AllowAnyMethod();
            //     });
            //});



            //services.AddCors(options =>

            //   options.AddPolicy("testPolicy", builder =>
            //   {
            //       builder
            //       .AllowAnyHeader()
            //       .AllowAnyMethod()
            //       .WithOrigins("http://localhost:53607", "https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod", "http://localhost:53583")
            //       .AllowCredentials()
            //       .SetIsOriginAllowed((host) => true);
            //   }));

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("testPolicy", builder =>
            //    {
            //        builder.WithOrigins("http://localhost:53607", "https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod").AllowAnyMethod().AllowAnyHeader();
            //    });
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddCors(opts =>
            //{
            //    opts.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.WithOrigins("http://localhost:53607",
            //                                "https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod");
            //        });
            //});


            services.AddDbContext<LocationDBContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            services.AddControllers();

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("testPolicy");
            //app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            //app.UseCors(options => options.AllowAnyOrigin());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
