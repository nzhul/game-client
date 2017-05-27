using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour {

	private const string userNameKey = "Username";
	private const string passwordKey = "Password";
	private const string confirmPasswordKey = "ConfirmPassword";
	private const string emailKey = "Email";

	private IDictionary<string, InputField> inputFields;

	// Use this for initialization
	void Start () {

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
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RegisterBtnClicked()
	{
		string username = this.inputFields[Register.userNameKey].text;
		string password = this.inputFields[Register.passwordKey].text;
		string confirmPassword = this.inputFields[Register.confirmPasswordKey].text;
		string email = this.inputFields[Register.emailKey].text;

		StartCoroutine(SendRegistrationRequest(username, password, confirmPassword, email));
	}

	IEnumerator SendRegistrationRequest(string username, string password, string confirmPassword, string email)
	{
		//RegistrationData registrationData = new RegistrationData
		//{
		//	Username = username,
		//	Email = email,
		//	Password = password,
		//	ConfirmPassword = confirmPassword
		//};

		//string formData = JsonUtility.ToJson(registrationData); asd

		WWWForm formData = new WWWForm();
		formData.AddField(Register.emailKey, email);
		formData.AddField(Register.passwordKey, password);
		formData.AddField(Register.confirmPasswordKey, confirmPassword);

		UnityWebRequest request = UnityWebRequest.Post("http://server.com/api/account/register", formData);
		//request.SetRequestHeader("Content-Type", "application/json");

		yield return request.Send();

		if (request.isError)
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("Registration successfull!");
		}

	}

	//[Serializable]
	//private class RegistrationData
	//{
	//	//Note: Unity do not serialize properties!
	//	public string Email;
	//	public string Password;
	//	public string ConfirmPassword;
	//	public string Username;
	//}
}
