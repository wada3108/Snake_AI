using UnityEngine;
using TMPro;

public class ManageTitle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI bestText;

    void Start()
    {
        DataManagerStatic.UpdateBest();
        scoreText.text = "Score: " + DataManagerStatic.GetScore().ToString();
        bestText.text = "Best: " + DataManagerStatic.GetBest().ToString();
        DataManagerStatic.ResetScore();
    }
}
