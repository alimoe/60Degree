using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class IOSControl : Core.MonoSingleton<IOSControl> {

	public List<string> productInfo = new List<string>();

	[DllImport("__Internal")]
	private static extern void InitIAPManager();//初始化
	
	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();//判断是否可以购买
	
	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);//获取商品信息
	
	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);//购买商品

	void Start () {
#if UNITY_IPHONE
		InitIAPManager();
#endif
	}

	public void OnGetProduct(string s){
		productInfo.Add (s);
		//BuyProduct (s);
		//Debug.Log ("OnGetProduct " + s);
	}
	
	//获取商品回执
	public void OnPurchaseSuccess(string s){
		Debug.Log ("OnPurchaseSuccess " + s);
		if (onPurchaseSuccessCallback != null)onPurchaseSuccessCallback ();
	}

	public void OnLoginSuccess()
	{
		
	}

	private Action onPurchaseSuccessCallback;
	private Action onLoginSuccessCallback;
	private Action onReportScoreSuccessCallback;

	//interface to Application;
	public void Login (Action callback = null)
	{
		onLoginSuccessCallback = callback;
	}
	public void ReportScore (int score, Action callback = null)
	{
		onReportScoreSuccessCallback = callback;
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
