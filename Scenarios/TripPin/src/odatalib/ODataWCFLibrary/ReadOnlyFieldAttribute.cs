namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ReadOnlyFieldAttribute : Attribute
    {
    }
}
