using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] SceneChange sceneChange;

    void Update()
    {
        if (!checkArea()) sceneChange.StopGame();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "body") sceneChange.StopGame();
    }

    bool checkArea()
    {
        Vector3 pos = this.transform.position;
        int height = DataManagerStatic.GetPlayAreaHeight();
        int width = DataManagerStatic.GetPlayAreaWidth();
        if (pos.x < -width / 2 || width / 2 + 1 <  pos.x) return false;
        if (pos.y < -height / 2 || height / 2 + 1 < pos.y) return false;
        return true;
    }
}
