using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.OData;
using BoundActionSample.Models;

public class MyEntitiesController : ODataController
{
    public IEnumerable<MyEntity> Get()
    {
        return new MyEntity[] { new MyEntity() { Id = Guid.NewGuid(), Name = "Name" } };
    }

    [HttpPost]
    public string MyAction([FromODataUri] Guid key)
    {
        return "Hello World!";
    }
}