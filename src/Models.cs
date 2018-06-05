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
