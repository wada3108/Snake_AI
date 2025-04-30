using UnityEngine;
using TMPro;

public class ManageSettings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown areaSize;
    [SerializeField] TMP_Dropdown speed;

    void Start()
    {
        areaSize.value = DataManagerStatic.GetPlayAreaHeightValue();
        speed.value = DataManagerStatic.GetSpeedValue();

        areaSize.onValueChanged.AddListener(ChangeSize);
        speed.onValueChanged.AddListener(ChangeSpeed);

        ChangeSize(areaSize.value);
        ChangeSpeed(speed.value);
    }

    void ChangeSize(int value)
    {
        DataManagerStatic.SetPlayAreaHeight(value);
        DataManagerStatic.SetPlayAreaWidth(value);
    }

    void ChangeSpeed(int value)
    {
        DataManagerStatic.SetSpeed(value);
    }
}
