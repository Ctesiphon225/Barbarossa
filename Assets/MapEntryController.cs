using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEntryController : MonoBehaviour
{
    public Text mapName;
    public Text xSize;
    public Text ySize;
    public Button button;
    public bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(Activate);
    }

    void Activate() {
        isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
