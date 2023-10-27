using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

class Program
{
    static async Task Main(string[] args)
    {
        var youtubeClient = new YoutubeClient();
        var videoUrl = "https://www.youtube.com/watch?v=UT5F9AXjwhg";
        var video = await youtubeClient.Videos.GetAsync(videoUrl);

        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

        var videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality(); // Select the best video quality

        var outputVideoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{video.Title}.{videoStreamInfo.Container}");
        var outputAudioPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{video.Title}.mp3");

        using (var stream = await youtubeClient.Videos.Streams.GetAsync(videoStreamInfo))
        using (var output = File.Create(outputVideoPath))
        {
            await stream.CopyToAsync(output);
        }

        Console.WriteLine($"The video has been downloaded to: {outputVideoPath}");

        // Convert the video to MP3
        var inputFile = new MediaFile { Filename = outputVideoPath };
        var outputFile = new MediaFile { Filename = outputAudioPath };

        using (var engine = new Engine())
        {
            engine.GetMetadata(inputFile);
            engine.Convert(inputFile, outputFile);
        }

        Console.WriteLine($"The video has been converted to MP3: {outputAudioPath}");
    }
}
