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
            GameObject item = Instantiate(leaderboardItemPrefab, contentParent);
            SetLeaderboardItem(item, i + 1, entries[i].name, entries[i].score);

        }
    }

    void SetLeaderboardItem(GameObject item, int rank, string name, int score)
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
