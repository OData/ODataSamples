// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Service.Sample.TrippinInMemory.Api;
using Microsoft.Restier.Core;
using Microsoft.OData.Service.Sample.TrippinInMemory.Models;
using System;
using System.Net.Http;
using System.Web.Http.Routing;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.OData.Service.Sample.TrippinInMemory.Controllers
{
    public class TrippinController : ODataController
    {
        private TrippinApi _api;
        private TrippinApi Api
        {
            get
            {
                if (_api == null)
                {
                    _api = (TrippinApi)this.Request.GetRequestContainer().GetService<ApiBase>();
                }

                return _api;
            }
        }

        /// <summary>
        /// Restier only supports put and post entity set.
        /// Use property name to simulate the bound action.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="name">The value of last name to be updated.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPut]
        [ODataRoute("People({key})/LastName")]
        public IHttpActionResult UpdatePersonLastName([FromODataUri]string key, [FromBody] string name)
        {
            var person = Api.People.Single(p => p.UserName == key);
            if (Api.UpdatePersonLastName(person, name))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add a trip to Me.
        /// </summary>
        /// <param name="trip">The trip to add</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("Me/Trips")]
        public IHttpActionResult AddTripToMe([FromBody]Trip trip)
        {
            return AddTrip(Api.Me, trip);
        }

        /// <summary>
        /// Add a trip to a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="trip">The trip to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("People({key})/Trips")]
        [ODataRoute("People/{key}/Trips")]
        public IHttpActionResult AddTripToPerson([FromODataUri]string key, [FromBody]Trip trip)
        {
            var person = Api.People.Where(p => p.UserName == key).FirstOrDefault();
            return AddTrip(person, trip);
        }

        /// <summary>
        /// Delete a trip from Me.
        /// </summary>
        /// <param name="tripId">The id of the trip to delete</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpDelete]
        [ODataRoute("Me/Trips/{TripId}")]
        public IHttpActionResult DeleteTripFromMe([FromODataUri]int tripId)
        {
            return DeleteTrip(Api.Me, tripId);
        }

        /// <summary>
        /// Delete a trip from a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="tripId">The id of the trip to delete.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpDelete]
        [ODataRoute("People({key})/Trips/{TripId}")]
        [ODataRoute("People/{key}/Trips/{TripId}")]
        public IHttpActionResult DeleteTripFromPerson([FromODataUri]string key, [FromODataUri]int tripId)
        {
            var person = Api.People.Single(p => p.UserName == key);
            return DeleteTrip(person, tripId);
        }

        /// <summary>
        /// Add a Friend to Me.
        /// </summary>
        /// <param name="uri">The ref of the friend to add</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("Me/Friends/$ref")]
        public IHttpActionResult AddFriendToMe([FromBody]Uri uri)
        {
            return AddFriend(Api.Me, uri);
        }

        /// <summary>
        /// Add a trip to a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="uri">The ref of the friend to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("People({key})/Friends/$ref")]
        [ODataRoute("People/{key}/Friends/$ref")]
        public IHttpActionResult AddFriendToPerson([FromODataUri]string key, [FromBody]Uri uri)
        {
            var person = Api.People.Where(p => p.UserName == key).FirstOrDefault();
            return AddFriend(person, uri);
        }

        /// <summary>
        /// Add a new Friend for Me.
        /// </summary>
        /// <param name="friend">The friend to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("Me/Friends")]
        public IHttpActionResult AddNewFriendToMe([FromBody]Person friend)
        {
            return AddNewFriend(Api.Me, friend);
        }

        /// <summary>
        /// Add a friend to a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="friend">The friend to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpPost]
        [ODataRoute("People({key})/Friends")]
        public IHttpActionResult AddNewFriendToPerson([FromODataUri]string key, [FromBody]Person friend)
        {
            var person = Api.People.Where(p => p.UserName == key).FirstOrDefault();
            return AddNewFriend(person, friend);
        }

        /// <summary>
        /// Remove a friend from Me.
        /// </summary>
        /// <param name="friendId">The id of the friend to remove</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        [HttpDelete]
        [ODataRoute("Me/Friends/$ref")]
        public IHttpActionResult RemoveFriendFromMe()
        {
            string friendId = Request.GetQueryNameValuePairs().Where(k => k.Key == "id").FirstOrDefault().Value;
            return RemoveFriend(Api.Me, friendId);
        }

        /// TODO: doesn't get called
        /// <summary>
        /// Delete a friend from a person.
        /// </summary>
        /// <param name = "key" > Key of people entity set, parsed from uri.</param>
        /// <param name = "friendId" > The id of the friend to remove</param>
        /// <returns><see cref = "IHttpActionResult" ></ returns >
        [HttpDelete]
        [ODataRoute("People({key})/Friends/$ref")]
        [ODataRoute("People/{key}/Friends/$ref")]
        public IHttpActionResult RemoveFriendFromPerson([FromODataUri]string key)
        {
            string friendId = Request.GetQueryNameValuePairs().Where(k => k.Key == "id").FirstOrDefault().Value;
            var person = Api.People.Single(p => p.UserName == key);
            return RemoveFriend(person, friendId);
        }

        /// <summary>
        /// Add a trip to a person.
        /// </summary>
        /// <param name="person">Person to add the trip to.</param>
        /// <param name="trip">The trip to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private IHttpActionResult AddTrip(Person person, Trip trip)
        {
            if (person != null && trip != null)
            {
                if (person.Trips.Any(t => t.TripId == trip.TripId))
                {
                    return StatusCode(System.Net.HttpStatusCode.Conflict);
                }
                person.Trips.Add(trip);
                return StatusCode(System.Net.HttpStatusCode.Created);
            }

            return NotFound();
        }

        /// <summary>
        /// Delete a trip from a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="tripId">The id of the trip to delete.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private IHttpActionResult DeleteTrip(Person person, int tripId)
        {
            if (person != null)
            {
                var trip = person.Trips.Where(t => t.TripId == tripId).FirstOrDefault();
                if (trip != null)
                {
                    person.Trips.Remove(trip);
                }

                return StatusCode(System.Net.HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        /// <summary>
        /// Add a friend to a person.
        /// </summary>
        /// <param name="person">Person to add the trip to.</param>
        /// <param name="friend">The ref of the friend to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private IHttpActionResult AddFriend(Person person, Uri friendRef)
        {
            if (person != null)
            {
                var friendId = GetKeyFromUri<string>(Request, friendRef, Api.ServiceProvider);
                var friend = Api.People.Where(f => f.UserName == friendId).FirstOrDefault();
                if (friend != null)
                {
                    person.Friends.Add(friend);
                    return Ok(friend);
                }
            }

            return NotFound();
        }

        /// <summary>
        /// Add a new friend to a person.
        /// </summary>
        /// <param name="person">Person to add the trip to.</param>
        /// <param name="friend">The friend to add.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private IHttpActionResult AddNewFriend(Person person, Person friend)
        {
            if (person != null && friend != null)
            {
                if(Api.People.Any(f => f.UserName == friend.UserName))
                {
                    return StatusCode(System.Net.HttpStatusCode.Conflict);
                }
                person.Friends.Add(friend);
                var datasource = Api.DataStoreManager.GetDataStoreInstance(Restier.Providers.InMemory.Utils.InMemoryProviderUtils.GetSessionId());
                if (datasource != null)
                {
                    datasource.People.Add(person);
                }

                return StatusCode(System.Net.HttpStatusCode.Created);
            }

            return NotFound();
        }

        /// <summary>
        /// Remove a friend from a person.
        /// </summary>
        /// <param name="key">Key of people entity set, parsed from uri.</param>
        /// <param name="friendRef">The ref of the friend to delete.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private IHttpActionResult RemoveFriend(Person person, string friendRef)
        {
            if (String.IsNullOrEmpty(friendRef))
            {
                return StatusCode(System.Net.HttpStatusCode.BadRequest);
            }

            if (person != null)
            {
                var friendId = GetKeyFromUri<string>(Request, new Uri(friendRef), Api.ServiceProvider);
                var friend = person.Friends.Where(f => f.UserName == friendId).FirstOrDefault();
                if (friend != null)
                {
                    person.Friends.Remove(friend);
                }

                return StatusCode(System.Net.HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        /// <summary>
        /// Get the key value from a URI.
        /// </summary>
        /// <param name="request">Request message.</param>
        /// <param name="uri">The uri to return.</param>
        /// <returns><see cref="IHttpActionResult"></returns>
        private static TKey GetKeyFromUri<TKey>(HttpRequestMessage request, Uri uri, IServiceProvider api)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            var urlHelper = request.GetUrlHelper() ?? new UrlHelper(request);

            string serviceRoot = urlHelper.CreateODataLink(
                request.ODataProperties().RouteName,
                request.GetPathHandler(), new List<ODataPathSegment>());

            var odataPath = request.GetPathHandler().Parse(
                TrippinApi.RemoveSessionIdFromUri(new Uri(serviceRoot)).AbsoluteUri, TrippinApi.RemoveSessionIdFromUri(uri).LocalPath, api);
                
            var keySegment = odataPath.Segments.OfType<KeySegment>().FirstOrDefault();
            if (keySegment == null)
            {
                throw new InvalidOperationException("The link does not contain a key.");
            }

            // Note: assumes a single key value
            var value = keySegment.Keys.FirstOrDefault().Value;
            return (TKey)value;
        }
    }
}