namespace Microsoft.Test.OData.Services.ODataWCFService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IODataServiceDescriptor
    {
        Type ServiceType { get; }
        string ServiceName { get; }
    }
}
