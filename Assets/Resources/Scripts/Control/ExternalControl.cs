using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine.SocialPlatforms;

public class ExternalControl : Core.MonoSingleton<ExternalControl> {

	public List<string> productInfo = new List<string>();
	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void InitIAPManager();

	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();

	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);

	[DllImport("__Internal")]
	private static extern void InitGameCenterManager();

	[DllImport("__Internal")]
	private static extern void ReportScore(Int64 score);

	[DllImport("__Internal")]
	private static extern void InitAppirater();

	[DllImport("__Internal")]
	private static extern void ShowLeadboard();
	#endif

	void Start () {
        InitResolution();
#if UNITY_IOS
        InitIAPManager();
		InitAppirater();
		InitGameCenterManager();
#endif

		Login();
	
                  }
    public void InitResolution()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        if (ratio > 1.7f)
        {
            Camera.main.orthographicSize = 6.7f;
        }
        else
        {
            Camera.main.orthographicSize = 5.5f;
        }
    }
	public void Login()
	{
#if UNITY_ANDROID
		//Social.localUser.Authenticate((bool success) => {
			// handle success or failure
		//});
#endif
	}
	public void OnGetProduct(string s){
		productInfo.Add (s);
		//BuyProduct (s);
		//Debug.Log ("OnGetProduct " + s);
	}
	public void OnPurchaseSuccess(string s){
		//Debug.Log ("OnPurchaseSuccess " + s);
		if (onPurchaseSuccessCallback != null)onPurchaseSuccessCallback ();
	}
	public void OnPurchaseFailed(string s){
		//Debug.Log ("OnPurchaseSuccess " + s);
		if (onPurchaseFailedCallback != null)onPurchaseFailedCallback ();
	}

	private Action onPurchaseFailedCallback;
	private Action onPurchaseSuccessCallback;
	private Action onLoginSuccessCallback;
	private Action onReportScoreSuccessCallback;



	public void ReportGameScore (int score, Action callback = null)
	{
		//onReportScoreSuccessCallback = callback;
		#if UNITY_IOS
			ReportScore (score);
		#endif
		#if UNITY_ANDROID
			//Social.ReportScore(score, "CgkIi92DvMULEAIQAA", (bool success) => {
				// handle success or failure
			//});
		#endif
	}
	public void ShowGameCenterLeaderBoard()
	{
		#if UNITY_IOS
			ShowLeadboard();
		#endif
		#if UNITY_ANDROID
			//Social.ShowLeaderboardUI();
		#endif
	}

	public void PurchaseGuide(Action callback, Action failed = null)
	{
		onPurchaseSuccessCallback = callback;
		onPurchaseFailedCallback = failed;
		#if UNITY_IOS
		if (IsProductAvailable ()) {
			BuyProduct("com.abacus.guidence");
		}
		#endif
		
	}

	public void PurchaseExtraTime(Action callback, Action failed = null)
	{
		onPurchaseSuccessCallback = callback;
		onPurchaseFailedCallback = failed;
		#if UNITY_IOS
		if (IsProductAvailable ()) {
			BuyProduct("com.abacus.extraTime");
		}
		#endif

	}
	public void PurchaseEnergy(Action callback, Action failed = null)
	{
		onPurchaseSuccessCallback = callback;
		onPurchaseFailedCallback = failed;
		#if UNITY_IOS
			if (IsProductAvailable ()) {
				BuyProduct("com.abacus.energyRefill");
			}
		#endif
		#if UNITY_ANDROID
		/*
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("Purchase");*/
		#endif
	}
	
	
}
