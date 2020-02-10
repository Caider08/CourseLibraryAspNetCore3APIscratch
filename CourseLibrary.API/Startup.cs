using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Services;
using Library.API.Entities;
using Library.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //allows us to access settings ... for instance in appsettings.json
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //to add services to the dependency injection container
            //service = a component attended for common consumption throughout the app
            //.AddMvC() was used to add things such as pages for views and TagHelpers

            services.AddHttpCacheHeaders((expirationModelOptions) =>
               {
                   expirationModelOptions.MaxAge = 60;
                   expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;


            });

            services.AddResponseCaching();

            //Authentication typically configured here
            services.AddControllers(setupAction =>
            {
                //if this is set to false...API will return a default content type if given format is invalid / not supported by our API
                setupAction.ReturnHttpNotAcceptable = true;

                setupAction.CacheProfiles.Add("240SecondsCacheProfile",
                                                   new CacheProfile()
                                                   {
                                                       Duration = 240
                                                   });
                
                //if want to support XML (old way)
                //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            })

            //the above accepts Date/Time
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "https://courseLibrary.com/modelValidationproblem",
                            Title = "One or more model validation errors occured.",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "See the errors property for details.",
                            Instance = context.HttpContext.Request.Path

                        };

                        problemDetails.Extensions.Add("traceid", context.HttpContext.TraceIdentifier);

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };

                });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            //register PropertyMappingService
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            //register PropertyCheckerService
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ILibraryRepository, LibraryRepository>();

            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlServer(
                    //usually you'll store this in an Environment Variable or Config file
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //uses stuff from ConfigureServices ...thus why it's called AFTER it
            //describes how it will respond to requests (Request Pipeline)

            //WHAT IS IMPORTANT is that EACH REQUEST travels through each piece of middleWare in the proper order so each piece of MiddleWare can potentially STOP IT
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later please");
                    });
                }
                //this is also where you'd log the fault
                        );
            }

            app.UseResponseCaching();

            //all of this app.Etc... is adding middleware
            //ordering is important ... we want the Cacheing middleware before Controller middleware
            //this comes after UseResponseCaching() because the ETag middleware shouldn't be used when Cache can be accessed

            //below is used for Etag Middleware
            app.UseHttpCacheHeaders();

            //Controller Middleware below
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
