using Microsoft.OData.Edm;

namespace ParserExt
{
    class ParserExtModel : ModelWrapper
    {
        public IEdmType Person;
        public IEdmType Pet;
        public IEdmEntitySet People;
        public IEdmEntitySet PetSet;

        public ParserExtModel()
        {
            Person = Model.FindType("TestNS.Person");
            Pet = Model.FindType("TestNS.Pet");
            People = Model.FindDeclaredEntitySet("People");
            PetSet = Model.FindDeclaredEntitySet("PetSet");
        }

        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ParserExt.ParserExtModel.xml");
        }
    }
}
