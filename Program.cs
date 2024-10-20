using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            { "https://www.youtube.com/watch?v=sz_LgBAGYyo", "Lecture 7 - How to Build Products Users Love (Kevin Hale)" },
            { "https://www.youtube.com/watch?v=oQOC-qy-GDY", "Lecture 8 - How to Get Started, Doing Things that Don't Scale, Press" },
            { "https://www.youtube.com/watch?v=uFX95HahaUs", "Lecture 9 - How to Raise Money (Marc Andreessen, Ron Conway, Parker Conrad)" },
            { "https://www.youtube.com/watch?v=RfWgVWGEuGE", "Lecture 10 - Culture (Brian Chesky, Alfred Lin)" },
            { "https://www.youtube.com/watch?v=H8Dl8rZ6qwE", "Lecture 11 - Hiring and Culture, Part 2 (Patrick and John Collison, Ben Silbermann)" },
            { "https://www.youtube.com/watch?v=tFVDjrvQJdw", "Lecture 12 - Building for the Enterprise (Aaron Levie)" },
            { "https://www.youtube.com/watch?v=dQ7ZvO5DpIw", "Lecture 13 - How to be a Great Founder (Reid Hoffman)" },
            { "https://www.youtube.com/watch?v=6fQHLK1aIBs", "Lecture 14 - How to Operate (Keith Rabois)" },
            { "https://www.youtube.com/watch?v=uVhTvQXfibU", "Lecture 15 - How to Manage (Ben Horowitz)" },
            { "https://www.youtube.com/watch?v=qAws7eXItMk", "Lecture 16 - How to Run a User Interview (Emmett Shear)" },
            { "https://www.youtube.com/watch?v=F4K_qVlYQkg", "Lecture 17 - How to Design Hardware Products (Hosain Rahman)" },
            { "https://www.youtube.com/watch?v=EHzvmyMJEK4", "Lecture 18 - Legal and Accounting Basics for Startups (Kirsty Nathoo, Carolynn Levy)" },
            { "https://www.youtube.com/watch?v=SHAh6WKBgiE", "Lecture 19 - Sales and Marketing; How to Talk to Investors (Tyler Bosmeny; YC Partners)" },
            { "https://www.youtube.com/watch?v=59ZQ-rf6iIc", "Lecture 20 - Later-stage Advice (Sam Altman)" },
            { "https://www.youtube.com/watch?v=cfV70T6Owpw", "YC Startup Talk for Students, 2022" },


        };
        //// Пример использования метода
        //string videoUrl = "https://www.youtube.com/watch?v=n_yHZ_vKjno";
        //string outputVideoName = "Lecture 6 - Growth (Alex Schultz)"; // Имя для сохранения видео

        foreach(var url in urls)
        {
            Console.WriteLine($"Обработка видео: {url.Value}");
            await ProcessVideoAsync(url.Key, url.Value);
            Console.WriteLine($"Конец обработки видео: {url.Value}");
        }
    }

    static async Task ProcessVideoAsync(string videoUrl, string outputVideoName)
    {
        // Получаем текущую директорию программы
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // Пути для сохранения видео и аудио
        string videoFilePath = Path.Combine(currentDirectory, $"{outputVideoName}.webm");
        string audioFilePath = Path.Combine(currentDirectory, $"{outputVideoName}_audio.mp3");
        string outputFilePath = Path.Combine(currentDirectory, $"{outputVideoName}_final.mp4");

        // Шаг 1: Команда для yt-dlp, скачиваем видео (с использованием правильного формата .webm)
        string ytDlpCommand = $"yt-dlp -f bestvideo+bestaudio --merge-output-format webm -o \"{videoFilePath}\" {videoUrl}";

        // Лог скачивания видео
        Console.WriteLine($"Скачивание видео по ссылке: {videoUrl}");
        await RunCommandAsync("cmd.exe", "/c " + ytDlpCommand); // Исправляем вызов RunCommandAsync

        // Проверяем, скачано ли видео
        if (!File.Exists(videoFilePath))
        {
            Console.WriteLine($"Видео не найдено по пути: {videoFilePath}");
            return;
        }

        // Шаг 2: Команда для vot-cli, получаем ссылку на переведённое аудио
        string votCliCommand = $"vot-cli {videoUrl}";
        Console.WriteLine("Запуск vot-cli для перевода аудио...");
        string translatedAudioUrl = RunCommandAndGetOutput(votCliCommand);

        // Извлекаем URL аудиофайла из вывода vot-cli
        string audioUrl = ExtractAudioUrl(translatedAudioUrl);
        if (string.IsNullOrEmpty(audioUrl))
        {
            Console.WriteLine("Не удалось получить ссылку на переведённое аудио.");
            return;
        }

        // Лог скачивания аудио
        Console.WriteLine($"Скачивание аудио по ссылке: {audioUrl}");
        await DownloadFileAsync(audioUrl, audioFilePath);

        // Проверяем, скачано ли аудио
        if (!File.Exists(audioFilePath))
        {
            Console.WriteLine($"Аудио не найдено по пути: {audioFilePath}");
            return;
        }

        // Шаг 3: Команда для FFmpeg, проверяем пути
        Console.WriteLine($"Видео файл: {videoFilePath}");
        Console.WriteLine($"Аудио файл: {audioFilePath}");

        // Укажем полный путь к ffmpeg.exe, если он не в PATH
        string ffmpegPath = @"C:\ProgramData\chocolatey\bin\ffmpeg.exe";  // Укажи правильный путь к ffmpeg.exe

        // Шаг 4: Команда для FFmpeg с логированием и отслеживанием прогресса
        string ffmpegCommand = $"-y -loglevel info -progress - -nostats -i \"{videoFilePath}\" -i \"{audioFilePath}\" -c:v libx264 -preset fast -c:a aac -map 0:v:0 -map 1:a:0 -shortest \"{outputFilePath}\"";

        // Лог выполнения команды FFmpeg
        Console.WriteLine("Запуск FFmpeg для сшивания аудио и видео с логированием...");
        await RunCommandAsync(ffmpegPath, ffmpegCommand); // Передаем путь к FFmpeg и саму команду

        // Проверяем, создано ли новое видео
        if (File.Exists(outputFilePath))
        {
            Console.WriteLine($"Видео с новой аудиодорожкой создано: {outputFilePath}");
        }
        else
        {
            Console.WriteLine("Не удалось создать видео с новой аудиодорожкой.");
        }
    }

    static async Task DownloadFileAsync(string fileUrl, string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(fileUrl);
            response.EnsureSuccessStatusCode();

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }

    static string RunCommandAndGetOutput(string command)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;  // Для получения ошибок
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;

        try
        {
            Process process = Process.Start(processInfo);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);  // Выводим весь лог команды
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Ошибка при выполнении команды: {error}");
            }
            return output;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return null;
        }
    }

    static async Task RunCommandAsync(string executablePath, string command)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo(executablePath, command);
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;  // Для получения ошибок и прогресса
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = processInfo;

        // Подписываемся на асинхронное чтение потоков
        process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
        process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

        process.Start();

        // Асинхронно читаем оба потока
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();  // Ожидаем завершения процесса
    }

    static string ExtractAudioUrl(string votCliOutput)
    {
        // Поиск строки с URL аудиофайла в выводе vot-cli
        string keyword = "Audio Link";
        int startIndex = votCliOutput.IndexOf(keyword);
        if (startIndex != -1)
        {
            int urlStart = votCliOutput.IndexOf('"', startIndex) + 1;
            int urlEnd = votCliOutput.IndexOf('"', urlStart);
            return votCliOutput.Substring(urlStart, urlEnd - urlStart);
        }
        return null;
    }
}
