using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile grassTile1;
    [SerializeField] Tile grassTile2;

    void Start()
    {
        int height = DataManagerStatic.GetPlayAreaHeight();
        int width = DataManagerStatic.GetPlayAreaWidth();
        for (int i = -height / 2; i < (height + 1) / 2; i++)
        {
            for (int j = -width / 2; j < (width + 1) / 2; j++) {
                if ((i + j) % 2 == 0) tilemap.SetTile(new Vector3Int(i, j, 0), grassTile1);
                else tilemap.SetTile(new Vector3Int(i, j, 0), grassTile2);
            }
        }   
    }
}
