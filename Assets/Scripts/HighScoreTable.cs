using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    private List<Transform> entryTransformList;
    private int currentScore;

    void Start()
    {
        entryTemplate.gameObject.SetActive(false);
        currentScore = PlayerPrefs.GetInt("currentScore");
        
        //Add current score if not added already
        if((currentScore != 0) && (PlayerPrefs.GetInt("addedCurrent") != 1))
        {
            PlayerPrefs.SetInt("addedCurrent", 1);
            AddEntry(currentScore);
        }

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        //Sort scores
        if(highscores != null)
        {
            for(int i = 0; i < highscores.highscoreEntryList.Count; i++)
            {
                for(int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
                {
                    if(highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                    {
                        HighscoreEntry tmp = highscores.highscoreEntryList[i];
                        highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                        highscores.highscoreEntryList[j] = tmp;
                    }
                }
            }

            entryTransformList = new List<Transform>();
            int k = 0;
            foreach(HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
            {
                //Limit to 10 high scores
                if(k < 10)
                    CreateEntryTransform(highscoreEntry, entryContainer, entryTransformList);
                k++;
            }
        }
    }

    void Update()
    {
    }

    private void CreateEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(28f, -22f * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch(rank)
        {
        case 1:
            rankString = "1ST";
            break;
        case 2:
            rankString = "2ND";
            break;
        case 3:
            rankString = "3RD";
            break;
        default:
            rankString = rank + "TH"; break;
        }
        entryTransform.Find("Number").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("Score").GetComponent<Text>().text = score.ToString();
        transformList.Add(entryTransform);
    }

    private void AddEntry(int score)
    {
        HighscoreEntry entry = new HighscoreEntry { score = score };
        
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            highscores = new Highscores() { highscoreEntryList = new List<HighscoreEntry>() };
        }
        highscores.highscoreEntryList.Add(entry);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable] private class HighscoreEntry
    {
        public int score;
    }
}
