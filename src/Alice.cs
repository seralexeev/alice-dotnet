using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public class Alice : Controller {
  static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

  public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
      .ConfigureServices(srv => srv.AddMvc())
      .Configure(app => app.UseMvc());

  static Replies _replies = new Replies {
    ["привет хай здравствуй здравствуйте алиса"] = x => x.Reply("Привет"),
    ["как дела делишки"] = x => x.Reply("Хорошо, а у тебя?"),
    ["*"] = x => x.Reply("Я не знаю что тебе сказать")
  };

  [HttpPost("/alice")]
  public AliceResponse WebHook([FromBody] AliceRequest req) => _replies.Match(req);
}
