using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class CFileManager
{
    private static string SaveObjectToJson<T>(T in_obj)
    {
        string res_json = string.Empty;
        try
        {
            res_json = JsonConvert.SerializeObject(in_obj, Formatting.Indented);
        }
        catch (Exception ex)
        {
            string log_text = string.Format("json can't serialize {0}", typeof(T).Name);
            Debug.LogErrorFormat("{0}: {1}", log_text, ex.Message);
        }

        return res_json;
    }

    private static T LoadObjectFromJson<T>(string inJson)
    {
        T res_obj = default(T);

        if (string.IsNullOrEmpty(inJson))
            return res_obj;

        try
        {
            res_obj = JsonConvert.DeserializeObject<T>(inJson);
        }
        catch (Exception ex)
        {
            string log_text = string.Format("json {0} can't deserialize to {1}", inJson.Substring(0, 100), typeof(T).Name);
            Debug.LogErrorFormat("{0}: {1}", log_text, ex.Message);
        }

        return res_obj;
    }

    public void JsonToFile<T>(T in_obj, string in_file_name)
    {
        string path = Application.dataPath + "/StreamingAssets/" + in_file_name + ".txt";

        using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            string json_string = SaveObjectToJson(in_obj);
            Byte[] info = new UTF8Encoding(true).GetBytes(json_string);
            fs.Write(info, 0, info.Length);
        }
    }

    public T ReadJsonFromFile<T>(string in_path)
    {
        T out_obj;
        string json_string = string.Empty;
        using (FileStream fs = File.Open(in_path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            using (var stream = new StreamReader(fs))
            {
                json_string = stream.ReadToEnd();
            }
        }
        out_obj = LoadObjectFromJson<T>(json_string);
        return out_obj;
    }

}

