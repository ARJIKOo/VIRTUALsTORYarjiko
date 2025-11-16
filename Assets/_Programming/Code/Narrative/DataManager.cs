using UnityEngine;

[CreateAssetMenu(fileName = "New Data Manager", menuName = "Virtual Story/Data Manager")]
public class DataManager : ScriptableObject
{
    public StoryEvent storyEvent;
    public int line;
}
