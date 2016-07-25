using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace ODataSamples.Common.Model
{
    public class V4Model : ModelWrapper
    {
        public IEdmEntityType Product { get; private set; }
        public IEdmEntityType Supplier { get; private set; }
        public IEdmEntitySet ProductsSet { get; private set; }

        public V4Model()
        {
            Product = (IEdmEntityType)Model.FindType("ODataDemo.Product");
            Supplier = (IEdmEntityType)Model.FindType("ODataDemo.Supplier");
            ProductsSet = Model.FindDeclaredEntitySet("Products");
        }

        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ODataSamples.Common.Model.V4.xml");
        }
    }

    public class TripPinModel : ModelWrapper
    {
        public IEdmEntityType Person { get; private set; }

        public IEdmEntityType Trip { get; private set; }

        public IEdmEntitySet PeopleSet { get; private set; }

        public TripPinModel()
        {
            Person = (IEdmEntityType)Model.FindType("Microsoft.OData.SampleService.Models.TripPin.Person");
            Trip = (IEdmEntityType)Model.FindType("Microsoft.OData.SampleService.Models.TripPin.Trip");
            PeopleSet = Model.FindDeclaredEntitySet("People");
        }

        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ODataSamples.Common.Model.TripPin.xml");
        }
    }

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

    public class AnnotationModel : ModelWrapper
    {
        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ODataSamples.Common.Model.AnnotationModel.xml");
        }
    }

    public class SchoolModel : ModelWrapper
    {
        public IEdmEntityType Student { get; private set; }

        public IEdmEntityType Course { get; private set; }

        public IEdmEntityType Teacher { get; private set; }

        public IEdmEntitySet StudentSet { get; private set; }

        public IEdmEntitySet CourseSet { get; private set; }

        public IEdmEntitySet TeacherSet { get; private set; }

        public SchoolModel()
        {
            Student = (IEdmEntityType)Model.FindType("Microsoft.OData.SampleService.Models.School.Student");
            Course = (IEdmEntityType)Model.FindType("Microsoft.OData.SampleService.Models.School.Course");
            Teacher = (IEdmEntityType)Model.FindType("Microsoft.OData.SampleService.Models.School.Teacher");
            StudentSet = Model.FindDeclaredEntitySet("Students");
            CourseSet = Model.FindDeclaredEntitySet("Courses");
            TeacherSet = Model.FindDeclaredEntitySet("Teachers");
        }

        protected override IEdmModel GetModel()
        {
            return LoadMetadataFromResource("ODataSamples.Common.Model.School.xml");
        }
    }
}
