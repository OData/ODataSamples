# ODataActionSample
-------------------------

This sample illustrates various ways to implement OData actions in Web API.

The sample implements a simple movie rental scenario. The application displays a list of movies,
and the user can check out or return a movie. You can also add a movie to the list. 

Actions:

* Check out a movie:  `/odata/Movies(1)/ODataActionsSample.Models.CheckOut`
* Return a movie:     `/odata/Movies(1)/ODataActionsSample.Models.Return`
* Check out sevaral:  `/odata/Movies/ODataActionsSample.Models.CheckOutMany`
* Add a movie:        `/odata/CreateMovie`

These actions demonstrate several options for OData actions:

* Actions bound to an entity
* Actions bound to an entity collection
* Unbound actions

**Be noted**: if it's hosted using IIS, the "." in the request URI could be a problem. In this case, use the non-namespace qualified action call.

Actions:

* Check out a movie:  `/odata/Movies(1)/CheckOut`
* Return a movie:     `/odata/Movies(1)/Return`
* Check out sevaral:  `/odata/Movies/CheckOutMany`
* Add a movie:        `/odata/CreateMovie`

# Demo

If run it, you can see a Html page, on it you can check out, return, check out many or add new movie.

<img width="890" alt="image" src="https://user-images.githubusercontent.com/9426627/199660620-9d8b4bf8-2a14-49df-b3b0-6b8fb4fb7bf8.png">
