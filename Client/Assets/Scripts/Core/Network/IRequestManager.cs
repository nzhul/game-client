using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Network
{
	public interface IRequestManager
	{
		IEnumerator Post(string url, IDictionary<string, string> formData, Action<string> successCallback = null, Action<int, string> failCallback = null);

		IEnumerator Get(string url, string token = null, Action<string> successCallback = null, Action<int, string> failCallback = null);
	}
}