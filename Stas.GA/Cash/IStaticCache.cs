namespace Stas.GA;
public interface IStaticCache<T> {
    int Count { get; }
    int DeletedCache { get; }
    int ReadCache { get; }
    int ReadMemory { get; }
    string CoeffString { get; }
    float Coeff { get; }
    T Read(string addr, Func<T> func);
    void UpdateCache();
    bool Remove(string key);
}
