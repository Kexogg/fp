using System.Diagnostics;
using TagsCloudContainerCore.Models;

namespace TagsCloudContainerCore.TextProcessor.MyStem;

public class MyStemWrapper : IDisposable
{
    private static string PathToBinary =>
        Environment.OSVersion.Platform == PlatformID.Win32NT ? "Mystem/mystem.exe" : "Mystem/mystem";
    private Process? _process;
    private StreamWriter _inputWriter;
    private StreamReader _outputReader;
    private StreamReader _errorReader;

    public Result<None> StartProcess(string arguments = "-in")
    {
        if (!File.Exists(PathToBinary))
        {
            return Result.Fail<None>("MyStem binary not found");
        }
        var psi = new ProcessStartInfo
        {
            FileName = PathToBinary,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        if (process == null || process.HasExited)
        {
            return Result.Fail<None>("Failed to start MyStem process");
        }

        _process = process;
        _inputWriter = _process.StandardInput;
        _outputReader = _process.StandardOutput;
        _errorReader = _process.StandardError;
        return Result.Ok();
    }
    

    //TODO: fix (TSystemError) (Broken pipe) util/stream/output.cpp:322: fflush failed Aborted.
    public Result<MyStemProcessedWord?> ProcessWord(string word)
    {
        if (_process == null || _process.HasExited)
        {
            return Result.Fail<MyStemProcessedWord?>("Process is not running.");
        }
        
        _inputWriter.WriteLine(word);
        //Possible fix?
        //_inputWriter.Flush();
        var line = _outputReader.ReadLine();
        if (line != null) return ParseResult(line);
        var error = _errorReader.ReadLine();
        return Result.Fail<MyStemProcessedWord?>($"MyStem process failed: {error}");
    }

    private void StopProcess()
    {
        _inputWriter.Close();
        _outputReader.Close();
        _process?.WaitForExit();
        _process?.Dispose();
    }

    public void Dispose()
    {
        StopProcess();
        GC.SuppressFinalize(this);
    }


    //TODO: оптипизировать 
    private static Result<MyStemProcessedWord?> ParseResult(string raw)
    {
        // Пример парса: "сделал{сделать=V,сов,пе=прош,ед,изъяв,муж}"
        // Пример ошибки: ъ{ъ??}
        var startIndex = raw.IndexOf('{') + 1;
        var endIndex = raw.IndexOf('}');
        var metadata = raw.Substring(startIndex, endIndex - startIndex).Split(',');

        if (metadata.Length == 0 || !metadata[0].Contains('=') || metadata[0].Contains('{'))
        {
            return null;
        }

        var rootForm = metadata[0].Split('=')[0];
        if (rootForm.Contains('?'))
        {
            rootForm = rootForm.Remove(rootForm.IndexOf('?'));
        }
        var partOfSpeech = metadata[0].Split('=')[1];

        if (Enum.TryParse(partOfSpeech, out PartOfSpeech pos))
        {
            return new MyStemProcessedWord(rootForm, pos);
        }
        
        return Result.Fail<MyStemProcessedWord?>("Invalid part of speech in raw string");
    }
}

