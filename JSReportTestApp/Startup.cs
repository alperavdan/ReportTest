﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using jsreport.AspNetCore;
using jsreport.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using jsreport.Binary;


namespace JSReportTestApp
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddJsReport(new LocalReporting()
                     .UseBinary((RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
        jsreport.Binary.JsReportBinary.GetBinary() :
        jsreport.Binary.Linux.JsReportBinary.GetBinary()))
                    .AsUtility()
                    .Create());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Test Api",
                    Version = "0.1.0",
                    TermsOfService = "None"
                });
                //Set the comments path for the swagger json and ui.
                var basePath = AppContext.BaseDirectory; //Depricated PlatformServices.Default.Application.ApplicationBasePath
                var xmlPath = Path.Combine(basePath, "Test.Api.xml");
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<FileUploadOperation>(); //Register File Upload Operation Filter
            });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test");
            });
        }
    }
}
