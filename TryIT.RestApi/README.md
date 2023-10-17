## RestApi method

```
using TryIT.RestApi;

Api api = new Api(new ApiConfig
{
    HttpClient = new HttpClient()
    {
        Timeout = TimeSpan.FromSeconds(10)
    },
    EnableRetry = true
});

var response = await api.GetAsync("https://localhost:8080/weatherforecast");
```