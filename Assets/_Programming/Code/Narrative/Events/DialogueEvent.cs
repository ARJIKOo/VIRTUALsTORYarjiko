using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Virtual Story/Dialogue Event")]
public class DialogueEvent : StoryEvent
{
    public DialogueEvent() : base(EventType.dialogue) {}
    
    public Dialogue[] dialogue;
    
    public StoryEvent nextEvent;
}