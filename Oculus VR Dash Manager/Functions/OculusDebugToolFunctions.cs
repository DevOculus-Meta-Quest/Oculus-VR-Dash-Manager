using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System;

public class OculusDebugToolFunctions : IDisposable
{
    private Process process;
    private StreamWriter streamWriter;

    public OculusDebugToolFunctions()
    {
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Oculus\Support\oculus-diagnostics\OculusDebugToolCLI.exe",
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

    public async Task ExecuteCommandWithFileAsync(string command)
    {
        string tempFilePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFilePath, command);

        string cliCommand = $"-f \"{tempFilePath}\"";
        Debug.WriteLine($"Sending command: {cliCommand}");

        await ExecuteCommandAsync(cliCommand);
    }

    public void Dispose()
    {
        process?.Close();
        process?.Dispose();
        streamWriter?.Close();
        streamWriter?.Dispose();
    }
}
