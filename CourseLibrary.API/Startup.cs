using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Entities;
using Library.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            //Authentication typically configured here
            services.AddControllers();

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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}