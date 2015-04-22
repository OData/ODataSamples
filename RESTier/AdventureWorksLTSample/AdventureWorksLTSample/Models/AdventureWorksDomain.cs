using Microsoft.Restier.Conventions;
using Microsoft.Restier.Core;
using Microsoft.Restier.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorksLTSample.Models
{
    [EnableConventions]
    public class AdventureWorksDomain : DbDomain<AdventureWorksContext>
    {
        public AdventureWorksContext Context
        {
            get { return DbContext; }
        }

        #region Products entity set
        private IQueryable<Product> OnFilterProducts(IQueryable<Product> entitySet)
        {
            return entitySet.Where(s => s.ProductID % 3 == 0).AsQueryable();
        }

        public async Task<int> GetProductsCountAsync()
        {
            return await OnFilterProducts(this.Context.Products).CountAsync();
        }
        #endregion

        #region imperative view of ColoredProducts entity set
        /// <summary>
        /// Imperative view (invoking both OnFilterProducts & OnFilterColoredProducts)
        /// </summary>
        protected IQueryable<Product> ColoredProducts
        {
            get
            {
                // OnFilterProducts will be applied first
                return this.Source<Product>("Products").Where(s => !string.IsNullOrEmpty(s.Color));
            }
        }

        private IQueryable<Product> OnFilterColoredProducts(IQueryable<Product> entitySet)
        {
            return entitySet.Where(s => s.ProductID % 4 == 0).AsQueryable();
        }

        public async Task<int> GetColoredProductsCountAsync()
        {
            // here duplicate the 'ColoredProducts' logic because 'ColoredProducts' returns an expression that
            // needs to be 'sourced' by entity framework provider instead directly returning results.
            var products = OnFilterProducts(this.Context.Products);
            var coloredProducts = products.Where(s => !string.IsNullOrEmpty(s.Color));
            return await OnFilterColoredProducts(coloredProducts).CountAsync();
        }
        #endregion
    }
}