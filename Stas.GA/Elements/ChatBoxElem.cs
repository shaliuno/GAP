namespace Stas.GA;

public class ChatBoxElem : Element {
    public ChatBoxElem(nint ptr, string name= "ChatBoxElem") : base(ptr, name) {
    }
    public byte isOpened => ui.m.Read<byte>(Address + 0x17f);
    int last_count = 0;
    List<string> _mesa = new List<string>();
    public Element up_arrow => GetChildFromIndices(1, 2, 2, 0);
    public Element down_arrow => GetChildFromIndices(1, 2, 2, 1);
    public Element open_inp_btn => GetChildFromIndices(1, 2, 2, 2);
    public Element input => GetChildAtIndex(3);
    public Element mess_elems => GetChildFromIndices(1, 2, 1);
    public Element lme; // last_message_elem;
    public Element arrows => GetChildFromIndices(1, 2, 2);
    public List<string> messages {
        get {
            if (mess_elems != null) {
                while (mess_elems.chld_count > last_count) {
                    try {
                        lme = new Element(mess_elems[last_count].Address);
                        _mesa.Add(lme.Text);
                    } catch (Exception ex) {
                    }
                    last_count += 1;
                }
            }
            return _mesa;
        }
    }
}
