using System;
using System.Collections.Generic;

public enum ScoreLoadingStatus
{
    Ok,
    FileOrDirectoryNotFound,
    DatabaseError,
    UnknownError
};

public class OnScoreDataLoadedEventArgs : EventArgs
{
    public ScoreLoadingStatus status;
    public string errorMessage;
    public List<PlayerDataModel> data;
}

/// <summary>
/// The classes, that implements this interface, should contains functional for save/load game data.
/// It is assumed that we can use JSON data loader or Network data loader without changing basic
/// code of work with loader.
/// </summary>
public interface IDataLoader
{
    /// <summary>
    /// This method is used for loading data. It return true if data loaded correctly and false else.
    /// In addition it return the message with description of operation result.
    /// </summary>
    /// <param name="loadedData">list of loaded data models</param>
    /// <param name="resultMessage">message with result of operation (success or error description)</param>
    /// <returns></returns>
    void LoadScoreDataAsync(EventHandler<OnScoreDataLoadedEventArgs> callback);

    /// <summary>
    /// This method is used for saving your data
    /// </summary>
    /// <param name="scoreDataPack">data for saving</param>
    void SaveScoreEntryAsync(PlayerDataModel entry, Action callback);
}