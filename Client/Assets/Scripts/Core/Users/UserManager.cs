using UnityEngine;

namespace Assets.Scripts.Core.Users
{
	public class UserManager : MonoBehaviour
	{
		private const string tokenKey = "token";

		public void SaveUserToken(string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				PlayerPrefs.SetString(UserManager.tokenKey, token);
				PlayerPrefs.Save();
			}
		}

		public string GetUserToken()
		{
			return PlayerPrefs.GetString(UserManager.tokenKey);
		}
	}
}
