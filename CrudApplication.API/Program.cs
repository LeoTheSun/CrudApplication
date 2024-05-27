using CrudApplication.Application.Services;
using CrudApplication.Core.Abstractions;
using CrudApplication.Core.Models;
using CrudApplication.DataAccess;
using CrudApplication.DataAccess.Entities;
using CrudApplication.DataAccess.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NLog;
using NLog.Web;


namespace CrudApplication.API
{
    public static class Program
    {
        public static string FormatQuery(string query)
        {
            var start = query.IndexOf(']');
            query = query.Substring(start + 1);
            start = query.IndexOf(']');
            query = query.Substring(start + 1);
            query = query.Replace("\n", "\n    ");
            return $"    {query.Trim()}";
        }

        public static void Main(string[] args)
        {
            var loggerBuilder = LogManager.Setup().LoadConfigurationFromAppSettings();
            var logger = loggerBuilder.GetCurrentClassLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();
                builder.Services.AddHttpLogging(options => {
                    options.LoggingFields =
                        HttpLoggingFields.All ^
                        HttpLoggingFields.RequestBody ^
                        HttpLoggingFields.ResponseBody/*
                        //HttpLoggingFields.Duration*/;
                });
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddDbContext<UserDbContext>(options => {
                    options
                        .UseSqlite(builder.Configuration.GetConnectionString(nameof(UserDbContext)))
                        .LogTo(msg => loggerBuilder.GetLogger("OnlyMessage").Trace(FormatQuery(msg)), Microsoft.Extensions.Logging.LogLevel.Information, DbContextLoggerOptions.None);
                });
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                var app = builder.Build();
                app.UseHttpLogging();
                using (var scope = app.Services.CreateScope())
                {
                    using (var userDbContext = scope.ServiceProvider.GetRequiredService(typeof(UserDbContext)) as UserDbContext)
                    {
                        if (userDbContext == null)
                        {
                            throw new Exception("Failed connection to database");
                        }
                        userDbContext.Database.EnsureDeleted();
                        logger.Trace("SQL Query:");
                        userDbContext.Database.EnsureCreated();
                        var login = "Admin";
                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes("Admin");
                        byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
                        var password = Convert.ToHexString(hashBytes);
                        var name = "Admin";
                        var gender = 2;
                        var admin = true;
                        var createdOn = new DateTime(1970, 1, 1);
                        var createdBy = "Admin";
                        var modifiedOn = new DateTime(1970, 1, 1);
                        var modifiedBy = "Admin";
                        var userEntity = new UserEntity()
                        {
                            Login = login,
                            Password = password,
                            Name = name,
                            Gender = gender,
                            Admin = admin,
                            CreatedOn = createdOn,
                            CreatedBy = createdBy,
                            ModifiedOn = modifiedOn,
                            ModifiedBy = modifiedBy,
                        };
                        userDbContext.Users.Add(userEntity);
                        logger.Trace("SQL Query:");
                        userDbContext.SaveChanges();
                    }
                }
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception exception)
            {
                logger.Fatal(exception);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}