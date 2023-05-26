namespace Stas.Utils;

public class ut {
    static FixedSizedLog log;
    public const int w8 = 1000 / 60;
    public static void SetLog(FixedSizedLog _log) {
        log = _log;
    }
    public static void AddToLog(string str, MessType _mt = MessType.Ok) {
        log?.Add(str, _mt);
    }
}