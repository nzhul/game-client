//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace Assets.Scripts.Core.Network
//{
//	public class RequestManager_Old
//	{
//		public static string serverRootUrl = "http://localhost:5000";

//		public IEnumerator Get(string url, string token = null, Action<string> successCallback = null, Action<int, string> failCallback = null)
//		{
//			UnityWebRequest request = UnityWebRequest.Get(url);

//			if (!string.IsNullOrEmpty(token))
//			{
//				request.SetRequestHeader("Authorization", "Bearer " + token);
//			}

//			yield return request.Send();

//			if (request.isNetworkError || request.responseCode != 200)
//			{
//				if (failCallback != null)
//				{
//					failCallback((int)request.responseCode, request.downloadHandler.text);
//				}

//				Debug.Log("Failed request | Error code: " + (int)request.responseCode + " | " + request.downloadHandler.text);
//			}
//			else
//			{
//				if (successCallback != null)
//				{
//					successCallback(request.downloadHandler.text);
//				}

//				Debug.Log("Successful request: " + request.downloadHandler.text);
//			}
//		}

//		public IEnumerator Post(string url, IDictionary<string, string> formData, Action<string> successCallback = null, Action<int, string> failCallback = null)
//		{
//			WWWForm form = new WWWForm();

//			foreach (var item in formData)
//			{
//				form.AddField(item.Key, item.Value);
//			}

//			UnityWebRequest request = UnityWebRequest.Post(url, form);

//			yield return request.Send();

//			if (request.isNetworkError || request.responseCode != 200)
//			{
//				if (failCallback != null)
//				{
//					failCallback((int)request.responseCode, request.downloadHandler.text);
//				}

//				Debug.Log("Failed request | Error code: " + (int)request.responseCode + " | " + request.downloadHandler.text);
//			}
//			else
//			{
//				if (successCallback != null)
//				{
//					successCallback(request.downloadHandler.text);
//				}

//				Debug.Log("Successful request: " + request.downloadHandler.text);
//			}
//		}


//	}
//}
