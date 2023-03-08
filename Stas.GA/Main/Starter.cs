using System.IO;
namespace Stas.GA; 
internal class Starter {
    public static void Main() {
        AppDomain.CurrentDomain.UnhandledException += (sender, exceptionArgs) => {
            var errorText = "Program exited with message:\n " + exceptionArgs.ExceptionObject;
            ui.AppendToLog(errorText);
            Environment.Exit(1);
        };
      
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(DisposeAllResourceHere);
        ui.Init();
        var title_name = ui.sett.title_name + " - Notepad";
        Run();
        async void Run() {
            using (ui.draw_main = new DrawMain(title_name)) {
                await ui.draw_main.Run();
            }
        }
    }
  
    static void DisposeAllResourceHere(object sender, EventArgs e) {
        //todo: need make dispose for same type - i not sure ui have IDisposable
        // all Base need call: OnGameClose  
    }
}
