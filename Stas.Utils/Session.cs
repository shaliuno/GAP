using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Stas.Utils;
public delegate void Decoderdelegate(Session sess, Opcode opc, MemoryStream ms);

public partial class Session : IEquatable<Session> {
    public TcpClient tcp { get; }
    public IPAddress ip { get; }
    public int port { get; }
    public string key { get; }
    Decoderdelegate Decode { get; }
    public Action<string> IfClose { get; }
    public DateTime dt_last_ping;
    string tName { get; }
    bool b_debug = true;
    bool b_running = true;
    bool b_pull_ok = true;

    public Session(TcpClient _tcp, Decoderdelegate _decoder, string name, Action<string> _close) {
        tcp = _tcp;
        Decode = _decoder;
        IfClose = _close;
        tName = "Sess " + name;
        tcp.NoDelay = true;
        var rep = (IPEndPoint)tcp.Client.RemoteEndPoint;
        ip = rep.Address;
        port = rep.Port;
        key = (ip + ":" + port).To8ByteHashString();
        StrartResive();
        //this thread just send Poll packet for check connection
        var pull = new Thread(() => {
            while (b_running) {
                try {
                    if (tcp == null || tcp.GetStream() == null || tcp.Client == null || !tcp.Client.Connected) {
                        b_pull_ok = false; //if whilw was break
                        Thread.Sleep(3000); //poll interval 
                        continue;
                    }
                    b_pull_ok = !(tcp.Client.Poll(1, SelectMode.SelectRead) && tcp.Client.Available == 0);
                    ut.AddToLog(tName + ".pull OK");
                }
                catch (Exception ex) {
                    b_pull_ok = false; //if whilw was break
                    Thread.Sleep(3000); //poll interval 
                    ut.AddToLog(tName + "... "+ex.Message, MessType.Error);
                }
                Thread.Sleep(900); //poll interval 
            }
        });
        pull.IsBackground = true;
        pull.Start();
    }

    public virtual bool IsValid(Opcode opc) {
        return true;
    }
    async void StrartResive() {     
        bool need_head = true;
        byte[] buffer = new byte[1024];
        Packet pkt = null;
        while(b_running) {
            if (!b_pull_ok) {
                Thread.Sleep(1000);
            }
            try {
                if(need_head) {
                    var bhead = new byte[5]; //opc +4byte=>data size like int
                    var stream = tcp.GetStream();
                    await stream.ReadAsync(bhead, 0, 5);
                    var ob = bhead[0]; //opcode byte
                    if (!Enum.IsDefined(typeof(Opcode), ob)) {
                        OnError(tName + ".resive  opc+" + ob + " NOT Defined", ErrType.EnumDontHaveOpc);
                    }
                    var opc  =(Opcode)ob;
                    if(opc == Opcode.Unknown) {
                        OnError(tName + ".resive  opc=Unknown", ErrType.OpcIsUnknow);
                    }
                    if (!IsValid(opc)) {
                        OnError(tName + ".resive  opc=NOT valid", ErrType.OpcIsNotValid);
                    }
                    if(b_debug)
                        ut.AddToLog(tName+ ".need_head=>opc=[" + opc + "]");
                    var psize = BitConverter.ToInt32(bhead, 1);
                    if(psize > 0) { //тоетсь в пакете не только опкод но и дата
                        pkt = new Packet(opc, psize);
                        need_head = false;//set stram to read to end packet
                    }
                    else {
                        Decode(this, opc, null); //need_head = true;
                    }
                }
                else {
                    if(pkt.size - pkt.done <= buffer.Length) {
                        var buff = new byte[pkt.size - pkt.done];
                        var done = await tcp.GetStream().ReadAsync(buff, 0, buff.Length);
                        pkt.ms.Write(buff, 0, done);
                        pkt.done += done;
                        if(pkt.done == pkt.size) {
                            need_head = true;
                            pkt.ms.Seek(0, SeekOrigin.Begin);//reset pos adfter full filling
                            Decode(this, pkt.opc, pkt.ms);
                            pkt = null;//dispose
                        }
                    }
                    else {//fill buffer part
                        var curr = await tcp?.GetStream()?.ReadAsync(buffer, 0, buffer.Length);
                        Debug.Assert(curr > 0); //если 0 - проблемы на стороне севера с отправкой
                        pkt.ms.Write(buffer, 0, curr);//add curr byte to stream(on lst position)
                        pkt.done += curr;
                    }
                }
            }
            catch(Exception ex) {
                ut.AddToLog(tName + "StrartResive ex: " + ex.Message);
                this.Close();
                break;
            }
        }
        this.Close();
    }

    public enum ErrType {
        EnumDontHaveOpc, OpcIsUnknow,
        OpcIsNotValid,
    }
    public virtual void OnError(string err_message, ErrType err_t) {
        ut.AddToLog(err_message, MessType.Error);
    }
    public void Close() {
        try {
            b_pull_ok = false;
            b_running = false;
            Thread.Sleep(500);
            tcp?.Close();
            tcp?.Dispose();
        }
        catch(Exception ex) {
            ut.AddToLog(tName + ".Close ex: " + ex.Message, MessType.Error);
        }
        IfClose?.Invoke(key);
    }

    public bool Equals(Session other) {
        if (!b_pull_ok)
            return false;
        return other != null && other.tcp.GetHashCode() == this.tcp.GetHashCode();
    }
}
