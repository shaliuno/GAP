namespace Stas.GA;

public class If_I_Dead :Element {
    public If_I_Dead(nint ptr, string name = "If_I_Dead") : base(ptr, name) {
    }
    public Element res_in_town => GetTextElem_by_Str("resurrect in town");
    public Element res_at_checkpoint => GetTextElem_by_Str("resurrect at checkpoint");
}
public class UltimatumElem:Element{
    public UltimatumElem(nint ptr, string name = "UltimatumElem" ) : base(ptr, name) {

    }
    public Element accept =>GetTextElem_by_Str("accept trial");
    public Element confirm => GetTextElem_by_Str("confirm");
    public int selected_choice { get {
            var ea = GetChildFromIndices(2, 4, 0);
            for(int i = 0; i < ea.children.Count; i++) {
                if(ea.children[i].b_selected) {
                    return i;
                }
            }
            return -1;
        } }
}
public class EscDialog :Element { // modal, poping after logion
    public EscDialog(nint ptr, string name = "EscDialog") : base(ptr, name) { 
    }
    public Element options => GetTextElem_by_Str("options");
    public Element resume => GetTextElem_by_Str("resume game");
    public Element exit_to_character => GetTextElem_by_Str("exit to character selection");
}
public class ModalDialog :Element { // modal, poping after logion
    public ModalDialog(nint ptr, string name = "ModalDialog") : base(ptr, name) {
    }
    public Element open_options => GetTextElem_by_Str("open options");
    public Element later => GetTextElem_by_Str("later");
    public Element keep => GetTextElem_by_Str("keep");
    public Element yes => GetTextElem_by_Str("yes");
    public Element no => GetTextElem_by_Str("no");
    public Element ok => GetTextElem_by_Str("ok");
}

public class DelveDarknessElem :Element {
    public DelveDarknessElem(nint ptr, string name = "DelveDarknessElem") : base(ptr, name) {
    }
    public int darkness { get { 
            if(!this.IsVisible)   
                return 0; 
            var elem = this.GetChildFromIndices(0, 0, 1, 0);
            int.TryParse(elem?.Text, out var res);
            return res;
        } 
    } 
}
public class BanditDialog :Element {
    public BanditDialog(nint ptr, string name = "BanditDialog") : base(ptr, name) {
    }
    int bpi => (int)chld_count - 1; //buttons_panel_index
    public Element help => GetTextElem_by_Str("help");
    public Element kill => GetTextElem_by_Str("kill");
    public BanditType BanditType => GetBanditType();

    BanditType GetBanditType() {
        var helpButtonText = help?.GetChildAtIndex(0)?.Text?.ToLower();
        if(helpButtonText != null) {
            if(helpButtonText.Contains("kraityn")) return BanditType.Kraityn;
            if(helpButtonText.Contains("alira")) return BanditType.Alira;
            if(helpButtonText.Contains("oak")) return BanditType.Oak;
        }
        return BanditType.error;
    }
}
public enum BanditType {
    error,
    Kraityn,
    Alira,
    Oak
}
