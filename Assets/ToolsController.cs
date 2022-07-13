using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolsController : MonoBehaviour
{
    public TextMeshProUGUI label;

    public int displayed = 0;
    public List<GameObject> tiles;
    public List<GameObject> features;
    public List<GameObject> units;

    public List<GameObject> tileItems;
    public List<GameObject> featureItems;
    public List<GameObject> unitItems;

    public ItemController selectedItem;
    public GameObject selectedGameObject;

    public GameObject content;
    public GameObject item;

    public string prevLabel = "";

    // Start is called before the first frame update
    void Start()
    {
        tileItems = new List<GameObject>();
        featureItems = new List<GameObject>();
        unitItems = new List<GameObject>();

        int index = 0;
        foreach (var tile in tiles) {
            var tileItem = Instantiate(item, content.transform);
            tileItem.transform.position = new Vector3(tileItem.transform.position.x, tileItem.transform.position.y + (index * 50.0f), tileItem.transform.position.z);
            tileItems.Add(tileItem);
            tileItem.GetComponent<ItemController>().Init("something", this, index, tile);
            index++;
        }
    }

    public void ActivateSelection() {
        if (selectedItem != null) {
            selectedItem.Deactivate();
        }

        if (label.text == "Tiles") {
            foreach (var tileItem in tileItems) {
                tileItem.SetActive(true);
            }
        } else if (label.text == "Features") {
            foreach (var featureItem in featureItems) {
                featureItem.SetActive(true);
            }
        } else if (label.text == "Units") {
            foreach (var unitItem in unitItems) {
                unitItem.SetActive(true);
            }
        }
    }

    public void ActivateItem(ItemController item) {
        if (selectedItem != null) {
            selectedItem.Deactivate();
        }
        selectedItem = item;
        selectedGameObject = item.mapObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (label.text != prevLabel) {
            prevLabel = label.text;
            ActivateSelection();
        }
    }
}
