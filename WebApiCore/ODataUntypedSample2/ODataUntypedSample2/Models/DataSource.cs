// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;
using System.Linq;

namespace ODataUntypedSample.Models
{
    public static class DataSource
    {
        public static EdmEntityObjectCollection Movies => GetMovies();

        public static IEdmEntityObject GetMovie(int key) => Movies.FirstOrDefault(m =>
        {
            if (m.TryGetPropertyValue("ID", out object value))
            {
                return key == (int)value;
            }

            return false;
        });

        private static EdmEntityObjectCollection _movies;

        private static EdmEntityObjectCollection GetMovies()
        {
            if (_movies != null)
            {
                return _movies;
            }

            IEdmModel model = EdmModelHelper.EdmModel;
            IEdmEntityType movieType = model.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Movie");
            IEdmEnumType genreType = model.SchemaElements.OfType<IEdmEnumType>().First(c => c.Name == "Genre");
            IEdmComplexType addressType = model.SchemaElements.OfType<IEdmComplexType>().First(c => c.Name == "Address");

            IEdmCollectionTypeReference movieCollectionType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(movieType, true)));
            _movies = new EdmEntityObjectCollection(movieCollectionType);

            EdmEntityObject aMovie = new EdmEntityObject(movieType);
            aMovie.TrySetPropertyValue("ID", 1); // ID
            aMovie.TrySetPropertyValue("Title", "Conan");
            aMovie.TrySetPropertyValue("Genre", CreateGenre(genreType, "Comedy"));
            aMovie.TrySetPropertyValue("Locations", CreateAddressCollection(addressType));
            _movies.Add(aMovie);

            EdmEntityObject bMovie = new EdmEntityObject(movieType);
            bMovie.TrySetPropertyValue("ID", 2); // ID
            bMovie.TrySetPropertyValue("Title", "James");
            bMovie.TrySetPropertyValue("Genre", CreateGenre(genreType, "Adult"));
            bMovie.TrySetPropertyValue("Locations", CreateAddressCollection(addressType));
            _movies.Add(bMovie);

            return _movies;
        }

        private static EdmComplexObjectCollection CreateAddressCollection(IEdmComplexType addressType)
        {
            EdmComplexObject addressA = CreateAddress(addressType, "Redmond", "28TH ST");
            EdmComplexObject addressB = CreateAddress(addressType, "Issaquah", "Sunset ST");

            IEdmCollectionTypeReference addressCollectionType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true)));
            EdmComplexObjectCollection addresses = new EdmComplexObjectCollection(addressCollectionType);
            addresses.Add(addressA);
            addresses.Add(addressB);
            return addresses;
        }

        private static EdmComplexObject CreateAddress(IEdmComplexType addressType, string city, string street)
        {
            EdmComplexObject address = new EdmComplexObject(addressType);
            address.TrySetPropertyValue("City", city);
            address.TrySetPropertyValue("Street", street);
            return address;
        }

        private static EdmEnumObject CreateGenre(IEdmEnumType genreType, string value)
        {
            return new EdmEnumObject(genreType, value);
        }
    }
}
