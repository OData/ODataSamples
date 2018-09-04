// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Xml.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace ODataUntypedSample.Models
{
    public static class EdmModelHelper
    {
        private static IEdmModel _edmModel;

        public static IEdmModel EdmModel => GetEdmModel();

        private static IEdmModel GetEdmModel()
        {
            if (_edmModel != null)
            {
                return _edmModel;
            }

            string csdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
    <edmx:DataServices>
        <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityType Name=""Movie"">
                <Key>
                    <PropertyRef Name=""ID"" />
                </Key>
                <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
                <Property Name=""Title"" Type=""Edm.String"" />
                <Property Name=""Genre"" Type=""NS.Genre"" Nullable=""false"" />
                <Property Name=""Locations"" Type=""Collection(NS.Address)"" />
            </EntityType>
            <ComplexType Name=""Address"">
                <Property Name=""City"" Type=""Edm.String"" />
                <Property Name=""Street"" Type=""Edm.String"" />
            </ComplexType>
            <EnumType Name=""Genre"">
                <Member Name=""Comedy"" Value=""0"" />
                <Member Name=""Cartoon"" Value=""1"" />
                <Member Name=""Adult"" Value=""2"" />
            </EnumType>
        </Schema>
        <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
            <EntityContainer Name=""Container"">
                <EntitySet Name=""Movies"" EntityType=""NS.Movie"" />
            </EntityContainer>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>";

            _edmModel = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            return _edmModel;
        }
    }
}
