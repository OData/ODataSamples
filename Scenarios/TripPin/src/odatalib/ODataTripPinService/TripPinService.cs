// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    /// <summary>
    /// The class implements an OData service using WCF as the host
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class TripPinService : ODataService<TripPinServiceDataSource>
    {
    }
}
