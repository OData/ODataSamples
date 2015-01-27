namespace Microsoft.Test.OData.Services.ODataWCFService.Vocabularies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    [Flags]
    public enum CorePermission
    {
        None = 0,
        Read = 1,
        ReadWrite = 3
    }
}
