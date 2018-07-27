# ODataActionSample
-------------------------

This sample illustrates various ways to implement OData actions in ASP.NET Core.

The sample implements a simple movie rental scenario. The application displays a list of movies,
and the user can check out or return a movie. You can also add a movie to the list. 

Actions:

Check out a movie:  /odata/Movies(1)/ODataActionsSample.Models.CheckOut
Return a movie:     /odata/Movies(1)/ODataActionsSample.Models.Return
Check out sevaral:  /odata/Movies/ODataActionsSample.Models.CheckOutMany
Add a movie:        /odata/CreateMovie

These actions demonstrate several options for OData actions:

* Actions bound to an entity
* Actions bound to an entity collection
* Unbound actions


