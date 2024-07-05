using DaewooLMS.Data;
using LMSProject.Services.Interface;
using LMSProject.Services.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaewooLMS
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
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = false;
                //options.User.AllowedUserNameCharacters = false;
            }).AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();

            services.AddAuthentication(jwoption =>
            {
                //jwoption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //jwoption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                jwoption.DefaultScheme = "JwtBearer_OR_COOKIE";
                jwoption.DefaultChallengeScheme = "JwtBearer_OR_COOKIE";

            }).AddCookie("UserCookies", options => {
                options.LoginPath = "/Home/Login";
                options.AccessDeniedPath = "/Home/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(2);
            }).AddCookie("AdminCookies", options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(2);
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ValidIssuer = "https://localhost:44363/",
                    //ValidAudience = "https://localhost:44363/",
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ValidateLifetime = true
                };
            }).AddPolicyScheme("JwtBearer_OR_COOKIE", "JwtBearer_OR_COOKIE", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    //string authorization = context.Request.Headers[HeaderNames.Authorization];
                    //var path = context.Request.Path;
                    //if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                    //    return JwtBearerDefaults.AuthenticationScheme;
                    ////if (path == "/" || path.ToString().Contains("Home"))
                    ////    return "AdminCookies";
                    ////else
                    //return CookieAuthenticationDefaults.AuthenticationScheme;

                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return JwtBearerDefaults.AuthenticationScheme;
                    return "UserCookies"; // Default to UserCookies if no Bearer token is found
                };
            });
            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                   "JwtBearer_OR_COOKIE"
               );
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                var userCookieSchemePolicyBuilder = new AuthorizationPolicyBuilder("UserCookies");
                options.AddPolicy("UserCookieScheme", userCookieSchemePolicyBuilder
                    .RequireAuthenticatedUser()
                    .Build());

                var adminCookieSchemePolicyBuilder = new AuthorizationPolicyBuilder("AdminCookies");
                options.AddPolicy("AdminCookieScheme", adminCookieSchemePolicyBuilder
                    .RequireAuthenticatedUser()
                    .Build());

            });




            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddScoped<IAuthInterface, AuthService>();
            services.AddTransient<IHTMLtoWord, HTMLtoWordService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
