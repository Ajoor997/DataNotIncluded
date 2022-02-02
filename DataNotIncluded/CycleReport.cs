using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace DataNotIncluded
{
    class CycleReport
    {
        private string currSaveFilePath;
        private Dictionary<string,List<string>> csvFormatRaw;
        private DataNotIncludedConfigs modConfig;

        public CycleReport(DataNotIncludedConfigs config)
        {
            // Init
            Debug.Log("[DataNotIncluded] : Processing Daily Reports v2");
            this.csvFormatRaw = new Dictionary<string, List<string>>();
            config.UpdateTypesDict();
            this.modConfig = config;
            // get CoreGame Object that contains the data
            List<ReportManager.DailyReport> reports = ReportManager.Instance.reports;
            // Get Actual Data
            if (reports != null) { // Do if Not Empty

               reports.ForEach(delegate (ReportManager.DailyReport currentCycleReport)
                {
                    ProcessData(currentCycleReport);
                });
            }
            reports = null;
        }
        public void DestroyCycleReport()
        {
            this.csvFormatRaw.Clear();
            this.csvFormatRaw = null;
            this.currSaveFilePath = null;
            this.modConfig = null;
        }
        public void UpdateDate() {
            Debug.Log("[DataNotIncluded] : Updating Data");
            List<ReportManager.DailyReport> reports = ReportManager.Instance.reports;
            ReportManager.DailyReport todaysReport = reports[reports.Count - 1];
            ProcessData(todaysReport);
            reports = null;
            todaysReport = null;
        }
        private void ProcessData(ReportManager.DailyReport currentCycleReport) {
            string row;
            string contextUpdated;
            string sourceUpdated;
            float value;
            float added;
            float removed;
            currentCycleReport.reportEntries.ForEach(delegate (ReportManager.ReportEntry entry) {
                if (this.modConfig.types.ContainsKey(entry.reportType.ToString()))
                {
                    if (this.modConfig.types[entry.reportType.ToString()] == true)
                    {
                        if (this.modConfig.Totals)
                        {
                            added = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.Positive)));
                            removed = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.Negative)));
                            value = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.Net)));
                            // 	Cycle	Added	Removed	Net
                            row = (currentCycleReport.day.ToString() + "," + added.ToString() + "," + removed.ToString() + "," + value.ToString());
                            this.UpdateDictionary(entry.reportType.ToString(), row, "_TOTALS", "Cycle,Added,Removed,Net");
                        }

                        //ChekChilderns
                        if (entry.HasContextEntries())
                        {
                            for (int index = 0; index < entry.contextEntries.size; index++)
                            {
                                added = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.contextEntries[index].Positive)));
                                removed = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.contextEntries[index].Negative)));
                                value = this.RemoveNegative(this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), entry.contextEntries[index].Net)));

                                contextUpdated = entry.contextEntries[index].context;
                                if (contextUpdated.Contains("link"))
                                {
                                    contextUpdated = getValueFromPattern(contextUpdated);
                                }
                                // Cycle    Context    Added  Removed  Net
                                row = (currentCycleReport.day.ToString() + "," + contextUpdated + "," + added.ToString() + "," + removed.ToString() + "," + value.ToString());
                                this.UpdateDictionary(entry.reportType.ToString(), row, "_EXPANDED", "Cycle,Context,Added,Removed,Net");

                                // Get Corresponding Notes
                                if (this.modConfig.ignoreNotes.Contains(entry.reportType.ToString()))
                                {
                                    entry.contextEntries[index].IterateNotes(delegate (ReportManager.ReportEntry.Note note)
                                    {
                                        value = this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), note.value));
                                        sourceUpdated = note.note.ToString();
                                        if (sourceUpdated.Contains("link"))
                                        {
                                            sourceUpdated = getValueFromPattern(sourceUpdated);
                                        }
                                    // Cycle Context Source  Value
                                    row = (currentCycleReport.day.ToString() + "," + contextUpdated + "," + sourceUpdated + "," + value.ToString());
                                        this.UpdateDictionary(entry.reportType.ToString(), row, "_NOTES", "Cycle,Context,Source,Value");
                                    });
                                }
                            }
                        }
                        // Get Corresponding Notes
                        if (this.modConfig.Totals && this.modConfig.ignoreNotes.Contains(entry.reportType.ToString()))
                        {
                            entry.IterateNotes(delegate (ReportManager.ReportEntry.Note note)
                            {
                                value = this.AdjustKprifix(entry.reportType.ToString(), this.RoundNess(entry.reportType.ToString(), note.value));
                                sourceUpdated = note.note.ToString();
                                if (sourceUpdated.Contains("link"))
                                {
                                    sourceUpdated = getValueFromPattern(sourceUpdated);
                                }
                                // Cycle    Source  Value
                                row = (currentCycleReport.day.ToString() + "," + sourceUpdated + "," + value.ToString());
                                this.UpdateDictionary(entry.reportType.ToString(), row, "_NOTES_TOTALS", "Cycle,Context,Value");
                            });
                        }
                    }
                }

            });
        }
        private float RemoveNegative(float value) {
            if(value<0 && this.modConfig.Negatives) {
                return (value*-1);
            }
            else {
                return value;
            }
        }
        private float AdjustKprifix(string type, float value)
        {
            int k = 1000;
            if (type=="OxygenCreated") {
                if (this.modConfig.KPrefix == false) {
                    value *= k;
                }
            }
            else if (type == "EnergyCreated" || type == "EnergyWasted" || type == "CaloriesCreated")
            {
                if (this.modConfig.KPrefix == true) {
                    value /= k;
                }
            }
            return value;
        }
        private float RoundNess(string type, float value)
        {
            if (type == "OxygenCreated" || type == "EnergyCreated" || type == "EnergyWasted" || type == "CaloriesCreated" || 
                type == "IdleTime" || type == "PersonalTime" || type == "TravelTime" || type == "WorkTime") { 
                int digits = this.modConfig.RoundDigits;
                return (float)Math.Round(value, digits);
            }
            return value;
        }
        private void CreateDataFolder() {
            this.currSaveFilePath = Path.GetDirectoryName(SaveLoader.GetActiveSaveFilePath()) + "\\data_v2\\";
            //Create Date File If Needed
            if (!System.IO.Directory.Exists(this.currSaveFilePath))
            {
                System.IO.Directory.CreateDirectory(this.currSaveFilePath);
            }
        }
        public void WriteCsv(){
            this.CreateDataFolder();
            foreach (var item in this.csvFormatRaw)
            {
                string fileName = "_" + item.Key + ".csv";
                string filePath = Path.Combine(this.currSaveFilePath, Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath()) + fileName);

                TextWriter tw = new StreamWriter(filePath);

                foreach (string line in item.Value)
                {
                    tw.WriteLine(line);
                }
                tw.Close();
            }
            return;
        }
        public void WriteXML() {
            this.CreateDataFolder();
            foreach (var item in this.csvFormatRaw)
            {
                string fileName = "_" + item.Key + ".xml";
                string filePath = Path.Combine(this.currSaveFilePath, Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath()) + fileName);
                string headers = item.Value[0];

                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                tw.WriteLine("\t<" + item.Key + ">");
                foreach (string line in item.Value.GetRange(1, item.Value.Count - 1)) {
                    tw.WriteLine("\t\t<CycleData>");
                    int index = 0;
                    foreach (string cell in line.Split(',')) {
                        string header = headers.Split(',')[index];
                        tw.WriteLine("\t\t\t<" + header + ">"+ cell + "</"+ header + ">");
                        index += 1;
                    }          
                    tw.WriteLine("\t\t</CycleData>");
                }
                tw.WriteLine("\t</" + item.Key + ">");
                tw.Close();
            }
            return;
        }

        public void WriteJSON() {
            this.CreateDataFolder();
            foreach (var item in this.csvFormatRaw)
            {
                string fileName = "_" + item.Key + ".json";
                string filePath = Path.Combine(this.currSaveFilePath, Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath()) + fileName);
                string  headers = item.Value[0];

                TextWriter tw = new StreamWriter(filePath);
                tw.WriteLine("{\n\t\"" + item.Key.Split('_')[0] + "\":{");
                int id = 0;
                foreach (string line in item.Value.GetRange(1, item.Value.Count-1))
                {
                    tw.WriteLine("\t\t\"" + id.ToString() + "\":{");
                    int index = 0;
                    int size = line.Split(',').Length;
                    string comma;
                    foreach (string cell in line.Split(','))
                    {
                        string header = headers.Split(',')[index];
                        size -= 1;
                        if (size != 0) {
                            comma = ",";
                        }
                        else {
                            comma = "";
                        }
                        tw.WriteLine("\t\t\t\"" + header + "\":\"" + cell + "\""+ comma);
                        index += 1;
                    }
                    if (id == item.Value.Count-1) {
                        tw.WriteLine("\t\t}");
                    }
                    else {
                        tw.WriteLine("\t\t},");
                    }
                    id += 1;
                }
                tw.WriteLine("\t}\n}");
                tw.Close();
            }
            return;
        }

        public string GetPATH() {
            return this.currSaveFilePath;
        }

        private string getValueFromPattern(string codedStr) {
            string pat = @"(.*)<.*>(.*)<.*>";
            Regex regX = new Regex(pat, RegexOptions.IgnoreCase);
            Match matchedCase = regX.Match(codedStr);
            if (matchedCase.Success == true)
            {
                codedStr = matchedCase.Groups[1].Value + matchedCase.Groups[2].Value;
            }
            return codedStr;
        }
        private void UpdateDictionary(string key , string data, string prefix, string headers) {
            if (csvFormatRaw.ContainsKey(key + prefix) == false) {
                //Append data to headers
                List<string> items = new List<string>();
                items.Add(headers);
                items.Add(data);
                this.csvFormatRaw.Add(key + prefix, items);
            }
            else {
                this.csvFormatRaw[key + prefix].Add(data);
            }
            return;
        }
    }
}