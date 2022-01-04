using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    #region Attributes
    static Tile tile;
    static Tilemap tileMap;
    static int n, m;
    #endregion

    private void Awake()
    {
        tile = Resources.Load<Tile>("Tiles/outlined2");
        tileMap = GetComponent<Tilemap>();
    }

    static public void GenerateWorld(int n, int m)
    {
        WorldGenerator.n = n;
        WorldGenerator.m = m;
        for(int i = 0;i < n; i++)
        {
            for(int j = 0;j < m; j++)
            {
                tileMap.SetTile(new Vector3Int(j, i, 1), tile);
            }
        }
        Camera.main.transform.position = new Vector3(0, (n-1)*0.25f, -10);
        Camera.main.orthographicSize = Mathf.Ceil((n - 1) * 0.25f) + 0.5f;
    }

    static public void ShowAvailableMoves(List<Vector2Int> positions)
    {
        foreach(Vector2Int position in positions)
        {
            tileMap.SetTileFlags(new Vector3Int(position.y, position.x, 1), TileFlags.None);
            tileMap.SetColor(new Vector3Int(position.y, position.x, 1), new Color(0.75f, 0.75f, 0.75f));
        }
    }

    static public void NormalizeTiles()
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                tileMap.SetTileFlags(new Vector3Int(j, i, 1), TileFlags.None);
                tileMap.SetColor(new Vector3Int(j, i, 1), new Color(1, 1, 1));
            }
        }
    }

}
