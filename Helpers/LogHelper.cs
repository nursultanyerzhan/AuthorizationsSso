
namespace AuthorizationsSso.Helpers;

public class LogHelper
{
    public static void WriteToLogFile(string message)
    {
        try
        {
            var now = DateTime.Now;

            string fileName = "RequestLogs\\" + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            using (var stw = File.AppendText(filePath))
            {
                stw.WriteLine("{0}",
                    $"{now.ToString("dd.MM.yyyy HH:mm:ss")} : {message}"
                );

                stw.Flush();
                stw.Close();
            }
        }
        catch(Exception ex)
        { }

    }
}