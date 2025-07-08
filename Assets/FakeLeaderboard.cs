using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FakeLeaderboard : MonoBehaviour
{
    [Header("Prefab and UI")]
    public GameObject leaderboardItemPrefab; // Assign your item prefab here
    public Transform contentParent;          // Assign your UI container (e.g., ScrollView content)

    [Header("Name Pool (International + Indian)")]
    public string[] allNames = new string[]
    {
        // International names
        "Joy", "David", "Alice", "Chris", "Lily",
        "Max", "Sophia", "Ethan", "Emma", "Daniel",
        // Indian names
        "Aarav", "Vivaan", "Aditya", "Krishna", "Arjun",
        "Diya", "Ishita", "Anaya", "Meera", "Priya"
    };

    void Start()
    {
        GenerateLeaderboard();
    }

    void GenerateLeaderboard()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        List<string> namesPool = new List<string>(allNames);

        // Add 9 random names
        for (int i = 0; i < 9; i++)
        {
            int index = Random.Range(0, namesPool.Count);
            string randomName = namesPool[index];
            namesPool.RemoveAt(index); // Avoid duplicates

            int randomScore = Random.Range(1000, 10000);
            entries.Add(new LeaderboardEntry(randomName, randomScore));
        }

        // Add "You" with your real score
        int yourScore = PlayerPrefs.GetInt("topscore", 0);
        entries.Add(new LeaderboardEntry("You", yourScore));

        // Sort descending by score
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Create leaderboard UI
        for (int i = 0; i < entries.Count; i++)
        {
            bool isYou = entries[i].name == "You";
            GameObject item = Instantiate(leaderboardItemPrefab, contentParent);
            SetLeaderboardItem(item, i + 1, entries[i].name, entries[i].score, isYou);

        }
    }

    //void SetLeaderboardItem(GameObject item, int rank, string name, int score)
    //{
    //    TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
    //    foreach (TextMeshProUGUI t in texts)
    //    {
    //        string lowerName = t.name.ToLower();
    //        if (lowerName.Contains("rank"))
    //            t.text = rank.ToString();
    //        else if (lowerName.Contains("name"))
    //            t.text = name;
    //        else if (lowerName.Contains("score"))
    //            t.text = score.ToString();
    //    }
    //}

    void SetLeaderboardItem(GameObject item, int rank, string name, int score, bool isYou = false)
    {
        TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI t in texts)
        {
            string lowerName = t.name.ToLower();
            if (lowerName.Contains("rank"))
                t.text = rank.ToString();
            else if (lowerName.Contains("name"))
                t.text = name;
            else if (lowerName.Contains("score"))
                t.text = score.ToString();

            if (isYou)
            {
                // Change font color or style
                t.color = Color.yellow; // Or any eye-catching color
                t.fontStyle = FontStyles.Bold;
            }
        }

        if (isYou)
        {
            // Optionally change background color
            Image bg = item.GetComponent<Image>();
            if (bg != null)
                bg.color = new Color(1f, 1f, 0.5f); // Light yellow background
        }
    }

    class LeaderboardEntry
    {
        public string name;
        public int score;

        public LeaderboardEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}
