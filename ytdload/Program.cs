using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Playlists;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using YoutubeExplode.Common;

class Program
{
    static async Task Main(string[] args)
    {
        var youtubeClient = new YoutubeClient();
        var playlistUrl = "https://www.youtube.com/playlist?list=PLL6BW4AG0XYfzmVoZJGEj2wemXrCX92LC";

        // Get all playlist videos
        var videos = await youtubeClient.Playlists.GetVideosAsync(playlistUrl);

        foreach (var video in videos)
        {
            var videoUrl = $"https://www.youtube.com/watch?v={video.Id}";
            var videoInfo = await youtubeClient.Videos.GetAsync(videoUrl);

            var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoInfo.Id);

            // Get the audio-only stream
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().FirstOrDefault();

            if (audioStreamInfo != null)
            {
                var outputAudioPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{videoInfo.Title}.mp3");

                using (var stream = await youtubeClient.Videos.Streams.GetAsync(audioStreamInfo))
                using (var output = File.Create(outputAudioPath))
                {
                    await stream.CopyToAsync(output);
                }

                Console.WriteLine($"Audio from video {videoInfo.Title} has been downloaded and saved as MP3: {outputAudioPath}");
            }
            else
            {
                Console.WriteLine($"No audio stream available for video {videoInfo.Title}");
            }
        }
    }
}
