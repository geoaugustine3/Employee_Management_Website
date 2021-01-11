using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EmployeeManagement
{
    
    public class Startup
    {
        private readonly IConfiguration _config;

        // Notice we are using Dependency Injection here
        public Startup(IConfiguration config)
        {
            _config = config;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            //AddIdentity() method adds the default identity system configuration for the specified user and role types.
            //To store and retrieve User and Role information of the registered users using EntityFrameWork Core 
            //from the underlying SQL Server database.We specify this using AddEntityFrameworkStores<AppDbContext>() 
            //passing our application DbContext class as the generic argument.
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<AppDbContext>();


            // Needs the user to be authenticated to perform any task in this website, 
            // unless specified '[AllowAnonymous]'.
                services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


            // If anyone try to access an unauthorized resource, by default we are redirected to
            // Account/AccessDenied path.To change the default access denied route, modify the code in 
            // ConfigureServices() method of the Startup class. With the above change, if anyone try to access 
            // an unauthorized resource, we will be redirected to /Administration/AccessDenied path. Then include 
            // AccessDenied action in the Administration controller and the corresponding view.

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            // The options parameter type is AuthorizationOptions. Use AddPolicy() method to create the policy.
            // The first parameter is the name of the policy and the second parameter is the policy itself.
            // To satisfy this policy requirements, the user must be logged-in in the particular role and have 
            // 'Delete Role' and 'Create Role' claims permitted.
            services.AddAuthorization(options =>
            {                                           // 'DeleteRolePolicy' is the given policy name. 
                options.AddPolicy("DeleteRolePolicy",       
                    policy => policy.RequireClaim("Delete Role").RequireClaim("Create Role")
                    );
            });

            // To satisfy EditRolePolicy, the user must be logged-in in the particular role and have 'Edit Role' claim.
            services.AddAuthorization(options =>
            {                                           // 'EditRolePolicy' is the given policy name. 
                options.AddPolicy("EditRolePolicy",
                    policy => policy.RequireClaim("Edit Role")
                    );
            });

            

            // we create a policy and include one or more claims in that policy. We can do the same thing with roles as well. 
            // Create a policy and include one or more roles in that policy. We can use it with 'Authorize' attribute at 
            // 'Controller' or 'Controller action'.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
                        
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
                // Redirects the user to the custom error page, if environments other than 'Developer' enviroment
               app.UseExceptionHandler("/Error");
               app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Page not found Error 404 is defined using custom error page

            }

            app.UseStaticFiles();

            //call UseAuthentication() method to add the Authentication middleware to the application's request processing
            //pipeline. We want to be able to authenticate users before the request reaches the MVC middleware.
            app.UseAuthentication();
             
            //  app.UseMvcWithDefaultRoute();
            app.UseMvc(Routes => { Routes.MapRoute("default", "{controller}/{action}/{id?}"); });

           // app.UseMvc();

        /*
app.Run(async (context) =>
 {
     await context.Response.WriteAsync("Hello World!");

 });  */

        }
    }
}
