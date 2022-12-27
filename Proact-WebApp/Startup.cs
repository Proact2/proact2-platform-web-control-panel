using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web.UI;
using Proact_WebApp.Service;
using System;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Proact_WebApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices( IServiceCollection services ) {
            services.AddDistributedMemoryCache();

            services.Configure<CookiePolicyOptions>( options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.HandleSameSiteCookieCompatibility();
            } );

            services.AddOptions();

            services.AddMicrosoftIdentityWebAppAuthentication( Configuration, "AzureAdB2C" )
                    .EnableTokenAcquisitionToCallDownstreamApi( new string[] { Configuration["ProactWebAppScope"] } )
                    .AddInMemoryTokenCaches();


            services.AddSession( options => {
                options.Cookie.Name = ".Proact.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            } );

            // Add APIs
            services.AddUsersService( Configuration );
            services.AddProjectService( Configuration );
            services.AddProjectPropertiesService( Configuration );
            services.AddMedicalTeamsService( Configuration );
            services.AddMedicsService( Configuration );
            services.AddPatientsService( Configuration );
            services.AddNursesService( Configuration );
            services.AddSessionService( Configuration );
            services.AddSurveysQuestionsSetsService( Configuration );
            services.AddSurveysService( Configuration );
            services.AddLexiconService( Configuration );
            services.AddLexiconImportFileReaderService( Configuration );
            services.AddInstituteService( Configuration );
            services.AddProtocolService( Configuration );
            services.AddContactService( Configuration );
            services.AddResearcherService( Configuration );
            services.AddEmailEditorService( Configuration );
            services.AddProactMobileSettings( Configuration );

            services.AddControllersWithViews( options => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add( new AuthorizeFilter( policy ) );
            } ).AddMicrosoftIdentityUI();

            services.AddRazorPages();

            services.AddOptions();
            services.Configure<OpenIdConnectOptions>( Configuration.GetSection( "AzureAdB2C" ) );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env ) {
            if ( env.IsDevelopment() ) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler( "/Home/Error" );
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints( endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}" );
                endpoints.MapRazorPages();
            } );

        }
    }
}
