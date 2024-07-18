// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services;
    using System.Data.Services.Providers;
    using System.Linq;
    using System.Reflection;
    using System.Diagnostics;

    /// <summary>Implements the <see cref="IDataServiceUpdateProvider"/>.</summary>
    /// <remarks>All the changes requested by calling method on this class are just remembered in a list of pending actions
    /// which are only applied once the SaveChanges method is called.
    /// Note that this class implements support for updating resource reference and resource reference set properties
    /// but it treats each such property on its own. We don't support bi-directional links or relationships.
    /// So for example if there's a resource reference from Product to its Category and a resource reference set from Category to its products
    /// and a Product is modified to reference a certain Category, that Category will not be automatically modifies to include the Product in its
    /// list of products.</remarks>
    public class DSPUpdateProvider : IDataServiceUpdateProvider2
    {
        /// <summary>The data context to apply the change to.</summary>
        private DSPContext dataContext;

        /// <summary>The metadata describing the types to work with.</summary>
        private DSPMetadata metadata;

        /// <summary>List of pending changes to apply once the <see cref="SaveChanges"/> is called.</summary>
        /// <remarks>This is a list of actions which will be called to apply the changes. Discarding the changes is done
        /// simply by clearing this list.</remarks>
        private List<Action> pendingChanges;

        /// <summary>List of action to invoke.</summary>
        private List<IDataServiceInvokable> actionsToInvoke;

        /// <summary>Constructor.</summary>
        /// <param name="dataContext">The data context to apply the changes to.</param>
        /// <param name="metadata">The metadata describing the types to work with.</param>
        public DSPUpdateProvider(DSPContext dataContext, DSPMetadata metadata)
        {
            this.dataContext = dataContext;
            this.metadata = metadata;
            this.pendingChanges = new List<Action>();
            this.actionsToInvoke = new List<IDataServiceInvokable>();
        }

        /// <summary>The data context to apply the change to.</summary>
        protected DSPContext DataContext
        {
            get { return this.dataContext; }
        }

        /// <summary>The metadata describing the types to work with.</summary>
        protected DSPMetadata Metadata
        {
            get { return this.metadata; }
        }
        
        /// <summary>
        /// Get the association defined on the source resource's specified property, and return the other end of the association
        /// </summary>
        /// <param name="sourceResource">The source resource</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>The other end of the association</returns>
        private static ResourceAssociationSetEnd GetAssociationRelatedEnd(DSPResource sourceResource, string propertyName)
        {
            ResourceProperty targetProperty = sourceResource.ResourceType.Properties.FirstOrDefault(p => p.Name == propertyName);
            Debug.Assert(targetProperty != null, "Property with name " + propertyName + " cannot be found");

            ResourceAssociationSet associationSet = targetProperty.GetAnnotation().ResourceAssociationSet;

            // Detect which end of the association is the target property
            //  - ResourceType and ResourceProperty must match
            //  - Association between the same resource type and property should not be allowed
            // Note that we can have the same resource property declared on two different types and have association between them
            return (associationSet.End1.ResourceType == sourceResource.ResourceType
                && associationSet.End1.ResourceProperty == targetProperty) ? associationSet.End2 : associationSet.End1;
        }

        /// <summary>Validates that the <paramref name="resource"/> is a <see cref="DSPResource"/>.</summary>
        /// <param name="resource">The resource instance to validate.</param>
        /// <returns>The resource instance casted to <see cref="DSPResource"/>.</returns>
        public static DSPResource ValidateDSPResource(object resource)
        {
            DSPResource dspResource = resource as DSPResource;
            if (resource != null && dspResource == null)
            {
                throw new ArgumentException("The specified resource is not a DSPResource. That is not supported by this provider.");
            }

            return dspResource;
        }

        /// <summary>
        /// Apply default values (if any) to resource. This is called after a new resource is created, or after a resource is reset
        /// </summary>
        /// <param name="resource">The resource to apply default values to</param>
        protected virtual void ApplyResourceDefaultValue(DSPResource resource)
        {
            if (resource != null)
            {
                foreach (var prop in resource.ResourceType.Properties)
                {
                    if (prop.Kind == ResourcePropertyKind.ResourceSetReference)
                    {
                        // navigation collections must always have value
                        resource.SetValue(prop.Name, new List<DSPResource>());
                    }
                }
            }
        }

        #region IUpdatable Members

        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        /// <remarks>Adds resource instance <paramref name="resourceToBeAdded"/> into a resource reference set
        /// <paramref name="propertyName"/> on resource <paramref name="targetResource"/>.
        /// Both resources, that is <paramref name="targetResource"/> and <paramref name="resourceToBeAdded"/>, are specified 
        /// in the parameters as the resource "handle".
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            // We don't use resource "handles" so both resources passed in as parameters are the real resource instances.
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);
            DSPResource dspResourceToBeAdded = ValidateDSPResource(resourceToBeAdded);
            ResourceAssociationSetEnd relatedEnd = GetAssociationRelatedEnd(dspTargetResource, propertyName);

            // this association is bi-directional if otherEnd.ResourceProperty is not null
            // We choose not to support * - * relationship fix up here
            if (relatedEnd.ResourceProperty != null && relatedEnd.ResourceProperty.Kind == ResourcePropertyKind.ResourceReference)
            {
                // for * - 1 bi-directional relationship, we use the fix-up logic existing in SetReference
                // simply turn this call into 1 - * set reference call
                this.SetReference(dspResourceToBeAdded, relatedEnd.ResourceProperty.Name, dspTargetResource);
            }
            else
            {
                // all other scenario: no fix up needed
                this.InternalAddReferenceToCollection(dspTargetResource, propertyName, dspResourceToBeAdded);
            }
        }
        
        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        /// <remarks>This method gets called if there was some problem applying changes specified by the request and the changes need to be reverted.
        /// All changes made by the methods on this class should be reverted when this method returns. Note that the class might get used to perform some more
        /// changes after this call.</remarks>
        public void ClearChanges()
        {
            // Simply clear the list of pending changes and actions
            this.actionsToInvoke.Clear();
            this.pendingChanges.Clear();
        }

        /// <summary>
        /// Creates the resource of the given type and belonging to the given container
        /// </summary>
        /// <param name="containerName">container name to which the resource needs to be added</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and belonging to the given container</returns>
        /// <remarks>The method should create a new instance of the resource type specified by the <paramref name="fullTypeName"/> 
        /// and add it to the resource set specified by the <paramref name="containerName"/>.
        /// The method should then return the "handle" to the resource just created.
        /// All properties of the new resource should have their default values.
        /// This method is called in two slightly different cases:
        ///   - entity resource creation - in this case the <paramref name="containerName"/> specifies the name of the resource set
        ///       the newly created entity should be added to and the <paramref name="fullTypeName"/> is the FullName of the resource type representing
        ///       entity type. The method should create new instance of the type and add it to the resource set.
        ///   - complex resource creation - in this case the <paramref name="containerName"/> is null and the <paramref name="fullTypeName"/>
        ///       specified the FullName of the resource type representing a complex type. The method should just create new instance of the type
        ///       and return it. Later the <see cref="SetValue"/> will be called to set the complex type instance returned as a value of some
        ///       complex property.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            ResourceType resourceType;
            if (!this.metadata.TryResolveResourceType(fullTypeName, out resourceType))
            {
                throw new ArgumentException("Unknown resource type '" + fullTypeName + "'.");
            }

            // Create new instance of the DSPResource (this will create empty property bag, which is the same as all properties having default values)
            DSPResource newResource = new DSPResource(resourceType);
            this.ApplyResourceDefaultValue(newResource);

            if (containerName != null)
            {
                // We're creating an entity and should add it to the resource set
                // This check here is just for documentation, the method should never be called with non-entity type in this case.
                if (resourceType.ResourceTypeKind != ResourceTypeKind.EntityType)
                {
                    throw new ArgumentException("The specified resource type '" + fullTypeName + "' is not an entity type, but resource set was specified.");
                }

                IList<DSPResource> resourceSetList = this.dataContext.GetResourceSetEntities(containerName);

                // And register pending change to add the resource to the resource set list
                this.pendingChanges.Add(() =>
                {
                    resourceSetList.Add(newResource);
                });
            }
            else
            {
                // We're creating a complex type instance, so no additional operation is needed.
                // This check here is just for documentation the method should never be called with non-complex type in this case.
                if (resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                {
                    throw new ArgumentException("The specified resource type '" + fullTypeName + "' is not a complex type.");
                }
            }

            // The method should return the resource "handle", we don't have handles so we return the resource itself directly.
            return newResource;
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="targetResource">resource that needs to be deleted</param>
        /// <remarks>This method gets a "handle" to a resource in the <paramref name="targetResource"/> and should pend a change which
        /// deletes that resource.
        /// That includes removing the resource from its resource set and freeing up all the resources associated with that resource.
        /// Note that this method is not called for complex type instances, only entity resurces are deleted in this way. Complex type instances 
        /// should be deleted when the entity type which points to them is deleted.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void DeleteResource(object targetResource)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            ResourceSet resourceSet = dspTargetResource.ResourceType.GetAnnotation().ResourceSet;
            IList<DSPResource> resourceSetList = this.dataContext.GetResourceSetEntities(resourceSet.Name);

            // Add a pending change to remove the resource from the resource set
            this.pendingChanges.Add(() =>
            {
                resourceSetList.Remove(dspTargetResource);
            });
        }

        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        /// <remarks>This method should obtain a single result from the specified <paramref name="query"/>. It should fail if no or more than one result
        /// can be obtain by evaluating such query.
        /// The result should then be converted to its resource "handle" and that handle should be returned from the method.
        /// The <paramref name="fullTypeName"/> is the expected FullName of the resource type of the resource to be retrieved. If this parameter is null
        /// the method should ignore it. If it's not null, the method should check that the resource returned by the query is of this resource type
        /// and fail if that's not the case.</remarks>
        public virtual object GetResource(System.Linq.IQueryable query, string fullTypeName)
        {
            // Since we're not using resource handles we're going to return the resource itself.
            object resource = null;
            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                }

                resource = r;
            }

            if (resource != null)
            {
                if (fullTypeName != null)
                {
                    ResourceType resourceType;
                    if (!this.metadata.TryResolveResourceType(fullTypeName, out resourceType))
                    {
                        throw new ArgumentException("Unknown resource type '" + fullTypeName + "'.");
                    }

                    if (resource.GetType() != resourceType.InstanceType)
                    {
                        throw new System.ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
                    }
                }

                return resource;
            }

            return null;
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        /// <remarks>The method gets a resource "handle" in the <paramref name="targetResource"/> and the name of a resource property
        /// defined on it and should return the value of that property.</remarks>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            return dspTargetResource.GetValue(propertyName);
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        /// <remarks>Removes resource instance <paramref name="resourceToBeRemoved"/> from a resource reference set
        /// <paramref name="propertyName"/> on resource <paramref name="targetResource"/>.
        /// Both resources, that is <paramref name="targetResource"/> and <paramref name="resourceToBeRemoved"/>, are specified 
        /// in the parameters as the resource "handle".
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            // We don't use resource "handles" so both resources passed in as parameters are the real resource instances.
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);
            DSPResource dspResourceToBeRemoved = ValidateDSPResource(resourceToBeRemoved);
            ResourceAssociationSetEnd relatedEnd = GetAssociationRelatedEnd(dspTargetResource, propertyName);

            // this association is bi-directional if otherEnd.ResourceProperty is not null
            // We choose not to support * - * relationship fix up here
            if (relatedEnd.ResourceProperty != null && relatedEnd.ResourceProperty.Kind == ResourcePropertyKind.ResourceReference)
            {
                // we should null the reference on the other end.
                this.SetValue(dspResourceToBeRemoved, relatedEnd.ResourceProperty.Name, null);
            }

            InternalRemoveReferenceFromCollection(dspTargetResource, propertyName, dspResourceToBeRemoved);
        }
        
        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        /// <remarks>This method should reset resource properties to their default values.
        /// The resource is specfied by its resource "handle" by the <paramref name="resource"/>.
        /// The method can choose to modify the existing resource or create a new one and it should return a resource "handle"
        /// to the resource which has its properties with default values. If it chooses to return a new resource it must also
        /// replace that old resource with the new one in its resource set and all the references which may point to it.
        /// The returned resource must have the same identity as the one on the input. That means all its key properties must have the same value.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual object ResetResource(object resource)
        {
            DSPResource dspResource = ValidateDSPResource(resource);

            this.pendingChanges.Add(() =>
            {
                foreach (var resourceProperty in dspResource.ResourceType.Properties)
                {
                    // We must only reset values of non-key properties, the key properties must be persited (to maintain the identity of the resource instance)
                    if ((resourceProperty.Kind & ResourcePropertyKind.Key) != ResourcePropertyKind.Key)
                    {
                        dspResource.SetValue(resourceProperty.Name, null);
                    }
                }

                this.ApplyResourceDefaultValue(dspResource);
            });

            return resource;
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public object ResolveResource(object resource)
        {
            // We're not using resource handles, so the resource is also the handle itself
            // It is possible to represent resources with "handles" here instead. This method is meant to translate the resource handle 
            // passed in the parameter "resource" to the underlying resource instance.
            return resource;
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        /// <remarks>All changes made by methods on this class should not be persisted until this SaveChanges method gets called.
        /// After this method returns the changes should be persited in the underlying data storage.
        /// Note that this class might be used to perform additional update operations after this method is called.</remarks>
        public void SaveChanges()
        {
            // Invoke all actions
            foreach (var action in this.actionsToInvoke)
            {
                action.Invoke();
            }

            this.actionsToInvoke.Clear();

            // Just run all the pending changes we gathered so far
            foreach (var pendingChange in this.pendingChanges)
            {
                pendingChange();
            }

            this.pendingChanges.Clear();
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="relatedResource">value of the property</param>
        /// <remarks>Sets a new value for a resource reference property.
        /// - Create new reference if the old value of the reference property was null and the new value <paramref name="relatedResource"/> is not-null.
        /// - Update the reference if the old value of the reference property was non-null and the new value <paramref name="relatedResource"/> is not-null.
        /// - Remove the reference if the old value of the reference property was non-null and the new value <paramref name="relatedResource"/> is null.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void SetReference(object targetResource, string propertyName, object relatedResource)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);
            ResourceAssociationSetEnd relatedEnd = GetAssociationRelatedEnd(dspTargetResource, propertyName);

            if (relatedEnd.ResourceProperty != null)
            {
                // this is a bi-directional relationship
                // fix up logic:

                DSPResource existingRelatedResource = dspTargetResource.GetValue(propertyName) as DSPResource;

                if (relatedEnd.ResourceProperty.Kind == ResourcePropertyKind.ResourceReference)
                {
                    // this is a 1 - 1 relationship:
                    // Target may already be in a relationship.
                    // Null the existing back pointer
                    if (existingRelatedResource != null)
                    {
                        this.SetValue(existingRelatedResource, relatedEnd.ResourceProperty.Name, null);
                    }

                    if (relatedResource != null)
                    {
                        // relatedResource may be in a 1 - 1 relationship as well
                        // if so recursively null its back pointers, since it can only be related to a single entity
                        // if it's in a 1 - * relationship, then we don't care
                        this.SetReference(relatedResource, relatedEnd.ResourceProperty.Name, null);

                        // set relatedResource's back pointer. This should be done separated from above
                        this.SetValue(relatedResource, relatedEnd.ResourceProperty.Name, targetResource);
                    }
                }
                else
                {
                    // 1 - * relationship
                    if (existingRelatedResource != null)
                    {
                        // remove from collection
                        InternalRemoveReferenceFromCollection(existingRelatedResource, relatedEnd.ResourceProperty.Name, dspTargetResource);
                    }

                    if (relatedResource != null)
                    {
                        this.InternalAddReferenceToCollection(relatedResource, relatedEnd.ResourceProperty.Name, dspTargetResource);
                    }
                }
            }

            // Our reference properties are just like normal properties we just set the property value to the new value
            //   we don't perform any special actions for references.
            // So just call the SetValue which will do exactly that.
            this.SetValue(targetResource, propertyName, relatedResource);
        }

        /// <summary>
        /// Sets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        /// <remarks>This method should pend a change which will set a resource property with name <paramref name="propertyName"/>
        /// to value specified by <paramref name="propertyValue"/> on the resource specified by the resource "handle" <paramref name="targetResource"/>.
        /// All changes made by this method should be creates as pending until SaveChanges is called which will commit them (or if it's not called and ClearChanges
        /// is called instead they should be discarded).</remarks>
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            DSPResource dspTargetResource = ValidateDSPResource(targetResource);

            // When using JSON to send updates, if the server does not understand the value been set,
            // it will pass it to the provider directly. We may get a "JsonObjectRecords" that represent
            // the property value sent by the request. We should block this as we are not in the business
            // of parsing JSON inputs.
            if (propertyValue != null && (propertyValue.GetType().Name == "JsonObjectRecords" || propertyValue.GetType().Name == "Hashtable"))
            {
                throw new DataServiceException(500, "Invalid value set for property \"" + propertyName + "\"");
            }

            // Add a pending change to modify the value of the property
            this.pendingChanges.Add(() =>
            {
                dspTargetResource.SetValue(propertyName, propertyValue);
            });
        }

        #endregion

        #region IDataServiceUpdateProvider Members

        /// <summary>
        /// Check concurrency values (Etags) for consistency. When there is a mismatch, fail the operation
        /// </summary>
        /// <param name="resourceCookie">The resource to be checked</param>
        /// <param name="checkForEquality">Whether we should check for equality (In this provider we will always check for equality)</param>
        /// <param name="concurrencyValues">A sequence of concurrency values to be checked.</param>
        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> concurrencyValues)
        {
            if (checkForEquality == null)
            {
                throw new DataServiceException(400, "Missing If-Match header");
            }

            foreach (var concurrencyToken in concurrencyValues)
            {
                object value = this.GetValue(resourceCookie, concurrencyToken.Key);
                if (!Object.Equals(value, concurrencyToken.Value))
                {
                    throw new DataServiceException(412, String.Format("Concurrency: precondition failed for property '{0}'", concurrencyToken.Key));
                }
            }
        }

        #endregion

        #region IDataServiceUpdateProvider2 Members

        /// <summary>
        /// Schedules an action for invokation.
        /// </summary>
        /// <param name="invokable">The invokable instance to invoke upon SaveChanges.</param>
        public void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            this.actionsToInvoke.Add(invokable);
        }

        #endregion

        /// <summary>
        /// Handle the task of adding a resource to a collection. This method does not enforce cascade update rules.
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">property name</param>
        /// <param name="dspResourceToBeAdded">value of the property which needs to be added</param>
        private void InternalAddReferenceToCollection(object targetResource, string propertyName, DSPResource dspResourceToBeAdded)
        {
            // All resource set reference properties must be of type IList<DSPResource> (assumption of this provider)
            // Note that we don't support bi-directional relationships so we only handle the one resource set reference property in isolation.

            IList<DSPResource> list = this.GetValue(targetResource, propertyName) as IList<DSPResource>;
            if (list == null)
            {
                throw new ArgumentException("The value of the property '" + propertyName + "' does not implement IList<DSPResource>, which is a requirement for resource set reference property.");
            }

            this.pendingChanges.Add(() =>
            {
                list.Add(dspResourceToBeAdded);
            });
        }

        /// <summary>
        /// Handle the task of removing a resource to a collection. This method does not enforce cascade update rules.
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">property name</param>
        /// <param name="dspResourceToBeAdded">value of the property which needs to be added</param>
        private void InternalRemoveReferenceFromCollection(object targetResource, string propertyName, DSPResource dspResourceToBeRemoved)
        {
            // We will use the GetValue we already implement to get the IList
            IList<DSPResource> list = this.GetValue(targetResource, propertyName) as IList<DSPResource>;
            if (list == null)
            {
                throw new ArgumentException("The value of the property '" + propertyName + "' does not implement IList<DSPResource>, which is a requirement for resource set reference property.");
            }

            this.pendingChanges.Add(() =>
            {
                list.Remove(dspResourceToBeRemoved);
            });
        }
    }
}
