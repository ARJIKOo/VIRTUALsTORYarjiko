using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Video;
using TMPro;

public class ConversationSystem : MonoBehaviour
{
    
    [Header("Custom UI")]
    public Image specialImage;
    
    bool isWaitingForVideoToEnd = false;

    List<char> punctuation = new List<char> { ',', '.', '!', '?' };

    public DataManager dataManager;
    public GameObject visuals;
    public StoryEvent currentEvent;
    
    [Header("Particles")]
    //public List<GameObject> allParticleSystems;
    public string testpranch;
    
    
    
    public bool lineOver = true, eventOver = false, selectionOver = true;
    public int lineIndex = 0;

    public List<RectTransform> dialogueBoxObjects;
    public List<Image> dialogueBoxImages;
    public List<TextMeshProUGUI> textBoxes;
    public List<TextMeshProUGUI> narratorTexts;

    public Transform selectionGroup;
    public GameObject selectionButton;

    public RawImage[] eventRenderers;
    bool flipShutter;

    public VideoPlayer vidPlayer;
    bool isPlaying = false;
    public GameObject skipObject;

    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    
    public GameObject settingsPanel; // დააგდე შენი UI პანელი აქ

    private bool isActive = false;
    
    public TransitionManager transitionManager; // Drag your TransitionManager object here

    

    Dialogue currentDialogue;
    SceneEvent currentSceneEvent;

    void Start()
    {
       
                specialImage.gameObject.SetActive(false);
        
        //allParticleSystems = new List<GameObject>(FindObjectsOfType<GameObject>());

        audioSource = GetComponent<AudioSource>();
        // დავიწყოთ Coroutine-ით, რომ ფეიდიც ჩავსვათ და მერე Proceed()
        StartCoroutine(InitialFadeAndStart());
        
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        
    }
    
    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    
    public void ToggleSettings()
    {
        isActive = !isActive;
        settingsPanel.SetActive(isActive);
    }


    IEnumerator InitialFadeAndStart()
    {
        // დაწყებისას ფეიდ ინ
        yield return StartCoroutine(TransitionManager.Instance.FadeIn());

        // ახლა ჩვეულებრივ გაგრძელდეს ივენთების დამუშავება
        Proceed();
    }


    public void LoadData()
    {
        StopAllCoroutines();
        DestroySelection();

        currentEvent = dataManager.storyEvent;
        lineIndex = dataManager.line;

        // სწორად ვთვლით, დასრულდა თუ არა ეს ივენთი
        if (currentEvent.eventType == EventType.dialogue)
        {
            DialogueEvent dlgEvt = currentEvent as DialogueEvent;
            eventOver = (lineIndex >= dlgEvt.dialogue.Length);
        }
        else
        {
            eventOver = false;
        }

        // ტექსტი და სურათები რომ სწორი მდგომარეობით ჩანდეს
        if (currentEvent.eventType == EventType.dialogue && !eventOver)
        {
            currentDialogue = (currentEvent as DialogueEvent).dialogue[lineIndex];
            currentSceneEvent = currentDialogue._event;
            FastForwardAll();
        }

        // პირდაპირ არ ვუშვებთ HandleEvent-ს, თუ ივენთი დასრულებულია
        if (!eventOver)
            HandleEvent(currentEvent);
    }


    public void SaveData()
    {
        if (dataManager)
        {
            dataManager.storyEvent = currentEvent;
            dataManager.line = lineIndex;
        }
    }

    
        public void Proceed()
        {
            // ❌ თუ ვიდეოს დასრულებას ველოდებით, დაბლოკე
            if (isWaitingForVideoToEnd || transitionManager.IsTransitioning) // ✅ დამატებული
                return;

            StopAllCoroutines();
            StartCoroutine(HandleProceed());
        }

    

    IEnumerator HandleProceed()
    {
        if (!lineOver)
        {
            FastForwardAll();
            lineOver = true;
            yield break;
        }

        // თუ ვიდეო ივენთია და ჯერ არ დასრულებულა, არ გააგრძელო
        if (currentEvent.eventType == EventType.video && !eventOver)
        {
            yield break;
        }

// თუ ივენთი დასრულებულია ან დიალოგია და დასრულდა, გადადით შემდეგზე
        if (eventOver || IsDialogueFinished())
        {
            yield return HandleNextEvent();
            yield break;
        }

        // თუ ჯერ კიდევ არ დასრულებულა, განაგრძე
        HandleEvent(currentEvent);
        skipObject.SetActive(currentEvent.skipEvent);
    }
    
    bool IsDialogueFinished()
    {
        if (currentEvent.eventType == EventType.dialogue)
        {
            DialogueEvent dlgEvt = currentEvent as DialogueEvent;
            return lineIndex >= dlgEvt.dialogue.Length;
        }
        return false;
    }





    void HandleEvent(StoryEvent evt)
    {
        
        // ✅ აქ ვაკონტროლებთ სურათს
        if (specialImage != null)
        {
            if (evt != null && evt.name == "kstartivents")
                specialImage.gameObject.SetActive(false);
            else
                specialImage.gameObject.SetActive(true);
        }

        
        HandleParticleObject(evt);
        switch (evt.eventType)
        {
            case EventType.dialogue:
                NextLine(evt as DialogueEvent, true);
                break;
            case EventType.selection:
                if (selectionOver) PresentSelection(evt as SelectionEvent);
                break;
            case EventType.video:
                PlayVideo(evt as VideoEvent);
                break;
            case EventType.game:
                PlayGame(evt as GameEvent);
                break;
            case EventType.exit:
                QuitGame();
                break;
        }
    }
    
    private GameObject lastParticleObject = null;

    private void HandleParticleObject(StoryEvent evt)
    {
        // 🔙 First, move previous object (if any)
        if (lastParticleObject != null)
        {
            if (evt.useCustomPositionForPrevious)
            {
                lastParticleObject.transform.position = evt.customPositionForPrevious;
                Debug.Log($"🔙 Moved previous particle object ({lastParticleObject.name}) to custom position: {evt.customPositionForPrevious}");
            }
            else
            {
                Vector3 prevPos = lastParticleObject.transform.position;
                prevPos.z = 1f;
                lastParticleObject.transform.position = prevPos;
                Debug.Log($"🔙 Moved previous particle object ({lastParticleObject.name}) to Z = 1");
            }
        }

        // 🔛 Now handle the current object
        if (string.IsNullOrEmpty(evt.particleObjectName))
            return;

        GameObject particleObject = GameObject.Find(evt.particleObjectName);
        if (particleObject == null)
        {
            Debug.LogWarning($"⚠️ Particle object not found: {evt.particleObjectName}");
            return;
        }

        if (evt.enableParticleObject)
        {
            particleObject.SetActive(true);

            if (evt.useCustomPositionForCurrent)
            {
                particleObject.transform.position = evt.customPositionForCurrent;
                Debug.Log($"📍 Activated {particleObject.name} at custom current position: {evt.customPositionForCurrent}");
            }
        }
        else
        {
            particleObject.SetActive(false);
            Debug.Log($"❌ Deactivated {particleObject.name}");
        }

        // 🔄 Store for next event
        lastParticleObject = particleObject;
    }




    void QuitGame()
    {
        Debug.Log("Exiting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void NextEvent()
    {
        StartCoroutine(HandleNextEvent());
    }

    IEnumerator HandleNextEvent()
    {
        // გაშავება
        yield return transitionManager.FadeOut();

        // შეცვალე ივენთი
        eventOver = false;
        audioSource.Stop();

        switch (currentEvent.eventType)
        {
            case EventType.dialogue:
                currentEvent = (currentEvent as DialogueEvent).nextEvent;
                break;
            case EventType.video:
                currentEvent = (currentEvent as VideoEvent).nextEvent;
                break;
        }

        // დაამუშავე ახალი ივენთი
        HandleEvent(currentEvent);

        // გაუფერულდეს ეკრანი
        yield return transitionManager.FadeIn();

        skipObject.SetActive(currentEvent.skipEvent);
    }


    void NextLine(DialogueEvent dialogueEvent, bool proceed)
    {
        StartCoroutine(HandleNextLine(dialogueEvent, proceed));
    }

    IEnumerator HandleNextLine(DialogueEvent dialogueEvent, bool proceed)
    {
       

        audioSource.Stop();
        lineOver = false;

        currentDialogue = dialogueEvent.dialogue[lineIndex];
        currentSceneEvent = currentDialogue._event;

        PlayMusic(currentDialogue.bgm);

        if (currentSceneEvent.image)
            yield return StartCoroutine(RenderEvent(currentSceneEvent.image, currentSceneEvent.transitionFade));



        if (currentSceneEvent.voiceover)
            audioSource.PlayOneShot(currentSceneEvent.voiceover);

        

        ApplyDialogueBoxSettings(currentSceneEvent);

        if (proceed) lineIndex++;

        if (lineIndex == dialogueEvent.dialogue.Length)
        {
            eventOver = true;
            lineIndex = 0;
        }

        yield break;

    }


    void ApplyDialogueBoxSettings(SceneEvent sceneEvent)
    {
        bool useRewrite = false;

        foreach (var box in sceneEvent.dialogueBoxes)
        {
            if (box.rewriteText)
            {
                useRewrite = true;
                break;
            }
        }

        if (useRewrite)
            StartCoroutine(SequentialRewrite(sceneEvent));
        else
            StartCoroutine(SequentialWrite(sceneEvent));
    }

    
    IEnumerator SequentialWrite(SceneEvent sceneEvent)
    {
        for (int i = 0; i < sceneEvent.dialogueBoxes.Count; i++)
        {
            var settings = sceneEvent.dialogueBoxes[i];

            dialogueBoxObjects[i].gameObject.SetActive(settings.enabled);

            if (settings.enabled)
            {
                dialogueBoxObjects[i].anchoredPosition = settings.position;
                dialogueBoxObjects[i].sizeDelta = settings.size;

                if (dialogueBoxImages[i] != null)
                    dialogueBoxImages[i].enabled = settings.dialogBoxbool;

                if (textBoxes[i] != null)
                {
                    textBoxes[i].fontSize = settings.fontSize;
                    textBoxes[i].text = "";
                }

                if (narratorTexts[i] != null)
                {
                    narratorTexts[i].text = string.IsNullOrEmpty(settings.narrator.name) ? "" : settings.narrator.name + ":";
                }

                yield return StartCoroutine(WriteText(i, settings.text, sceneEvent.speed + 7));
            }
            else
            {
                if (textBoxes[i] != null) textBoxes[i].text = "";
                if (narratorTexts[i] != null) narratorTexts[i].text = "";
            }
        }

        lineOver = true;
    }


    void PlayMusic(AudioClip music)
    {
        if (music && music != musicSource.clip)
        {
            musicSource.clip = music;
            musicSource.Play();
        }
        else if (!music)
        {
            musicSource.clip = null;
        }
    }

    IEnumerator WriteText(int boxIndex, string text, float speed)
    {
        if (dialogueBoxImages[boxIndex] != null)
            dialogueBoxImages[boxIndex].color = RGBa(255, 255, 255, 255);

        textBoxes[boxIndex].text = "";
        string displayedText = "";

        for (int i = 0; i < text.Length; i++)
        {
            char currentChar = text[i];
            displayedText += currentChar;

            float delay = 0.2f / speed;
            if (punctuation.Contains(currentChar) && i + 1 < text.Length && int.TryParse(text[i + 1].ToString(), out int nextDelay))
            {
                delay = nextDelay * 0.2f;
                text = text.Remove(i + 1, 1);
            }

            textBoxes[boxIndex].text = displayedText;
            yield return new WaitForSeconds(delay);
        }

        textBoxes[boxIndex].text = text;

        if (AllBoxesComplete())
            lineOver = true;
    }

    bool AllBoxesComplete()
    {
        for (int i = 0; i < textBoxes.Count; i++)
        {
            if (dialogueBoxObjects[i].gameObject.activeSelf && textBoxes[i].text == "")
                return false;
        }
        return true;
    }

    IEnumerator RenderEvent(Texture eventImage, float time)
    {
        float timer = 0f;
        RawImage nextRenderer = flipShutter ? eventRenderers[0] : eventRenderers[1];
        RawImage previousRenderer = flipShutter ? eventRenderers[1] : eventRenderers[0];

        nextRenderer.texture = eventImage;
        previousRenderer.texture = eventImage;

        while (timer < time)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1f, 0f, timer / time);

            nextRenderer.color = new Vector4(1, 1, 1, 1 - alpha);
            previousRenderer.color = new Vector4(1, 1, 1, alpha);

            yield return null;
        }

        flipShutter = !flipShutter;
    }

    void FastForwardAll()
    {
        for (int i = 0; i < currentSceneEvent.dialogueBoxes.Count; i++)
        {
            var settings = currentSceneEvent.dialogueBoxes[i];
            if (textBoxes[i].gameObject.activeSelf)
            {
                textBoxes[i].text = settings.text;
            }
        }

        if (currentSceneEvent?.image)
        {
            foreach (RawImage img in eventRenderers)
                img.texture = currentSceneEvent.image;
        }

        RawImage nextRenderer = flipShutter ? eventRenderers[0] : eventRenderers[1];
        RawImage previousRenderer = flipShutter ? eventRenderers[1] : eventRenderers[0];

        nextRenderer.color = new Vector4(1, 1, 1, 1);
        previousRenderer.color = new Vector4(1, 1, 1, 0);
    }

    void PresentSelection(SelectionEvent selectionEvent)
    {
        Texture eventImage = selectionEvent.image;
        foreach (RawImage img in eventRenderers)
            img.texture = eventImage;

        foreach (var box in dialogueBoxObjects)
        {
            box.gameObject.SetActive(false);
        }

        foreach (Selection selection in selectionEvent.selection)
        {
            Button newButton = Instantiate(selectionButton, selectionGroup).GetComponent<Button>();
            TMP_Text selectionText = newButton.transform.GetChild(0).GetComponent<TMP_Text>();
            selectionText.text = selection.selectionText;
            selectionText.color = selection.selectionColor;
            newButton.onClick.AddListener(() => Select(selection));
        }

        selectionOver = false;
    }

    public void Select(Selection selection)
    {
        DestroySelection();
        currentEvent = selection.outcomeEvent;
        Proceed();
    }

    void DestroySelection()
    {
        foreach (Transform button in selectionGroup)
            Destroy(button.gameObject);

        foreach (var box in dialogueBoxObjects)
        {
            box.gameObject.SetActive(true);
        }

        selectionOver = true;
    }

    void PlayVideo(VideoEvent videoEvent)
    {
        foreach (var tb in textBoxes)
            tb.text = "";

        foreach (var nt in narratorTexts)
            nt.text = "";

        foreach (var img in dialogueBoxImages)
            img.color = RGBa(255, 255, 255, 1);

        if (videoEvent.voiceover) audioSource.PlayOneShot(videoEvent.voiceover);
        PlayMusic(videoEvent.video.bgm);
        
        isWaitingForVideoToEnd = true; // ✅ აქ ჩაამატე


#if UNITY_WEBGL
        if (!string.IsNullOrEmpty(videoEvent.videoURL))
        {
            vidPlayer.source = VideoSource.Url;
            vidPlayer.url = videoEvent.videoURL;
        }
        else
        {
            Debug.LogWarning("WebGL build requires a video URL in VideoEvent.");
            return;
        }
#else
        vidPlayer.source = VideoSource.VideoClip;
        vidPlayer.clip = videoEvent.video.eventVideo;
#endif

        // ახალი RenderTexture შექმნა
        RenderTexture renderTex = new RenderTexture(1920, 1080, 24);
        vidPlayer.targetTexture = renderTex;

        // ვიდეოს წინასწარი მომზადება
        vidPlayer.Prepare();

        // ველოდებით სანამ მზად იქნება
        StartCoroutine(WaitForVideoAndPlay(renderTex, videoEvent));

        lineOver = false; // დასაწყისში false
        StartCoroutine(RenderEvent(renderTex, videoEvent.video.transitionFade));

        // ✅ დამატებული ტექსტბოქსის გამოტანა ვიდეოზე
        if (videoEvent._event != null)
            ApplyDialogueBoxSettings(videoEvent._event);

        // ✅ დამატებული ტექსტბოქსის გამოტანა ვიდეოზე
        if (videoEvent._event != null)
            ApplyDialogueBoxSettings(videoEvent._event);

        // ვუსმენთ ვიდეოს დასრულებას
        vidPlayer.loopPointReached += OnVideoFinished;
        
    }
    
    IEnumerator WaitForVideoAndPlay(RenderTexture renderTex, VideoEvent videoEvent)
    {
        while (!vidPlayer.isPrepared)
        {
            yield return null; // ველოდებით ყოველ ფრეიმზე
        }

        vidPlayer.Play();
        StartCoroutine(RenderEvent(renderTex, videoEvent.video.transitionFade));

        if (videoEvent._event != null)
            ApplyDialogueBoxSettings(videoEvent._event);

        
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        vidPlayer.loopPointReached -= OnVideoFinished;
        eventOver = true;
        isWaitingForVideoToEnd = false; // ✅ დამატებულია

        Proceed();
    }



    void PlayGame(GameEvent gameEvent)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            Instantiate(gameEvent.eventPrefab);
            visuals.SetActive(false);
        }
    }

    public void StopGame(StoryEvent nextEvent)
    {
        isPlaying = false;
        visuals.SetActive(true);
        currentEvent = nextEvent;
        Proceed();
    }

    Vector4 RGBa(int r, int g, int b, int a)
    {
        return new Vector4(r, g, b, a) / 255f;
    }
    
    
   
    
    IEnumerator SequentialRewrite(SceneEvent sceneEvent)
    {
        for (int i = 0; i < sceneEvent.dialogueBoxes.Count; i++)
        {
            var settings = sceneEvent.dialogueBoxes[i];
            dialogueBoxObjects[i].gameObject.SetActive(settings.enabled);

            if (settings.enabled)
            {
                dialogueBoxObjects[i].anchoredPosition = settings.position;
                dialogueBoxObjects[i].sizeDelta = settings.size;

                if (dialogueBoxImages[i] != null)
                    dialogueBoxImages[i].enabled = settings.dialogBoxbool;

                if (textBoxes[i] != null)
                {
                    textBoxes[i].fontSize = settings.fontSize;
                    textBoxes[i].text = "";
                }

                if (narratorTexts[i] != null)
                {
                    narratorTexts[i].text = string.IsNullOrEmpty(settings.narrator.name) ? "" : settings.narrator.name + ":";
                }

                if (settings.rewriteText)
                {
                    yield return StartCoroutine(RewriteMultipleTexts(i, settings, sceneEvent.speed + 7));
                }
                else
                {
                    yield return StartCoroutine(WriteText(i, settings.text, sceneEvent.speed + 7));
                }
            }
        }

        lineOver = true;
    }
    
    
    IEnumerator RewriteMultipleTexts(int boxIndex, DialogueBoxSettings settings, float speed)
    {
        string displayedText = "";

        int count = Mathf.Min(settings.rewriteCount, settings.rewriteTexts.Count);

        for (int r = 0; r < count; r++)
        {
            string nextText = settings.rewriteTexts[r];

            // წაშალე წინა ტექსტი
            for (int i = displayedText.Length - 1; i >= 0; i--)
            {
                displayedText = displayedText.Substring(0, i);
                textBoxes[boxIndex].text = displayedText;
                yield return new WaitForSeconds(0.03f);
            }

            // დაწერე ახალი ტექსტი
            displayedText = "";
            for (int i = 0; i < nextText.Length; i++)
            {
                displayedText += nextText[i];
                textBoxes[boxIndex].text = displayedText;
                yield return new WaitForSeconds(0.5f / speed);
            }

            // თითოეული ჩანაცვლების შემდეგ მცირე პაუზა
            yield return new WaitForSeconds(1f);
        }
    }




}



