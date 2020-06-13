using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Network.Services
{
    public class RequestManager : MonoBehaviour
    {
        public static string SERVER_ROOT = "http://localhost:5000/api/";

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
                //DontDestroyOnLoad(gameObject);
            }
        }

        public void Get(string endpoint, string[] @params, string token, OnRequestFinishedDelegate callback)
        {
            endpoint = string.Format(endpoint, @params);
            Uri uri = new Uri(SERVER_ROOT + endpoint);

            HTTPRequest request = new HTTPRequest(uri, callback);
            request.AddHeader("Authorization", "Bearer " + token);
            request.Send();
        }

        public void Get(string endpoint, string[] @params, List<KeyValuePair<string, string>> queryParams, string token, OnRequestFinishedDelegate callback)
        {
            string endpointWithQueryParams = this.AppendQueryParams(endpoint, queryParams);
            this.Get(endpointWithQueryParams, @params, token, callback);
        }

        // INFO: I know this can be done better with Uri, but .net 2.0 (mono) doesn't have HttpUtilites which helps parsing the url.
        // Maybe i can thing of a cleaver way in future.
        private string AppendQueryParams(string endpoint, List<KeyValuePair<string, string>> queryParams)
        {
            endpoint += "?";

            int iteration = 0;
            foreach (var queryParam in queryParams)
            {
                endpoint += queryParam.Key + "=" + queryParam.Value;
                if (iteration != queryParams.Count - 1)
                {
                    endpoint += "&";
                }
                iteration++;
            }

            return endpoint;
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

        public void Post<T>(string endpoint, T inputModel, string token, OnRequestFinishedDelegate callback)
        {
            Uri uri = new Uri(SERVER_ROOT + endpoint);
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, callback);
            request.AddHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", "Bearer " + token);
            }

            if (inputModel != null)
            {
                //string payload = JsonUtility.ToJson(inputModel);
                string payload = JsonConvert.SerializeObject(inputModel);
                request.RawData = Encoding.UTF8.GetBytes(payload);
            }

            request.Send();
        }

        public void Post<T>(string endpoint, T inputModel, OnRequestFinishedDelegate callback)
        {
            this.Post<T>(endpoint, inputModel, null, callback);
        }

        // TODO: make overload method for this without inputModel
        // It can be non-generic
        public void Put<T>(string endpoint, T inputModel, string token, OnRequestFinishedDelegate callback)
        {
            Uri uri = new Uri(SERVER_ROOT + endpoint);
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Put, callback);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");

            if (inputModel != null)
            {
                //string payload = JsonUtility.ToJson(inputModel);
                string payload = JsonConvert.SerializeObject(inputModel);
                request.RawData = Encoding.UTF8.GetBytes(payload);
            }
            else
            {
                request.AddHeader("Content-Length", "0");
            }

            request.Send();
        }

        public void Put(string endpoint, string[] @params, string token, OnRequestFinishedDelegate callback)
        {
            endpoint = string.Format(endpoint, @params);
            Uri uri = new Uri(SERVER_ROOT + endpoint);

            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Put, callback);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Content-Length", "0");

            request.Send();
        }

        public void Delete(string endpoint, string[] @params, string token, OnRequestFinishedDelegate callback)
        {
            endpoint = string.Format(endpoint, @params);
            Uri uri = new Uri(SERVER_ROOT + endpoint);

            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Delete, callback);
            request.AddHeader("Authorization", "Bearer " + token);
            request.Send();
        }
    }
}
