using System.Linq;
using AspNetCoreODataSpatial.CustomBinders;
using AspNetCoreODataSpatial.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using NetTopologySuite.Geometries;

namespace AspNetCoreODataSpatial
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
            services.AddDbContext<CustomerContext>(opt => opt.UseInMemoryDatabase("CustomerGeoContext"));
            services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5)
                                                    .AddRouteComponents("odata", GetEdmModel(), svcs =>
                                                    {
                                                        svcs.AddSingleton<IFilterBinder, CustomFilterBinder>();
                                                    }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataModelBuilder();
            builder.EntitySet<Customer>("Customers");

            var customerType = builder.EntityType<Customer>();
            customerType.HasKey(c => c.Id).Ignore(c => c.Loc);
            customerType.Property(c => c.Name);

            var customer = builder.StructuralTypes.First(t => t.ClrType == typeof(Customer));
            customer.AddProperty(typeof(Customer).GetProperty("EdmLoc")).Name = "Loc";
            return builder.GetEdmModel();
        }

        /*private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Customer>("Customers");
            return builder.GetEdmModel();
        }*/

        /*public static IEdmModel GetEdmModel()
        {
            var builder = new ODataModelBuilder();

            builder.EntitySet<Customer>("Customers");
            var customer = builder.EntityType<Customer>();
            customer.Property(c => c.Id);
            customer.Property(c => c.Name);
            customer.ComplexProperty(c => c.Loc);
            customer.HasKey(c => c.Id);

            var point = builder.ComplexType<Point>();
            point.Property(p => p.X);
            point.Property(p => p.Y);
            point.Property(p => p.Z);
            point.Property(p => p.M);
            point.ComplexProperty(p => p.CoordinateSequence);
            point.Property(p => p.NumPoints);

            var coordinateSequence = builder.ComplexType<CoordinateSequence>();
            coordinateSequence.Property(c => c.Dimension);
            coordinateSequence.Property(c => c.Measures);
            coordinateSequence.Property(c => c.Spatial);
            coordinateSequence.Property(c => c.HasZ);
            coordinateSequence.Property(c => c.HasM);
            coordinateSequence.Property(c => c.ZOrdinateIndex);
            coordinateSequence.Property(c => c.MOrdinateIndex);
            coordinateSequence.Property(c => c.Count);

            return builder.GetEdmModel();
        }*/
    }
}
