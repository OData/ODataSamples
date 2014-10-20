using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace ODataSamples.Common.Model
{
    public class ParserExtModel : ModelWrapper
    {
        public IEdmEntityType Person;
        public IEdmType Pet;
        public IEdmType Fish;
        public IEdmEntitySet People;
        public IEdmEntitySet PetSet;

        public ParserExtModel()
        {
            Person = (IEdmEntityType)Model.FindType("TestNS.Person");
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

    public class CraftModel : ModelWrapper
    {
        private EdmModel model;

        public EdmSingleton MyLogin;
        public EdmNavigationProperty MailBox;

        public CraftModel()
        {
            model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            model.AddElement(address);

            var mail = new EdmEntityType("NS", "Mail");
            var mailId = mail.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            mail.AddKeys(mailId);
            model.AddElement(mail);

            var person = new EdmEntityType("NS", "Person");
            model.AddElement(person);
            var personId = person.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            person.AddKeys(personId);

            person.AddStructuralProperty("Addr", new EdmComplexTypeReference(address, /*Nullable*/false));
            MailBox = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                ContainsTarget = true,
                Name = "Mails",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = mail,
            });


            var container = new EdmEntityContainer("NS", "DefaultContainer");
            model.AddElement(container);
            MyLogin = container.AddSingleton("MyLogin", person);
        }

        protected override IEdmModel GetModel()
        {
            return this.model;
        }
    }
}
