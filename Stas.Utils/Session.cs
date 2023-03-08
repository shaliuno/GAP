using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Stas.Utils;
public delegate void Decoderdelegate(Session sess, Opcode opc, MemoryStream ms);

public class Session : IEquatable<Session> {
    public TcpClient tcp { get; }
    public IPAddress ip { get; }
    public int port { get; }
    public string key { get; }
    Decoderdelegate Decode { get; }
    public Action<string> IfClose { get; }
    public DateTime dt_last_ping;
    string tName { get; }
    bool b_debug = true;
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
        var pull = new Thread(() => {
            while (true) {
                try {
                    if (tcp == null || tcp.GetStream() == null || tcp.Client == null || !tcp.Client.Connected)
                        break;
                    b_pull_ok = !(tcp.Client.Poll(1, SelectMode.SelectRead) && tcp.Client.Available == 0);
                    ut.AddToLog(tName + ".ping OK");
                }
                catch (Exception) {
                    break;
                }
                Thread.Sleep(500);
            }
            b_pull_ok = false; //if whilw was break
        });
        pull.IsBackground = true;
        pull.Start();
    }

    bool b_pull_ok = true;
    public virtual bool IsValid(Opcode opc) {
        return true;
    }
    async void StrartResive() {     
        bool need_head = true;
        byte[] buffer = new byte[1024];
        Packet pkt = null;
        while(b_pull_ok) {
            try {
                if(need_head) {
                    var bhead = new byte[5];
                    await tcp.GetStream().ReadAsync(bhead, 0, 5);
                    var ob = bhead[0]; //opcode byte
                    if (!Enum.IsDefined(typeof(Opcode), ob)) {
                        ut.AddToLog(tName + ".resive  opc+"+ob+" NOT Defined", MessType.Warning);
                        Close();
                    }
                    var opc  =(Opcode)ob;
                    if(opc == Opcode.Unknown) {
                        ut.AddToLog(tName + ".resive  opc=Unknown");
                        Close();
                    }
                    if (!IsValid(opc)) {
                        ut.AddToLog(tName + ".resive  opc=NOT valid");
                        Close();
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

    #region SEND
    public void SendMS(Opcode opc, MemoryStream ms) {
        Debug.Assert(ms.Length < int.MaxValue);
        var head = Concat(new byte[] { (byte)opc }, BitConverter.GetBytes((int)ms.Length));
        try {
            tcp.GetStream().Write(head, 0, head.Length);
            //await tcp.GetStream().WriteAsync(ms.ToArray(), 0, (int)ms.Length);
            byte[] buffer = new byte[1024 * 4];
            int read = 0;
            while((read = ms.Read(buffer, 0, buffer.Length)) != 0) {
                tcp.GetStream().Write(buffer, 0, read);
            }
            ms.Close();
            ms.Dispose();
        }
        catch(Exception ex) {
            ut.AddToLog(tName + ".SendMS" + ex.Message,  MessType.Error);
            Close();
        }
    }
    public void SendOpc(Opcode opc) {
        Send(opc, BitConverter.GetBytes((int)0));
    }
    public void SendInt(Opcode opc, int id) {
        Send(opc, BitConverter.GetBytes(id));
    }
    public void SendText(Opcode opc, string text) {
        var b = Encoding.UTF8.GetBytes(text);
        Send(opc, b);
    }
    /// <summary>
    /// Send raw byte array
    /// </summary>
    /// <param name="opc">opc</param>
    /// <param name="ba">raw data</param>
    public void SendBa(Opcode opc, byte[] ba) {
        Send(opc, ba);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="opc"></param>
    /// <param name="data">not content packet size</param>
    void Send(Opcode opc, byte[] data) {
        if(data == null) {
            ut.AddToLog(tName + ".Send: byte array == null", MessType.Error);
            return;
        }
        try {
            var buffer = Concat(new byte[] { (byte)opc }, BitConverter.GetBytes(data.Length), data);
            tcp.GetStream().Write(buffer, 0, buffer.Length);
        }
        catch(Exception ex) {
            ut.AddToLog(tName + ".SendData ex=" + ex.Message, MessType.Error);
            Close();
        }
    }
    public byte[] Concat(byte[] a, byte[] b) {
        Debug.Assert(a != null && b != null, Environment.StackTrace);
        byte[] buffer1 = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, buffer1, 0, a.Length);
        Buffer.BlockCopy(b, 0, buffer1, a.Length, b.Length);
        return buffer1;
    }
    public byte[] Concat(byte[] a, byte[] b, byte[] c) {
        return Concat(Concat(a, b), c);
    }
    #endregion
    public void Close() {
        try {
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
