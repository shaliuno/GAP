using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.CompilerServices;
using V2 = System.Numerics.Vector2;
using System.Collections.Concurrent;
using System.Runtime;

namespace Stas.GA;
public class NavGrid {
    public NavGrid(int width, int height, WalkableFlag[,] walkArray) {
        Width = width;
        Height = height;
        WalkArray = walkArray;
    }
    public const int gcs = 23;
    public int lcid = 0; //last cell id

    public readonly int Height;
    public readonly int Width;
    public readonly WalkableFlag[,] WalkArray;
  
}
[Flags]
public enum WalkableFlag : byte {
    NonWalkable = 1,
    Walkable = 2,
    Passed = 4,
    PossibleSegmentStart = Walkable | Passed | 8,
    PossibleSegment = Walkable | 16,
    PossibleSegmentPassed = PossibleSegment | Passed,
    FailedCenter = Walkable | Passed | 32
}

public static class WalkableFlagExtension {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contain(this WalkableFlag enm, WalkableFlag flag) {
        return (flag & enm) == flag;
    }
}
public static class NavGridProvider {

    public static unsafe NavGrid FromBitmap(Bitmap image) {
        var imageWidth = image.Width;
        var imageHeight = image.Height;

        // Lock the bitmap bits to get a pointer to the pixel data
        var data = image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        // Get a pointer to the pixel data
        int* pData = (int*)data.Scan0;

        var grid = CreateGrid(
            imageWidth,
            imageHeight,
            point => {
                var color = Color.FromArgb(pData[point.Y * imageWidth + point.X]);

                return color.R > 128;
            });

        // Unlock the bitmap bits to release the pointer
        image.UnlockBits(data);

        return grid;
    }

    public static NavGrid CreateGrid(int imageWidth, int imageHeight, Func<Point, bool> isWalkableFunc) {
        var walkArray = new WalkableFlag[imageWidth, imageHeight];

        // Process on multiple threads
        Parallel.For(
            0,
            imageHeight,
            y => {
                for (int x = 0; x < imageWidth; x++) {
                    walkArray[x, y] = isWalkableFunc(new Point(x, y)) ? WalkableFlag.Walkable : WalkableFlag.NonWalkable;
                }
            });

        return new NavGrid(imageWidth, imageHeight, walkArray);
    }
}