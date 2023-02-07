using System.IO;
namespace Stas.GA; 
internal class Starter {
    public static void Main() {
        AppDomain.CurrentDomain.UnhandledException += (sender, exceptionArgs) => {
            var errorText = "Program exited with message:\n " + exceptionArgs.ExceptionObject;
            File.AppendAllText("Error.log", $"{DateTime.Now:g} {errorText}\r\n{new string('-', 30)}\r\n");
            Environment.Exit(1);
        };
      
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(DisposeAllResourceHere);
        ui.ReloadSett();
        //ui.sett.b_debug_native_dll = true;
        if (ui.sett.b_debug_native_dll) {
            ui.Init();
            Console.WriteLine("Press Q to exit");
            while (true) {
                var k = Console.ReadKey();
                if (k.Key == ConsoleKey.Q)
                    break;
                Thread.Sleep(100);
            }
        }
        else {
            var drawler = new DrawMain();
        }
    }
    static void DisposeAllResourceHere(object sender, EventArgs e) {
        //todo: need make dispose for same type - i not sure ui have IDisposable
        // all Base need call: OnGameClose  
        DrawMain.scene?.Dispose();
    }
}
