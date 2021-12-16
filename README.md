# StreetPerfectHttpClientNet
StreetPerfect C# HTTP Client


The http client has been separated from the native client to make things easier for everyone.
The Models are now a sub probject for both teh http and native clients.

The http client is basically built with ReFit - it handles everything.
I only added the dependency injection startup stuff and the token auto-refreshing.
