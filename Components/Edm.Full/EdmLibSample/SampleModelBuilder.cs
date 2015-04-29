using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace EdmLibSample
{
    public class SampleModelBuilder
    {
        private readonly EdmModel _model = new EdmModel();
        public IEdmModel GetModel()
        {
            return _model;
        }
    }
}
