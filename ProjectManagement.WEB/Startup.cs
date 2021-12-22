using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProjectManagement.BLL.Services;
using ProjectManagement.DAL.Abstraction;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.Repositories;
using ProjectManagement.WEB;
using ProjectManagement.WEB.Authorization;
using ProjectManagement.WEB.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Midterm2
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
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            // EF Identity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<DatabaseContext>();

            // DAL
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(ITeamManagementRepository<>), typeof(TeamManagementRepository<>));
            services.AddTransient(typeof(IProjectsManagementRepository<>), typeof(ProjectsManagementRepository<>));
            services.AddTransient(typeof(ITaskManagementRepository<>), typeof(TaskManagementRepository<>));

            //BLL
            services.AddTransient<IUserManager, ProjectManagementUserManager>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IWorkLogService, WorkLogService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Midterm2", Version = "v1" });

                // Adds the authorize button in swagger UI 
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Uses the token from the authorize input and sends it as a header
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            var builder = services.AddIdentityServer((options) =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                                   .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                                   .AddInMemoryClients(IdentityConfig.Clients);

            builder.AddDeveloperSigningCredential();
            builder.AddResourceOwnerValidator<PasswordValidator>();

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy("ProjectAdminOrCreator", policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new ProjectAdminOrCreatorRequirement());                 
                    });

                    options.AddPolicy("TaskAdminCreatorOrMember", policyBuilder =>
                    {
                        policyBuilder.AddRequirements(new TaskAdminCreatorOrMemberRequirement());
                    });

                    options.AddPolicy("TaskAdminAssigneeOrTeamMember", policyBuilder =>
                    {
                        policyBuilder.AddRequirements(new TaskAdminAssigneeOrTeamMemberRequirement());
                    });

                    options.AddPolicy("WorkLogAdminOrTaskAssignee", policyBuilder =>
                    {
                        policyBuilder.AddRequirements(new WorkLogAdminOrTaskAssigneeRequirement());
                    });

                    options.AddPolicy("WorkLogAdminProjectCreatorOrTeamMember", policyBuilder =>
                    {
                        policyBuilder.AddRequirements(new WorkLogAdminProjectCreatorOrTeamMemberRequirement());
                    });

                })
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.Audience = "https://localhost:5001/resources";
                });

            services.AddTransient<IAuthorizationHandler, ProjectAdminOrCreatorHandler>();
            services.AddTransient<IAuthorizationHandler, TaskAdminCreatorOrMemberHandler>();
            services.AddTransient<IAuthorizationHandler, TaskAdminAssigneeOrTeamMemberHandler>();
            services.AddTransient<IAuthorizationHandler, WorkLogAdminOrTaskAssigneeHandler>();
            services.AddTransient<IAuthorizationHandler, WorkLogAdminProjectCreatorOrTeamMemberHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DatabaseSeeder.Seed(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Midterm2 v1"));
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();


            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
