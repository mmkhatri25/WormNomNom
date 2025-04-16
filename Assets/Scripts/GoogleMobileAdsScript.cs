using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class GoogleMobileAdsScript : MonoBehaviour
{

    public static GoogleMobileAdsScript Instance { get; private set; }
    public  string admob_BannerId;
    public  string admob_InterstitialId;
    public  string admob_rewardedId;
    public static BannerView admob_bannerView;
    private InterstitialAd admob_interstitial;

    private RewardedAd rewardedAd;

    [HideInInspector]
    public GameObject callBackObj;

    [HideInInspector]
    public string callBackFunction;

    public Text coinText;
    public Text keysText, keysTextSaveMe;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

        RequestBannerAd();
        RequestAndLoadInterstitialAd();
        RequestAndLoadRewardedAd();
    }
    public void RequestBannerAd()
    {
        print("Requesting Banner ad.");

        if (admob_bannerView != null)
        {
            admob_bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        admob_bannerView = new BannerView(admob_BannerId, AdSize.SmartBanner, AdPosition.Top);

        // Add Event Handlers
        admob_bannerView.OnBannerAdLoaded += () =>
        {
            print("Banner ad loaded.");
        };
        admob_bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            print("Banner ad failed to load with error: " + error.GetMessage());
        };
        admob_bannerView.OnAdFullScreenContentOpened += () =>
        {
            print("Banner ad opening.");
        };
        admob_bannerView.OnAdFullScreenContentClosed += () =>
        {
            print("Banner ad closed.");
        };
        admob_bannerView.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Banner ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);
            print(msg);
        };
    }

    public void DestroyBannerAd()
    {
        if (admob_bannerView != null)
        {
            admob_bannerView.Destroy();
        }

        RequestBannerAd();
    }
    private AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }

    public void ShowBanner()
    {
//#if !UNITY_EDITOR
        print("Banner ad showing.");
        admob_bannerView.LoadAd(CreateAdRequest());
            admob_bannerView.Show();
        
//#endif
    }

    public void HideBanner()
    {
//#if !UNITY_EDITOR
        print("Banner ad hiding.");
        
            admob_bannerView.Hide();
        //DestroyBannerAd();

//#endif
    }

    public void RequestAndLoadInterstitialAd()
    {
        print("Requesting Interstitial ad.");



        // Clean up interstitial before using it
        if (admob_interstitial != null)
        {
            admob_interstitial.Destroy();
        }

        // Load an interstitial ad
        InterstitialAd.Load(admob_InterstitialId, CreateAdRequest(),
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    print("Interstitial ad failed to load with error: " +
                        loadError.GetMessage());


                    RequestAndLoadInterstitialAd();
                    return;
                }
                else if (ad == null)
                {
                    print("Interstitial ad failed to load.");

                    RequestAndLoadInterstitialAd();
                    return;
                }

                print("Interstitial ad loaded.");
                admob_interstitial = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    print("Interstitial ad opening.");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    print("Interstitial ad closed.");
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    print("Interstitial ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    print("Interstitial ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    print("Interstitial ad failed to show with error: " +
                                error.GetMessage());

                    RequestAndLoadInterstitialAd();
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    print(msg);


                };
            });
    }






    public void showAdmobInter()
    {
        if (admob_interstitial != null /* && admob_interstitial.CanShowAd()*/)
        {
            admob_interstitial.Show();

            Invoke("RequestAndLoadInterstitialAd", 3f);
            //();
        }
        else
        {
            RequestAndLoadInterstitialAd();
        }

    }

    public bool isRewardReady()
    {
        return rewardedAd.CanShowAd();

    }

   

    public void RequestAndLoadRewardedAd()
    {
        print("Requesting Rewarded ad.");
        RewardedAd.Load(admob_rewardedId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    print("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());


                    RequestAndLoadRewardedAd();

                    return;
                }
                else if (ad == null)
                {
                    print("Rewarded ad failed to load.");
                    RequestAndLoadRewardedAd();
                    return;
                }

                print("Rewarded ad loaded.");
                rewardedAd = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    print("Rewarded ad opening.");
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    print("Rewarded ad closed.");

                    RequestAndLoadRewardedAd();
                    PlayerController.onReviveGame?.Invoke();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    print("Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    print("Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    print("Rewarded ad failed to show with error: " +
                               error.GetMessage());
                    RequestAndLoadRewardedAd();
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    print(msg);


                };
            });
    }
    public void showAdmobVideo()//For key
    {
        if (rewardedAd != null)
        {

            rewardedAd.Show((Reward reward) =>
            {
                RequestAndLoadRewardedAd();
            });
        }
        else
        {
            print("Rewarded ad Not loaded on showing... keys");

            RequestAndLoadRewardedAd();
        }
    }

   
    public void ShowRewardBasedVideoForCoin()//For coins
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
               
                RequestAndLoadRewardedAd();
                //Invoke("RequestAndLoadRewardedAd", 3f);
            });
        }
        else
        {
            print("Rewarded ad Not loaded on showing... coins");
            RequestAndLoadRewardedAd();
        }
        
    }
}