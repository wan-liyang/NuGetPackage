How to use HttpHandler helper class

1. add code below into ```Startup.cs >> ConfigureServices``` method

```
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
```

2. initialize helpers with our singleton HttpContextAccessor resolved by the built-in dependency injection container using app.ApplicationServices.GetRequiredService<IHttpContextAccessor>()

add code below into ```Startup.cs >> Configure``` method

```
HttpHandler.Config.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
```

add code below into ```Startup.cs >> Configure``` method, to config url query string encryption password

```
HttpHandler.Config.ConfigurePassword("password");
```


Access Url: https://localhost:44392/HttpHandlerTest?id=1

RequestUtility.Url.FullDisplayUrl : https://localhost:44392/HttpHandlerTest?id=1
RequestUtility.Url.FulEncodedUrl : https://localhost:44392/HttpHandlerTest?id=1

RequestUtility.Url.FullUrlWithoutQuery : https://localhost:44392/HttpHandlerTest

RequestUtility.Url.Scheme : https
RequestUtility.Url.Port : 44392
RequestUtility.Url.Host : https://localhost:44392
RequestUtility.Url.AppPath :
RequestUtility.Url.HostAndAppPath : https://localhost:44392
RequestUtility.Url.Page : /HttpHandlerTest
RequestUtility.Url.Query : ?id=1

RequestUtility.ClientIPAddress : ::1
RequestUtility.UserAgent : Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36
RequestUtility.SessionId :