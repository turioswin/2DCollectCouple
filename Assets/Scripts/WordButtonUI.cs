using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordButtonUI : MonoBehaviour
{
    public int pairId;     // Уникальный ID пары
    public string word;    // Слово для отображения

    private GameManager manager;
    private Image buttonImage;
    private TMP_Text buttonText;
    private Button button;
    private Color originalColor;
    private bool isHighlighted = false;
    private bool isVisible = true;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();

        if (buttonImage != null)
            originalColor = buttonImage.color;

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    public void SetManager(GameManager gameManager)
    {
        manager = gameManager;
    }

    public void OnClick()
    {
        if (!isHighlighted || button.interactable) // Защита от повторного нажатия
            manager.OnWordButtonClicked(this);
    }

    public void Highlight(bool on)
    {
        isHighlighted = on;
        if (buttonImage != null)
            buttonImage.color = on ? new Color(0.9f, 0.85f, 0.2f) : originalColor; // Желтый при выделении
    }

    public void MarkIncorrect()
    {
        isHighlighted = false;
        if (buttonImage != null)
            buttonImage.color = new Color(1f, 0f, 0f, 1f); // Красный при ошибке
    }

    public void ResetState()
    {
        isHighlighted = false;
        if (buttonImage != null)
            buttonImage.color = originalColor;
        MakeVisible(true);
    }

    public void MakeVisible(bool visible)
    {
        isVisible = visible;

        if (buttonImage != null)
            buttonImage.enabled = visible;

        if (buttonText != null)
            buttonText.enabled = visible;

        if (button != null)
            button.interactable = visible;
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}

