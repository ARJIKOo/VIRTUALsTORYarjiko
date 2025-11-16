using UnityEngine;

[CreateAssetMenu(fileName = "New Selection Event", menuName = "Virtual Story/Selection Event")]
public class SelectionEvent : StoryEvent
{   
    public SelectionEvent() : base(EventType.selection) {}
    public Selection[] selection;
    public Texture image;
}