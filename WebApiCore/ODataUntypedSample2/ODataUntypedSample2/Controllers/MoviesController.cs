// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using ODataUntypedSample.Models;

namespace ODataUntypedSample.Controllers
{
    public class MoviesController : ODataController
    {
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(DataSource.Movies);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            IEdmEntityObject movie = DataSource.GetMovie(key);
            if (movie != null)
            {
                return Ok(movie);
            }

            return NotFound();
        }

        [EnableQuery]
        public IActionResult GetLocations(int key)
        {
            IEdmEntityObject movie = DataSource.GetMovie(key);
            if (movie != null)
            {
                if (movie.TryGetPropertyValue("Locations", out object value))
                {
                    return Ok((EdmComplexObjectCollection)value);
                }
            }

            return NotFound();
        }
    }
}
