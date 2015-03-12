﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class IOSControl : Core.MonoSingleton<IOSControl> {

	public List<string> productInfo = new List<string>();

	[DllImport("__Internal")]
	private static extern void InitIAPManager();
	
	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();
	
	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);
	
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

	void Start () {
        InitResolution();
#if UNITY_IPHONE
        
		InitIAPManager();
		InitAppirater();
		InitGameCenterManager();
#endif
                  }
    public void InitResolution()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        if (ratio > 1.7f)
        {
            Camera.main.orthographicSize = 7;
        }
        else
        {
            Camera.main.orthographicSize = 6;
        }
    }
	public void OnGetProduct(string s){
		productInfo.Add (s);
		//BuyProduct (s);
		//Debug.Log ("OnGetProduct " + s);
	}
	
	//获取商品回执
	public void OnPurchaseSuccess(string s){
		//Debug.Log ("OnPurchaseSuccess " + s);
		if (onPurchaseSuccessCallback != null)onPurchaseSuccessCallback ();
	}



	private Action onPurchaseSuccessCallback;
	private Action onLoginSuccessCallback;
	private Action onReportScoreSuccessCallback;


	public void ReportGameScore (Int64 score, Action callback = null)
	{
		//onReportScoreSuccessCallback = callback;
		#if UNITY_IPHONE
			ReportScore (score);
		#endif
	}
	public void ShowGameCenterLeaderBoard()
	{
		#if UNITY_IPHONE
			ShowLeadboard();
		#endif
	}

	public void Purchase(Action callback)
	{
		onPurchaseSuccessCallback = callback;
		#if UNITY_IPHONE
		if (IsProductAvailable ()) {
			BuyProduct("com.abacus.energyRefill");
		}
		#endif
	}


}
