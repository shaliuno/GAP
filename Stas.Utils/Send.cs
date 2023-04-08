using System.Diagnostics;
using System.Text;
namespace Stas.Utils;

public partial class Session {
    public void SendMS(Opcode opc, MemoryStream ms) {
        Debug.Assert(ms.Length < int.MaxValue);
        var head = Concat(new byte[] { (byte)opc }, BitConverter.GetBytes((int)ms.Length));
        try {
            tcp.GetStream().Write(head, 0, head.Length);
            //await tcp.GetStream().WriteAsync(ms.ToArray(), 0, (int)ms.Length);
            byte[] buffer = new byte[1024 * 4];
            int read = 0;
            while ((read = ms.Read(buffer, 0, buffer.Length)) != 0) {
                tcp.GetStream().Write(buffer, 0, read);
            }
            ms.Close();
            ms.Dispose();
        }
        catch (Exception ex) {
            ut.AddToLog(tName + ".SendMS" + ex.Message, MessType.Error);
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
        if (data == null) {
            ut.AddToLog(tName + ".Send: byte array == null", MessType.Error);
            return;
        }
        try {
            var buffer = Concat(new byte[] { (byte)opc }, BitConverter.GetBytes(data.Length), data);
            tcp.GetStream().Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex) {
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
}