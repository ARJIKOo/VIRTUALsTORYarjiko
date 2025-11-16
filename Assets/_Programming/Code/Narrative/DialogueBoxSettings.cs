using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class DialogueBoxSettings
{
    public CameraEffects cameraEffects; // Drag this in Inspector

    public bool enabled = false;                        // ჩართულია თუ არა ბოქსი
    public bool dialogBoxbool = false;                  // აჩვენოს თუ არა ბოქსის ვიზუალი
    public Vector2 position = new Vector2(0, 0);       // პოზიცია
    public Vector2 size = new Vector2(1280, 225);      // ზომა
    public float fontSize = 36;                        // ფონტის ზომა
    
    //public Font font;
    [TextArea(1, 5)]
    public string text = "";                           // ტექსტი

    public SceneNarrator narrator = new SceneNarrator(); // მთხრობელი თითო ბოქსისთვის
    
    [Header("Rewrite Mode")]
    public bool rewriteText = false;                   // გამოიყენოს თუ არა წაშლის და თავიდან წერის რეჟიმი
    [TextArea(1, 5)]
    public string textBeforeRewrite = "";              // პირველი ტექსტი

    [Tooltip("რამდენჯერ შეცვალოს ტექსტი ბოქსში ავტომატურად")]
    public int rewriteCount = 1;                       // რამდენჯერ უნდა შეცვალოს ტექსტი

    [Tooltip("ყველა ტექსტი, რაც თანმიმდევრულად უნდა გამოაჩინოს შეცვლისას")]
    public List<string> rewriteTexts = new List<string>(); // სიით ჩაწერე ტექსტები რომლებიც უნდა გამოაჩინოს ერთ ბოქსში ეტაპობრივად
    
    

}