using UnityEngine;
using TMPro;

public class DrawingPromptManager : MonoBehaviour
{
    public TextMeshProUGUI promptTextUI;
    public DrawingPrompt[] prompts; // Add this
    private int currentIndex = 0;

    void Start()
    {
        ShowPrompt(prompts[currentIndex]);
    }

    public void ShowPrompt(DrawingPrompt prompt)
    {
        promptTextUI.text = prompt.promptText;
    }

    public void ShowNextPrompt()
    {
        currentIndex++;
        if (currentIndex >= prompts.Length)
            currentIndex = 0; // loop back to start

        ShowPrompt(prompts[currentIndex]);
    }
}