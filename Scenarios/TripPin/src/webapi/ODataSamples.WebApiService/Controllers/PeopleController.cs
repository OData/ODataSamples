namespace ODataSamples.WebApiService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Query;
    using System.Web.OData.Routing;
    using ODataSamples.WebApiService.DataSource;
    using ODataSamples.WebApiService.Helper;
    using ODataSamples.WebApiService.Models;

    public class PeopleController : ODataController
    {
        #region Person Operation

        // GET odata/People
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        public IHttpActionResult Get()
        {
            return Ok(TripPinSvcDataSource.Instance.People.AsQueryable());
        }

        // GET odata/People('key')
        [EnableQuery]
        public IHttpActionResult Get([FromODataUri] string key, ODataQueryOptions<Person> queryOptions)
        {
            IEnumerable<Person> appliedPeople = TripPinSvcDataSource.Instance.People.Where(item => item.UserName == key);

            if (appliedPeople.Count() == 0)
            {
                return NotFound();
            }

            // TODO : Bug https://aspnetwebstack.codeplex.com/workitem/2033, should get from ODataQueryOptions
            if (Request.Headers.IfNoneMatch.Count > 0)
            {
                if (Request.Headers.IfNoneMatch.ElementAt(0).Tag.Equals("*"))
                {
                    return StatusCode(HttpStatusCode.NotModified);
                }
                else
                {
                    appliedPeople = queryOptions.IfNoneMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>();
                }
            }

            if (appliedPeople.Count() == 0)
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            else
            {
                return Ok(appliedPeople.Single());
            }
        }

        // GET odata/People('key')/Property
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("People({key})/AddressInfo")]
        [ODataRoute("People({key})/Emails")]
        [ODataRoute("People({key})/Trips")]
        [ODataRoute("People({key})/Friends")]
        public IHttpActionResult GetPersonCollectionProperty([FromODataUri] string key)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(person, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/People('key')/Property
        [HttpGet]
        [ODataRoute("People({key})/Gender")]
        [ODataRoute("People({key})/UserName")]
        [ODataRoute("People({key})/LastName")]
        [ODataRoute("People({key})/FirstName")]
        [ODataRoute("People({key})/Introduction")]
        public IHttpActionResult GetPersonProperty([FromODataUri] string key)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(person, propertyName);
            return (propertyValue == null)
                ?
                StatusCode(HttpStatusCode.NoContent)
                :
                ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/People('key')/Property/$value
        [HttpGet]
        [ODataRoute("People({key})/Gender/$value")]
        [ODataRoute("People({key})/UserName/$value")]
        [ODataRoute("People({key})/LastName/$value")]
        [ODataRoute("People({key})/FirstName/$value")]
        [ODataRoute("People({key})/Introduction/$value")]
        public object GetPersonPropertyValue([FromODataUri] string key)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                throw new HttpException("Don't find model with key:" + key);
            }

            var segments = this.Url.Request.RequestUri.Segments;
            var propertyName = segments[segments.Length - 2].TrimEnd('/');
            object val = ControllerHelper.GetPropertyValueFromModel(person, propertyName);
            return val == null ? (object)StatusCode(HttpStatusCode.NoContent) : (object)val.ToString();
        }

        // POST odata/People
        public IHttpActionResult Post(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripPinSvcDataSource.Instance.People.Add(person);
            return Created(person);
        }

        // PUT odata/People('key')
        public IHttpActionResult Put([FromODataUri] string key, Person person, ODataQueryOptions<Person> queryOptions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != person.UserName)
            {
                return BadRequest("The UserName of Person does not match the key");
            }

            IEnumerable<Person> appliedPeople = TripPinSvcDataSource.Instance.People.Where(item => item.UserName == key);

            // If if-match heaser is null, should add new entity
            if (appliedPeople.Count() == 0 && Request.Headers.IfMatch.Count == 0)
            {
                TripPinSvcDataSource.Instance.People.Add(person);
                return Created(person);
            }

            // TODO : Bug https://aspnetwebstack.codeplex.com/workitem/2033, should get from ODataQueryOptions
            if (Request.Headers.IfMatch.Count > 0)
            {
                if (Request.Headers.IfMatch.ElementAt(0).Tag.Equals("*"))
                {
                    TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                    TripPinSvcDataSource.Instance.People.Add(person);
                    return Updated(person);
                }
                else
                {
                    IQueryable<Person> ifMatchCustomers = queryOptions.IfMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>();

                    if (ifMatchCustomers.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                        TripPinSvcDataSource.Instance.People.Add(person);
                        return Updated(person);
                    }
                }
            }
            else if (Request.Headers.IfNoneMatch.Count > 0)
            {
                if (Request.Headers.IfNoneMatch.ElementAt(0).Tag.Equals("*"))
                {
                    TripPinSvcDataSource.Instance.People.Add(person);
                    return Created(person);
                }
                else
                {
                    var ifNoneMatchPeople = queryOptions.IfNoneMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>().ToList();
                    if (ifNoneMatchPeople.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                        TripPinSvcDataSource.Instance.People.Add(person);
                        return Updated(person);
                    }
                }
            }
            else
            {
                // TODO : Should return 428 as the protocal.
                return BadRequest("428");
            }
        }

        // PATCH odata/People('key')
        public IHttpActionResult Patch([FromODataUri] string key, Delta<Person> patch, ODataQueryOptions<Person> queryOptions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<Person> appliedPeople = TripPinSvcDataSource.Instance.People.Where(item => item.UserName == key);

            if (appliedPeople.Count() == 0 && Request.Headers.IfMatch.Count == 0)
            {
                var person = new Person();
                patch.Patch(person);
                TripPinSvcDataSource.Instance.People.Add(person);
                return Created(person);
            }

            // TODO : Bug https://aspnetwebstack.codeplex.com/workitem/2033, should get from ODataQueryOptions
            if (Request.Headers.IfMatch.Count > 0)
            {
                if (Request.Headers.IfMatch.ElementAt(0).Tag.Equals("*"))
                {
                    var person = appliedPeople.Single();
                    patch.Patch(person);
                    return Updated(person);
                }
                else
                {
                    IQueryable<Person> ifMatchCustomers = queryOptions.IfMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>();

                    if (ifMatchCustomers.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        var person = appliedPeople.Single();
                        patch.Patch(person);
                        return Updated(person);
                    }
                }
            }
            else if (Request.Headers.IfNoneMatch.Count > 0)
            {
                if (Request.Headers.IfNoneMatch.ElementAt(0).Tag.Equals("*"))
                {
                    var person = appliedPeople.Single();
                    patch.Patch(person);
                    TripPinSvcDataSource.Instance.People.Add(person);
                    return Created(person);
                }
                else
                {
                    var ifNoneMatchPeople = queryOptions.IfNoneMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>().ToList();
                    if (ifNoneMatchPeople.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        var person = appliedPeople.Single();
                        patch.Patch(person);
                        return Updated(person);
                    }
                }
            }
            else
            {
                // TODO : Should return 428 as the protocal.
                return BadRequest("428");
            }
        }

        // DELETE odata/People('key')
        public IHttpActionResult Delete([FromODataUri] string key, ODataQueryOptions<Person> queryOptions)
        {
            IEnumerable<Person> appliedPeople = TripPinSvcDataSource.Instance.People.Where(item => item.UserName == key);

            if (appliedPeople.Count() == 0)
            {
                return NotFound();
            }

            // TODO : Bug https://aspnetwebstack.codeplex.com/workitem/2033, should get from ODataQueryOptions
            if (Request.Headers.IfMatch.Count > 0)
            {
                if (Request.Headers.IfMatch.ElementAt(0).Tag.Equals("*"))
                {
                    TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    IQueryable<Person> ifMatchPeople = queryOptions.IfMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>();

                    if (ifMatchPeople.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                }
            }
            else if (Request.Headers.IfNoneMatch.Count > 0)
            {
                if (Request.Headers.IfNoneMatch.ElementAt(0).Tag.Equals("*"))
                {
                    return StatusCode(HttpStatusCode.PreconditionFailed);
                }
                else
                {
                    var ifNoneMatchPeople = queryOptions.IfNoneMatch.ApplyTo(appliedPeople.AsQueryable()).Cast<Person>().ToList();
                    if (ifNoneMatchPeople.Count() == 0)
                    {
                        return StatusCode(HttpStatusCode.PreconditionFailed);
                    }
                    else
                    {
                        TripPinSvcDataSource.Instance.People.Remove(appliedPeople.Single());
                        return StatusCode(HttpStatusCode.NoContent);
                    }
                }
            }
            else
            {
                // TODO : Should return 428 as the protocal.
                return BadRequest("428");
            }
        }

        #endregion


        #region Person Trips operation

        // GET odata/People('key')/Trips(tripId)
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})")]
        public IHttpActionResult GetPersonTrip([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }
            return Ok(trip);
        }

        // GET odata/People('key')/Trips(tripId)/Property
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("People({key})/Trips({tripId})/Tags")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems")]
        public IHttpActionResult GetPersonTripCollectionProperty([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(trip, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/People('key')/Trips(tripId)/Property
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})/TripId")]
        [ODataRoute("People({key})/Trips({tripId})/ShareId")]
        [ODataRoute("People({key})/Trips({tripId})/Name")]
        [ODataRoute("People({key})/Trips({tripId})/Description")]
        [ODataRoute("People({key})/Trips({tripId})/StartsAt")]
        [ODataRoute("People({key})/Trips({tripId})/EndsAt")]
        public IHttpActionResult GetPersonTripProperty([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(trip, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/People('key')/Trips(tripId)/Property
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})/TripId/$value")]
        [ODataRoute("People({key})/Trips({tripId})/ShareId/$value")]
        [ODataRoute("People({key})/Trips({tripId})/Name/$value")]
        [ODataRoute("People({key})/Trips({tripId})/Description/$value")]
        [ODataRoute("People({key})/Trips({tripId})/StartsAt/$value")]
        [ODataRoute("People({key})/Trips({tripId})/EndsAt/$value")]
        public string GetPersonTripPropertyValue([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                throw new HttpException("Don't find model with key:" + key);
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                throw new HttpException("Don't find model with key:" + key);
            }

            var segments = this.Url.Request.RequestUri.Segments;
            var propertyName = segments[segments.Length - 2].TrimEnd('/');
            return ControllerHelper.GetPropertyValueFromModel(person, propertyName).ToString();
        }

        // POST odata/People('key')/Trips
        [HttpPost]
        [ODataRoute("People({key})/Trips")]
        public IHttpActionResult PostNewTripToPerson([FromODataUri] string key, Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            trip.TripId = person.Trips.Max(item => item.TripId) + 1;
            person.Trips.Add(trip);
            return Created(trip);
        }

        // PUT odata/People('key')/Trips(tripId)
        [HttpPut]
        [ODataRoute("People({key})/Trips({tripId})")]
        public IHttpActionResult PutTripInPerson([FromODataUri] string key, [FromODataUri] int tripId, Trip trip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (tripId != trip.TripId)
            {
                return BadRequest("The TripId of Trip does not match the tripId.");
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var oldTrip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (oldTrip == null)
            {
                return NotFound();
            }

            person.Trips.Remove(oldTrip);
            person.Trips.Add(trip);
            return Updated(trip);
        }

        // PATCH odata/People('key')/Trips(tripId)
        [ODataRoute("People({key})/Trips({tripId})")]
        public IHttpActionResult PatchTripInPerson([FromODataUri] string key, [FromODataUri] int tripId, Delta<Trip> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            patch.Patch(trip);
            return Updated(trip);
        }

        // DELETE odata/People('key')/Trips(tripId)
        [HttpDelete]
        [ODataRoute("People({key})/Trips({tripId})")]
        public IHttpActionResult DeleteTripFromPerson([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            person.Trips.Remove(trip);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion


        #region Person Friends operation

        // GET odata/People('key')/Friends('friendUserName')
        [HttpGet]
        [ODataRoute("People({key})/Friends({friendUserName})")]
        public IHttpActionResult GetPersonFriend([FromODataUri] string key, [FromODataUri] string friendUserName)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var friend = person.Friends.SingleOrDefault(item => item.UserName == friendUserName);
            if (friend == null)
            {
                return NotFound();
            }
            return Ok(friend);
        }

        // GET odata/People('key')/Friends('friendUserName')/Property
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("People({key})/Friends({friendUserName})/AddressInfo")]
        [ODataRoute("People({key})/Friends({friendUserName})/Emails")]
        [ODataRoute("People({key})/Friends({friendUserName})/Trips")]
        [ODataRoute("People({key})/Friends({friendUserName})/Friends")]
        public IHttpActionResult GetPersonFriendCollectionProperty([FromODataUri] string key, [FromODataUri] string friendUserName)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var friend = person.Friends.SingleOrDefault(item => item.UserName == friendUserName);
            if (friend == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(friend, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // GET odata/People('key')/Friends('friendUserName')/Property
        [HttpGet]
        [ODataRoute("People({key})/Friends({friendUserName})/Gender")]
        [ODataRoute("People({key})/Friends({friendUserName})/UserName")]
        [ODataRoute("People({key})/Friends({friendUserName})/LastName")]
        [ODataRoute("People({key})/Friends({friendUserName})/FirstName")]
        public IHttpActionResult GetPersonFriendProperty([FromODataUri] string key, [FromODataUri] string friendUserName)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var friend = person.Friends.SingleOrDefault(item => item.UserName == friendUserName);
            if (friend == null)
            {
                return NotFound();
            }

            var propertyName = this.Url.Request.RequestUri.Segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(friend, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // POST odata/People('key')/Friends
        [HttpPost]
        [ODataRoute("People({key})/Friends")]
        public IHttpActionResult AddNewPersonFriend([FromODataUri] string key, [FromBody] Person friend)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            if (person.Friends.Any(item => item.UserName == friend.UserName))
            {
                return BadRequest(string.Format("Person {0} already has friend {1}", key, friend.UserName));
            }

            TripPinSvcDataSource.Instance.People.Add(friend);
            person.Friends.Add(friend);
            return Created(friend);
        }

        // POST odata/People('key')/Friends/$ref
        [HttpPost]
        [ODataRoute("People({key})/Friends/$ref")]
        public IHttpActionResult LinkToPersonFriends([FromODataUri] string key, [FromBody] Uri link)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            string relatedKey = Request.GetKeyValue<string>(link);

            if (person.Friends.Any(item => item.UserName == relatedKey))
            {
                return BadRequest(string.Format("Person {0} already has friend {1}", key, relatedKey));
            }

            var newFriend = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == relatedKey);
            person.Friends.Add(newFriend);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // Delete odata/People('key')/Friends/$ref?$id={'relatedKey'}
        [HttpDelete]
        [ODataRoute("People({key})/Friends({relatedKey})/$ref")]
        public IHttpActionResult DeleteLinkOfPersonFriends([FromODataUri] string key, [FromODataUri] string relatedKey)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var friend = person.Friends.SingleOrDefault(item => item.UserName == relatedKey);
            if (friend == null)
            {
                return NotFound();
            }

            person.Friends.Remove(friend);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion


        #region Person Trip PlanItem operation

        // GET odata/People('key')/Trips(tripId)/PlanItems(planItemId)
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})")]
        public IHttpActionResult GetPersonTripPlanItem([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var planItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (planItem == null)
            {
                return NotFound();
            }
            return Ok(planItem);
        }

        // GET odata/People('key')/Trips(tripId)/PlanItems/Namespace.DerivedType
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems/ODataSamples.WebApiService.Models.Event")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems/ODataSamples.WebApiService.Models.Flight")]
        public IHttpActionResult GetPersonTripDerivedPlanItem([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var derivedType = this.Url.Request.RequestUri.Segments.Last();

            return GetDerivedTypeItems(trip.PlanItems, derivedType);
        }

        // GET odata/People('key')/Trips(tripId)/PlanItems(planItemId)/Namespace.DerivedType
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight")]
        public IHttpActionResult GetPersonTripDerivedPlanItem([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var planItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (planItem == null)
            {
                return NotFound();
            }

            var derivedType = this.Url.Request.RequestUri.Segments.Last();
            if (derivedType != planItem.GetType().ToString())
            {
                return NotFound();
            }
            return Ok(planItem);
        }

        // GET odata/People('key')/Trips(tripId)/PlanItems(planItemId)/Namespace.DerivedType/Property
        [HttpGet]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event/OccursAt")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event/Description")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight/FlightNumber")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight/Airline")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight/From")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight/To")]
        public IHttpActionResult GetPersonTripDerivedPlanItemProperty([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var planItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (planItem == null)
            {
                return NotFound();
            }

            var segments = this.Url.Request.RequestUri.Segments;

            var derivedType = segments[segments.Length - 2].TrimEnd('/');
            if (derivedType != planItem.GetType().ToString())
            {
                return NotFound();
            }

            var propertyName = segments.Last();
            var propertyValue = ControllerHelper.GetPropertyValueFromModel(planItem, propertyName);
            return ControllerHelper.GetOKHttpActionResult(this, propertyValue);
        }

        // POST odata/People('key')/Trips(tripId)/PlanItems
        [HttpPost]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems")]
        public IHttpActionResult PostNewPlanItemInTrip([FromODataUri] string key, [FromODataUri] int tripId, PlanItem planItem)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            planItem.PlanItemId = trip.PlanItems.Max(item => item.PlanItemId) + 1;
            trip.PlanItems.Add(planItem);
            return Created(planItem);
        }

        // PUT odata/People('key')/Trips(tripId)/PlanItems(planItemId)
        [HttpPut]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event")]
        public IHttpActionResult PutPlanItemInTrip([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId, PlanItem planItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (planItemId != planItem.PlanItemId)
            {
                return BadRequest("The PlanItem Id of PlanItem does not match the planItemId parameter.");
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var oldPlanItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (oldPlanItem == null)
            {
                return NotFound();
            }

            trip.PlanItems.Remove(oldPlanItem);
            trip.PlanItems.Add(planItem);
            return Updated(planItem);
        }

        // PATCH odata/People('key')/Trips(tripId)/PlanItems(planItemId)
        [HttpPatch]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event")]
        public IHttpActionResult PatchPlanItemInTrip([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId, Delta<PlanItem> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var planItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (planItem == null)
            {
                return NotFound();
            }

            patch.Patch(planItem);
            return Updated(planItem);
        }

        // DELETE odata/People(key)/Trips(tripId)/PlanItems(planItemId)
        [HttpDelete]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight")]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Event")]
        public IHttpActionResult DeletePlanItemFromTrip([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var planItem = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId);
            if (planItem == null)
            {
                return NotFound();
            }

            trip.PlanItems.Remove(planItem);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT odata/People('key')/Trips(tripId)/PlanItems(planItemId)/ODataSamples.WebApiService.Models.Flight/Airline/$ref
        [HttpPut]
        [ODataRoute("People({key})/Trips({tripId})/PlanItems({planItemId})/ODataSamples.WebApiService.Models.Flight/Airline/$ref")]
        public IHttpActionResult UpdateFlightAirline([FromODataUri] string key, [FromODataUri] int tripId, [FromODataUri] int planItemId, [FromBody] Uri link)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var flight = trip.PlanItems.SingleOrDefault(item => item.PlanItemId == planItemId) as Flight;
            if (flight == null)
            {
                return NotFound();
            }

            var relatedKey = Request.GetKeyValue<string>(link);
            var newAirline = TripPinSvcDataSource.Instance.Airlines.SingleOrDefault(item => item.AirlineCode == relatedKey);

            flight.Airline = newAirline;
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion


        #region Function & Action

        // GET odata/People('key')/GetFavoriteAirline()
        [HttpGet]
        public IHttpActionResult GetFavoriteAirline([FromODataUri] string key)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var airlines = TripPinSvcDataSource.Instance.Airlines;
            var countDict = new Dictionary<string, int>();

            foreach (var a in airlines)
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

            var favoriteAirline = airlines.Single(a => a.AirlineCode.Equals(favoriteAirlineCode));
            return Ok(favoriteAirline);
        }

        // GET odata/People('key')/GetFriendsTrips('UserName')
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        public IHttpActionResult GetFriendsTrips([FromODataUri] string key, string userName)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var friend = person.Friends.SingleOrDefault(p => p.UserName.Equals(userName));
            if (friend == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(friend.Trips);
            }
        }

        //GET odata/People('key')/Trips(TripId)/Namespace.GetInvolvedPeople() 
        [HttpGet]
        [EnableQuery(PageSize = Utility.DefaultPageSize)]
        [ODataRoute("People({key})/Trips({tripId})/ODataSamples.WebApiService.Models.GetInvolvedPeople()")]
        public IHttpActionResult GetInvolvedPeople([FromODataUri] string key, [FromODataUri] int tripId)
        {
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var trip = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (trip == null)
            {
                return NotFound();
            }

            var shareID = trip.ShareId;

            List<Person> sharingPersons = new List<Person>();

            foreach (var onePerson in TripPinSvcDataSource.Instance.People)
            {
                foreach (var t in onePerson.Trips)
                {
                    if (shareID.Equals(t.ShareId))
                    {
                        sharingPersons.Add(onePerson);
                        break;
                    }
                }
            }

            return Ok(sharingPersons.AsQueryable());
        }


        // POST odata/People('key')/ShareTrip
        [HttpPost]
        public IHttpActionResult ShareTrip([FromODataUri] string key, ODataActionParameters parameters)
        {
            var userName = parameters["userName"].ToString();
            var tripId = Convert.ToInt32(parameters["tripId"]);
            var person = TripPinSvcDataSource.Instance.People.SingleOrDefault(item => item.UserName == key);
            if (person == null)
            {
                return NotFound();
            }

            var tripInstance = person.Trips.SingleOrDefault(item => item.TripId == tripId);
            if (tripInstance == null)
            {
                return NotFound();
            }

            var friendInstance = person.Friends.SingleOrDefault(item => item.UserName == userName);
            if (friendInstance == null)
            {
                return NotFound();
            }

            if (friendInstance != null &&
                friendInstance.Trips != null &&
                !friendInstance.Trips.Any(item => item.TripId == tripId))
            {
                //TODO, Deep Copy trip model
                var newTrip = tripInstance;
                var maxTripId = friendInstance.Trips.Select(item => item.TripId).Max();
                newTrip.TripId = maxTripId + 1;
                friendInstance.Trips.Add(newTrip);
                return Updated(friendInstance);
            }
            else
            {
                return BadRequest();
            }
        }

        #endregion


        #region Private Methods

        private IHttpActionResult GetDerivedTypeItems(List<PlanItem> planItems, string derivedTypeStr)
        {
            Type dericedType = Type.GetType(derivedTypeStr);
            if (dericedType == null)
            {
                return NotFound();
            }

            var methodInfoOfType = typeof(Enumerable).GetMethod("OfType", BindingFlags.Public | BindingFlags.Static);
            methodInfoOfType = methodInfoOfType.MakeGenericMethod(new Type[] { dericedType });
            var derivedTypeItems = methodInfoOfType.Invoke(null, new object[] { planItems });
            return ControllerHelper.GetOKHttpActionResult(this, derivedTypeItems);
        }

        #endregion
    }
}
