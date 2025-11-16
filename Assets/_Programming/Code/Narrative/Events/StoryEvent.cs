using UnityEngine;

public enum EventType {dialogue, selection, video, game,exit}
public class StoryEvent : ScriptableObject
{
    public StoryEvent previousEvent;
    public StoryEvent skipEvent;
    
    [HideInInspector]
    public EventType eventType; // Field to store the event type
    
    [Header("Particle Settings")]
    public string particleObjectName; // Store scene GameObject name
    public bool enableParticleObject = true;
    public bool useCustomPositionForCurrent = false;
    public Vector3 customPositionForCurrent;

    public bool useCustomPositionForPrevious = false;
    public Vector3 customPositionForPrevious;
    public bool objectContinue = true;


    
    // Constructor
    protected StoryEvent(EventType type) {
        eventType = type;
    }
}