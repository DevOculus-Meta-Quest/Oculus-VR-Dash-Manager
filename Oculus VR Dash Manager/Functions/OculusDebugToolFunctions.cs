using OVR_Dash_Manager.Functions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class OculusDebugToolFunctions : IDisposable
{
    private const string OculusDebugToolPath = @"C:\Program Files\Oculus\Support\oculus-diagnostics\OculusDebugToolCLI.exe";
    private Process process;
    private StreamWriter streamWriter;

    public OculusDebugToolFunctions() => InitializeProcess();

    public void Dispose()
    {
        process?.Close();
        process?.Dispose();
        streamWriter?.Close();
        streamWriter?.Dispose();
    }

    public async Task ExecuteCommandAsync(string command)
    {
        Debug.WriteLine($"Sending command: {command}");
        await streamWriter.WriteLineAsync(command);
        await streamWriter.WriteLineAsync("exit");
        await streamWriter.FlushAsync();
    }

    public void ExecuteCommandWithFile(string tempFilePath)
    {
        try
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = OculusDebugToolPath,
                    Arguments = $"-f \"{tempFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"Output: {output}\nError: {error}", "Command Execution Result");
            }
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex, "An error occurred while executing the command with the file.");
        }
    }

    private void InitializeProcess()
    {
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = OculusDebugToolPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, args) => Debug.WriteLine(args.Data);
        process.ErrorDataReceived += (sender, args) => Debug.WriteLine(args.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        streamWriter = process.StandardInput;
    }
}