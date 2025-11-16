using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimedObjectSwitcher : MonoBehaviour
{
    [Header("Objects to deactivate after 1 minute")]
    public GameObject[] objectsToDeactivate;

    [Header("Objects to activate after 1 minute")]
    public GameObject[] objectsToActivate;

    [Header("Delay before switching (in seconds)")]
    public float switchDelay = 60f;

    [Header("UI Timer Text")]
    public Text timerText; // ან TMPro.TextMeshProUGUI თუ TMP-ს იყენებ

    private float timeRemaining;

    private bool switched = false;

    public StoryEvent NextEvent;
    public GameObject gameExit;

    private ConversationSystem ConSys;

    private void Start()
    {
        ConSys = FindObjectOfType<ConversationSystem>();
        StartCoroutine(SwitchObjectsAfterDelay());
        timeRemaining = switchDelay; // ← აქ გამოგრჩა!

    }

    private void Update()
    {
        if (switched) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            // განახლება UI-ზე
            if (timerText != null)
                timerText.text =  Mathf.CeilToInt(timeRemaining).ToString();
        }
        else
        {
            ConSys.StopGame(NextEvent);
            Destroy(gameExit);
        }
    }

    IEnumerator SwitchObjectsAfterDelay()
    {
        yield return new WaitForSeconds(switchDelay);

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        Debug.Log("[TimedObjectSwitcher] Objects switched after " + switchDelay + " seconds.");
    }
}