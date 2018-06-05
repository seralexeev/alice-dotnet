using System;
using System.Linq;
using System.Collections.Generic;

public class Replies {
  Func<AliceRequest, AliceResponse> _default = req => req.Reply("Что то пошло не так");
  Dictionary<string, Func<AliceRequest, AliceResponse>> _reps = new Dictionary<string, Func<AliceRequest, AliceResponse>>();
  public Func<AliceRequest, AliceResponse> this[string words] {
    set {
      foreach (var word in words.ToLower().Split()) {
        _reps[word] = value;
      }
    }
    get => _reps[words];
  }

  public AliceResponse Match(AliceRequest req) =>
    (req.Request.OriginalUtterance
      .ToLower().Split()
      .Select(x => _reps.TryGetValue(x, out var val) ? val : null)
      .FirstOrDefault() ?? (_reps.TryGetValue("*", out var wildcard) ? wildcard : null) ?? _default)(req);
}

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

  public static bool ContainOneOf(this string text, string words) {
    var set = words.ToLower().Split().ToHashSet();
    return text.ToLower().Split().Any(x => set.Contains(x));
  }
}