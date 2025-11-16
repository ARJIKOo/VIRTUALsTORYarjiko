using System;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public Image[] hearts; // 3 ცალკე სურათი UI-ში
    [SerializeField] public int currentLife = 3;

    public StoryEvent LoseEvent;
    public StoryEvent WinEvent;
    public GameObject destobject;

    private float gameTimer = 90f;
    private bool gameEnded = false;

    ConversationSystem convSys;

    private void Start()
    {
        convSys = FindObjectOfType<ConversationSystem>();
    }

    private void Update()
    {
        if (gameEnded) return;

        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0)
        {
            gameEnded = true;

            if (currentLife > 0)
            {
                // მოგება
                Destroy(destobject);
                convSys.StopGame(WinEvent);
            }
            else
            {
                // თუ დრო გავიდა მაგრამ უკვე წაგებული იყო, არაფერი
            }
        }
    }

    public void TakeDamage()
    {
        if (currentLife <= 0 || gameEnded) return;

        currentLife--;
        hearts[currentLife].enabled = false;

        if (currentLife == 0)
        {
            gameEnded = true;
            Destroy(destobject);
            convSys.StopGame(LoseEvent);
        }
    }
}