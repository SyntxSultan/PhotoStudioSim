using DG.Tweening;
using UnityEngine;

public class ComputerConnectionUI : MonoBehaviour
{
    [SerializeField] private RectTransform connectionPanel;
    private const float animationDuration = 0.8f;

    private void Start()
    {
        connectionPanel.gameObject.SetActive(false);
    }

    public void ShowConnectionPanel()
    {
        connectionPanel.gameObject.SetActive(true);
        DOTween.Sequence().Append(
            connectionPanel.DOAnchorPosY(0f, animationDuration).SetEase(Ease.OutBack)
            );
    }

    public void HideConnectionPanel()
    {
        DOTween.Sequence().Append(
            connectionPanel.DOAnchorPosY(-500f, animationDuration).SetEase(Ease.InBack)
            ).OnComplete(() => connectionPanel.gameObject.SetActive(false));
    }
}
