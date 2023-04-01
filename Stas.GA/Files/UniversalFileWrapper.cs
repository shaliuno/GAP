namespace Stas.GA;

public class UniversalFileWrapper<T> :FileInMemory  where T : RemoteObjectBase, new() {
    public UniversalFileWrapper(Func<long> address) : base(address) {
    }
    //We mark this fields as private coz we don't allow to read them directly dut to optimisation. Use EntriesList and methods instead.
    protected Dictionary<long, T> EntriesAddressDictionary { get; set; } = new Dictionary<long, T>();
    protected List<T> CachedEntriesList { get; set; } = new List<T>();

    public List<T> EntriesList {
        get {
            CheckCache();
            return CachedEntriesList;
        }
    }

    public T GetByAddress(long address) {
        CheckCache();
        EntriesAddressDictionary.TryGetValue(address, out var result);
        return result;
    }

    public void CheckCache() {
        if(EntriesAddressDictionary.Count != 0)
            return;

        foreach(var addr in RecordAddresses()) {
            if(!EntriesAddressDictionary.ContainsKey(addr)) {
                var nt = new T();
                nt.Update(new IntPtr(addr), "CheckCache");
                EntriesAddressDictionary.Add(addr, nt);
                EntriesList.Add(nt);
                EntryAdded(addr, nt);
            }
        }
    }

    protected virtual void EntryAdded(long addr, T entry) {
    }
}
