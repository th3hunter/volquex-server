using System;
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
        public static string Version;
        public static Models.Usuarios Usuario;
        public static string TokenSesion;
        public static string WebRootPath;
        public static HttpRequest Request;
        public static class Firebase
        {
            public static string CloudMessagingKey;
            public static string WebAPIKey;
        }

        public IConfiguration Configuration { get; }

        public Startup( IConfiguration configuration, 
                        IHostingEnvironment env)
        {
            Configuration = configuration;
            DataConnection.DefaultSettings = new MySettings(configuration);

            // Lee la clave del archivo de configuración y lo guarda en la variable estática
            Key = configuration.GetSection("Configuraciones").GetValue<string>("Key");
            Firebase.CloudMessagingKey = configuration.GetSection("Firebase").GetValue<string>("CloudMessagingKey");
            Firebase.WebAPIKey = configuration.GetSection("Firebase").GetValue<string>("WebAPIKey");
            Version = configuration.GetSection("Configuraciones").GetValue<string>("Version");
            
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
				app.UseHttpsRedirection();
            }

            // global cors policy
            app.UseCors( x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader() );

            // No habilito la redirección HTTPS cuando es desarrollo
            if (!env.IsDevelopment())
                app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
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
                string connectionString = "", envVariable = "";

                // Primero la obtiene del appsettings
                connectionString = configuration.GetConnectionString("Default");

                // HEROKU: Detecta si hay la variable de entorno
                envVariable = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Si hubo
                if (envVariable != "" && envVariable != null)
                {
                    // Arma la connection string a partir de la variable de entorno
                    // Ejemplo
                    // postgres://ojunflcdtkendq:be88fc41989efe90fda30380a6dae8ec9259cc19f237f11135b68a52371a6ce5@ec2-54-235-146-51.compute-1.amazonaws.com:5432/d8lhbkcpmedcej"

                    // Elimina el caracter //
                    envVariable = envVariable.Replace("//", "");

                    // Separa las partes de la conexión en un arreglo
                    char[] delimiterChars = { '/', ':', '@', '?' };
                    string[] strConn = envVariable.Split(delimiterChars);
                    strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    // Arma la connection string
                    connectionString = String.Format("User ID={0};Password={1};Host={2};Port={3};Database={4};ApplicationName=Volquex;Pooling=true;",
                        strConn[1],
                        strConn[2],
                        strConn[3],
                        strConn[4],
                        strConn[5]);
                }

                yield return
                    new ConnectionStringSettings
                    {
                        Name = "Default",
                        ProviderName = "PostgreSQL",
                        ConnectionString = connectionString
                    };
            }
        }
    }
}
