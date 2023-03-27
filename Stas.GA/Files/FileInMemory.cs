namespace Stas.GA;

public abstract class FileInMemory {
    private readonly Func<long> fAddress;
    protected string tName = "FileInMemory";
    protected FileInMemory(Func<long> address) {

        Address = address();
        fAddress = address;
    }

    public long Address { get; }
    private int NumberOfRecords => ui.m.Read<int>(fAddress() + 0x30, tName, 0x40);

    protected IEnumerable<long> RecordAddresses() {
        if(fAddress() == 0) {
            yield return 0;
            yield break;
        }

        var cnt = NumberOfRecords;

        if(cnt == 0) {
            yield return 0;
            yield break;
        }

        var firstRec = ui.m.Read<long>(fAddress() + 0x30, tName, 0x0);
        var lastRec = ui.m.Read<long>(fAddress() + 0x30, tName, 0x8);
        var recLen = (lastRec - firstRec) / cnt;

        for(var i = 0; i < cnt; i++) {
            yield return firstRec + i * recLen;
        }
    }
}
