using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LinqToDB.Data;
using LinqToDB.Configuration;
using Volquex.Utils;

namespace Volquex
{
    public class Startup
    {
        public static string Key;
        public static string Token;
        public static string WebRootPath;
        public static HttpRequest Request;

        public IConfiguration Configuration { get; }

        public Startup( IConfiguration configuration, 
                        IHostingEnvironment env)
        {
            Configuration = configuration;
            DataConnection.DefaultSettings = new MySettings(configuration);

            // Lee la clave del archivo de configuración y lo guarda en la variable estática
            Key = configuration.GetSection("Configuraciones").GetValue<string>("Key");

            WebRootPath = env.WebRootPath;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add authentication 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
            })
                // Call custom authentication extension method
                .AddCustomAuth(null);

            services.AddMvc(options =>
            {
                // Configura todos los endpoints para que requieran autenticación
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().Build()));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();
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

            // global cors policy
            app.UseCors( x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader() );

            // No habilito la redirección HTTPS cuando es desarrollo
            if (!env.IsDevelopment())
                app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }

    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class MySettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "Default";
        public string DefaultDataProvider => "PostgreSQL";
        private IConfiguration configuration;

        public MySettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "Default",
                        ProviderName = "PostgreSQL",
                        ConnectionString = configuration.GetConnectionString("Default")
                    };
            }
        }
    }
}
