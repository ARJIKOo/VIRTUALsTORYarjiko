using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public bool newSong;
    public bool dialogBoxbool; // ✅ New bool field to indicate if a dialog box is shown

    [TextArea(1,5)]
    public string line="ტესტ";
    
    public SceneEvent _event;
    public SceneNarrator _narrator;

    public AudioClip bgm;
}

[System.Serializable]
public class SceneEvent
{
    [Range(-2, 2)]
    public int speed;
    public AudioClip voiceover;
    public Texture image;
    public float transitionFade = 0.25f;

    public List<DialogueBoxSettings> dialogueBoxes = new List<DialogueBoxSettings>()
    {
        new DialogueBoxSettings(),
        new DialogueBoxSettings(),
        new DialogueBoxSettings()
    };
}


[System.Serializable]
public class SceneNarrator
{
    public string name;
    ///public Sprite sprite;
}

[System.Serializable]
public class Selection
{
    public string selectionText;
    public Color selectionColor = Color.white;

    public Sprite selectionSprite;
    public Color selectionUIColor = Color.black;

    public StoryEvent outcomeEvent;
}

[System.Serializable]
public class VideoDisplay
{
    public VideoClip eventVideo;
    public float transitionFade=0.25f;
    public AudioClip bgm;
}