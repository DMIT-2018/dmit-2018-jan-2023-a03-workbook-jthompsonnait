
#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaylistManagementSystem.BLL;
using PlaylistManagementSystem.DAL;

namespace PlaylistManagementSystem
{
    //  your class needs to be public so it can be used outside of this project
    //  this class also needs to be static
    public static class PlaylistManagementExtension
    {
        //  our method can be anything, how it must match
        //      the builder.Services.xxxx(options => ...)
        //  This will be found in your website Program.cs

        //  the first parameter is the class that you are attempting to extend

        //  the second parameter is the options value in your call statement
        //  it is receiving the connection string for your application.
        public static void AddBackendDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            //  register the DBContext class in Chinook2018 with the service collection
            services.AddDbContext<PlaylistManagementContext>(options);

            //  add any services that you create in the class library
            //  using .AddTransient<t>(...)
            services.AddTransient<PlaylistManagementService>((ServiceProvider) =>
                {
                    var context = ServiceProvider.GetService<PlaylistManagementContext>();
                    return new PlaylistManagementService(context);
                }
            );
        }
    }
}
