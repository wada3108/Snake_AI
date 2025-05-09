using UnityEngine;
using TMPro;

public class ManageScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    void Update()
    {
        scoreText.text = "SCORE: " + DataManagerStatic.GetScore().ToString();
    }
}
