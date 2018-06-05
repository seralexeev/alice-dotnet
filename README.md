# alice dotnet

Данный репозиторий содержит пример создания скилла для Алисы на языке c# под платформу .net core 2.1

## Подготовка

- Для начала необходимо установить `dotnet sdk`
  - (с официального сайта) идем на https://www.microsoft.com/net/download и скачиваем `2.1 SDK (v2.1.300)` и устанавливаем
  - brew
  - apt-get
- Создаем новый проект

```bash
$ dotnet new console -o alice
```

- добавляем нужные зависимости:

```bash
$ dotnet add package Microsoft.AspNetCore
$ dotnet add package Microsoft.AspNetCore.Mvc
```

## Создание сервера

1.  Удаляем файл Program.cs, он нам не нужен и создаем файл `Alice.cs`, в этом файле будет наш контроллер вебхука
2.  Добавляем импорты:

```csharp
// Alice.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
```

3.  Создаем класс `Alice` унаследованный от класса `Controller` это файл будет содержать бутстрапинг сервера и простой ответ

```csharp
// Alice.cs
public class Alice : Controller {

}
```

4.  Добавляем метод `CreateWebHostBuilder` - этот метод создаст объект `IWebHostBuilder`, добавит сервисы необходимые для `mvc` и настроит дефолтный роутинг

```csharp
// Alice.cs
public class Alice : Controller {
  public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
      .ConfigureServices(srv => srv.AddMvc())
      .Configure(app => app.UseMvc());
}
```

5.  Далее создадим точку входа в наше приложение которое получит объект из пунка 4 и запустит хост

```csharp
static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();
```

6.  Наш веб сервер готов, запустить можно его просто командой

```bash
$ dotnet run
```

сервер запустится и выведет на экран примерно следующее:

```bash
info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
      User profile is available. Using '/Users/seralexeev/.aspnet/DataProtection-Keys' as key repository; keys will not be encrypted at rest.
Hosting environment: Production
Content root path: /Users/seralexeev/projects/alice-dotnet
Now listening on: http://localhost:5000
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

Наш сервер будет доступен на 5000 и 5001 портах, изменить это поведение можно добавив параметр `--urls`

```bash
$ dotnet run --urls "http://localhost:3000"

Hosting environment: Production
Content root path: /Users/seralexeev/projects/alice-dotnet
Now listening on: http://localhost:3000
Application started. Press Ctrl+C to shut down.
```

## Описание протокола

Для взаимодействия с api опишем классы в соответсвии с https://tech.yandex.ru/dialogs/alice/doc/protocol-docpage

```csharp
// Models.cs
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SessionModel {
  [JsonProperty("new")]
  public bool New { get; set; }

  [JsonProperty("session_id")]
  public string SessionId { get; set; }

  [JsonProperty("message_id")]
  public int MessageId { get; set; }

  [JsonProperty("skill_id")]
  public string SkillId { get; set; }

  [JsonProperty("user_id")]
  public string UserId { get; set; }
}

public class ResponseModel {
  [JsonProperty("text")]
  public string Text { get; set; }

  [JsonProperty("tts")]
  public string Tts { get; set; }

  [JsonProperty("end_session")]
  public bool EndSession { get; set; }

  [JsonProperty("buttons")]
  public ButtonModel[] Buttons { get; set; }
}

public class ButtonModel {
  [JsonProperty("title")]
  public string Title { get; set; }

  [JsonProperty("payload")]
  public object Payload { get; set; }

  [JsonProperty("url")]
  public string Url { get; set; }

  [JsonProperty("hide")]
  public bool Hide { get; set; }
}

public class MetaModel {
  [JsonProperty("locale")]
  public string Locale { get; set; }

  [JsonProperty("timezone")]
  public string Timezone { get; set; }

  [JsonProperty("client_id")]
  public string ClientId { get; set; }
}

public class AliceRequest {
  [JsonProperty("meta")]
  public MetaModel Meta { get; set; }

  [JsonProperty("request")]
  public RequestModel Request { get; set; }

  [JsonProperty("session")]
  public SessionModel Session { get; set; }

  [JsonProperty("version")]
  public string Version { get; set; }
}

public enum RequestType {
  SimpleUtterance,
  ButtonPressed
}

public class RequestModel {
  [JsonProperty("command")]
  public string Command { get; set; }

  [JsonProperty("type")]
  public RequestType Type { get; set; }

  [JsonProperty("original_utterance")]
  public string OriginalUtterance { get; set; }

  [JsonProperty("payload")]
  public JObject Payload { get; set; }
}

public class AliceResponse {
  [JsonProperty("response")]
  public ResponseModel Response { get; set; }

  [JsonProperty("session")]
  public SessionModel Session { get; set; }

  [JsonProperty("version")]
  public string Version { get; set; } = "1.0";
}
```

и добавим метод расширения для `AliceRequest` для простого создания ответа

```csharp
// Extensions.cs
public static class Extensions {
  public static AliceResponse Reply(
    this AliceRequest req,
    string text,
    bool endSession = false,
    ButtonModel[] buttons = null) => new AliceResponse {
      Response = new ResponseModel {
        Text = text,
        Tts = text,
        EndSession = endSession
      },
      Session = req.Session
    };
}
```

## Ответ webhook

На последнем шаге необходимо добавить эндпоинт который и зарегистрируем в платформе диалогов

```csharp
// Alice.cs
public class Alice : Controller {
  ...

  [HttpPost("/alice")]
  public AliceResponse WebHook([FromBody] AliceRequest req) {
    return req.Reply("Привет");
  }

  ...
}
```

В итоге наш котнтроллер будет выглядить примерно так:

```csharp
// Alice.cs
public class Alice : Controller {
  static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

  public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
      .ConfigureServices(srv => srv.AddMvc())
      .Configure(app => app.UseMvc());

  [HttpPost("/alice")]
  public AliceResponse WebHook([FromBody] AliceRequest req) {
    return req.Reply("Привет");
  }
}
```

# Сборка в контейнере и запуск

```bash
$ docker build -t alice .
$ docker run --rm -it -p 5000:80 alice
```

# Консоль разработчика

Для проверки нашего еще неопубликованного скила необходимо зарегистрироваться и создать "Навык в Алисе" на https://dialogs.yandex.ru/developer/

# Тестирование

- Для тестирования оффлайн можно использовать юнит тесты или интеграционные тесты, описанные в классе `AliceTests`
- Для тестирования через консоль разработчика можно использовать `ngrok`
  - Запускаем проект в режиме `watch`, dotnet будет следить за изменениями в файлах и перезапускать сервер: `dotnet watch -p src run`
  - Запускаем `ngrok`: `ngrok http 5000`
  - Прописываем сгенерированный адрес в консоль разработчика, в итоге получаем возможность писать код и сразу проверять его во вкладке тестирования в консоле разработчика
