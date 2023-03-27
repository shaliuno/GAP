using System.Runtime.InteropServices;
namespace Stas.GA;

public partial class ui {
    static BaseItemTypes _bat;
    public static BaseItemTypes BaseItemTypes =>
        _bat ??= new BaseItemTypes(() => FindFile("Data/BaseItemTypes.dat"));
    static Dictionary<string, FileInfo> AllFiles;
    public readonly struct FileInfo {
        public FileInfo(long ptr, int changeCount) {
            Ptr = ptr;
            ChangeCount = changeCount;
        }

        public long Ptr { get; }
        public int ChangeCount { get; }
    }

    static long FindFile(string name) {
        AllFiles ??= GetAllFiles();
        try {
            if (AllFiles.TryGetValue(name, out var result))
                return result.Ptr;
        }
        catch (Exception) {
            ui.AddToLog(tName + ".FindFile Couldn't find the file in memory: " + name, MessType.Error);
            Environment.Exit(1);
        }

        return 0;
    }
    static Dictionary<string, FileInfo> GetAllFiles() {
        var files = new Dictionary<string, FileInfo>();
        var fileRootAddress = ui.base_offsets[PattNams.FileRoot];
        for (int rbIndex = 0; rbIndex < 16; rbIndex++) {
            var fileRootBlock = ui.m.Read<FileRootBlock>(fileRootAddress + rbIndex * 0x28);
            for (int bIndex = 0; bIndex < (fileRootBlock.Capacity + 1) / 8; bIndex++) {
                var basePtr = fileRootBlock.FileNodesPtr + bIndex * 0xc8;
                var hasValues = ui.m.ReadMemoryArray<byte>(new nint(basePtr), 8);
                for (int index = 0; index < 8; index++) {
                    if (hasValues[index] == 0xFF)
                        continue;

                    var fileEntryPtr = m.Read<long>(basePtr + 8 + index * 0x18 + 8);
                    var fileInfo = m.Read<FileInfoOffsets>(fileEntryPtr);
                    var key = m.ReadStringU(fileInfo.String);
                    files[key] = new FileInfo(fileEntryPtr, fileInfo.AreaCount);
                }
            }
        }
        return files;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FileInfoOffsets {
        [FieldOffset(0x08)] public long String;
        [FieldOffset(0x18)] public long Size;
        [FieldOffset(0x20)] public long Capacity;
        [FieldOffset(0x38)] public int AreaCount;
    }
}
