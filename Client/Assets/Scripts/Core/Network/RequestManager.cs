using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.Core.Network
{
    public class RequestManager : MonoBehaviour
    {
        public static string SERVER_ROOT = "http://localhost:5000/";

        private static RequestManager _instance;

        public static RequestManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;

                // for this to work the object must be on root level in the hierarchy
                // TODO: this might cause bugs since i will have two/three stacks of menus, each for each scene
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Get(string endpoint, string[] @params, OnRequestFinishedDelegate callback)
        {
            endpoint = string.Format(endpoint, @params);
            Uri uri = new Uri(SERVER_ROOT + endpoint);

            HTTPRequest request = new HTTPRequest(uri, callback);
            request.Send();
        }

        public void Post(string endpoint, IDictionary<string, string> formData, OnRequestFinishedDelegate callback)
        {
            Uri uri = new Uri(SERVER_ROOT + endpoint);
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, callback);
            request.AddHeader("Content-Type", "application/json");

            foreach (var item in formData)
            {
                request.AddField(item.Key, item.Value);
            }

            request.Send();
        }

        public void Post<T>(string endpoint, T inputModel, OnRequestFinishedDelegate callback)
        {
            Uri uri = new Uri(SERVER_ROOT + endpoint);
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, callback);
            request.AddHeader("Content-Type", "application/json");

            if (inputModel != null)
            {
                string payload = JsonUtility.ToJson(inputModel);
                request.RawData = Encoding.UTF8.GetBytes(payload);
            }

            request.Send();
        }
    }
}
