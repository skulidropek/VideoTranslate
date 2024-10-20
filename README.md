# Видео процессор на основе yt-dlp, vot-cli и FFmpeg

Этот проект скачивает видео с YouTube, получает перевод аудио с помощью `vot-cli`, а затем сшивает видео с переведённой аудиодорожкой с использованием `FFmpeg`.

## Требования

Перед началом работы необходимо установить следующие инструменты:

### 1. Python и yt-dlp
Для работы с `yt-dlp` потребуется Python. Скачайте и установите Python с официального сайта: https://www.python.org/downloads/

После установки Python, установите `yt-dlp` через командную строку:

pip install yt-dlp

### 2. FFmpeg
FFmpeg необходим для обработки видео и аудио. Установить его можно через пакетный менеджер Chocolatey.

1. Установите [Chocolatey](https://chocolatey.org/install) (если не установлен):
    Откройте PowerShell от имени администратора и выполните команду:

    Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

2. После установки Chocolatey установите FFmpeg:

    choco install ffmpeg

### 3. vot-cli
`vot-cli` используется для перевода аудио. Установите `vot-cli` через Python:

pip install vot-cli

### 4. .NET SDK
Данный скрипт написан на C# и требует установленного .NET SDK. Скачайте его с официального сайта: https://dotnet.microsoft.com/download

## Как запустить

1. Клонируйте репозиторий или скопируйте исходный код в локальный проект.
2. Откройте проект в любом редакторе кода (Visual Studio, Visual Studio Code и т.д.).
3. Убедитесь, что все вышеуказанные зависимости установлены.
4. Откройте командную строку или терминал в папке с проектом.
5. Запустите проект командой:

dotnet run

## Как изменить видео URL

Чтобы изменить видео для обработки, просто передайте новую ссылку на видео в вызов метода `ProcessVideoAsync` в файле `Program.cs`:

await ProcessVideoAsync("https://www.youtube.com/watch?v=YOUR_VIDEO_URL", "output_video");

Замените `"YOUR_VIDEO_URL"` на нужную ссылку.

## Примечания

- Убедитесь, что FFmpeg добавлен в переменные среды PATH, или укажите путь к `ffmpeg.exe` вручную в коде.
- В некоторых случаях `vot-cli` может возвращать ошибки. Проверьте, правильно ли работает сервис перевода.
