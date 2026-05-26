// CutscenePlayer.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutscenePlayer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;

    [Header("Image Fade")]
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private float fadeInDuration = 0.35f;

    [Header("End Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float endFadeDuration = 1.2f;
    [SerializeField] private float endHoldDelay = 0.4f;

    [Header("Testing")]
    [SerializeField] private CutsceneData testCutscene;

    private CutsceneData currentCutscene;
    private int currentLine;
    private Coroutine typingRoutine;
    private Coroutine lineRoutine;
    private bool isTyping;
    private bool isTransitioning;
    private string currentFullText;

    void Start()
    {
        //editor testing, pls empty testCutscene on real builds
        currentCutscene = CutsceneState.SelectedCutscene != null
            ? CutsceneState.SelectedCutscene
            : testCutscene;
        CutsceneState.Clear(); //clear so next playthrough needs a fresh assignment

        currentLine = 0;
        ShowLine(isFirstLine: true);
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            OnAdvanceInput();
    }

    void ShowLine(bool isFirstLine = false)
    {
        var line = currentCutscene.lines[currentLine];
        currentFullText = line.text;

        dialogueText.text = "";
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
        if (currentLine >= currentCutscene.lines.Length)
            OnCutsceneComplete();
        else
            ShowLine();
    }

    void OnCutsceneComplete()
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

        HandleCompletion();
    }

    void HandleCompletion()
    {
        switch (currentCutscene.onComplete)
        {
            case CutsceneCompletionAction.LoadScene:
                if (!string.IsNullOrEmpty(currentCutscene.nextSceneName))
                    SceneManager.LoadScene(currentCutscene.nextSceneName);
                else
                    Debug.Log("your phone linging (nextSceneName empty)");
                break;

            case CutsceneCompletionAction.DoNothing:
                Debug.Log("cutscene finished");
                break;
        }
    }
}