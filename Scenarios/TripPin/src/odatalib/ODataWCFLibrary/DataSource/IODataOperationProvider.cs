namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IODataOperationProvider
    {
        QueryContext QueryContext { set; }
    }
}
