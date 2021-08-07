using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataNotIncluded
{
    class CycleReport
    {
        private DataOject gameData;
        private readonly Dictionary<string, bool> extract;

        // err file is open
        public void WriteCsv(Dictionary<string, List<Dictionary<string, string>>> data) {
            string currSaveFilePath = Path.GetDirectoryName(SaveLoader.GetActiveSaveFilePath()) + "\\data\\";
            bool exists = System.IO.Directory.Exists(currSaveFilePath);

            if (!exists)
                System.IO.Directory.CreateDirectory(currSaveFilePath);

            foreach (var item in data)
            {
                string fileName = "_" + item.Key + ".csv";
                string filePath = Path.Combine(currSaveFilePath, Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath()) +  fileName);
                bool headerWritten = false;
                StringBuilder header = new StringBuilder();
                StringBuilder body = new StringBuilder();

                item.Value.ForEach(delegate (Dictionary<string, string> line)
                {
                    int index = 0;
                    foreach (var dict in line)
                    {
                        if (headerWritten == false)
                        {
                            header.Append(dict.Key);
                            if (line.Count - 1 != index)
                            {
                                header.Append(",");
                            }
                        }
                        body.Append(dict.Value);
                        if (line.Count-1 != index)
                        {
                            body.Append(",");
                        }
                        index++;
                    }
                    body.Append("\n");
                    headerWritten = true;
                });
                header.Append("\n");
                int finalFileSize = (header.ToString().Length + body.ToString().Length);
                if (finalFileSize != 1)
                {
                    File.WriteAllText(filePath, header.ToString() + body.ToString());
                }
            }
        }

        public void Debugger()
        {
        }
        public void ExtractData()
        {
            // Handling Headers
            this.gameData.dataHeader.ForEach(delegate (DataHeader currHeader) 
            {
                //Debug.Log(currHeader.header);
            
            });

            // Handling Rows
            Dictionary<string,List<Dictionary<string, string>>> generalData = new Dictionary<string,List<Dictionary<string, string>>>();
            generalData.Clear();
            

            this.gameData.dataRows.ForEach(delegate (DataRow currDataRow)
            {
               
                if (this.extract[currDataRow.type.ToString()]==true)
                {
                    Dictionary<string, string> rawDataGeneral = new Dictionary<string, string>();
                    rawDataGeneral.Clear();

                    if (generalData.ContainsKey(currDataRow.type.ToString()) == false)
                    {
                        generalData.Add(currDataRow.type.ToString(), new List<Dictionary<string, string>>());
                    }

                    // General
                    rawDataGeneral.Add("Cycle", currDataRow.cycle.ToString());
                    rawDataGeneral.Add("Added", currDataRow.added.ToString());
                    rawDataGeneral.Add("Removed", currDataRow.removed.ToString());
                    rawDataGeneral.Add("Net", currDataRow.net.ToString());
                    generalData[currDataRow.type.ToString()].Add(rawDataGeneral);

                    //Specific
                    List<Dictionary<string, string>> rawData = new List<Dictionary<string, string>>();

                    currDataRow.rowContexts.ForEach(delegate (Context currDataRowContext) {
                        string contextUpdated = currDataRowContext.rowHeader;

                        if (currDataRowContext.rowHeader.Contains("link"))
                        {
                            string pat = @"(.*)<.*>(.*)<.*>";
                            Regex regX = new Regex(pat, RegexOptions.IgnoreCase);
                            Match matchedCase = regX.Match(contextUpdated);
                            if(matchedCase.Success==true)
                            {
                                contextUpdated = matchedCase.Groups[1].Value + matchedCase.Groups[2].Value;
                            }
                        }

                        Dictionary<string, string> row = new Dictionary<string, string>
                        {
                            { "Context", contextUpdated },
                            { "Cycle", currDataRow.cycle.ToString() },
                            { "Added", currDataRowContext.Positive.ToString() },
                            { "Removed", currDataRowContext.Negative.ToString() },
                            { "Net", currDataRowContext.Net.ToString() }
                        };
                        rawData.Add(row);
                        row = null;
                    });
                    //SAVE
                    if (generalData.ContainsKey(currDataRow.type.ToString() + "_EXPANDED") == false) {
                        generalData.Add(currDataRow.type.ToString() + "_EXPANDED", rawData);
                    }
                    else
                    {
                        generalData[currDataRow.type.ToString() + "_EXPANDED"] = generalData[currDataRow.type.ToString() + "_EXPANDED"].Concat(rawData).ToList();
                    }
                    rawData = null;
                    rawDataGeneral = null;
                }
            });
            // WRITE
            this.WriteCsv(generalData);
            generalData = null;
        }
     
        public CycleReport(DataOject gameData)
        {
            this.gameData = gameData;

            //Config: let user select?
            this.extract = new Dictionary<string, bool>();
            this.extract.Clear();
            this.extract.Add("DuplicantHeader", false);
            this.extract.Add("CaloriesCreated", true);
            this.extract.Add("StressDelta", true);
            this.extract.Add("DiseaseAdded", true);
            this.extract.Add("DiseaseStatus", true);
            this.extract.Add("LevelUp", true);
            this.extract.Add("ToiletIncident", false);
            this.extract.Add("ChoreStatus", true);
            this.extract.Add("DomesticatedCritters", true);
            this.extract.Add("WildCritters", true);
            this.extract.Add("RocketsInFlight", false);
            this.extract.Add("TimeSpentHeader", false);
            this.extract.Add("WorkTime", true);
            this.extract.Add("TravelTime", true);
            this.extract.Add("PersonalTime", true);
            this.extract.Add("IdleTime", true);
            this.extract.Add("BaseHeader", false);
            this.extract.Add("OxygenCreated", true);
            this.extract.Add("EnergyCreated", true);
            this.extract.Add("EnergyWasted", true);
            this.extract.Add("ContaminatedOxygenToilet", false);
            this.extract.Add("ContaminatedOxygenSublimation", false);
        }
    }

    public struct Context {

        public string rowHeader;
        public float Positive;
        public float Negative;
        public float Net;
        public List<Note> ContextNotes;

        public void AddNote(Note value)
        {
            this.ContextNotes.Add(value);
        }

        public Context(string rowHeader, float Positive, float Negative, float Net)
        {
            this.rowHeader = rowHeader;
            this.Positive = Positive;
            this.Negative = Negative;
            this.Net = Net;
            this.ContextNotes = new List<Note>();
        }
    }

    public struct Note
    {
        public string label;
        public float value;

        public Note(string label, float value)
        {
            this.label = label;
            this.value = value;
        }
    }

    public struct ToolTip
    {
        public string stringValue;
        public ReportManager.ReportEntry.Order order;
        public ToolTip(string stringValue, ReportManager.ReportEntry.Order order)
        {
            this.stringValue = stringValue;
            this.order = order;
        }
    }
    public struct DataOject
    {
        public List<DataHeader> dataHeader;
        public List<DataRow> dataRows;


        public DataOject(List<DataHeader> dataHeader, List<DataRow> dataRows)
        {
            this.dataHeader = dataHeader;
            this.dataRows = dataRows;
        }
    }

    public struct DataRow {

        public ReportManager.ReportType type;
        public float added;
        public float removed;
        public float net;
        public int cycle;
        public List<Context> rowContexts;
        public List<Note> notes;


        public void AddRowContext(Context value)
        {
            this.rowContexts.Add(value);
        }
        public void AddNote(Note value)
        {
            this.notes.Add(value);
        }

        public DataRow(int cycle, ReportManager.ReportType type, float added, float removed, float net)
        {
            this.cycle = cycle;
            this.type = type;
            this.added = added;
            this.removed = removed;
            this.rowContexts = new List<Context>();
            this.notes = new List<Note>();
            this.net = net;
        }    
    }

    public struct DataHeader
    {
        public string header;
        public ToolTip positiveTooltip;
        public ToolTip negativeTooltip;

        public DataHeader(string header, ToolTip positiveTooltip, ToolTip negativeTooltip)
        {
            this.header = header;
            this.positiveTooltip = positiveTooltip;
            this.negativeTooltip = negativeTooltip;
        }

    }
}
