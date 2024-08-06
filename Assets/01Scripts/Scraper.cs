using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HtmlAgilityPack;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

public class Scraper : MonoBehaviour
{
    private string url = "https://en.wikipedia.org/wiki/Category:Rick_Astley_songs";
    [SerializeField] TextMeshProUGUI text;
    

    void Awake()
    {
        StartCoroutine(GetWikipediaData(url));
    }
    private void Start()
    {
        List<string> shuffledList = ShuffledList<string>(allSongs);
        foreach (var song in shuffledList) 
        {
            text.text += song + "\n";
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
                string htmlContent = webRequest.downloadHandler.text;
                ParseHtml(htmlContent);
            }
        }
    }
    List<string> allSongs = new List<string>();

    List<T> ShuffledList<T>(List<T> list)
    {
        int count = list.Count;

        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, list.Count);
            var pickedNumber = list[i];
            list[i] = list[j];
            list[j] = pickedNumber;
        }
        if (list.FindAll(x=> x == list.Contains(x))) 
        {
        
        }
        return list;
    }
    



    private void ParseHtml(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='mw-category-group']//ul/li/a");
        if (nodes == null)
        {
            Debug.Log("No data found.");
            return;
        }
        List<string> check = new List<string>(); 
        foreach (var node in nodes)
        {
            string song = node.InnerText.Trim();
            if(!check.Contains(song)) 
            {
                check.Add(song);
            }
        }
        allSongs = check;
    }
}
