using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HtmlAgilityPack;
using UnityEngine.Networking;
using TMPro;
using System.Linq;
using System.Web;

public class Scraper : MonoBehaviour
{
    [SerializeField] GameObject scrollViewContent;
    [SerializeField] GameObject middleContent;
    [SerializeField] GameObject songTextPrefab;
    [SerializeField] GameObject selectedSongSpace;
    [SerializeField] GameObject pauseUI;
    private string url = "https://en.wikipedia.org/wiki/Category:Rick_Astley_songs";
    List<string> allSongsList = new List<string>();
    List<string> shuffledList;
    bool isDataReady;
    bool isBindingComplete;

    void Awake()
    {
        pauseUI.SetActive(false);
        StartCoroutine(GetWikipediaData(url));
    }
    private void Start()
    {
        StartCoroutine(MakeASongsList());
    }
    bool isShuffleStart;
    float shuffleTime = 0;
    bool isPause;
    private void Update()
    {
        if (isShuffleStart)
        {
            Debug.Log(shuffleTime);
            shuffleTime += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PauseFunction();;
        }
        if (isPause && Input.GetKeyDown(KeyCode.Return))
        {
            Application.Quit();
        }
    }
    void PauseFunction() 
    {
        if (!isPause)
        {
            ShowMenu();
            Time.timeScale = 0;
            isPause = true;

        }
        else if (isPause) 
        {
            HideMenu();
            Time.timeScale = 1;
            isPause = false;
        }
        
        void ShowMenu()
        {
            pauseUI.SetActive(true);
        }
        void HideMenu() 
        {
            pauseUI.SetActive(false);
        }
    }


    public void ShuffleStart() 
    {
        if (isShuffleStart) return;
        StartCoroutine(ShuffleEffect());
    }

    IEnumerator ShuffleEffect() 
    {
        string selectedSong = "";
        isShuffleStart = true;
        Debug.Log("Shuffle Start");
        while (shuffleTime <= 5f) 
        {
            MakeShuffledList();
            yield return new WaitForSeconds(0.1f);
        }

        while (shuffleTime <= 10f) 
        {
            SelectSong(selectedSong);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForEndOfFrame();

        shuffleTime = 0;
        isShuffleStart = false;
    }

    private IEnumerator MakeASongsList()
    {
        Debug.Log("Waiting...");
        yield return new WaitUntil(() => isDataReady);
        Debug.Log("Start Make A Songs List");
        if (isDataReady && !isBindingComplete)
        {
            foreach (var gettingSong in allSongsList)
            {
                var song = Instantiate(songTextPrefab, scrollViewContent.transform);
                song.name = gettingSong;
                var component = song.GetComponent<TextMeshProUGUI>();
                component.text = gettingSong;
            }
            isBindingComplete = true;
        }
        StopCoroutine(MakeASongsList());
        Debug.Log("Create Finished");
    }
    void SelectSong(string selectedSong)
    {
        int random = Random.Range(0, 10);
        DestroyChild(selectedSongSpace);
        selectedSong = shuffledList[random];

        var song = Instantiate(songTextPrefab, selectedSongSpace.transform);
        song.name = selectedSong;
        var component = song.GetComponent<TextMeshProUGUI>();
        component.text = selectedSong;
    }

    public void MakeShuffledList() 
    {
        DestroyChild(middleContent);
        Shuffle(shuffledList);
        for (int i = 0; i < 10; i++)
        {
            var song = Instantiate(songTextPrefab, middleContent.transform);
            song.name = shuffledList[i];
            var component = song.GetComponent<TextMeshProUGUI>();
            component.text = shuffledList[i];
        }
    }
    void DestroyChild(GameObject parent)
    {
        if (parent.transform.childCount > 0)
        {
            int childCount = parent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
    }
    void Shuffle<T>(List<T> list)
    {
        int count = list.Count;

        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, list.Count);
            var pickedNumber = list[i];
            list[i] = list[j];
            list[j] = pickedNumber;
        }
    }

    private IEnumerator GetWikipediaData(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Connected");
                string htmlContent = webRequest.downloadHandler.text;
                ParseHtml(htmlContent);
            }
        }
    }


    private void ParseHtml(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='mw-category-group']//ul/li/a | //div[@class='mw-category-group']//ul/li/span/a");
        if (nodes == null)
        {
            Debug.Log("No data found.");
            return;
        }

        HashSet<string> tempSongSet = new HashSet<string>();
        foreach (var node in nodes)
        {
            string song = HttpUtility.HtmlDecode(node.InnerText.Trim());
            if (song.Contains("song"))
            {
                string remover = "(song)";
                if (song.Contains("(Rick Astley song)")) 
                {
                    remover = "(Rick Astley song)";
                }
                string replacedSong = song.Replace(remover, string.Empty);
                tempSongSet.Add(replacedSong);
            }
            else 
            {
                tempSongSet.Add(song);
            }
        }

        allSongsList = tempSongSet.ToList();
        shuffledList = allSongsList;
        isDataReady = true;
        Debug.Log("Complete Data Binding");
    }
}
