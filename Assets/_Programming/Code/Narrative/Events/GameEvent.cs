using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Virtual Story/Game Event")]
public class GameEvent : StoryEvent
{
    public GameEvent() : base(EventType.game) {}
    
    public GameObject eventPrefab;
}