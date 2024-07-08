// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.OData.Core;

    /// <summary>Action provider for the DSP. This also implements the <see cref="IDataServiceActionProvider"/>.</summary>
    public class DSPActionProvider : IDataServiceActionProvider
    {
        /// <summary>Service actions, the key is the name of the service action.</summary>
        Dictionary<string, ServiceAction> serviceActions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DSPActionProvider()
        {
            this.serviceActions = new Dictionary<string, ServiceAction>();
        }

        /// <summary>
        /// Adds a service action.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="action">The action to run.</param>
        /// <param name="returnResourceType">The return type.</param>
        /// <param name="bindingKind">The binding kind of the first parameter.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The new service action.</returns>
        public ServiceAction AddServiceAction(string name, Func<DSPUpdateProvider, IEnumerable<object>, object> action, ResourceType returnResourceType, OperationParameterBindingKind bindingKind, params ServiceActionParameter[] parameters)
        {
            if (returnResourceType != null && (returnResourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection || returnResourceType.ResourceTypeKind == ResourceTypeKind.EntityType))
            {
                throw new ArgumentException("The returnResourceType can only be primitive, complex or collection for now.");
            }

            ServiceAction serviceAction = new ServiceAction(name, returnResourceType, null, bindingKind, parameters);
            serviceAction.CustomState = new ServiceActionAnnotation() { Action = action };
            this.serviceActions.Add(name, serviceAction);
            serviceAction.SetReadOnly();
            return serviceAction;
        }

        #region IDataServiceActionProvider Members

        /// <summary>
        /// Determines if the specified service action should be advertised on the specified resource.
        /// </summary>
        /// <param name="operationContext">The operation context for the current request.</param>
        /// <param name="serviceAction">The service action in question.</param>
        /// <param name="resourceInstance">The resource instance for the action binding.</param>
        /// <param name="resourceInstanceInFeed">true if the resource is from a feed.</param>
        /// <param name="actionToSerialize">The action representation to write.</param>
        /// <returns>true if the action should be advertised, false otherwise.</returns>
        public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            // For now always return true.
            return true;
        }

        /// <summary>
        /// Returns invokable which represents an invokation of an action.
        /// </summary>
        /// <param name="operationContext">The operation context to for the service action.</param>
        /// <param name="serviceAction">The service action to invoke.</param>
        /// <param name="parameterTokens">The parameter tokens.</param>
        /// <returns>The invokable.</returns>
        public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
        {
            DSPUpdateProvider updateProvider = operationContext.GetService(typeof(IDataServiceUpdateProvider)) as DSPUpdateProvider;
            return new DSPInvokable(serviceAction, updateProvider, parameterTokens);
        }

        /// <summary>
        /// Returns all service actions.
        /// </summary>
        /// <param name="operationContext">The operation context to get service actions for.</param>
        /// <returns>Enumeration of service actions.</returns>
        public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
        {
            return this.serviceActions.Values;
        }

        /// <summary>
        /// Returns service actions for the binding parameter type.
        /// </summary>
        /// <param name="operationContext">The operation context to get service actions for.</param>
        /// <param name="bindingParameterType">The type of the binding parameter.</param>
        /// <returns>List of actions which bind to the specified type.</returns>
        public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
        {
            return this.serviceActions.Values.Where(a => a.Parameters.Count > 0 && a.Parameters.First().ParameterType == bindingParameterType);
        }

        /// <summary>
        /// Tries to resolve action by its name.
        /// </summary>
        /// <param name="operationContext">The operation context to get service actions for.</param>
        /// <param name="serviceActionName">The name of the action to lookup.</param>
        /// <param name="serviceAction">The service action found, or null otherwise.</param>
        /// <returns>true if the specified service action was found, false otherwise.</returns>
        public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
        {
            return this.serviceActions.TryGetValue(serviceActionName, out serviceAction);
        }

        #endregion
    }
}
