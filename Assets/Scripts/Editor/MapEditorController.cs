using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorController : MonoBehaviour
{
    public List<Button> buttons;
    
    public List<GameObject> windows;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].onClick.AddListener(delegate{OpenWindow(i);});
        }
        CloseWindows();
    }

    void OpenWindow(int index) {
        Debug.Log("open" + index);
        windows[index].SetActive(true);
    }

    void CloseWindows() {
        foreach (var window in windows) {
            window.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            CloseWindows();
        }
    }
}
