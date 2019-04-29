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
using Swashbuckle.AspNetCore.Swagger;

namespace Expenses.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(it => {
                it.SwaggerDoc("v1", new Info { 
                    Title = "Budget - Expenses API",
                    Version = "v1"                    
                });
            });

            string connectionString = Configuration.GetConnectionString("ExpensesConnection");
            services.AddDbContext<ExpenseContext>(options => options.UseMySql(connectionString));
            services.AddSingleton<DbConnectionFactory>(() => new MySqlConnection(connectionString));
            
            services.AddScoped<IExpenseQueries, ExpenseQueries>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger().UseSwaggerUI(it => {
                it.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses.Api V1");
                it.RoutePrefix = string.Empty;
            });
        }
    }
}
