using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;
    public ToolsController tools;
    public int index;
    public GameObject mapObject;

    private Color idle = new Color(0.2f, 0.2f, 0.2f);
    private Color activated = new Color(0.6f, 0.6f, 0.8f);

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = idle;
        button.onClick.AddListener(Activate);
    }

    public void Init(string newText, ToolsController newTools, int newIndex, GameObject newMapObject) {
        text.text = newText;
        tools = newTools;
        index = newIndex;
        mapObject = newMapObject;
    }

    void Activate() {
        GetComponent<Image>().color = activated;
        tools.ActivateItem(this);
    }

    public void Deactivate() {
        GetComponent<Image>().color = idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
