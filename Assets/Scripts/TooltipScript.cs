using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipScript : MonoBehaviour
{
    //Tooltip Instance
    public static TooltipScript Instance { get; private set; }

    //Get Tooltip Parts
    [SerializeField] private RectTransform canvasRectTransform;
    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject text;

    //Tooltip Delegate
    private System.Func<string> getTooltipTextFunc;

    //Tooltip Timer
    private bool startTooltipTimer;
    private float tooltipCurrentTimer = 0f;
    private float tooltipMaxTimer = 0.5f;

    private void Awake()
    {
        //Setup Tooltip
        Instance = this;

        backgroundRectTransform = transform.Find("Tooltip Background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("Tooltip Text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();

        HideTooltip();
    }

    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        //Get background size
        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 padding = new Vector2(8, 8);
        backgroundRectTransform.sizeDelta = textSize + padding;
    }

    private void Update()
    {
        if (startTooltipTimer)
        {
            if (tooltipCurrentTimer < tooltipMaxTimer)
            {
                tooltipCurrentTimer += Time.deltaTime;
            }
            else
            {
                background.SetActive(true);
                text.SetActive(true);
                SetText(getTooltipTextFunc());
            }
        }
        else
        {
            tooltipCurrentTimer = 0f;
        }

        //Continuously draw text
        SetText(getTooltipTextFunc());

        //Get mouse position
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;

        //check if tooltip is off screen
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            // Tooltip left screen on right side
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }

        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            // Tooltip left screen on right side
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }

    private void ShowTooltip(string tooltipText)
    {
        ShowTooltip(() => tooltipText);
    }

    private void ShowTooltip(System.Func<string> getTooltipTextFunc)
    {
        startTooltipTimer = true;
        gameObject.SetActive(true);
        this.getTooltipTextFunc = getTooltipTextFunc;
    }

    private void HideTooltip()
    {
        startTooltipTimer = false;
        tooltipCurrentTimer = 0f;
        background.SetActive(false);
        text.SetActive(false);
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipText)
    {
        Instance.ShowTooltip(tooltipText);
    }

    public static void ShowTooltip_Static(System.Func<string> getTooltipTextFunc)
    {
        Instance.ShowTooltip(getTooltipTextFunc);
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }
}
