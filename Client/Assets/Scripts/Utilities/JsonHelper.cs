//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.Scripts.Utilities
//{
//    // TODO:
//    // Consider using this instead of json.net - performance difference
//    // https://jacksondunstan.com/articles/3303
//    // for this to work i will have to change the API to return 
//    // Single object with one property -> Items[]
//    public class JsonHelper
//    {
//        //Usage:
//        //YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
//        public static IList<T> GetJsonArray<T>(string json)
//        {
//            string newJson = "{ \"array\": " + json + "}";
//            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
//            return wrapper.array;
//        }

//        //Usage:
//        //string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
//        public static string ArrayToJson<T>(IList<T> array)
//        {
//            Wrapper<T> wrapper = new Wrapper<T>();
//            wrapper.array = array;
//            return JsonUtility.ToJson(wrapper);
//        }

//        [System.Serializable]
//        private class Wrapper<T>
//        {
//            public IList<T> array;
//        }
//    }
//}
