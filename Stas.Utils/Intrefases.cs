using V2 = System.Numerics.Vector2;
namespace Stas.Utils;

public interface i_ui {
    void i_AddToLog(string str, MessType _mt = MessType.Ok);
    bool i_b_running { get; }
    void AddToLogLocal(string str, MessType _mt = MessType.Ok);
    i_AreaInstance i_curr_map { get; }
    i_Entity i_me { get; }

}
public interface i_AreaInstance { 
    bool b_ready { get; }
    //Image<Rgba32> image { get; }
}
public interface i_Entity {
    V2 gpos { get; }
}
public interface i_Settings { 

}


