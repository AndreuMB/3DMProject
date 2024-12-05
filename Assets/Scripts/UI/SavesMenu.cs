using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenu : MonoBehaviour
{
    public GameObject entryPrefab;
    public GameObject content;
    public TMP_Text titleTxt;
    Options options;
    FileInfo[] info;
    void Start()
    {
        options = FindObjectOfType<Options>();
        ClosePanel();
    }

    void GenerateEntries(SavePanelOptions spo)
    {
        CleanEntries();

        // string filePath = Application.persistentDataPath + EnvReader.GetEnvVariable("SAVES_PATH");
        string filePath = Application.persistentDataPath + "/save/";
        bool exists = Directory.Exists(filePath);
        if (!exists) Directory.CreateDirectory(filePath);
        DirectoryInfo dir = new(filePath);

        info = dir.GetFiles("*.*");
        info = info.OrderBy(x => x.LastWriteTime).ToArray();

        if (spo == SavePanelOptions.SaveGame)
        {
            titleTxt.text = "SAVE GAME";
            GameObject entry = Instantiate(entryPrefab, content.transform);
            entry.transform.SetAsFirstSibling();
            entry.GetComponentInChildren<TMP_Text>().text = "+ New Save";
            entry.GetComponent<Button>().onClick.AddListener(() =>
            {
                int saveNumber = info.Where(save => !save.Name.Contains(".meta")).Count();
                const int SAVE_DELAY = 1;
                if (info.Length != 0)
                {
                    double timeBtwSaves = (DateTime.Now - info.Last().LastWriteTime).TotalSeconds;
                    if (info.Length != 0 && timeBtwSaves < SAVE_DELAY) return;
                }

                // string savefileName = "Savefile";
                string savefileName = "Savefile" + saveNumber;
                // options.SaveGame(savefileName + "_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".json");
                // GenerateEntries(SavePanelOptions.SaveGame);
                AddEntry(savefileName);
                info = dir.GetFiles("*.*");
            });
        }
        else
        {
            titleTxt.text = "LOAD GAME";
        }

        foreach (FileInfo f in info)
        {
            string[] section = f.Name.Split(".");
            if (section[section.Length - 1] == "meta") continue;
            GameObject entry = Instantiate(entryPrefab, content.transform);
            entry.GetComponentInChildren<TMP_Text>().text = f.Name;
            if (spo == SavePanelOptions.SaveGame)
            {
                entry.transform.SetSiblingIndex(1);
                entry.GetComponent<Button>().onClick.AddListener(() => OverwriteEntry(f, entry));
            }
            else
            {
                entry.transform.SetAsFirstSibling();
                // entry.GetComponent<Button>().onClick.AddListener(() => options.LoadGame(f.Name));
                entry.GetComponent<Button>().onClick.AddListener(() => DataSystem.LoadGame(f.Name));
            }

        }
    }

    void OverwriteEntry(FileInfo f, GameObject entry)
    {
        if (AddEntry(f.Name)) DeleteEntry(f, entry);
    }

    void DeleteEntry(FileInfo f, GameObject entry)
    {
        if (File.Exists(f.FullName))
        {
            File.Delete(f.FullName);
        }
        f.Delete();
        // check if file exists
        if (File.Exists(f.FullName + ".meta"))
        {
            File.Delete(f.FullName + ".meta");
        }
        Destroy(entry);

        // GameObject entry = Instantiate(entryPrefab,content.transform);
        // entry.transform.SetAsFirstSibling();
        // entry.GetComponent<Button>().onClick.AddListener(() => {
        //     string filenameFull = f.Name.Split(" ")[0] + " " + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss");
        //     if (filenameFull == f.Name) return;
        //     options.SaveGame(f.Name.Split(" ")[0] + " " + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss"));
        //     f.Delete();
        //     // GenerateEntries(SavePanelOptions.SaveGame);
        // });
    }

    bool AddEntry(string filename)
    {
        string filenameFull = filename.Split("_")[0] + "_" + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + ".json";
        if (filenameFull == filename) return false;

        FileInfo newSave = options.SaveGame(filenameFull);

        GameObject entry = Instantiate(entryPrefab, content.transform);
        entry.transform.SetSiblingIndex(1);
        entry.GetComponentInChildren<TMP_Text>().text = filenameFull;

        entry.GetComponent<Button>().onClick.AddListener(() => OverwriteEntry(newSave, entry));

        return true;
    }
    void CleanEntries()
    {
        foreach (Transform entry in content.transform)
        {
            Destroy(entry.gameObject);
        }
    }

    public void OpenPanelSave()
    {
        gameObject.SetActive(true);
        GenerateEntries(SavePanelOptions.SaveGame);
    }

    public void OpenPanelLoad()
    {
        gameObject.SetActive(true);
        GenerateEntries(SavePanelOptions.LoadGame);
    }


    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

public enum SavePanelOptions
{
    SaveGame,
    LoadGame
}

