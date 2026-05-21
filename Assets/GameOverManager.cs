using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Endings")]
    [SerializeField] private EndingData[] endings;

    [Header("UI References")]
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;

    [Header("Image Fade")]
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private float fadeInDuration = 0.35f;

    [Header("Testing")]
    [SerializeField] private int testEndingIndex = 0;

    [Header("End Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float endFadeDuration = 1.2f;
    [SerializeField] private float endHoldDelay = 0.4f;

    public static int EndingIndex = -1;

    private EndingData currentEnding;
    private int currentLine;
    private Coroutine typingRoutine;
    private Coroutine lineRoutine;
    private bool isTyping;
    private bool isTransitioning;
    private string currentFullText;

    void Start()
    {
        int idx = EndingIndex >= 0 ? EndingIndex : testEndingIndex;
        idx = Mathf.Clamp(idx, 0, endings.Length - 1);
        currentEnding = endings[idx];
        currentLine = 0;
        ShowLine(isFirstLine: true);
    }

    void Update()
    {
        if (isTransitioning) return; //ignore input mid-fade

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            OnAdvanceInput();
    }

    void ShowLine(bool isFirstLine = false)
    {
        var line = currentEnding.lines[currentLine];
        currentFullText = line.text;

        dialogueText.text = ""; //clear text here
        if (continueIndicator) continueIndicator.SetActive(false);

        bool spriteChanged = isFirstLine || (line.image != null && displayImage.sprite != line.image);

        if (lineRoutine != null) StopCoroutine(lineRoutine);
        lineRoutine = StartCoroutine(PlayLine(line.image, spriteChanged));
    }

    IEnumerator PlayLine(Sprite newSprite, bool fade)
    {
        if (fade && newSprite != null)
        {
            isTransitioning = true;

            if (displayImage.sprite != null && displayImage.color.a > 0f)
                yield return FadeImage(1f, 0f, fadeOutDuration);

            displayImage.sprite = newSprite;

            yield return FadeImage(0f, 1f, fadeInDuration);

            isTransitioning = false;
        }
        else if (newSprite != null)
        {
            displayImage.sprite = newSprite;
        }
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypewriterRoutine());
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        float t = 0f;
        Color c = displayImage.color;
        c.a = from;
        displayImage.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            displayImage.color = c;
            yield return null;
        }

        c.a = to;
        displayImage.color = c;
    }

    IEnumerator TypewriterRoutine()
    {
        isTyping = true;
        if (continueIndicator) continueIndicator.SetActive(false);

        dialogueText.text = currentFullText;
        dialogueText.maxVisibleCharacters = 0;

        int total = currentFullText.Length;
        float interval = 1f / charactersPerSecond;
        float timer = 0f;
        int visible = 0;

        while (visible < total)
        {
            timer += Time.deltaTime;
            while (timer >= interval && visible < total)
            {
                timer -= interval;
                visible++;
                dialogueText.maxVisibleCharacters = visible;
            }
            yield return null;
        }

        isTyping = false;
        if (continueIndicator) continueIndicator.SetActive(true);
    }

    void OnAdvanceInput()
    {
        if (isTyping)
        {
            if (typingRoutine != null) StopCoroutine(typingRoutine);
            dialogueText.text = currentFullText;
            dialogueText.maxVisibleCharacters = currentFullText.Length;
            isTyping = false;
            if (continueIndicator) continueIndicator.SetActive(true);
        }
        else
        {
            Advance();
        }
    }

    void Advance()
    {
        currentLine++;
        if (currentLine >= currentEnding.lines.Length)
            OnEndingComplete();
        else
            ShowLine();
    }

    void OnEndingComplete()
    {
        if (continueIndicator) continueIndicator.SetActive(false);
        StartCoroutine(EndFadeRoutine());
    }

    IEnumerator EndFadeRoutine()
    {
        isTransitioning = true;

        if (endHoldDelay > 0f)
            yield return new WaitForSeconds(endHoldDelay);

        if (fadeGroup != null)
        {
            float t = 0f;
            float start = fadeGroup.alpha;
            while (t < endFadeDuration)
            {
                t += Time.deltaTime;
                fadeGroup.alpha = Mathf.Lerp(start, 0f, t / endFadeDuration);
                yield return null;
            }
            fadeGroup.alpha = 0f;
        }

        Debug.Log("ending finished");
        //expand here for scene changes
    }
}