using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace ODataSamples.Common.Model
{
    public class ParserExtModel : ModelWrapper
    {
        public IEdmType Person;
        public IEdmType Pet;
        public IEdmType Fish;
        public IEdmEntitySet People;
        public IEdmEntitySet PetSet;

        public ParserExtModel()
        {
            Person = Model.FindType("TestNS.Person");
            Pet = Model.FindType("TestNS.Pet");
            Fish = Model.FindType("TestNS.Fish");
            People = Model.FindDeclaredEntitySet("People");
            PetSet = Model.FindDeclaredEntitySet("PetSet");
        }

        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ODataSamples.Common.Model.ParserExtModel.xml");
        }
    }

    class CraftModel : ModelWrapper
    {
        private EdmModel model;
        public CraftModel()
        {
            model = new EdmModel();
            var person = new EdmEntityType("NS", "Person");
            var address = new EdmComplexType("NS", "Address");
            person.AddStructuralProperty("Addr", new EdmComplexTypeReference(address, /*Nullable*/false));
            model.AddElement(person);
            model.AddElement(address);
        }

        protected override IEdmModel GetModel()
        {
            return this.model;
        }
    }
}
