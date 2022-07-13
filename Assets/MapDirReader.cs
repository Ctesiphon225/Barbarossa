using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapDirReader : MonoBehaviour
{
    public GameObject template;
    public GameObject window;
    public string mapFolderPath;
    public List<MapEntryController> maps;
    public List<string> mapFolders;
    public EditorController editor;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (var folderPath in Directory.GetDirectories(mapFolderPath)) {
            var map = Instantiate(template, window.transform);
            map.transform.position = new Vector3(map.transform.position.x, (-35.0f * (i+1)) + map.transform.position.y, map.transform.position.z);
            var mapController = map.GetComponent<MapEntryController>();
            maps.Add(mapController);
            mapFolders.Add(folderPath);
            mapController.button.onClick.AddListener(Load);

            using (var reader = new StreamReader(folderPath + "\\description.txt")) {  
                Debug.Log(folderPath + "\\description.txt");
                string line;
                int currentLine = 0;
                for (currentLine = 0; (line = reader.ReadLine()?.Trim()) != null; ++currentLine) {
                    if (line.Length == 0) { continue; } 

                    var parts = line.Split(':');
                    if (parts.Length < 2) { continue; } 
                    Debug.Log(line);

                    if (parts[0] == "Name") {
                        mapController.mapName.text = parts[1];
                    } else if (parts[0] == "Size") {
                        var sizes = parts[1].Split('x');
                        mapController.xSize.text = sizes[0];
                        mapController.ySize.text = sizes[1];
                    } 
                }
            }

            i++;            
        }
    }

    void Load() {
        for (int i = 0; i < maps.Count; i++) {
            if (maps[i].isActive) {
                Debug.Log("Loading map: " + mapFolders[i] + "\\map.txt");
                LoadMap(mapFolders[i]);
                maps[i].isActive = false;
                gameObject.SetActive(false);
                return;
            }
        }
    }

    void LoadMap(string mapPath) {
        editor.LoadMap(File.ReadAllText(mapPath + "\\map.txt"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
