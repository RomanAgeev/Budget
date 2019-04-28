using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Expenses.Api.Queries;
using Expenses.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Expenses.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionString = Configuration.GetConnectionString("ExpensesConnection");            
            
            services.AddDbContext<ExpenseContext>(options => options.UseMySql(connectionString));

            services.AddSingleton<DbConnectionFactory>(() => new MySqlConnection(connectionString));
            
            services.AddScoped<IExpenseQueries, ExpenseQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
