using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SpaceAdventure.Asset;

public class CharacterSelector : MonoBehaviour
{
    public List<GameObject> partItems;
    public List<int> partPrices;
    public TextMeshProUGUI partName, buyButtonText;
    public Animation notEnoughAnimation;
    public Button nextButton;
    public Button previousButton;
    public GameObject coinImage;

    private int partIndex;

    void Start()
    {
        // Unlock the first character by default
        string firstKey = "PartBought-" + partItems[0].name;
        if (PlayerPrefs.GetInt(firstKey, 0) != 1)
        {
            PlayerPrefs.SetInt(firstKey, 1); // Mark first character as bought
        }

        // If no character is selected, default to the first one
        if (!PlayerPrefs.HasKey("CurrentCharacter"))
        {
            PlayerPrefs.SetInt("CurrentCharacter", 0);
        }

        partIndex = PlayerPrefs.GetInt("CurrentCharacter", 0);
        LoadPart();
    }

    public void Next()
    {
        if (partIndex < partItems.Count - 1)
        {
            partIndex++;
            LoadPart();
        }
    }

    public void Previous()
    {
        if (partIndex > 0)
        {
            partIndex--;
            LoadPart();
        }
    }

    public void Buy()
    {
        string key = "PartBought-" + partItems[partIndex].name;
        bool isBought = PlayerPrefs.GetInt(key, 0) == 1;
        int currentCharacter = PlayerPrefs.GetInt("CurrentCharacter", -1);
        int price = partPrices[partIndex];

        if (!isBought)
        {
            int balance = Wallet.GetAmount();
            if (balance >= price)
            {
                Wallet.SetAmount(balance - price);
                PlayerPrefs.SetInt(key, 1); // Mark as bought
                PlayerPrefs.SetInt("CurrentCharacter", partIndex); // Set as selected
                UpdateButtonState();
            }
            else
            {
                notEnoughAnimation.Play("Not-Enough-In");
            }
        }
        else
        {
            if (partIndex != currentCharacter)
            {
                PlayerPrefs.SetInt("CurrentCharacter", partIndex);
                UpdateButtonState(); // Now show "Selected"
            }
        }
    }

    private void LoadPart()
    {
        for (int i = 0; i < partItems.Count; i++)
        {
            partItems[i].SetActive(i == partIndex);
        }

        partName.text = partItems[partIndex].name;
        UpdateButtonState();

        // Update button interactivity
        previousButton.interactable = partIndex > 0;
        nextButton.interactable = partIndex < partItems.Count - 1;
    }

    private void UpdateButtonState()
    {
        string key = "PartBought-" + partItems[partIndex].name;
        bool isBought = PlayerPrefs.GetInt(key, 0) == 1;
        int currentCharacter = PlayerPrefs.GetInt("CurrentCharacter", -1);
        int price = partPrices[partIndex];

        if (!isBought)
        {
            buyButtonText.text = $"{price}";
            coinImage.SetActive(true); // Show coin when price is shown
        }
        else if (partIndex == currentCharacter)
        {
            buyButtonText.text = "Selected";
            coinImage.SetActive(false); // Hide coin when selected
        }
        else
        {
            buyButtonText.text = "Select";
            coinImage.SetActive(false); // Hide coin when bought
        }
    }
}
