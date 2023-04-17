
namespace Stas.Utils; 
public enum miType { Unknow=0,
    regular, transit, door, portal, IncursionPortal, @switch, Harvest, Ultimatum, Ritual, Sulphite, Blight,
    ExpedDeton, ExpedMarker, ExpedRemnant, ExpedArtifact, PartyMember, Stash, GuildStash, Chest, Barrel, loot,
    NPC, Archnemesis,
    waypoint,
    Quest, BlightPath,
    HoDecor, LeagueEvent,
    DynamiteWall
}
public struct MapItemWrap {
    public miType type;
    public MapIconsIndex index;
    public IconPriority priority;
    public IntPtr info_ptr;
    public IntPtr remn_ptr;
    public int remn_size;
}

public enum IconPriority : byte {
    Low,
    Medium,
    High,
    VeryHigh,
    Critical,
    Double
}