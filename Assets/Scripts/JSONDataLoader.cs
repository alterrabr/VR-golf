using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

/// <summary>
/// This version of DataLoader works with JSON files
/// </summary>
public class JSONDataLoader : IDataLoader
{
    /// <summary>
    /// The name of file with needed data
    /// </summary>
    private const string scoreFileName =  "PlayerScoreData.json";

    /// <summary>
    /// path relative Application.persistentDataPath
    /// </summary>
    private const string savesFolderPath = "Saves";

    /// <summary>
    /// Cached value of Application.persistentDataPath. We need to cache it because
    /// Application.persistentDataPath can only be called from main thread
    /// </summary>
    private readonly string AppPersistentDataPath = Application.persistentDataPath;

    /// <summary>
    /// Load score data from JSON file. It return true if loading completed correctly and return false else.
    /// In addition it return message with operation result description.
    /// </summary>
    /// <param name="loadedData">list of loaded data models</param>
    /// <param name="resultMessage">message with result of operation (success or error description)</param>
    /// <returns></returns>
    public List<PlayerDataModel> LoadScoreData(out ScoreLoadingError errorCode, out string resultMessage)
    {
        errorCode = ScoreLoadingError.Ok;
        string path = Path.Combine(AppPersistentDataPath, savesFolderPath);
        DirectoryInfo dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
        {
            errorCode = ScoreLoadingError.FileOrDirectoryNotFound;
            resultMessage = "Directory " + path + " not found";
            return null;
        }
        path = Path.Combine(path, scoreFileName);

        if(!File.Exists(path))
        {
            errorCode = ScoreLoadingError.FileOrDirectoryNotFound;
            resultMessage = "File " + path + " not found";
            return null;
        }
        using (StreamReader file = File.OpenText(path))
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                resultMessage = "Score data loaded correctly";
                return (List<PlayerDataModel>)serializer.Deserialize(file, typeof(List<PlayerDataModel>));
            }
            catch(JsonSerializationException ex)
            {
                errorCode = ScoreLoadingError.UnknownError;
                resultMessage = "JSON Data loader: " + ex.Message;
                return null;
            }
        }
    }

    /// <summary>
    /// Save data into the JSON file.
    /// This method will create folder and file if it not found they.
    /// It will rewrite file if it exist.
    /// </summary>
    /// <param name="scoreDataPack">data for saving</param>
    public void SaveScoreData(List<PlayerDataModel> scoreDataPack)
    {
        string path = Path.Combine(Application.persistentDataPath, savesFolderPath);
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
            Debug.Log("Directory " + path + " was created");
        }
        path = Path.Combine(path, scoreFileName);

        if (!File.Exists(path))
        {
            using (FileStream stream = File.Create(path))
            {
                Debug.Log("File " + path + " was created");
            }
        }

        using (StreamWriter writer = File.CreateText(path))
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, scoreDataPack);
                Debug.Log("Saving completed correctly");
            }
            catch (JsonSerializationException ex)
            {
                Debug.LogError("JSON Data loader: " + ex.Message);
            }
        }
    }
}