﻿using Assets.Scripts.Core.Network;
using Assets.Scripts.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class UIManager : MonoBehaviour
	{
		//private RequestManager_Old requestManager;
		UserManager userManager;

		//private void Start()
		//{
		//	this.requestManager = new RequestManager_Old();
		//	this.userManager = GameObject.FindObjectOfType<UserManager>();
		//	string token = this.userManager.GetUserToken();

		//	if (!string.IsNullOrEmpty(token))
		//	{

		//		string requestUrl = RequestManager_Old.serverRootUrl + "/api/values";
		//		StartCoroutine(this.requestManager.Get(requestUrl, token, OnSuccess));
		//	}
		//	else
		//	{
		//		// Load Login Screen
		//		Debug.Log("Token not found! -> Loading Login  screen");
		//	}
		//}

		//private void OnSuccess(string response)
		//{
		//	Debug.Log(response);
		//}
	}
}
