﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MexicanFood.Core.ApplicationService;
using MexicanFood.Core.ApplicationService.Implementation;
using MexicanFood.Core.DomainService;
using MexicanFood.Entities;
using MexicanFood.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MexicanFood.RestApi
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
            services.AddCors();

            services.AddDbContext<MexicanFoodContext>(opt => opt.UseSqlite("Data Source=mexicanFood.db"));
            services.AddDbContext<MexicanFoodContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddScoped<IRepository<Meal>, MealRepository>();
			services.AddScoped<IMealService, MealService>();
			
			services.AddMvc().AddJsonOptions(options => {
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			});
			
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				using (var scope = app.ApplicationServices.CreateScope())
				{
					var ctx = scope.ServiceProvider.GetService<MexicanFoodContext>();
					DBInitializer.SeedDb(ctx);
				}
			}
			else
			{
				//app.UseHsts();
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService<MexicanFoodContext>();
                    ctx.Database.EnsureCreated();
                }
            }

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //app.UseHttpsRedirection();
            app.UseMvc();
		}
	}
}
