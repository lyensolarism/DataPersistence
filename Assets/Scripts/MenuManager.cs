using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance => instance;

    public Record BestScore => saved.bestScore;

    public string Username => saved.username;

    public void UpdateBestScore(string username, int score)
    {
        if (saved.bestScore.score >= score)
        {
            return;
        }

        saved.bestScore = new Record
        {
            score = score,
            username = username
        };
        Save();
    }

    public void OnClickStart()
    {
        saved.username = username.GetComponent<TMP_InputField>().text;
        SceneManager.LoadScene(1);
    }

    public void OnClickQuit()
    {
        saved.username = username.GetComponent<TMP_InputField>().text;
        Save();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        filename = Application.persistentDataPath + "/savedata.json";
        Load();
    }

    void Load()
    {
        if (!File.Exists(filename))
        {
            saved = new SaveData
            {
                bestScore = new Record()
            };
            return;
        }

        saved = JsonUtility.FromJson<SaveData>(File.ReadAllText(filename));
        username.GetComponent<TMP_InputField>().text = saved.username;
        if (!string.IsNullOrEmpty(saved.bestScore.username))
        {
            bestScore.GetComponent<TextMeshProUGUI>().text
                = "Best Score : " + saved.bestScore.username + " : " + saved.bestScore.score;
        }
    }

    void Save()
    {
        if (string.IsNullOrWhiteSpace(saved.username))
        {
            return;
        }

        File.WriteAllText(filename, JsonUtility.ToJson(saved));
    }

    [Serializable]
    class SaveData
    {
        public Record bestScore;

        public string username;
    }

    static MenuManager instance;
    [SerializeField]
    GameObject bestScore;
    [SerializeField]
    GameObject username;
    SaveData saved;
    string filename;
}
