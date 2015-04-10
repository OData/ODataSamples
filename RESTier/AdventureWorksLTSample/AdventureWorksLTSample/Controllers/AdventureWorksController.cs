using AdventureWorksLTSample.Models;
using Microsoft.Restier.WebApi;

namespace AdventureWorksLTSample.Controllers
{
    public class AdventureWorksController : ODataDomainController<AdventureWorksDomain>
    {
        private AdventureWorksContext DbContext
        {
            get { return Domain.Context; }
        }
    }
}
