using System.Drawing;

namespace Stas.GA;

public struct Size2F {
    public Size2F(float w, float h) {
        Width = w; Height = h;
    }
    public float Width { get; set; }
    public float Height { get; set; }
}
public static class SpriteHelper {
    static SpriteHelper() {
        Icons = new();
        foreach (var icon in Enum.GetValues(typeof(MapIconsIndex))) {
            Icons[icon.ToString()] = (MapIconsIndex)icon;
        }
    }
    public static MapIconsIndex IconIndexByName(string name) {
        name = name.Replace(" ", "").Replace("'", "");
        Icons.TryGetValue(name, out var result);
        return result;
    }
    static readonly Dictionary<string, MapIconsIndex> Icons;
    public static readonly Size2F MapIconsSize = new Size2F(14, 18 + 6);
    public static RectangleF GetUV(MapIconsIndex index) {
        return GetUV((int)index, MapIconsSize);
    }
    public static RectangleF GetUV(int index, Size2F size) {
        if (index % (int)size.Width == 0) {
            return new RectangleF((size.Width - 1) / size.Width, ((int)(index / size.Width) - 1) / size.Height, 1 / size.Width,
                1 / size.Height);
        }

        return new RectangleF((index % size.Width - 1) / size.Width, index / (int)size.Width / size.Height, 1 / size.Width,
            1 / size.Height);
    }
    public static RectangleF GetUV(int x, int y, float width, float height) {
        return new RectangleF((x - 1) / width, (y - 1) / height, 1 / width, 1 / height);
    }
}
