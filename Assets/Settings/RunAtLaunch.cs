﻿using System.IO;
using UnityEngine;


public class RunAtLaunch : MonoBehaviour {

    // Object References
    public GameObject FolderBrowserDialog;
    public GameObject DataSourceManagerPanel;

    /// <summary>
    ///  Run this code at launch
    /// </summary>

    void Start()
    {

        Agent.FindWowInstalls();

        //Network.Disconnect();
        Settings.ApplicationPath = Application.streamingAssetsPath;
        if (!File.Exists("Settings.ini"))
        {
            File.WriteAllLines("Settings.ini", Settings.Data);
        }
        else
        {
            long length = new System.IO.FileInfo("Settings.ini").Length;
            if (length > 0)
            {
                string[] DataBuffer = File.ReadAllLines("Settings.ini");
                for(int i = 0; i < DataBuffer.Length; i++)
                {
                    Settings.Data[i] = DataBuffer[i];
                }
            }
            else
            {
                File.WriteAllLines("Settings.ini", Settings.Data);  // defaults
            }
        }
        SettingsInit();
        ADT.Initialize();

        if (Settings.Data[2] == Settings.WoWSource.Extracted.ToString())
        {
            DB2 db2 = new DB2();
            db2.Initialize();
        }

    }

    private void SettingsInit()
    {
        // Check if cache dir exists
        if (Settings.Data[0] == null || Settings.Data[0] == "")
        {
            // open dialog to pick cache dir
            //Debug.Log("pick cache dir");
            FolderBrowserDialog.SetActive(true);
            FolderBrowserDialog.GetComponent<DialogBox_BrowseFolder>().LoadInfo("Cache Folder",
                "Choose a location where the Cache folder will be created, or Cancel to save in the current folder. " +
                "\nRecomended 1 GB of free space, minimum." +
                "\nThe Cache folder can be changed later on from the Settings.",
                "Enter address or click Browse...");
            FolderBrowserDialog.GetComponent<DialogBox_BrowseFolder>().Link("DialogBoxCache_Ok", "DialogBoxCache_Cancel", this);
        }
        else
        {
            CheckWoWInstalls();
        }
    }

    private void CreateCacheDir()
    {
        if (!Directory.Exists(Settings.Data[0]))
        {
            Directory.CreateDirectory(Settings.Data[0]);
        }
    }

    void DialogBoxCache_Ok()
    {
        Settings.Data[0] = FolderBrowserDialog.GetComponent<DialogBox_BrowseFolder>().ChosenPath + @"\Cache";
        CreateCacheDir();
        Settings.Save();
        CheckWoWInstalls();
    }

    void DialogBoxCache_Cancel()
    {
        Settings.Data[0] = "Cache";
        CreateCacheDir();
        Settings.Save();
        CheckWoWInstalls();
    }

    public void CheckWoWInstalls()
    {
        Settings.GetInstalledGames();
        CreateCacheDir();
        Settings.Save();
        CheckDataSource();
    }
        
    public void CheckDataSource()
    {
        if (Settings.Data[2] == null || Settings.Data[2] == "")
        {
            // open Data Source Manager //
            DataSourceManagerPanel.GetComponent<DataSourceManager>().Initialize();
            DataSourceManagerPanel.SetActive(true);
        }

        if (Settings.Data[2] == "0") // game mode //
        {
            DataSourceManagerPanel.GetComponent<DataSourceManager>().Initialize();
            CascInitialize.Start();
        }
    }



}

