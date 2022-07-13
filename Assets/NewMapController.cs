using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewMapController : MonoBehaviour
{
    public Button button;
    public Text xSizeText;
    public Text ySizeText;
    public Text mapNameText;
    public string mapsPath;
    public EditorController editor;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(NewMap);
    }

    void NewMap() {
        var mapDir = mapsPath + "\\" + mapNameText.text;
        System.IO.Directory.CreateDirectory(mapDir);
        var xSize = int.Parse(xSizeText.text);
        var ySize = int.Parse(ySizeText.text);

        string[] lines =
        {
            "Name:" + mapNameText.text,
            "Description:This is a test map.",
            "Size:" + xSizeText.text + "x" + ySizeText.text,
            "Players:2"
        };

        File.WriteAllLines(mapDir + "\\description.txt", lines);

        var mapLines = new List<string>();
        for (int i = 0; i < ySize; i++) {
            string line = "";
            for (int j = 0; j < xSize; j++) {
                if (j < xSize - 1) {
                    line += "plains,";
                } else {
                    line += "plains";
                }
            }
            mapLines.Add(line);
        }

        mapLines.Add("Rivers");
        mapLines.Add("Railroads");
        mapLines.Add("Players");
        mapLines.Add("Players");
        mapLines.Add("Germany 1 1 1 human Axis");
        mapLines.Add("Poland 0,9 0,5 0,5 computer Allies");
        mapLines.Add("Units");
        mapLines.Add("Trenches");
        mapLines.Add("Fortresses");
        mapLines.Add("Airports");
        
        File.WriteAllLines(mapDir + "\\map.txt", mapLines);

        editor.LoadMap(File.ReadAllText(mapDir + "\\map.txt"));

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
