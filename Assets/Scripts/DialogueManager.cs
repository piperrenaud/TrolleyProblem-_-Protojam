using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;



public enum Speaker
{
    KeanuReeves,
    MichealReeves,
    MaxVerstappen,
    MarkLenny,
    AdrianLenny
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 6)]
    public string text;

    public Speaker speaker;
}



public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Lines")]
    public DialogueLine[] lines;

    [Header("UI")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueBodyText;
    public GameObject dialoguePanel;
    public GameObject NextSceneButton;
    public Button continueButton;

    [Header("Character Sprites")]
    public GameObject KeanuSprite;
    public GameObject MichealSprite;
    public GameObject MaxSprite;

    public float typewriterSpeed = 0.03f;






    private int currentIndex = 0;
    private bool isTyping = false;
    private bool skipRequested = false;
    private Coroutine typewriterCoroutine;
    private void Start()
    {
        SetAllSpritesInactive();

        if (continueButton != null)
            continueButton.onClick.AddListener(OnAdvance);

        ShowLine(currentIndex);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            OnAdvance();

        /*if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            OnAdvance();*/ // removing left click cuz its clunky
    }



    private void OnAdvance()
    {
        if (isTyping)
        {
            skipRequested = true;
        }
        else
        {
            currentIndex++;
            if (currentIndex < lines.Length)
                ShowLine(currentIndex);
            else
                EndDialogue();
        }
    }

    private void ShowLine(int index)
    {
        DialogueLine line = lines[index];

        //speakerNameText.text = line.speaker == Speaker.MaxVerstappen ? "Unknown" : line.speaker.ToString();
        speakerNameText.text = line.speaker.ToString();
        ActivateSpriteFor(line.speaker);

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        typewriterCoroutine = StartCoroutine(TypewriterRoutine(line.text));
    }

    private IEnumerator TypewriterRoutine(string fullText)
    {
        isTyping = true;
        skipRequested = false;
        dialogueBodyText.text = "";

        foreach (char c in fullText)
        {

            if (skipRequested)
            {
                dialogueBodyText.text = fullText;
                break;
            }

            dialogueBodyText.text += c;

            if (typewriterSpeed > 0f)
                yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        skipRequested = false;
    }

    //when i was using sprites to popup this was needed, but now that they are built in this becomes redundant,
    //but i will keep it here just in case i want to add some popups later
    private void ActivateSpriteFor(Speaker speaker)
    {
        SetAllSpritesInactive();

        switch (speaker)
        {
            case Speaker.KeanuReeves: if (KeanuSprite != null)
                {
                    KeanuSprite.SetActive(true); 
                }
                break;
            case Speaker.MichealReeves:
                if (MichealSprite != null)
                {
                    MichealSprite.SetActive(true);
                }
                break;
            case Speaker.MaxVerstappen:
                if (MaxSprite != null)
                {
                    MaxSprite.SetActive(true);
                }
                break;
        }
    }

    public void GetCurrentSpeaker()
    {
        return Speaker;
    }
    private void SetAllSpritesInactive()
    {
        if (KeanuSprite != null)
        {
            KeanuSprite.SetActive(false);
        }
        if (MaxSprite != null)
        {
            MaxSprite.SetActive(false);
        }
        if (MichealSprite != null)
        {
            MichealSprite.SetActive(false);
        }
    }

    private void EndDialogue()
    {
        SetAllSpritesInactive();
        dialoguePanel.SetActive(false);
        NextSceneButton.SetActive(true);
        
        Debug.Log("Dialogue finished do smn");
    }
}