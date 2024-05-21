using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter YouTube video URL: ");
        string videoUrl = Console.ReadLine();

        Console.WriteLine("Enter the directory where you want to save the file: ");
        string directoryPath = Console.ReadLine();

        Console.WriteLine("Enter '1' for video or '2' for audio: ");
        int choice = Convert.ToInt32(Console.ReadLine());

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        DownloadVideoAsync(videoUrl, directoryPath, choice).Wait();
    }

    static async Task DownloadVideoAsync(string videoUrl, string directoryPath, int choice)
    {
        var youtubeClient = new YoutubeClient();

        // Get video metadata
        var video = await youtubeClient.Videos.GetAsync(videoUrl);
        Console.WriteLine($"Title: {video.Title}");

        // Get available video streams
        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        IStreamInfo streamInfo;
        string fileExtension;

        if (choice == 1) // Video
        {
            var videoStreams = streamManifest.GetVideoStreams();
            string maxQualityLabel = videoStreams.Max(s => s.VideoQuality.Label);
            streamInfo = videoStreams.First(s => s.VideoQuality.Label == maxQualityLabel);
            fileExtension = streamInfo.Container.Name;
        }
        else // Audio
        {
            streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            fileExtension = "mp3";
        }

        // Construct the file path
        string filePath = Path.Combine(directoryPath, $"video-{video.Id}.{fileExtension}");

        // Download the video
        await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath);

        Console.WriteLine($"Download completed! File saved to: {filePath}");
    }
}
