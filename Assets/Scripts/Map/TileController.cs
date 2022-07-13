using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;



public class TileController : MonoBehaviour
{
    public GameObject highlight;
    public bool isHighlighted = false;
    public Tile tile;
    // public Text text;
    public TextMeshProUGUI text;
    public SpriteRenderer highlightRenderer;
    public SpriteRenderer tileRenderer;
    public TileType tileType;

    public GameObject unitIndicator;

    private static Dictionary<TileType, Color> tileColors = new Dictionary<TileType, Color>() {
        {TileType.Plains,       new Color(0.6f, 0.8f, 0.6f)},
        {TileType.Forest,       new Color(0.1f, 0.3f, 0.2f)},
        {TileType.Hills,        new Color(0.4f, 0.5f, 0.4f)},
        {TileType.Mountains,    new Color(0.2f, 0.2f, 0.15f)},
        {TileType.Swamp,        new Color(0.3f, 0.4f, 0.4f)},
        {TileType.Water,        new Color(0.2f, 0.2f, 0.3f)},
        {TileType.City,         new Color(0.5f, 0.5f, 0.5f)}
    };

    // Start is called before the first frame update
    void Start()
    {
        highlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (tile.unit != null) {
            unitIndicator.SetActive(true);
        } else {
            unitIndicator.SetActive(false);
        }
    }

    public void SetColor(TileType newTileType) {
        tileType = newTileType;
        tileRenderer.color = tileColors[tileType];
    }

    public void EnableHighlight(Player currentPlayer) {   
        tileRenderer.color *= 1.3f;   
        if (tile.unit != null) {  
            highlight.SetActive(true);
            // highlightRenderer.color = Color.green;
            if (tile.unit.GetComponent<UnitController>().player == currentPlayer) {
                highlightRenderer.color = Color.yellow;
            } else if (tile.unit.GetComponent<UnitController>().player != currentPlayer) {
                highlightRenderer.color = Color.red;
            }
        }
        isHighlighted = true;
    }

    public void DisableHighlight() {
        tileRenderer.color *= (1.0f / 1.3f);
        highlight.SetActive(false);
        isHighlighted = false;
    }
}
