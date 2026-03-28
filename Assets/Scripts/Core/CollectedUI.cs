using UnityEngine;
using TMPro;

public class CollectedUI : MonoBehaviour
{
    [SerializeField] TMP_Text collectedText;

    void Update()
    {
        if (GameSession.Instance != null)
        {
            // If score = collected items
            collectedText.text = $"Apples: {GameSession.Instance.GetScore()}";
        }
    }
}