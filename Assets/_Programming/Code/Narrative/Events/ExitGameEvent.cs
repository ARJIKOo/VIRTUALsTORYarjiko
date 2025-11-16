using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Exit Game Event", menuName = "Virtual Story/Exit Game Event")]
public class ExitGameEvent : StoryEvent
{
    private void OnEnable()
    {
        eventType = EventType.exit;
    }

    public void ExecuteEvent()
    {
#if UNITY_WEBGL
        Debug.Log("ExitGameEvent: Application.Quit() არ მუშაობს WebGL-ზე. ვუბრუნდებით მთავარ მენიუს.");
        SceneManager.LoadScene(0); // ინდექსით ჩატვირთვა
#else
        Application.Quit();
#endif
    }

    protected ExitGameEvent(EventType type) : base(type)
    {
    }
}