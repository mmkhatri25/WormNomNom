using UnityEngine;

public class IAPController : MonoBehaviour
{
    public Animation notEnoughAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPurchaseSuccess (int coins)
    {
        int avaialbleCoins = Wallet.GetAmount();
        Wallet.SetAmount(avaialbleCoins + coins);
        Wallet.DisplayAmount();
    }
    public void OnFailedPurchase()
    {
        //notEnoughAnimation.Play("Failed, Try again!");
        Debug.Log("failed purchase");
    }
}
