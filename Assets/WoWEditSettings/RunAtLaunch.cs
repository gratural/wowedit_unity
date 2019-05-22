﻿using Assets.Data.Agent;
using Assets.Data.WoW_Format_Parsers.ADT;
using Assets.Tools.CSV;
using System.IO;
using UnityEngine;
using Assets.WoWEditSettings;
using CASCLib;
using Assets.UI.CASC;

public class RunAtLaunch : MonoBehaviour {

    // Object References
    public GameObject FolderBrowserDialog;
    public GameObject DataSourceManagerPanel;
    public GameObject CASC;

    /// <summary>
    ///  Run this code at launch
    /// </summary>

    void Start()
    {
        UserPreferences.Load();

        Settings.ApplicationPath = Application.streamingAssetsPath;
        Settings.Load();

        SettingsInit();
        ADT.Initialize();
    }

    private void SettingsInit()
    {
        // Check if cache dir exists
        if (Settings.GetSection("path").GetString("cachepath") == null || Settings.GetSection("path").GetString("cachepath") == "")
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
            CheckWoWInstalls();
    }

    private void CreateCacheDir()
    {
        if (!Directory.Exists(Settings.GetSection("path").GetString("cachepath")))
            Directory.CreateDirectory(Settings.GetSection("path").GetString("cachepath") + @"\Cache");
    }

    void DialogBoxCache_Ok()
    {
        Settings.GetSection("path").SetValueOfKey("cachepath", FolderBrowserDialog.GetComponent<DialogBox_BrowseFolder>().ChosenPath + @"\Cache");
        CreateCacheDir();
        Settings.Save();
        CheckWoWInstalls();
    }

    void DialogBoxCache_Cancel()
    {
        Settings.GetSection("path").SetValueOfKey("cachepath", "Cache");
        CreateCacheDir();
        Settings.Save();
        CheckWoWInstalls();
    }

    public void CheckWoWInstalls()
    {
        CreateCacheDir();
        Settings.Save();
        CheckDataSource();
    }
        
    public void CheckDataSource()
    {
        if (Settings.GetSection("misc").GetString("wowsource") == null || 
            Settings.GetSection("misc").GetString("wowsource") == "")
        {
            // open Data Source Manager //
            DataSourceManagerPanel.GetComponent<DataSourceManager>().Initialize();
            DataSourceManagerPanel.SetActive(true);
        }

        if (Settings.GetSection("misc").GetString("wowsource") == "game") // game mode //
        {
            CASCConfig config   = CASCConfig.LoadLocalStorageConfig(Settings.GetSection("path").GetString("selectedpath"), 
                                                                    "wowt");
            CASC.GetComponent<CascHandler>().cascHandler = CASCHandler.OpenStorage(config);
            CASC.GetComponent<CascHandler>().cascHandler.Root.SetFlags(LocaleFlags.None, false, false);
        }
    }
}
