using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace ODataSamples.Common.Model
{
    public abstract class ModelWrapper
    {
        private IEdmModel _model;

        public IEdmModel Model
        {
            get { return _model ?? (_model = GetModel()); }
        }

        protected abstract IEdmModel GetModel();

        protected static IEdmModel LoadMetadataFromResource(string resourceName)
        {
            IEdmModel model;
            Stream edmxStream = null;

            try
            {
                edmxStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (edmxStream == null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("Failed to get stream with name '{0}', available resource names:", resourceName);
                    foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                    {
                        builder.AppendLine(name);
                    }
                    
                    throw new ApplicationException(builder.ToString());
                }

                Debug.Assert(edmxStream != null, "stream must exist.");

                using (var xmlReader = XmlReader.Create(edmxStream))
                {
                    IEnumerable<EdmError> errors;
                    bool valid = EdmxReader.TryParse(xmlReader, out model, out errors);
                    if (!valid)
                    {
                        ShowErrors(errors);
                    }
                    Debug.Assert(valid, "model should be parsed");

                    valid = model.Validate(out errors);
                    if (!valid)
                    {
                        // ShowErrors(errors);
                    }

                    //Debug.Assert(valid, "should not have semantic errors");
                }
            }
            finally
            {
                if (edmxStream != null) edmxStream.Dispose();
            }

            return model;
        }

        private static void ShowErrors(IEnumerable<EdmError> errors)
        {
            foreach (var edmError in errors)
            {
                Console.WriteLine("{0} - {1}", edmError.ErrorLocation, edmError.ErrorMessage);
            }
        }
    }
}
