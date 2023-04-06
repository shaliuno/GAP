using Stas.Utils;
namespace Stas.GA.Updater;

public partial class ui {
    static void Decode(Session sess, Opcode opc, MemoryStream ms) {//ms have only data=>no opcode and size
        try {
            switch (opc) {
                case Opcode.Login:
                    //string data = BYTE.ToString(data);
                    //Debug.Assert(raw.Contains('|'));
                    //string mail = raw.Split('|')[0];
                    //string pass = raw.Split('|')[1];
                    //user = users.FirstOrDefault(u => u.mail == mail && u.pass == pass);
                    //if (user != null) {
                    //    ui.AddToLog("Login ok: " + user.name);
                    //    sessions[sess] = user;
                    //    sess.Send(Opcode.Message, "Успешный вход: " + user.name);
                    //    var u = new User() { name = user.name, id = user.id };
                    //    sess.Send(Opcode.User, JSON.ToUT8Byte(user));
                    //    if (user.men)
                    //        sess.Send(Opcode.SiteList, GetSites(sess));
                    //}
                    //else {
                    //    ui.AddToLog("try loin from mail= " + mail + " pass=" + pass);
                    //    sess.Send(Opcode.Message, "Неправильная комбинация: " + STR.eof + mail + "+" + pass);
                    //}
                    break;

                case Opcode.Message:
                    break;
                case Opcode.Ping:
                    break;


                default:
                    //Disconnect(this.name + "Кикнут - неизвестный опкод - типа хакер?");
                    //throw new Exception("Неизвестный опкод " + opc);
                    ui.AddToLog(tName + ".decode unknown opc= " + opc, MessType.Error);
                    sess.tcp.Close();//защита от хака
                    break;
            }
        }
        catch (Exception ex) {
            ui.AddToLog(tName + ".decode ex: " + ex.Message, MessType.Error);
            sess.Close();//защита от хака
        }
    }
}
