using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductAPI.Context;
using ProductAPI.Models;
using ProductAPI.Models.DTO;
using ProductAPI.Repository;
using ProductAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI2
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
            services.AddDbContext<ApplicationDBContext>(option => 
                option.UseSqlServer(Configuration.GetConnectionString("DefaultConnetionString"))
            );
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<Product, ProductDTO>();
                configuration.CreateMap<ProductDTO, Product>();
            }, typeof(Startup));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
