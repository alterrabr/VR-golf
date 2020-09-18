using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This class implements transactions for score data provider. This allows to batch
/// multiple actions on data per one load/store operation
/// </summary>
public class ScoreTransaction
{
    // TODO: Maybe make this class IDisposable
    private IDataLoader dataProvider;
    private List<PlayerDataModel> database;

    bool wasModifiedInThisTransaction;

    /// <summary>
    /// Cached data for current transaction
    /// </summary>
    public List<PlayerDataModel> Database
    {
        get
        {
            return database;
        }
    }

    /// <summary>
    /// Contructor for ScoreTransaction
    /// </summary>
    public ScoreTransaction(IDataLoader provider)
    {
        dataProvider = provider;
    }

    /// <summary>
    /// Execute transaction asyncronously.
    /// <param name="onTransactionOpen">This callback called if transaction was started successfully</param>
    /// <param name="onTransactionOpen">This callback called if transaction was failed</param>
    /// </summary>
    public async void ExecuteAsyncTransaction(Action onTransactionOpen, Action<ScoreLoadingError> onTrnsactionFailed)
    {
        Debug.Assert(HasPendingTransaction() == false, "Failed to execute async transaction. The transaction was already opened");
        ScoreLoadingError status = await Task.Run(() => BeginTransaction());

        if (status == ScoreLoadingError.Ok || status == ScoreLoadingError.FileOrDirectoryNotFound)
        {
            onTransactionOpen?.Invoke();
        }
        else
        {
            onTrnsactionFailed?.Invoke(status);
        }

        EndTransaction();
    }

    /// <summary>
    /// Add entry to local copy of database for current transaction
    /// </summary>
    public void AddEntry(PlayerDataModel entry)
    {
        if (database != null)
        {
            database.Add(entry);

            wasModifiedInThisTransaction = true;
        }
        else
        {
            Debug.LogError("Failed to add entry. Transaction was not started");
        }
    }

    /// <summary>
    /// Check if trnsaction was started
    /// </summary>
    public bool HasPendingTransaction()
    {
        return database != null;
    }

    /// <summary>
    /// Begin transaction.
    /// <returns>True id transaction was started successfully. False of transaction is failed</returns>
    /// </summary>
    private ScoreLoadingError BeginTransaction()
    {
        Thread.Sleep(3000);
        ScoreLoadingError errorCode = ScoreLoadingError.Ok;
        if (database == null)
        {
            database = dataProvider.LoadScoreData(out errorCode, out string message);
            if (errorCode == ScoreLoadingError.Ok)
            {
                wasModifiedInThisTransaction = false;
            }
            else if (errorCode == ScoreLoadingError.FileOrDirectoryNotFound)
            {
                database = new List<PlayerDataModel>();
                wasModifiedInThisTransaction = false;
            }
            else
            {
                Debug.LogError("Failed to start transaction. Error: " + message);
            }
        }
        else
        {
            Debug.LogError("Failed to start transaction. Previous transaction is still open");
        }

        return errorCode;
    }

    /// <summary>
    /// End transaction. Invalidates local database. Writes all changes back to data provider
    /// </summary>
    private void EndTransaction()
    {
        if (database != null)
        {
            if (wasModifiedInThisTransaction)
            {
                wasModifiedInThisTransaction = false;
                dataProvider.SaveScoreData(database);
            }
            database = null;
        }
        else
        {
            Debug.LogError("EndTransaction() is called without matching BeginTransaction call()");
        }
    }
}
