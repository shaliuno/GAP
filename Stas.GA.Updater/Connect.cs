using Stas.Utils;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Stas.GA.Updater;

public partial class ui {
    static Session sess;
    static TcpClient tc;
    static int con_time_out = 1000;
    static void Connect() {
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
                            var cts = new CancellationTokenSource(con_time_out);
                            IPAddress.TryParse(sett.srv_ip, out var srv_ip);
                            await tc.ConnectAsync(srv_ip, sett.srv_port, cts.Token);
                            AddToLog("Connect to setver ok");
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
                            var lba = new byte[] { 1,2,3,4,5,6,7,9};
                            sess.SendBa(Opcode.Login, lba);
                            AddToLog("Sending Login OK");
                            break;
                        case State.Auth_ok:
                            sess.SendOpc(Opcode.Ping);
                            AddToLog("Sending Ping OK");
                            break;
                    }

                    //udp_screen.Send(Opcode.ScreenPing);
                }
                catch (Exception ex) {
                    curr_state = State.TryConnect;
                    AddToLog("Err: " + ex.Message);
                }
                Thread.Sleep(3000);
            }
        }); con_thread.IsBackground = true;

        con_thread.Start();
       
    }
}