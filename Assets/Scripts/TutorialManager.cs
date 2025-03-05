using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform bottomPanel;
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button fullScreenButton; // Add this field

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup[] panelCanvasGroups;
    private Coroutine currentFadeRoutine;

    [SerializeField] ScrollRect shelfCanvasScroll;

    [System.Serializable]
    public class TutorialStep
    {
        public RectTransform targetElement; 
        public Button targetButton;
        public string message;
    }

    [SerializeField] public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    private int currentStep = -1;

    private void Awake()
    {
        InitializeCanvasGroups();
        fullScreenButton.gameObject.SetActive(false); // Start with fullscreen button disabled

    }
    public void StartTutorial()
    {
        shelfCanvasScroll.vertical = false;
        if (tutorialSteps.Count > 0) ShowStep(0);
    }
    public IEnumerator startAfterTime(float time)
    {
        PlayerPrefs.SetInt("FIRSTTIME", 0);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(time);
        StartTutorial();
    }
    private void ShowStep(int stepIndex)
    {
        if (stepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        int previousStep = currentStep;
        currentStep = stepIndex;
        TutorialStep step = tutorialSteps[currentStep];

        ClearPreviousListeners(previousStep);
        ConfigurePanels(step.targetElement);
        tutorialText.text = step.message;

        // Clear any existing fullscreen button listeners
        fullScreenButton.onClick.RemoveAllListeners();
        fullScreenButton.gameObject.SetActive(false);

        if (step.targetButton != null)
        {
            step.targetButton.onClick.AddListener(NextStep);
        }
        else
        {
            // Enable fullscreen button if no target button is assigned
            fullScreenButton.gameObject.SetActive(true);
            fullScreenButton.onClick.AddListener(NextStep);
        }
    }

    private void ClearPreviousListeners(int previousStep)
    {
        if (previousStep >= 0 && tutorialSteps[previousStep].targetButton != null)
        {
            tutorialSteps[previousStep].targetButton.onClick.RemoveListener(NextStep);
        }
    }

    public void NextStep() => ShowStep(currentStep + 1);

     public void EndTutorial() {

        SetPanelsVisibility(false);
        fullScreenButton.gameObject.SetActive(false);
        shelfCanvasScroll.vertical = true;
        PlayerPrefs.SetInt("FIRSTTIME", 1);
        PlayerPrefs.Save();
    }


    private void InitializeCanvasGroups()
    {
        panelCanvasGroups = new CanvasGroup[4];
        panelCanvasGroups[0] = topPanel.GetComponent<CanvasGroup>();
        panelCanvasGroups[1] = bottomPanel.GetComponent<CanvasGroup>();
        panelCanvasGroups[2] = leftPanel.GetComponent<CanvasGroup>();
        panelCanvasGroups[3] = rightPanel.GetComponent<CanvasGroup>();

    }

    private void ConfigurePanels(RectTransform target)
    {
        SetPanelsVisibility(true);
        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(FadePanels(0.3f, 0.9f));
        // Get canvas dimensions
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.rect.size;

        // Get target bounds in canvas space
        Vector3[] targetCorners = new Vector3[4];
        target.GetWorldCorners(targetCorners);

        Vector2 min = canvasRect.InverseTransformPoint(targetCorners[0]);
        Vector2 max = canvasRect.InverseTransformPoint(targetCorners[2]);

        // Add pixel-perfect rounding to eliminate gaps
        min = new Vector2(Mathf.Floor(min.x), Mathf.Floor(min.y));
        max = new Vector2(Mathf.Ceil(max.x), Mathf.Ceil(max.y));

        // Convert to normalized coordinates (0-1) with pixel-perfect adjustment
        Vector2 normalizedMin = new Vector2(
            (min.x + canvasSize.x / 2) / canvasSize.x,
            (min.y + canvasSize.y / 2) / canvasSize.y
        );

        Vector2 normalizedMax = new Vector2(
            (max.x + canvasSize.x / 2) / canvasSize.x,
            (max.y + canvasSize.y / 2) / canvasSize.y
        );

        // Add slight overlap to prevent gaps
        float pixelOverlapx = .5f / canvasSize.x;
        float pixelOverlapy = .5f / canvasSize.y;
        normalizedMin.x -= pixelOverlapx;
        normalizedMax.x += pixelOverlapx;
        normalizedMin.y -= pixelOverlapy;
        normalizedMax.y += pixelOverlapy;

        
        // Set panel anchors with adjusted values
        SetPanel(topPanel,
            anchorMin: new Vector2(0, normalizedMax.y),
            anchorMax: new Vector2(1, 1));

        SetPanel(bottomPanel,
            anchorMin: new Vector2(0, 0),
            anchorMax: new Vector2(1, normalizedMin.y));

        SetPanel(leftPanel,
            anchorMin: new Vector2(0, normalizedMin.y),
            anchorMax: new Vector2(normalizedMin.x, normalizedMax.y));

        SetPanel(rightPanel,
            anchorMin: new Vector2(normalizedMax.x, normalizedMin.y),
            anchorMax: new Vector2(1, normalizedMax.y));
    }

    private IEnumerator FadePanels(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        foreach (CanvasGroup cg in panelCanvasGroups) cg.alpha = startAlpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            foreach (CanvasGroup cg in panelCanvasGroups)
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        foreach (CanvasGroup cg in panelCanvasGroups) cg.alpha = targetAlpha;

        if (targetAlpha == 0) SetPanelsVisibility(false);
    }


    private void SetPanel(RectTransform panel, Vector2 anchorMin, Vector2 anchorMax)
{
    panel.anchorMin = anchorMin;
    panel.anchorMax = anchorMax;
    panel.offsetMin = Vector2.zero;
    panel.offsetMax = Vector2.zero;
    panel.ForceUpdateRectTransforms(); // Add this line
}

    private void SetPanelsVisibility(bool visible)
    {
        topPanel.gameObject.SetActive(visible);
        bottomPanel.gameObject.SetActive(visible);
        leftPanel.gameObject.SetActive(visible);
        rightPanel.gameObject.SetActive(visible);
        tutorialText.gameObject.SetActive(visible);
    }
}