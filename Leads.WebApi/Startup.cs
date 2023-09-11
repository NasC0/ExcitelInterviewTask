namespace Leads.WebApi
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Swagger;
    using Leads.Database.Ef;
    using Leads.Database.File;
    using Leads.Database.Static;
    using Leads.DbAdapter;
    using Leads.Services;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            switch (this.Configuration["DbType"])
            {
                case "SqlServer":
                    services.AddDbContext<LeadsContext>(
                        options => options.UseSqlServer(this.Configuration.GetConnectionString("LeadsSqlServer")));

                    services.AddScoped<ILeadsDb, LeadsEfDb>();
                    services.AddScoped<ISubAreasDb, SubAreasEfDb>();
                    break;
                case "SQLite":
                    services.AddDbContext<LeadsContext>(
                        options => options.UseSqlite(this.Configuration.GetConnectionString("LeadsSQLite")));

                    services.AddScoped<ILeadsDb, LeadsEfDb>();
                    services.AddScoped<ISubAreasDb, SubAreasEfDb>();
                    break;
                case "FileAndStatic":
                    services.AddScoped<ILeadsDb, LeadsFileDb>(_ => new LeadsFileDb(this.Configuration.GetConnectionString("FileDirectory")));
                    services.AddScoped<ISubAreasDb, SubAreasStaticDatabase>();
                    break;
                default:
                    throw new NotImplementedException();
            }

            services.AddScoped<LeadsService>();
            services.AddScoped<SubAreasService>();

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Leads API", Version = "v1" });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
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
                app.UseHsts();
            }

            //  Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leads API V1"));

            app.UseHttpsRedirection();
            app.UseMvc();

            switch (this.Configuration["DbType"])
            {
                case "SqlServer":
                case "SQLite":
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        var context = serviceScope.ServiceProvider.GetService<LeadsContext>();

                        if (context.Database.IsInMemory())
                        {
                            context.Database.EnsureCreated();
                        }
                        else
                        {
                            context.Database.Migrate();
                        }
                    }
                    break;
                case "FileAndStatic":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
