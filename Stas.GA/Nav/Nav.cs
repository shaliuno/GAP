using System.Collections.Concurrent;
using System.Diagnostics;
using V2 = System.Numerics.Vector2;
using SixLabors.ImageSharp.PixelFormats;
namespace Stas.GA;

public class Nav {
    string tName = "Nav";
    public List<Node> path = null;
    int w8 { get; } = 100;
    public Nav() {
        Stopwatch sw = new Stopwatch();
        List<double> elaps = new List<double>();
        var worker = new Thread(() => {
            while (ui.b_running) {
                if (!b_ready) {
                    Thread.Sleep(100);
                    continue;
                }
                sw.Restart();
                //var curPlayerNode = _graph.Nodes.First();
                explorer.Update(ui.me.gpos); //curPlayerNode.Pos
                if (!explorer.HasLocation) {
                    ui.AddToLog(tName+".Explored done");
                    //RepaintBitmap(null, curPlayerNode, false);
                    Thread.Sleep(100);
                    continue; 
                }
                //UpdateImage();
                path = GraphPathFinder.FindPath(curPlayerNode, explorer.NextRunNode);
                if (path == null) {
                    //For some reason cannot find path there
                    //Shouldn't happen
                    explorer.NextRunNode.Unwalkable = true;
                }
                else if (path.Count == 0) {
                }
                else {
                    curPlayerNode = path[0];
                }

                if (elaps.Count > 60)
                    elaps.RemoveAt(0);
                elaps.Add(sw.Elapsed.TotalMilliseconds);
                var ft = elaps.Sum() / elaps.Count; //frame time
                var fps = Math.Round(1000f / ft, 1);
                ui.AddToLog(tName + ".worker: fps=[" + fps + "]");

                #region w8ting
                var t_elaps = (int)sw.Elapsed.TotalMilliseconds; //totale elaps
                if (t_elaps < w8) {
                    Thread.Sleep(w8 - t_elaps);
                }
                if (t_elaps > w8) {
                    Thread.Sleep(1);
                    ui.AddToLog(tName+ ".worker: Big tick time=[" + t_elaps + "]", MessType.Warning);
                }
                #endregion
            }
        });
        worker.IsBackground = true;
        worker.Start();

    }
    Random R = new Random();
    unsafe void UpdateImage(bool drawNodesSeparateColor = false) {
        if (!ui.curr_map.b_ready || ui.curr_map.image == null)
            return;
        var image = ui.curr_map.image;
        var blackArgb = new  Rgba32(0, 0, 0, 100);
        var redArgb = new Rgba32(255, 0, 0, 100);
        var greenArgb = new Rgba32(0, 255, 0, 100);
        var lightGrayArgb = new Rgba32(100, 100, 100, 100); 
        var orangeRedArgb = new Rgba32(255, 255, 0, 100);
        var darkGrayArgb = new Rgba32(40, 40, 40, 100);

        Parallel.For(0, _navGrid.Width * _navGrid.Height, i => {
                var y = i / _navGrid.Width;
                var x = i % _navGrid.Width;

                var gridVal = _navGrid.WalkArray[x, y];

                if ((gridVal & WalkableFlag.NonWalkable) != 0) {
                    image[x, y] = blackArgb;
                }
                else if (gridVal == WalkableFlag.PossibleSegmentStart) {
                    image[x, y] = redArgb;
                }
                else if (gridVal == WalkableFlag.PossibleSegmentPassed) {
                    image[x, y] = greenArgb;
                }
                else if (gridVal == WalkableFlag.Walkable) {
                    image[x, y] = lightGrayArgb;
                }
                else {
                    var node = _graph.MapSegmentMatrix[x, y];

                    if (node != null) {
                        if (node.IsVisited) {
                            image[x, y] = darkGrayArgb;
                        }
                        else if (node.IsRemovedByOptimizer) {
                            image[x, y ] = darkGrayArgb;
                        }
                        else {
                            int seed;

                            if (drawNodesSeparateColor)
                                seed = node.Id;
                            else
                                seed = node.GraphPart?.Id ?? node.Id;
                           
                            var randomColor = new Rgba32(R.Next(50,256), R.Next(50,256), R.Next(50,256), 255);
                            image[x, y ] = randomColor;
                        }
                    }
                    else {
                        image[x, y ] = orangeRedArgb;
                    }
                }
            });
        //ui.draw_main.AddOrGetImagePointer(AreaInstance.map_name, image, false, out ui.curr_map.map_ptr);
    }
    Graph _graph;
    public GraphMapExplorer explorer;
    public bool b_ready = false;
    public ConcurrentBag<GridCell> grid_cells = new ConcurrentBag<GridCell>();
    /// <summary>
    /// last sid id
    /// </summary>
    public int lcid = 0; 
    public void SaveVisited(bool _b_rewrite = false) {

    }
    NavGrid _navGrid;
    public GridCell Get_gc_by_gp(V2 gp) {
        //return grid_cells.FirstOrDefault(g => g.min.X <= gp.X && g.min.Y <= gp.Y 
        //                            && g.max.X >= gp.X && g.max.Y >= gp.Y);
        return null;
    }

    public Node curPlayerNode;
    public void GenerateNavGrid(int width, int height, WalkableFlag[,] walkArray) {
        lcid = 0;//reset it
        _navGrid = new NavGrid(width, height, walkArray);
        explorer = new GraphMapExplorer(_navGrid);
        _graph = explorer.Graph;
        explorer.ProcessSegmentation(ui.me.gpos);
        curPlayerNode = _graph.Nodes.First();
        b_ready = true;
    }
}
