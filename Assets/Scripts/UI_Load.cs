using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class UI_Load : MonoBehaviour
{
    public Map_Data mpd;
    public UI_MPDEdit mpdEdit;
    public string folder = "Maps";
    public Dropdown mapDD;
    public Image selectImage;
    public Material selectMaterial;
    List<string> list = new List<string>();
    public GameObject menu;
    public GameObject editPanel;
    public UI_MsgBox msgBox;
    public GameObject pref;
    public InputField folderLoc;
    public UI_Tabs tabs;
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        editPanel.SetActive(false);
        mapDD.ClearOptions();
        if (File.Exists("Settings.txt"))
        {
            StreamReader sr = new StreamReader("Settings.txt");
            folder = sr.ReadLine();
            folderLoc.text = folder;
            sr.Close();
        }
        if (Directory.Exists(folder))
        {
            DirectoryInfo directory = new DirectoryInfo(folder);

            if (directory.GetDirectories().Length > 0)
            {
                List<string> options = new List<string>();
                foreach (DirectoryInfo di in directory.GetDirectories())
                {
                    options.Add(di.Name);
                    list.Add(di.Name);
                }
                mapDD.AddOptions(options);
                selectMap(0);

                selectImage.material = selectMaterial;
            }
            else
            {
                //msgBox.Show("There is no Mechs in the Robo Folder.");
                
            }
        }
        else
        {
            Directory.CreateDirectory(folder);
            //msgBox.Show("There is no Mechs in the Robo Folder.");
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectMap(int value)
    {
        if (File.Exists(Path.Combine(folder, list[mapDD.value], "SelectImage.png")))
        {
            
            Texture2D tex = Helper.LoadTexture(Path.Combine(folder, list[mapDD.value], "SelectImage.png"));
            Sprite st = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

            selectImage.sprite = st;
        }
    }
    public void loadMap()
    {
        menu.SetActive(true);
        editPanel.SetActive(true);
        mpd.path = Path.Combine(folder, list[mapDD.value]);
        mpd.loadmpd();
        mpdEdit.PopulatePiecesList();
        mpdEdit.PopulateWorldObjectsList(0);
        mpdEdit.PopulateScriptList();
        mpdEdit.populateModelBoxes();
        pref.SetActive(false);
        this.gameObject.SetActive(false);
        tabs.select(0);
    }

    public void setFolder(string value)
    {
        if (Directory.Exists(value))
        {
            folder = value;
            mapDD.ClearOptions();
            list.Clear();
            DirectoryInfo directory = new DirectoryInfo(folder);

            if (directory.GetDirectories().Length > 0)
            {
                List<string> options = new List<string>();
                foreach (DirectoryInfo di in directory.GetDirectories())
                {
                    options.Add(di.Name);
                    list.Add(di.Name);
                }
                mapDD.AddOptions(options);
                selectMap(0);

                selectImage.material = selectMaterial;
            }
            else
            {
                //msgBox.Show("There is no Mechs in the Robo Folder.");
                
            }

            StreamWriter sw = new StreamWriter("Settings.txt");
            sw.WriteLine(folder);
            sw.Close();
        }

    }

    public void loadNewMap()
    {
        gameObject.SetActive(true);
        menu.SetActive(false);
        editPanel.SetActive(false);
        Transform[] objects = mpd.Map.GetComponentsInChildren<Transform>();
        for (int i = 1; i < objects.Length; i++)
            GameObject.Destroy(objects[i].gameObject);
        GameObject.Destroy(mpd.HitArea);
        GameObject.Destroy(mpd.SkyMap);
    }
}
