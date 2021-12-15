
I had to separate the http client so we could build the .NET proj.
The Models are copied from the StreetPerfectClient shared proj.

The http client is basically built with ReFit - it handles everything.
I only added the dependency injection startup stuff and the token auto-refreshing.
