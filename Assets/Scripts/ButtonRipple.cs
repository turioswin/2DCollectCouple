using UnityEngine;
using UnityEngine.UI;

public class ButtonRipple : MonoBehaviour
{
    public GameObject ripplePrefab; // Префаб круга с Image и прозрачностью
    public float rippleDuration = 0.5f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlayRipple);
    }

    void PlayRipple()
    {
        if (ripplePrefab == null) return;

        GameObject ripple = Instantiate(ripplePrefab, transform);
        ripple.transform.SetAsLastSibling();

        RectTransform rippleRect = ripple.GetComponent<RectTransform>();
        rippleRect.anchoredPosition = Vector2.zero;
        rippleRect.localScale = Vector3.zero;

        ripple.GetComponent<CanvasGroup>().alpha = 1;

        ripple.AddComponent<RippleAnimation>().StartAnimation(rippleDuration);
    }
}

public class RippleAnimation : MonoBehaviour
{
    private float duration;
    private float elapsed;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void StartAnimation(float duration)
    {
        this.duration = duration;
        elapsed = 0;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        canvasGroup.alpha = Mathf.Lerp(1, 0, t);

        if (t >= 1)
            Destroy(gameObject);
    }
}

