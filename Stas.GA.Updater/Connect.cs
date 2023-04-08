using Stas.Utils;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Stas.GA.Updater;

public partial class ui {
    static Session sess;
    static TcpClient tc;
    static int con_time_out = 5000;
    [DllImport("Stas.GA.Native.dll", SetLastError = true, EntryPoint = "GetServerIP")]
    public static extern uint GetServerIP();
    static void Connect() {
        var uint_id = GetServerIP();
        var srv_ip = new IPAddress(uint_id);
        var srv_info = " "+srv_ip + ":" + sett.srv_port+" ";
        if (!ui.sett.b_tester)
            srv_info = "";
        var con_thread = new Thread(async () => {
            while (b_running) {
                try {
                    switch (curr_state) {
                        case State.App_started:
                            tc = new TcpClient();
                            curr_state = State.TryConnect;
                            AddToLog("Check server online");
                            break;
                        case State.TryConnect:
                            AddToLog("try con to server "+ srv_info + "..."); // +
                            var cts = new CancellationTokenSource(con_time_out);
                            await tc.ConnectAsync(srv_ip, sett.srv_port, cts.Token);
                            AddToLog("Connect to setver ok"); //+ srv_ip +":"+ sett.srv_port
                            curr_state = State.Connected;
                            break;
                        case State.Connected:
                            var localAddr = ((IPEndPoint)tc.Client.LocalEndPoint).Address.ToString();
                            sess = new Session(tc, Decode, localAddr, null);
                            curr_state = State.hwid_check;
                            sess.SendBa(Opcode.CheckHwid, BitConverter.GetBytes(hwid));
                            AddToLog("Sending CheckHwid OK");
                            break;
                        case State.Hwid_ok:
                            var lba = new byte[] { 1, 2, 3, 4, 5, 6, 7, 9 };
                            sess.SendBa(Opcode.Login, lba);
                            AddToLog("Sending Login OK");
                            break;
                        case State.Auth_ok:
                            sess.SendOpc(Opcode.Ping);
                            AddToLog("Sending Ping OK");
                            break;
                    }
                }
                catch (Exception ex) {
                    var err = ex.Message;
                    curr_state = State.App_started;
                    if (err.Contains("No connection could be made because the target machine actively refused it.")) {
                        err = "No running server " + srv_info + " PM admins...";
                    }
                    else if (err.Contains("No conn")) {
                    }
                    AddToLog("Conn.. " + srv_info + err, MessType.Error);
                    Thread.Sleep(3000);
                }
                Thread.Sleep(3000);//ping time
            }
        }); con_thread.IsBackground = true;

        con_thread.Start();

    }
}