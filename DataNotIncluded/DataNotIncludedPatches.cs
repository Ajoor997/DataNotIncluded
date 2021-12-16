using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;
using System;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace DataNotIncluded
{
    public class DataNotIncludedMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(DataNotIncludedConfigs));
            PUtil.InitLibrary();            
        }
    }
    public class DataNotIncludedPatches
    {
        [HarmonyPatch(typeof(Game), "Load")]
        public static class GameOnLoadPatch
        {
            public static DataNotIncludedConfigs ModConfigs { get; private set; }
            private static CycleReport GameCycleData;
            public static Exception Err = null;
            public static bool Failed { get; set; } = false;
            private static void Postfix() {
                Debug.Log("[DataNotIncluded] : Report Manger Initialized!" + ReportManager.Instance.reports.Count.ToString());
                GameOnLoadPatch.ReadSettings();
                try {
                    GameCycleData = new CycleReport(GameOnLoadPatch.ModConfigs);
                }
                catch(Exception e) {
                    Debug.Log("[DataNotIncluded] : MOD HAS FAILED");
                    Failed = true;
                    Err = e;
                    DestroyCycleReportClass();
                }
            }
            public static void DestroyCycleReportClass()
            {
                GameCycleData.DestroyCycleReport();
                GameCycleData = null;
            }
            public static string GetPATH() {
                return GameCycleData.GetPATH();
            }
            public static void WriteCSV() {
                if (Failed == false) {
                    GameCycleData.WriteCsv();
                } else {
                    throw Err;
                }
            }
            public static void WriteJSON() {
                if (Failed == false)
                {
                    GameCycleData.WriteJSON();
                }
                else
                {
                    throw Err;
                }
            }
            public static void WriteXML(){
                if (Failed == false)
                {
                    GameCycleData.WriteXML();
                }
                else
                {
                    throw Err;
                }
            }
            public static void UpdateDate() {
                if (Failed == false) {
                    GameCycleData.UpdateDate();
                }
            }
            public static void ReadSettings()
            {
                Debug.Log("[DataNotIncluded] : Mod settings Initialized!");
                ModConfigs = POptions.ReadSettings<DataNotIncludedConfigs>();
                if (ModConfigs == null)
                {
                    ModConfigs = new DataNotIncludedConfigs();
                }

            }
        }
        [HarmonyPatch(typeof(ReportManager), "OnNightTime")]
        public class DataUpdater
        {
            private static void Postfix()
            {
                GameOnLoadPatch.UpdateDate();
            }
        }

        [HarmonyPatch(typeof(Game), "OnApplicationQuit")]
        public class ModDestroyer
        {
            private static void Prefix()
            {
                Debug.Log("[DataNotIncluded] : Mod Destruction");
                GameOnLoadPatch.DestroyCycleReportClass();
            }
        }

        // Append Export DataButton 
        [HarmonyPatch(typeof(PauseScreen), "OnPrefabInit")]
        public class DataNotIncludedExporter
        {
            private static void Postfix(PauseScreen __instance)
            {
                // Init
                Debug.Log("[DataNotIncluded] : Export Button Initialized!");
                FieldInfo buttonsObjectVariable = AccessTools.Field(typeof(KButtonMenu), "buttons");

                // Get List of Menu Buttons
                List<KButtonMenu.ButtonInfo> pauseMenuButtons = new List<KButtonMenu.ButtonInfo>(
                    (IEnumerable<KButtonMenu.ButtonInfo>) buttonsObjectVariable.GetValue(__instance));

                // Create the Button
                KButtonMenu.ButtonInfo ExportBtn = new KButtonMenu.ButtonInfo(
                    "Export Daily Reports",
                    (Action)266,
                    new UnityAction(OnExportDailyReports));
                // Append the button
                pauseMenuButtons.Insert(pauseMenuButtons.Count-1, ExportBtn);
                buttonsObjectVariable.SetValue(__instance, pauseMenuButtons);
                
                // Reset
                buttonsObjectVariable = null;
            }
            private static void OnExportDailyReports() {

                // Init       
                string title = "Exporter Finished Executing\n";
                string errTitle = "Error: Unkown\n";
                string errMsg = "Unkown Error Occured!\n";
                string successMsg = "data exported to game save file!\n";
                bool err = false;

                try {
                    if (GameOnLoadPatch.ModConfigs.CSV) {
                        GameOnLoadPatch.WriteCSV();
                    }
                    if (GameOnLoadPatch.ModConfigs.XML) {
                        GameOnLoadPatch.WriteXML();
                    }
                    if (GameOnLoadPatch.ModConfigs.JSON) {
                        GameOnLoadPatch.WriteJSON();
                    }
                }
                catch (Exception e) {
                    err = true;
                    Debug.Log("[DataNotIncluded][ERROR] : " + e);
                    Debug.Log("[DataNotIncluded][ERROR_CODE] : " + e.HResult);
                    if (e.HResult == -2147024864) // Sharing violation
                    {
                        errTitle = "Error: Sharing Vilation\n";
                        errMsg = "Cannot write into an oppend file, Please Close all csv files!\n";
                    }
                    if (GameOnLoadPatch.Failed == true)
                    {
                        errTitle = "Error: Mod Failure\n";
                        errMsg = "Report the following error to mod creator!\n";
                        errMsg += e.StackTrace + '\n';
                    }
                }
                if(err==true)
                {
                    title = title + errTitle + errMsg;
                }
                else
                {
                    title += successMsg + GameOnLoadPatch.GetPATH();
                }
                ((ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(
                    ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, null))
                    .PopupConfirmDialog(title, new System.Action(OnConfirm), null);
            }
            private static void OnConfirm()
            {
                Debug.Log("[DataNotIncluded] : Data Exportation Compeleted!");
            }
        }
    }
}
