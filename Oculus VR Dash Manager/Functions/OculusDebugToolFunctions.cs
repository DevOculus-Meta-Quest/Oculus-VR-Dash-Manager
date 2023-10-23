using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using YOVR_Dash_Manager.Functions;

public class OculusDebugToolFunctions : IDisposable
{
    private const string OculusDebugToolPath = @"C:\Program Files\Oculus\Support\oculus-diagnostics\OculusDebugToolCLI.exe";
    private Process process;
    private StreamWriter streamWriter;

    public OculusDebugToolFunctions()
    {
        InitializeProcess();
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
            Process process = new Process
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
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(ex, "An error occurred while executing the command with the file.");
        }
    }

    public void Dispose()
    {
        process?.Close();
        process?.Dispose();
        streamWriter?.Close();
        streamWriter?.Dispose();
    }
}