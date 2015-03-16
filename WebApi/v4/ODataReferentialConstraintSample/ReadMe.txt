ODataReferentialConstraintSample
------------------

Referential constraint is supported in OData WebApi v4, a constraint on the keys contained in the association type means referential constraint, these kind of key is foreign key.
 
1. Be able to configure the foreign key explicitly (HasRequired,HasOptional)
2. Be able to configure the foreign key using ForeignKeyAttribute
3. Be able to configure the foreign key convention
4. Be able to configure the multi-foreign key
5. Be able to set up the action On-Delete
6. Be able to present the referential constraint in metadata document

This sample illustrates how to:

1. Build DB context with referential constraint.
2. Build the edm model with referential constraint, set up the action On-Delete.
3. Verify the referential constraint in metadata ducument.
4. Monitor the action of Cascade Delete.

It is a console application, just open the solution and hit F5 to run it.

This sample is provided as part of the OData WebApi sample repository at
https://github.com/OData/ODataSamples