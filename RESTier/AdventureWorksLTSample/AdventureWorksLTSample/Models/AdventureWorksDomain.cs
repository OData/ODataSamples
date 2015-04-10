using Microsoft.Restier.EntityFramework;

namespace AdventureWorksLTSample.Models
{
    public class AdventureWorksDomain : DbDomain<AdventureWorksContext>
    {
        public AdventureWorksContext Context
        {
            get { return DbContext; }
        }
    }
}