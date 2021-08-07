using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;
using System;


namespace DataNotIncluded
{
    public class DataNotIncludedPatches
    {
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

                // Append the new button
                pauseMenuButtons.Insert(pauseMenuButtons.Count-1, ExportBtn);
                buttonsObjectVariable.SetValue(__instance, pauseMenuButtons);

                // Reset
                buttonsObjectVariable = null;
            }



            private static void OnExportDailyReports()
            {
                // Init
                Debug.Log("[DataNotIncluded] : Exporting Daily Reports");
                List<DataRow> dataRows = new List<DataRow>();
                List<DataHeader> dataHeaders = new List<DataHeader>();

                // GameObjects
                List<ReportManager.DailyReport> reports = ReportManager.Instance.reports;
                Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> reportDict = ReportManager.Instance.ReportGroups;

                // Get TooltipHeader
                foreach (var reportGroup in reportDict)
                { 
                    ToolTip positiveTooltip = new ToolTip(reportGroup.Value.positiveTooltip, reportGroup.Value.posNoteOrder);
                    ToolTip negativeTooltip = new ToolTip(reportGroup.Value.negativeTooltip, reportGroup.Value.posNoteOrder);
                    DataHeader dataGroupHeaders = new DataHeader(reportGroup.Value.stringKey, positiveTooltip, negativeTooltip);

                    dataHeaders.Add(dataGroupHeaders);
                }

                // Get Actual Data
                if (reports != null) { // Not Empty

                    reports.ForEach(delegate (ReportManager.DailyReport currentCycleReport)
                    {
                        currentCycleReport.reportEntries.ForEach(delegate (ReportManager.ReportEntry entry) {
                            DataRow dataRow = new DataRow(currentCycleReport.day, entry.reportType, entry.Positive, entry.Negative, entry.Net);

                            if (entry.HasContextEntries()) {
                                for (int index = 0; index < entry.contextEntries.size; index++)
                                {
                                    Context rowContext = new Context(entry.contextEntries[index].context,
                                        entry.contextEntries[index].Positive,
                                        entry.contextEntries[index].Negative,
                                        entry.contextEntries[index].Net);
                                    // Get Corresponding Notes
                                    entry.contextEntries[index].IterateNotes(delegate (ReportManager.ReportEntry.Note note)
                                    {
                                        Note rowNote = new Note(note.note, note.value);
                                        rowContext.AddNote(rowNote);
                                    });
                                    dataRow.AddRowContext(rowContext);
                                }
                            }
                            // Get Corresponding Notes
                            entry.IterateNotes(delegate (ReportManager.ReportEntry.Note note)
                            {
                                Note reportTypeNote = new Note(note.note, note.value);
                                dataRow.AddNote(reportTypeNote);
                            });
                            // Append
                            dataRows.Add(dataRow);
                        });
                    }); 
                }
                reports = null;
                reportDict = null;
                // Class Init
                CycleReport dataExtractor = new CycleReport(new DataOject(dataHeaders, dataRows));
                try
                {
                    dataExtractor.ExtractData();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}
