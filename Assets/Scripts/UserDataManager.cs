﻿using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void Load()
    {
        //Cek apakah ada data yang tersimpan sbagai PROGRESS_KEY
        if(!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            //jika tidak ada maka buat data baru
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            //jika ada maka timpa progress dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void LoadFromLocal()
    {
        if(!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Save(true); //jika tidak ada maka simpan data baru dan upload ke cloud
        }
        else
        {
            //jika ada maka timpa progress dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData> (json);
        }
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);

        if(uploadToCloud)
        {
            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();

            targetStorage.PutBytesAsync(data);
        }
    }

    public static bool HasResources (int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }

    public static IEnumerator LoadFromCloud(System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();

        bool isCompleted = false;
        bool isSuccessfull = false;
        const long maxAllowedSize = 1024 * 1024; //sama dengan 1 mb
        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if(!task.IsFaulted)
            {
                string json = Encoding.Default.GetString(task.Result);
                Progress = JsonUtility.FromJson<UserProgressData> (json);
                isSuccessfull = true;
            }
            isCompleted = true;
        });

        while(!isCompleted)
        {
            yield return null;
        }

        //jika sukses mendownload, maka simpan data hasil download 
        if(isSuccessfull)
        {
            Save();
        }
        else
        {
            LoadFromLocal(); //jika tidak ditemukan di cloud maka load dari penyimpanan lokal 
        }
        onComplete?.Invoke();
    }

    private static StorageReference GetTargetCloudStorage()
    {
        //Gunakan Device ID sebagai nama file yang akan disimpan dalam cloud
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
    }

}
