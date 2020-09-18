//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Firebase;
//using Firebase.Database;
//using System;
//using Newtonsoft.Json;

//public class FirebaseScoreDataLoader : IDataLoader
//{
//    public async void LoadScoreDataAsync(EventHandler<OnScoreDataLoadedEventArgs> callback)
//    {
//        OnScoreDataLoadedEventArgs args = null;

//        await FirebaseDatabase.DefaultInstance.GetReference("Score").GetValueAsync().ContinueWith( task =>
//        {
//            if (task.IsCompleted)
//            {
//                Debug.Log(task.Result.GetRawJsonValue());

//                string json = task.Result.GetRawJsonValue();

//                // TODO: Use firebase to retrieve deserialized data
//                List<PlayerDataModel> data = null;

//                try
//                {
//                    data = JsonConvert.DeserializeObject<List<PlayerDataModel>>(json);
//                }
//                catch (JsonSerializationException e)
//                {
//                    data = new List<PlayerDataModel>();
//                }

//                args = new OnScoreDataLoadedEventArgs()
//                {
//                    status = ScoreLoadingStatus.Ok,
//                    errorMessage = "",
//                    data = data
//                };
//            }
//            else
//            {
//                args = new OnScoreDataLoadedEventArgs()
//                {
//                    status = ScoreLoadingStatus.DatabaseError,
//                    errorMessage = "",
//                    data = new List<PlayerDataModel>()
//                };
//            }
//        });

//        callback?.Invoke(this, args);
//    }

//    public async void SaveScoreEntryAsync(PlayerDataModel entry, Action callback)
//    {
//    }
//}
