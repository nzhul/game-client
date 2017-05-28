using Assets.Scripts.Core.Network;
using Assets.Scripts.Core.Users;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	private IRequestManager requestManager;
	private IDictionary<string, InputField> inputFields;
	private const string userNameKey = "Username";
	private const string passwordKey = "Password";
	private const string tokenEndpoint = "/token";

	UserManager userManager;

	void Start () {

		this.userManager = GameObject.FindObjectOfType<UserManager>();
		this.requestManager = new RequestManager();

		this.inputFields = new Dictionary<string, InputField>();

		ICollection<InputField> fieldElements = this.GetComponentsInChildren<InputField>();
		foreach (var field in fieldElements)
		{
			if (!this.inputFields.ContainsKey(field.name))
			{
				this.inputFields.Add(field.name, field);
			}
		}
	}

	public void LoginBtnClicked()
	{
		string username = this.inputFields[Login.userNameKey].text;
		string password = this.inputFields[Login.passwordKey].text;

		IDictionary<string, string> formData = new Dictionary<string, string>();
		formData.Add(Login.userNameKey, username);
		formData.Add(Login.passwordKey, password);
		formData.Add("grant_type", "password");

		string requestUrl = RequestManager.serverRootUrl + Login.tokenEndpoint;

		StartCoroutine(this.requestManager.Post(requestUrl, formData, OnSuccessLogin));
	}

	private void OnSuccessLogin(string response)
	{
		TokenResponse tokenData = JsonUtility.FromJson<TokenResponse>(response);
		this.userManager.SaveUserToken(tokenData.access_token);
	}
}

[Serializable]
public class TokenResponse
{
	public string access_token;
	public string token_type;
	public int expires_in;
	public string userName;
	public string issued;
	public string expires;
}