using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;

using static Newtonsoft.Json.JsonConvert;
using static RequestType;

public class ServerFixture {
  readonly TestServer _server;
  public ServerFixture() => _server = new TestServer(Alice.CreateWebHostBuilder(null));
  public HttpClient Client => _server.CreateClient();
}

public class AliceTests : IClassFixture<ServerFixture> {
  readonly ServerFixture _server;
  readonly ITestOutputHelper _out;
  public AliceTests(ServerFixture server, ITestOutputHelper output) {
    _server = server;
    _out = output;
  }

  [Theory]
  [InlineData("Привет", "Привет")]
  [InlineData("дела", "Хорошо, а у тебя?")]
  [InlineData("погода", "Я не знаю что тебе сказать")]
  [InlineData("алиса", "Привет")]
  public async Task Test(string req, string expected) {
    var resp = await GetResponse(req);
    Assert.Equal(expected, resp.Response.Text);
  }

  async Task<AliceResponse> GetResponse(string text, string command = null) {
    var response = await _server.Client.PostAsync("/alice", CreateRequest(text, command));
    response.EnsureSuccessStatusCode();
    return await ParseResponse(response);
  }

  async Task<AliceResponse> ParseResponse(HttpResponseMessage res) {
    var json = await res.Content.ReadAsStringAsync();
    return DeserializeObject<AliceResponse>(json);
  }

  HttpContent CreateRequest(string text, string command = null) {
    var request = new AliceRequest {
      Meta = new MetaModel {
        Locale = "ru-RU",
        Timezone = "Europe/Moscow",
        ClientId = "ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)"
      },
      Request = new RequestModel {
        Command = command,
        OriginalUtterance = text,
        Type = SimpleUtterance
      }
    };
    return new StringContent(SerializeObject(request), Encoding.UTF8, "application/json");
  }
}
