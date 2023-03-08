namespace Stas.Utils;

public class ut {
    public static int w8 { get; } = 16;////1000 / 60 = 16(60 frame sec)
    public static FixedSizedLog log { get; private set; }
    public static void SetUI(FixedSizedLog _log) {
        log = _log;
    }
    public static void AddToLog(string str, MessType _mt = MessType.Ok) {
        log?.Add(str, _mt);
    }
}