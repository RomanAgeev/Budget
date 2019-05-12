using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Expenses.Api.Behaviors;
using Expenses.Api.Middleware;
using Expenses.Api.Queries;
using Expenses.Infrastructure;
using FluentValidation;
using Lamar;
using MediatR;
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

        public void ConfigureContainer(ServiceRegistry services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(it => {
                it.SwaggerDoc("v1", new Info { 
                    Title = "Budget - Expenses API",
                    Version = "v1"
                });
            });

            string connectionString = string.Format(
                Configuration.GetConnectionString("ExpensesConnection"),
                Configuration["EXPENSES_DB_PASSWORD"]);

            services.AddDbContext<ExpenseContext>(options => options.UseMySql(connectionString));
            
            services.Scan(it => {
                it.TheCallingAssembly();
                it.AssemblyContainingType<Expenses.Domain.IAssemblyFinder>();
                it.AssemblyContainingType<Expenses.Infrastructure.IAssemblyFinder>();
                it.WithDefaultConventions();
                it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                it.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
            });

            services.For<DbConnectionFactory>().Use(() => new MySqlConnection(connectionString));
            services.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            services.For<IMediator>().Use<Mediator>();

            services.For(typeof(IPipelineBehavior<,>)).Use(typeof(LoggingBehavior<,>));
            services.For(typeof(IPipelineBehavior<,>)).Use(typeof(ValidationBehavior<,>));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ExpenseContext context) {
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseMvc();

            app.UseSwagger().UseSwaggerUI(it => {
                it.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses.Api V1");
                it.RoutePrefix = string.Empty;
            });

            // NOTE: Initial migration can fail several times with a brand new docker volume
            context.Database.Migrate();
        }
    }
}
