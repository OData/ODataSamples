// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV3
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services;
    using System.Data.Services.Providers;
    using System.Linq;

    /// <summary>Invokable instance for the DSP. This also implements the <see cref="IDataServiceInvokable"/>.</summary>
    public class DSPInvokable : IDataServiceInvokable
    {
        /// <summary>The service action to run.</summary>
        ServiceAction serviceAction;

        /// <summary>The update provider to use.</summary>
        DSPUpdateProvider updateProvider;

        /// <summary>The parameter tokens.</summary>
        IEnumerable<object> parameterTokens;

        /// <summary>Set to true once the invokable was invoked.</summary>
        bool invoked;

        /// <summary>The result of the invokable.</summary>
        object result;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="updateProvider">The update provider to use.</param>
        /// <param name="parameterTokens">The parameter tokens for the action.</param>
        public DSPInvokable(ServiceAction serviceAction, DSPUpdateProvider updateProvider, IEnumerable<object> parameterTokens)
        {
            this.serviceAction = serviceAction;
            this.updateProvider = updateProvider;
            this.parameterTokens = parameterTokens;
        }

        #region IDataServiceInvokable Members

        /// <summary>
        /// Returns the result of the invokation.
        /// </summary>
        /// <returns>The result of the invokation.</returns>
        public object GetResult()
        {
            if (!this.invoked)
            {
                throw new InvalidOperationException("The service action has not been invoked yet and we're asked for the result.");
            }

            return this.result;
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        public void Invoke()
        {
            try
            {
                if (this.invoked)
                {
                    throw new DataServiceException(
                        500,
                        string.Format("Action {0} was executed twice in one request.", this.serviceAction.Name));
                }

                this.result = this.serviceAction.GetAnnotation().Action(this.updateProvider, this.MarshalParameters(this.parameterTokens).ToArray());
                this.invoked = true;
            }
            catch (Exception e)
            {
                throw new DataServiceException(
                    500,
                    /*errorCode*/ null,
                    string.Format("Exception executing action {0}", this.serviceAction.Name),
                    /*messageXmlLang*/ null,
                    e);
            }
        }

        /// <summary>
        /// Marshals parameters to get more friendly values for execution.
        /// </summary>
        /// <param name="parametersTokens">The parameter tokens to marshal.</param>
        /// <returns>The marshalled parameters.</returns>
        private IEnumerable<object> MarshalParameters(IEnumerable<object> parameterTokens)
        {
            var typedParameters = this.serviceAction.Parameters.Zip(parameterTokens, (actionParameter, parameterToken) => new { Parameter = actionParameter, Token = parameterToken });
            foreach (var typedParameter in typedParameters)
            {
                if (typedParameter.Parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    yield return this.updateProvider.GetResource((IQueryable)typedParameter.Token, typedParameter.Parameter.ParameterType.FullName);
                }
                else if (typedParameter.Parameter.ParameterType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
                {
                    // TODO: The entities here need to be retrieved by running IQueryable similarly to the EntityType case above.
                    throw new NotImplementedException("Support for entity collection parameters is not implemented yet.");
                }
                else
                {
                    yield return typedParameter.Token;
                }
            }
        }

        #endregion
    }
}
