
namespace Stas.GA;

public class IncomingUserRequest : Element {
    internal IncomingUserRequest() : base("IncomingUserRequest") {
        b_dynamic_childrens = true;
    }

    public bool b_trade => sent_you_elem?.Text == "sent you a trade request" && accept != null;
    public bool b_invate => sent_you_elem?.Text == "sent you a party invite" && accept != null;
    public Element sent_you_elem => GetTextElem_with_Str("sent you a");
    public Element trade_elem => GetTextElem_with_Str("trade"); // 
    public string from {
        get {
            return sent_you_elem?.Parent?.GetChildFromIndices(0, 1)?.Text;
        }
    }
    public Element accept => GetTextElem_by_Str("accept");
    public Element decline => GetTextElem_by_Str("decline");
}
