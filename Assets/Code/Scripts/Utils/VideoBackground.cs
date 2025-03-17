using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Code from ChatGPT

public class VideoBackground: MonoBehaviour
{
    public RawImage rawImage;
    public string videoFileName = "BackgroundVideo.mp4";
    private VideoPlayer videoPlayer;

    void Start()
    {
        string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rawImage.texture as RenderTexture;
        videoPlayer.url = videoPath;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => vp.Play();
    }
}
