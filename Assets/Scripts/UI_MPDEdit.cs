using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RuntimeHandle;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_MPDEdit : MonoBehaviour
{
    public Map_Data mpd;
    [Header("Transform Data")]
    public InputField PosX;
    public InputField PosY;
    public InputField PosZ;
    public InputField RotX;
    public InputField RotY;
    public InputField RotZ;
    public InputField ScaleX;
    public InputField ScaleY;
    public InputField ScaleZ;
    public EventSystem es;

    [Header("Pieces Select")]
    public int index = 0;
    List<string> list = new List<string>();
    List<GameObject> items = new List<GameObject>();
    List<bool> selected = new List<bool>();
    public GameObject Template;
    public RectTransform content;
    public Color selectedColor;
    public Color deselectedColor;
    public List<string> modelFiles = new List<string>();
    public Dropdown vMesh;
    public Dropdown cMesh;
    public InputField scriptField;

    [Header("World Objects Select")]
    public int index2 = 0;
    List<string> list2 = new List<string>();
    List<GameObject> items2 = new List<GameObject>();
    List<bool> selected2 = new List<bool>();
    public GameObject Template2;
    public RectTransform content2;
    public InputField scriptID;
    public Dropdown ModelList;
    public Dropdown filterList;
    public InputField filterText;
    public Transform[] Objects;
    public RuntimeTransformHandle handle3D;
    public GameObject Multi;
    public bool allowChange = true;

    [Header("Script Select")]
    public int index3 = 0;
    List<string> list3 = new List<string>();
    List<GameObject> items3 = new List<GameObject>();
    List<bool> selected3 = new List<bool>();
    public GameObject Template3;
    public RectTransform content3;
    public InputField scriptField2;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Objects != null && index2 < Objects.Length && Objects[index2] != null && !es.IsPointerOverGameObject())
        {
            handle3D.transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, Objects[index2].position) * 0.025f;
            TransformTextUpdate();
        }
    }

    public void save()
    {
        MultiDeselect();
        mpd.save();
    }
    #region Pieces
    public void PopulatePiecesList()
    {
        clear();
        for (int i = 0; i < mpd.Pieces.Length; i++)
        {
            if (mpd.Pieces[i].visualMesh != null)
                addItem((i + 1).ToString() + ". " + mpd.Pieces[i].visualMesh.name);
            else
                addItem((i + 1).ToString() + ". Empty");
        }

        ModelList.ClearOptions();
        ModelList.AddOptions(list);
    }

    public void SelectedIndexChanged(int _index)
    {
        //handle.target = robo.parts[_index].transform;
        if (index == _index)
            return;

       
        index = _index;
        for (int i = 0; i < list.Count; i++)
        {
            if (_index == i)
                selected[i] = true;
            else
                selected[i] = false;
        }
        

        for (int i = 0; i < list.Count; i++)
        {
            ColorBlock cb = items[i].GetComponent<Button>().colors;
            if (selected[i])
                cb.normalColor = selectedColor;
            else
                cb.normalColor = deselectedColor;
            items[i].GetComponent<Button>().colors = cb;
        }
        updatePieceData();
        if (filterList.value == 2)
            PopulateWorldObjectsList(2);
    }

    public void clear()
    {
        foreach (var item in items)
            GameObject.Destroy(item);

        list.Clear();
        items.Clear();
    }

    public void addItem(string item)
    {
        list.Add(item);
        GameObject GO = GameObject.Instantiate(Template, Template.transform.parent);
        items.Add(GO);
        selected.Add(false);
        Button b = GO.GetComponent<Button>();
        int lPos = list.Count - 1;
        b.onClick.AddListener(() => { SelectedIndexChanged(lPos); });
        
        Text t = GO.transform.GetChild(0).GetComponent<Text>();
        t.text = item;
        GO.SetActive(true); 
        Vector2 size = content.sizeDelta;
        size.y = list.Count * 24 + 10;
        content.sizeDelta = size;
    }

    public void populateModelBoxes()
    {
        DirectoryInfo di = new DirectoryInfo(mpd.path);
        FileInfo[] files = di.GetFiles();
        modelFiles = new List<string>();
        modelFiles.Add("Empty");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension == ".x")
                modelFiles.Add(files[i].Name);
        }
        vMesh.ClearOptions();
        vMesh.AddOptions(modelFiles);
        cMesh.ClearOptions();
        cMesh.AddOptions(modelFiles);
    }
    void updatePieceData()
    {
        allowChange = false;
        if (mpd.Pieces[index].visualMesh != null && modelFiles != null)
        {
            int l = 0;
            for (; l < modelFiles.Count; l++)
            {
                if (mpd.Pieces[index].visualMesh.name == modelFiles[l])
                    break;
            }

            cMesh.value = l;

            if (mpd.Pieces[index].colliderMesh != null)
            {
                l = 0;
                for (; l < modelFiles.Count; l++)
                {
                    if (mpd.Pieces[index].visualMesh.name == modelFiles[l])
                        break;
                }
                vMesh.value = l;
            }
            else
                vMesh.value = 0;
        }
        else
        {
            vMesh.value = 0;
            cMesh.value = 0;
            scriptField.text = "";
        }
        allowChange = true;
    }

    public void updatePieceMapData()
    {
        if (allowChange)
        { 
            mpd.updatePiece(index, modelFiles[vMesh.value], modelFiles[cMesh.value], scriptField.text);
            PopulatePiecesList();
            Transform[] scannedObjects = mpd.Map.GetComponentsInChildren<Transform>();
            for (int i = 1; i < scannedObjects.Length; i++)
            {
                ObjectData od = scannedObjects[i].GetComponent<ObjectData>();
                if (od != null && od.ModelID == index)
                {
                    if (modelFiles[vMesh.value] == "Empty")
                        GameObject.Destroy(scannedObjects[i].gameObject);
                    else
                        od.build();
                }
            }
        }
    }

    //public void EmptyPieceData()
    //{

    //    mpd.Pieces[index] = new PieceData();
    //    PopulatePiecesList();
    //    Transform[] scannedObjects = mpd.Map.GetComponentsInChildren<Transform>();
    //    for (int i = 1; i < scannedObjects.Length; i++)
    //    {
    //        ObjectData od = scannedObjects[i].GetComponent<ObjectData>();
    //        if (od != null && od.ModelID == index)
    //        {
    //            GameObject.Destroy(scannedObjects[i].gameObject);
    //        }
    //    }

    //}

    
    #endregion
    #region Sub Pieces
    public void PopulateWorldObjectsList(int Type)
    {
        clear2();
        Objects = mpd.Map.GetComponentsInChildren<Transform>();
        List<Transform> list = new List<Transform>();
        for (int i = 1; i < Objects.Length; i++)
        {
            bool validConditions = true;
            switch (Type)
            {
                case 1:
                    Vector3 v = Camera.main.WorldToViewportPoint(Objects[i].position);
                    validConditions = v.x < 1 && v.x > 0 && v.y < 1 && v.y > 0 && v.z > 0;
                    break;
                case 2:
                    validConditions = Objects[i].GetComponent<ObjectData>().ModelID == index && Objects[i].gameObject.name.Contains(filterText.text);    
                    break;
            }
            if (validConditions)
            {
                if (filterText.text == "" || filterText.text == null)
                    list.Add(Objects[i]);
                else if (Objects[i].gameObject.name.Contains(filterText.text))
                    list.Add(Objects[i]);
            }
        }
        Objects = list.ToArray();
        for (int i = 0; i < Objects.Length; i++)
           addItem2(Objects[i].gameObject.name);
        
    }

    public void refreshWorldObjectList()
    {
        PopulateWorldObjectsList(filterList.value);
    }
    public void SelectedIndexChanged2(int _index)
    {
        MultiDeselect();
        //handle.target = robo.parts[_index].transform;
        if (index2 == _index)
            return;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            index2 = _index;
            selected2[_index] = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            int[] range = new int[2];
            if (_index > index2)
            {
                range[0] = index2;
                range[1] = _index;
            }
            else
            {
                range[0] = _index;
                range[1] = index2;
            }


            for (int i = 0; i < list2.Count; i++)
            {
                if (i >= range[0] && i <= range[1])
                    selected2[i] = true;
                else
                    selected2[i] = false;
            }

            index2 = _index;
        }
        else
        {
            index2 = _index;
            for (int i = 0; i < list2.Count; i++)
            {
                if (_index == i)
                    selected2[i] = true;
                else
                    selected2[i] = false;
            }
        }


        for (int i = 0; i < list2.Count; i++)
        {
            ColorBlock cb = items2[i].GetComponent<Button>().colors;
            if (selected2[i])
                cb.normalColor = selectedColor;
            else
                cb.normalColor = deselectedColor;
            items2[i].GetComponent<Button>().colors = cb;
        }
        allowChange = false;
        updateWorldObjectData();
        TransformTextUpdate();
        MultiSelect();
        allowChange = true;
    }

    public void MultiSelect()
    {
        if (Multi == null)
            Multi = new GameObject("Multi Select");

        handle3D.target = Multi.transform;
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < Objects.Length; i++)
        {
            if (selected2[i] == true && Objects != null && index2 < Objects.Length && Objects[i] != null)
            {
               transforms.Add(Objects[i]);
            }
        }
        if (transforms.Count > 1)
        {
            Vector3 total = new Vector3();
            foreach (Transform t2 in transforms)
            {
                total += t2.position;
            }
            total = total / transforms.Count;
            Multi.transform.position = total;
            foreach (Transform t in transforms)
            {
                t.SetParent(Multi.transform, true);
            }
        }
        else
            handle3D.target = transforms[0];

    }

    public void MultiDeselect()
    {
        if (Multi != null)
        {
            Transform[] transforms = Multi.GetComponentsInChildren<Transform>();
            for (int i = 1; i < transforms.Length; i++)
            {
                transforms[i].SetParent(mpd.Map.transform, true);
            }
        }
    }

    public void clear2()
    {
        foreach (var item in items2)
            GameObject.Destroy(item);

        list2.Clear();
        items2.Clear();
    }

    public void addItem2(string item)
    {
        list2.Add(item);
        GameObject GO = GameObject.Instantiate(Template2, Template2.transform.parent);
        items2.Add(GO);
        selected2.Add(false);
        Button b = GO.GetComponent<Button>();
        int lPos = list2.Count - 1;
        b.onClick.AddListener(() => { SelectedIndexChanged2(lPos); });

        Text t = GO.transform.GetChild(0).GetComponent<Text>();
        t.text = item;
        GO.SetActive(true);

        Vector2 size = content2.sizeDelta;
        size.y = list2.Count * 24 + 10;
        content2.sizeDelta = size;
    }

    public void updateWorldObjectData()
    {
        if (Objects != null && index2 < Objects.Length && Objects[index2] != null)
        {
            ObjectData od = Objects[index2].GetComponent<ObjectData>();
            scriptID.text = (od.scriptID + 1).ToString();
            ModelList.value = od.ModelID;
            handle3D.target = Objects[index2];

        }
        
    }

    public void PositionCursor()
    {
        handle3D.type = HandleType.POSITION;

    }

    public void RotationCursor()
    {
        handle3D.type = HandleType.ROTATION;
    }

    public void spaceChange(int s)
    {
        if (s == 0)
            handle3D.space = HandleSpace.WORLD;
        else
            handle3D.space = HandleSpace.LOCAL;

        TransformTextUpdate();
    }

    public void TransformTextUpdate()
    {
        if (Objects != null && index2 < Objects.Length && Objects[index2] != null)
        {
            if (handle3D.space == HandleSpace.LOCAL)
            {
                PosX.text = Objects[index2].localPosition.x.ToString();
                PosY.text = Objects[index2].localPosition.y.ToString();
                PosZ.text = Objects[index2].localPosition.z.ToString();
                RotX.text = Objects[index2].localRotation.eulerAngles.x.ToString();
                RotY.text = Objects[index2].localRotation.eulerAngles.y.ToString();
                RotZ.text = Objects[index2].localRotation.eulerAngles.z.ToString();
            }
            else
            {
                PosX.text = Objects[index2].position.x.ToString();
                PosY.text = Objects[index2].position.y.ToString();
                PosZ.text = Objects[index2].position.z.ToString();
                RotX.text = Objects[index2].rotation.eulerAngles.x.ToString();
                RotY.text = Objects[index2].rotation.eulerAngles.y.ToString();
                RotZ.text = Objects[index2].rotation.eulerAngles.z.ToString();
            }

            ScaleX.text = Objects[index2].localScale.x.ToString();
            ScaleY.text = Objects[index2].localScale.y.ToString();
            ScaleZ.text = Objects[index2].localScale.z.ToString();
        }
    }

    public void TransformDataUpdate()
    {
        Vector3 position = new Vector3();
        if (float.TryParse(PosX.text, out position.x) &&
            float.TryParse(PosY.text, out position.y) &&
            float.TryParse(PosZ.text, out position.z))
        {
            if (handle3D.space == HandleSpace.LOCAL)
                Objects[index2].localPosition = position;
            else
                Objects[index2].position = position;
        }


        Vector3 euler = new Vector3();
        if (float.TryParse(RotX.text, out euler.x) &&
            float.TryParse(RotY.text, out euler.y) &&
            float.TryParse(RotZ.text, out euler.z))
        {
            if (handle3D.space == HandleSpace.LOCAL)
                Objects[index2].localRotation = Quaternion.Euler(euler);
            else
                Objects[index2].rotation = Quaternion.Euler(euler);
        }

        Vector3 scale = new Vector3();
        if (float.TryParse(ScaleX.text, out scale.x) &&
            float.TryParse(ScaleY.text, out scale.y) &&
            float.TryParse(ScaleZ.text, out scale.z))
            Objects[index2].localScale = scale;
    }

    public void updateObjectScript()
    {
        if (allowChange)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (selected2[i] == true && Objects != null && index2 < Objects.Length && Objects[i] != null)
                {
                    ObjectData od = Objects[i].GetComponent<ObjectData>();
                    int p;
                    if (int.TryParse(scriptID.text, out p))
                        od.scriptID = p - 1;
                }
            }
        }
    }

    public void updateObjectModel()
    {
        if (allowChange)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (selected2[i] == true && Objects != null && index2 < Objects.Length && Objects[i] != null)
                {
                    ObjectData od = Objects[i].GetComponent<ObjectData>();
                    if (mpd.Pieces[ModelList.value].visualMesh != null)
                    {
                        od.ModelID = ModelList.value;
                        od.build();
                    }
                }
            }
        }
    }

    public void addObject()
    {
        int ModelID = 0;
        if (mpd.Pieces[index].visualMesh != null)
            ModelID = index;
        else
        {
            for (int i = 0; i < mpd.Pieces.Length; i++)
            {

                if (mpd.Pieces[i].visualMesh != null)
                {

                    ModelID = i;
                    break;
                }
            }
        }
        GameObject go = new GameObject(mpd.Pieces[ModelID].visualMesh.name);
        ObjectData od = go.AddComponent<ObjectData>();
        od.ModelID = ModelID;
        od.scriptID = 0;
        od.data = mpd;
        go.transform.SetParent(mpd.Map.transform);
        go.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 10);
        od.build();
        PopulateWorldObjectsList(filterList.value);
    }

    public void removeObject()
    {

        for (int i = 0; i < Objects.Length; i++)
        {
            if (selected2[i] == true && Objects != null && index2 < Objects.Length && Objects[i] != null)
            {
                GameObject.Destroy(Objects[i].gameObject);
            }
        }
        PopulateWorldObjectsList(filterList.value);
        handle3D.target = mpd.Map.transform;
    }
    #endregion
    #region Scripts
    public void PopulateScriptList()
    {
        clear3();
        if (mpd.scripts.Count > 0)
        {
           for (int i = 0; i < mpd.scripts.Count; i++)
                addItem3((i + 1).ToString());
        }
    }
    public void SelectedIndexChanged3(int _index)
    {
        //handle.target = robo.parts[_index].transform;
        if (index3 == _index)
            return;


        index3 = _index;
        for (int i = 0; i < list3.Count; i++)
        {
            if (_index == i)
                selected3[i] = true;
            else
                selected3[i] = false;
        }


        for (int i = 0; i < list3.Count; i++)
        {
            ColorBlock cb = items3[i].GetComponent<Button>().colors;
            if (selected3[i])
                cb.normalColor = selectedColor;
            else
                cb.normalColor = deselectedColor;
            items3[i].GetComponent<Button>().colors = cb;
        }
        updateScriptData();
    }

    public void clear3()
    {
        foreach (var item in items3)
            GameObject.Destroy(item);

        list3.Clear();
        items3.Clear();
    }

    public void addItem3(string item)
    {
        list3.Add(item);
        GameObject GO = GameObject.Instantiate(Template3, Template3.transform.parent);
        items3.Add(GO);
        selected3.Add(false);
        Button b = GO.GetComponent<Button>();
        int lPos = list3.Count - 1;
        b.onClick.AddListener(() => { SelectedIndexChanged3(lPos); });

        Text t = GO.transform.GetChild(0).GetComponent<Text>();
        t.text = item;
        GO.SetActive(true);

        Vector2 size = content3.sizeDelta;
        size.y = list3.Count * 24 + 10;
        content3.sizeDelta = size;
    }

    public void updateScriptData()
    {
        scriptField2.text = mpd.scripts[index3];

    }

    public void setMapScriptData()
    {
        mpd.scripts[index3] = scriptField2.text;
    }
    #endregion
}
