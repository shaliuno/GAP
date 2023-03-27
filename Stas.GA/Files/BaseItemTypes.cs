
using System.Text.RegularExpressions;
namespace Stas.GA;

public class BaseItemTypes : FileInMemory {
    string tName = "BIT"; 
    public BaseItemTypes(Func<long> address) : base( address) {
        LoadItemTypes();
    }

    public Dictionary<string, BaseItemType> Contents { get; } = new Dictionary<string, BaseItemType>();
    public Dictionary<long, BaseItemType> ContentsAddr { get; } = new Dictionary<long, BaseItemType>();

    public BaseItemType GetFromAddress(long address) {
        ContentsAddr.TryGetValue(address, out var type);
        return type;
    }

    public BaseItemType Translate(string metadata) {
        if (Contents.Count == 0)
            LoadItemTypes();

        if (metadata == null) {
            ui.AddToLog(tName + ".Translate inp metadata==null", MessType.Error);
            return null;
        }

        if (!Contents.TryGetValue(metadata, out var type)) {
            ui.AddToLog(tName + ".Translate Key not found in BaseItemTypes: " + metadata, MessType.Warning);
            return null;
        }

        return type;
    }

    private void LoadItemTypes() {
        foreach (var i in RecordAddresses()) {
            var key =  ui.m.ReadStringU( ui.m.Read<long>(i));

            var baseItemType = new BaseItemType {
                Metadata = key,
                ClassName =  ui.m.ReadStringU( ui.m.Read<long>(i + 0x8, tName, 0)),
                Width =  ui.m.Read<int>(i + 0x18),
                Height =  ui.m.Read<int>(i + 0x1C),
                BaseName =  ui.m.ReadStringU( ui.m.Read<long>(i + 0x20)),
                DropLevel =  ui.m.Read<int>(i + 0x30),
                IsCorrupted =  ui.m.Read<byte>(i + 0xB9) != 0,
                IsBlessing =  ui.m.Read<byte>(i + 0xF2) != 0,
                Tags = new string[ ui.m.Read<long>(i + 0x68)]
            };

            var firstTag =  ui.m.Read<long>(i + 0x70);

            for (var k = 0; k < baseItemType.Tags.Length; k++) {
                var tagAddress = firstTag + 0x10 * k;
                baseItemType.Tags[k] =  ui.m.ReadStringU( ui.m.Read<long>(tagAddress, tName, 0), 255);
            }

            var tmpTags = key.Split('/');
            string tmpKey;

            if (tmpTags.Length > 3) {
                baseItemType.MoreTagsFromPath = new string[tmpTags.Length - 3];

                for (var k = 2; k < tmpTags.Length - 1; k++) {
                    // This Regex and if condition change Item Path Category e.g. TwoHandWeapons
                    // To tag strings type e.g. two_hand_weapon
                    tmpKey = Regex.Replace(tmpTags[k], @"(?<!_)([A-Z])", "_$1").ToLower().Remove(0, 1);

                    if (tmpKey[tmpKey.Length - 1] == 's')
                        tmpKey = tmpKey.Remove(tmpKey.Length - 1);

                    baseItemType.MoreTagsFromPath[k - 2] = tmpKey;
                }
            } else {
                baseItemType.MoreTagsFromPath = new string[1];
                baseItemType.MoreTagsFromPath[0] = "";
            }

            ContentsAddr.Add(i, baseItemType);

            if (!Contents.ContainsKey(key)) Contents.Add(key, baseItemType);
        }
    }
}
