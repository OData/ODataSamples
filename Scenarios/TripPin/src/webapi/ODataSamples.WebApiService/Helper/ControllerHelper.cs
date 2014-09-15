namespace ODataSamples.WebApiService.Helper
{
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using System.Web.OData;

    public static class ControllerHelper
    {
        public static object GetPropertyValueFromModel(object instance, string propertyName)
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new HttpException("Don't find property with name:" + propertyName);
            }
            var propertyValue = propertyInfo.GetValue(instance, new object[] { });

            return propertyValue;
        }

        public static IHttpActionResult GetOKHttpActionResult(ODataController controller, object propertyValue)
        {
            var okMethod = default(MethodInfo);
            var methods = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.Name == "Ok" && method.GetParameters().Length == 1)
                {
                    okMethod = method;
                    break;
                }
            }

            okMethod = okMethod.MakeGenericMethod(propertyValue.GetType());
            var returnValue = okMethod.Invoke(controller, new object[] { propertyValue });
            return (IHttpActionResult)returnValue;
        }
    }
}