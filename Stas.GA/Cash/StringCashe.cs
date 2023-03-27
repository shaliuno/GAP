namespace Stas.GA;

public class StringCache {
    readonly object locker = new object();
    public Dictionary<IntPtr, string> Debug { get; } = new Dictionary<IntPtr, string>();
    public int Count => Debug.Count;

    public void Clear() {
        lock (locker) {
            Debug.Clear();
        }

    }

    public string Read(IntPtr addr, Func<string> func) {
        if (Debug.TryGetValue(addr, out var result)) {
            return result;
        }

        result = func();

        lock (locker) {
            Debug[addr] = result;
        }

        return result;
    }
}
