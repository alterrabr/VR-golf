using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System;
using System.Threading.Tasks;

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

    public async void LoadScoreDataAsync(EventHandler<OnScoreDataLoadedEventArgs> callback)
    {
        var errorCode = ScoreLoadingStatus.Ok;
        string errorMessage = "";

        List<PlayerDataModel> data = await Task.Run(DoLoadingOperation);

        var args = new OnScoreDataLoadedEventArgs()
        {
            status = errorCode,
            errorMessage = errorMessage,
            data = data
        };

        callback?.Invoke(this, args);
    }

    private List<PlayerDataModel> DoLoadingOperation()
    {
        string path = Path.Combine(AppPersistentDataPath, savesFolderPath);
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        path = Path.Combine(path, scoreFileName);
#if false
        if (!dirInfo.Exists)
        {
            errorCode = ScoreLoadingStatus.FileOrDirectoryNotFound;
            errorMessage = "Directory " + path + " not found";
        }
        path = Path.Combine(path, scoreFileName);

        if (!File.Exists(path))
        {
            errorCode = ScoreLoadingStatus.FileOrDirectoryNotFound;
            errorMessage = "File " + path + " not found";
        }
#endif
        try
        {
            using (StreamReader file = File.OpenText(path))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var result = (List<PlayerDataModel>)serializer.Deserialize(file, typeof(List<PlayerDataModel>));
                    if (result == null)
                    {
                        result = new List<PlayerDataModel>();
                    }
                    return result;
                }
                catch (JsonSerializationException ex)
                {
                    // TODO: Log errors
                    return new List<PlayerDataModel>();
                }
            }
        }
        catch (Exception e)
        {
            return new List<PlayerDataModel>();
        }
    }

    public async void SaveScoreEntryAsync(PlayerDataModel entry, Action callback)
    {
        var data = await Task.Run(DoLoadingOperation);

        data.Add(entry);

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
                serializer.Serialize(writer, data);
                Debug.Log("Saving completed correctly");
            }
            catch (JsonSerializationException ex)
            {
                Debug.LogError("JSON Data loader: " + ex.Message);
            }
        }

        callback?.Invoke();
    }
}