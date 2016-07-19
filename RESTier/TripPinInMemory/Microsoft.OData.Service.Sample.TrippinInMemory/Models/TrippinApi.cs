using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.OData.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Publishers.OData.Model;

namespace Microsoft.OData.Service.Sample.TrippinInMemory.Models
{
    public class TrippinApi : ApiBase
    {
        private TripPinDataSource DataSource
        {
            get { return TripPinDataSource.Instance; }
        }

        protected override IServiceCollection ConfigureApi(IServiceCollection services)
        {
            services.AddService<IModelBuilder>((sp, next) => new ModelBuilder());
            return base.ConfigureApi(services);
        }

        private class ModelBuilder : IModelBuilder
        {
            public Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
            {
                var modelBuilder = new ODataConventionModelBuilder();
                modelBuilder.EntityType<Person>();
                return Task.FromResult(modelBuilder.GetEdmModel());
            }
        }

        public IQueryable<Person> People
        {
            get
            {
                return DataSource.People.AsQueryable<Person>();
            }
        }

        public Person Me
        {
            get
            {
                return DataSource.Me;
            }
        }

        public IQueryable<Airline> Airlines
        {
            get
            {
                return DataSource.Airlines.AsQueryable<Airline>();
            }
        }

        public IQueryable<Airport> Airports
        {
            get
            {
                return DataSource.Airports.AsQueryable<Airport>();
            }
        }

        /// <summary>
        /// Unbound function, not works with Restier 0.5.0-beta now directly
        /// </summary>
        /// <returns></returns>
        [Operation(EntitySet = "People")]
        public Person GetPersonWithMostFriends()
        {
            Person result = null;
            foreach (var person in People)
            {
                if (person.Friends == null)
                {
                    continue;
                }

                if (result == null)
                {
                    result = person;
                }

                if (person.Friends.Count > result.Friends.Count)
                {
                    result = person;
                }
            }

            return result;
        }

        [Operation(EntitySet = "People")]
        public Airline GetFavoriteAirline(string username)
        {

            var person = People.Single(p => p.UserName == username);
            if (person != null)
            {

                Dictionary<string, int> countDict = new Dictionary<string, int>();
                foreach (var a in Airlines)
                {
                    countDict.Add(a.AirlineCode, 0);
                }

                foreach (var t in person.Trips)
                {
                    foreach (var p in t.PlanItems)
                    {
                        Flight f = p as Flight;
                        if (f != null)
                        {
                            countDict[f.Airline.AirlineCode]++;
                        }
                    }
                }

                int max = -1;
                string favoriteAirlineCode = null;
                foreach (var record in countDict)
                {
                    if (max < record.Value)
                    {
                        favoriteAirlineCode = record.Key;
                        max = record.Value;
                    }
                }
                return Airlines.Single(a => a.AirlineCode.Equals(favoriteAirlineCode));
            }
            else
            {
                return null;
            }
        }

        [Operation(EntitySet = "People")]
        public bool UpdatePersonLastName(string key, string lastName)
        {
            var person = People.Single(p => p.UserName == key);
            if (person != null)
            {
                person.LastName = lastName;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}