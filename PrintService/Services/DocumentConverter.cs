// gswin64c.exe -r300 -sDEVICE=xpswrite -o output.xps -dNoPause -dBATCH .\sample.pdf

namespace PrintService.Services;

using System.Diagnostics;
using System.IO;

public class DocumentConverter
{
    public async Task<string> Convert(string pathToPdf)
    {
        if (!File.Exists(pathToPdf) || !pathToPdf.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ApplicationException($"The provided `{pathToPdf}` is not a pdf.");
        }
        string timeSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        string outputFileName = string.Concat(pathToPdf.AsSpan(0, pathToPdf.Length - 4), "-", timeSinceEpoch, ".xps");
        string arguments = $"-r300 -sDEVICE=xpswrite -o \"{outputFileName}\" -dNoPause -dBATCH \"{pathToPdf}\"";

        string baseDirectory = AppContext.BaseDirectory;
        string executablePath = Path.Combine(baseDirectory, "Resources", "converter.exe");

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new ApplicationException(
              $"Ghostscript process exited with code {process.ExitCode}.\nError Output:\n{error}"
            );
        }

        return outputFileName;
    }
}
