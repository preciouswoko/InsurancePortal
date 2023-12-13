using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using InsuranceCore.Implementations;
using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Data;
using InsuranceInfrastructure.Helpers;
using InsuranceInfrastructure.Logging;
using InsuranceInfrastructure.Repositories;
using InsuranceInfrastructure.Services;
using InsuranceInfrastructure.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace InsuranceManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services/*, ILoggerFactory loggerFactory*/)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Cookies";
                options.DefaultChallengeScheme = "Cookies";
                options.DefaultForbidScheme = "Cookies";
            }).AddCookie("Cookies", options =>
            {
                options.LoginPath = "/Auth/Login";
            });
            // loggerFactory.AddConsole(LogLevel.Debug);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");

            services.AddResponseCaching();
            services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<DapperDbContext>();


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IGenericRepository<ApplicationLogs>, GenericRepository<ApplicationLogs>>();
            services.AddScoped<IGenericRepository<FundTransferLookUp>, GenericRepository<FundTransferLookUp>>();
            services.AddScoped<IGenericRepository<Request>, GenericRepository<Request>>();
            services.AddScoped<IGenericRepository<InsuranceTable>, GenericRepository<InsuranceTable>>();
            services.AddScoped<IGenericRepository<Comments>, GenericRepository<Comments>>();
            services.AddScoped<IGenericRepository<Broker>, GenericRepository<Broker>>();
            services.AddScoped<IGenericRepository<InsuranceSubType>, GenericRepository<InsuranceSubType>>();
            services.AddScoped<IGenericRepository<InsuranceType>, GenericRepository<InsuranceType>>();
            services.AddScoped<IGenericRepository<Underwriter>, GenericRepository<Underwriter>>();
            services.AddScoped<IGenericRepository<AdminRoles>, GenericRepository<AdminRoles>>();
            services.AddScoped<IGenericRepository<AdminAuditLogs>, GenericRepository<AdminAuditLogs>>();
            services.AddScoped<IGenericRepository<AdminRoleDetails>, GenericRepository<AdminRoleDetails>>();
            services.AddScoped<IGenericRepository<BrokerInsuranceType>, GenericRepository<BrokerInsuranceType>>();
            services.AddScoped<IGenericRepository<BrokerSubInsuranceType>, GenericRepository<BrokerSubInsuranceType>>();

            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<IAumsService, AumsService>();
           // services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IOracleDataService, OracleDataService>();
            services.AddScoped<IT24Service, T24Service>();

            services.AddHttpClient();
            // Register your custom ILoggingService implementation as scoped
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            services.AddLogging();
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
           // services.AddScoped<AccessControlAttribute>();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // Register IOptions to make AppSettings accessible through dependency injection
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<AppSettings>>().Value);
            //services.AddScoped<PermissionFilter>();
            //services.AddSingleton<IHostedService, RenewalService>();

            services.AddHostedService<RenewalService>();
            // services.Configure<AumsSettings>(Configuration.GetSection("AumsSettings"));
            services.AddMemoryCache();
            services.AddSingleton<ISessionService, SessionService>();
            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(typeof(PermissionFilter));
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseSession(new SessionOptions()
            {
                Cookie = new CookieBuilder()
                {
                    Name = ".AspNetCore.Session.InsuranceManagement",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    SecurePolicy = CookieSecurePolicy.Always
                }
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseSession();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            // app.UseMyMiddleware();
            app.UseDeveloperExceptionPage();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Home}/{action=Index}/{id?}");
                    template: "{controller=Auth}/{action=Login}/{id?}");
            });
            app.UseMiddleware<PermissionMiddleware>();

        }
    }
}
