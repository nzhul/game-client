using Assets.Scripts.Core.Network;
using Assets.Scripts.Core.Users;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
	private const string userNameKey = "Username";
	private const string passwordKey = "Password";
	private const string confirmPasswordKey = "ConfirmPassword";
	private const string emailKey = "Email";
	private RequestManager_Old requestManager;
	private const string registrationEndpoint = "/api/account/register";

	UserManager userManager;

	private IDictionary<string, InputField> inputFields;

	void Start()
	{
		this.userManager = GameObject.FindObjectOfType<UserManager>();
		this.requestManager = new RequestManager_Old();

		inputFields = new Dictionary<string, InputField>();

		ICollection<InputField> fieldElements = this.GetComponentsInChildren<InputField>();
		foreach (var field in fieldElements)
		{
			if (!inputFields.ContainsKey(field.name))
			{
				inputFields.Add(field.name, field);
			}
		}
	}

	public void RegisterBtnClicked()
	{
		string username = this.inputFields[Register.userNameKey].text;
		string password = this.inputFields[Register.passwordKey].text;
		string confirmPassword = this.inputFields[Register.confirmPasswordKey].text;
		string email = this.inputFields[Register.emailKey].text;

		IDictionary<string, string> formData = new Dictionary<string, string>();
		formData.Add(Register.userNameKey, username);
		formData.Add(Register.passwordKey, password);
		formData.Add(Register.confirmPasswordKey, confirmPassword);
		formData.Add(Register.emailKey, email);

		string requestUrl = RequestManager_Old.serverRootUrl + Register.registrationEndpoint;

		StartCoroutine(this.requestManager.Post(requestUrl, formData));
	}
}