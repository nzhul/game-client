using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class JsonSaver
    {
        private static readonly string _fileName = "saveData1.json";

        public static string GetSaveFilename()
        {
            return Application.persistentDataPath + "/" + _fileName;
        }

        public void Save(SaveData data)
        {
            data.hashValue = string.Empty;

            string json = JsonConvert.SerializeObject(data);

            data.hashValue = GetSHA256(json);

            json = JsonConvert.SerializeObject(data);

            string saveFilename = GetSaveFilename();

            FileStream fileStream = new FileStream(saveFilename, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
        }

        public bool Load(ref SaveData data)
        {
            string loadFilename = GetSaveFilename();
            if (File.Exists(loadFilename))
            {
                using (StreamReader reader = new StreamReader(loadFilename))
                {
                    string json = reader.ReadToEnd();

                    if (CheckData(json))
                    {
                        data = JsonConvert.DeserializeObject<SaveData>(json);
                        //JsonUtility.FromJsonOverwrite(json, data);
                    }
                    else
                    {
                        Debug.LogWarning("JSONSAVER Load: invalid hash. Aborting file read...");
                    }
                }
                return true;
            }
            return false;
        }

        private bool CheckData(string json)
        {
            SaveData tempSaveData = JsonConvert.DeserializeObject<SaveData>(json);

            string oldHash = tempSaveData.hashValue;
            tempSaveData.hashValue = string.Empty;

            string tempJson = JsonConvert.SerializeObject(tempSaveData);
            string newHash = GetSHA256(tempJson);

            return (oldHash == newHash);
        }

        public void Delete()
        {
            File.Delete(GetSaveFilename());
        }

        private string GetSHA256(string text)
        {
            byte[] textToBytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed mySHA256 = new SHA256Managed();

            byte[] hashValue = mySHA256.ComputeHash(textToBytes);

            return GetHexStringFromHash(hashValue);
        }

        public string GetHexStringFromHash(byte[] hash)
        {
            string hexString = string.Empty;

            foreach (byte b in hash)
            {
                hexString += b.ToString("x2");
            }

            return hexString;
        }
    }
}
