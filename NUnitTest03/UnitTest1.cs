namespace NUnitTest03
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            TryIT.RestApi.Api api = new TryIT.RestApi.Api(new TryIT.RestApi.Models.HttpClientConfig
            {
                //HttpLogDelegate = async ctx =>
                //{
                //    if (ctx.Exception != null)
                //        Console.WriteLine($"[Error] {ctx.StartTimeUtc} {ctx.EndTimeUtc} {ctx.Stage} {ctx.Method} {ctx.Url} -> {ctx.Exception.Message}");
                //    else if (ctx.Response != null)
                //        Console.WriteLine($"[Response] {ctx.StartTimeUtc} {ctx.EndTimeUtc} {ctx.Stage} {ctx.Method} {ctx.Url} -> {ctx.Response.StatusCode}");
                //    else
                //        Console.WriteLine($"[Request] {ctx.StartTimeUtc} {ctx.EndTimeUtc} {ctx.Stage} {ctx.Method} {ctx.Url}");

                //    await Task.CompletedTask; // keep async signature
                //}
            });

            await api.GetAsync("https://www.google.com");

            Assert.Pass();
        }
    }
}