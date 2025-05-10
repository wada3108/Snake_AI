using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile grassTile1;
    public Tile grassTile2;
    public int height = 15, width = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = -height / 2; i < (height + 1) / 2; i++)
        {
            for (int j = -width / 2; j < (width + 1) / 2; j++) {
                if ((i + j) % 2 == 0) tilemap.SetTile(new Vector3Int(i, j, 0), grassTile1);
                else tilemap.SetTile(new Vector3Int(i, j, 0), grassTile2);
            }
        }   
    }
}
