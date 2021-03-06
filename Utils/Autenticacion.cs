using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Volquex.Models;

namespace Volquex.Utils
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "custom auth";
        public string Scheme => DefaultScheme;
    }
    
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Guarda el request en el Startup
            Startup.Request = Request;
            
            // Get Authorization header value
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
                return Task.FromResult(AuthenticateResult.Fail("No ha iniciado sesión."));

			string token = authorization.FirstOrDefault();
            RespuestaSimple respuesta;
            using (var db = new Volquex.Models.VolquexDB())
                respuesta = new Services.Sesiones(db).Autorizar(token);

            // Verifica que este token esté presente en la tabla SESIONES
            if (respuesta.Codigo != 200)
                return Task.FromResult(AuthenticateResult.Fail(respuesta.Texto));
            
            // Si todo está ok, asigna el usuario y retorna
            Startup.Usuario = respuesta.Contenido.Usuario;
            var identities = new List<ClaimsIdentity> { new ClaimsIdentity("custom auth type") };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);
			
			// Grabo el token globalmente
			Startup.TokenSesion = token;

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
    
    public static class AuthenticationBuilderExtensions
    {
        // Custom authentication extension method
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(CustomAuthOptions.DefaultScheme, configureOptions);
        }
    }
}