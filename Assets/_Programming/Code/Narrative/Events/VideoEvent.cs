using UnityEngine;

[CreateAssetMenu(fileName = "New Video Event", menuName = "Virtual Story/Video Event")]
public class VideoEvent : StoryEvent
{
    public VideoEvent() : base(EventType.video) {}

    public VideoDisplay video;
    public StoryEvent nextEvent;
    public AudioClip voiceover;
    public SceneEvent _event;  

    [Header("WebGL URL fallback")]
    public string videoURL;  // <-- Add this
}