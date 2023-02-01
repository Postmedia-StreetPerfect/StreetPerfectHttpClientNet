# StreetPerfectHttpClientNet
StreetPerfect C# HTTP Client


The http client has been separated from the native client to make things easier for everyone.
The Models are now a sub probject for both the http and native clients.

The http client is basically built with ReFit - it handles everything.
I only added the dependency injection startup stuff and the token auto-refreshing.

To use, reference the namespace;

using StreetPerfect.Http;

Then add the client using the extension method. Simply pass your client ID and secret (API key). Note the ClientId is now optional BUT must not be null or empty. 

```C#
builder.Services.AddStreetPerfectClient(c =>
{
	c.ClientId = appSettings.SPRestConfig?.ClientId;
	c.ClientSecret = appSettings.SPRestConfig?.ClientSecret;
	c.UseDevEndpoint = true;
});
```

Inject the HTTP client like you would the native client

```C#
private readonly ILogger<HomeController> _logger;
private readonly IStreetPerfectHttpClient _Client;
public HomeController(IStreetPerfectHttpClient Client, ILogger<HomeController> logger)
{
  _logger = logger;
  _Client = Client;
}
```
 
And use the HTTP client (almost) exactly like the native client - all HTTP client calls are asynchronous

```C#
public async Task<IActionResult> Index()
{
    ViewData["info"] = await _Client.GetInfo();
    return View();
}
       
```
 
You can also use the HTTP client to connect to your own HTTP enabled StreetPerfect server.

```C#
builder.Services.AddStreetPerfectClient(c =>
{
	c.BaseAddress = "http://localhost:8888"; //note /api/1 will be appeneded
});
```
