using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System.Collections.Generic;


namespace DataNotIncluded
{
    [ModInfo("https://github.com/Ajoor997/DataNotIncluded", "preview.png")]
    [JsonObject(MemberSerialization.OptIn)]
    public class DataNotIncludedConfigs
    {
        public Dictionary<string, bool> types;
        public List<string> ignoreNotes = new List<string>() { "DiseaseAdded", "DiseaseStatus", "LevelUp", "ChoreStatus", 
            "DomesticatedCritters", "WildCritters", "ToiletIncident", "RocketsInFlight"
        };

        [Option("The settings below will be applied \nwhen the export button is clicked.\n")]
        public LocText Description => null;

        [Option("CSV Format", "The data will be formated into CSV.", "Format")]
        [JsonProperty]
        public bool CSV { get; set; }
        [Option("JSON Format", "The data will be formated into JSON.", "Format")]
        [JsonProperty]
        public bool JSON { get; set; }

        [Option("XML Format", "The data will be formated into XML.", "Format")]
        [JsonProperty]
        public bool XML { get; set; }

        [Option("Include K prefix", "Adjust data to use the K unit prefix.", "General")]
        [JsonProperty]
        public bool KPrefix { get; set; }

        [Option("Round To Digit", "How many digits after decimal.", "General")]
        [Limit(0,4)]
        [JsonProperty]
        public int RoundDigits { get; set; }

        [Option("Replace Negatives", "Replace negatives with positives.", "General")]
        [JsonProperty]
        public bool Negatives { get; set; }

        [Option("Totals", "Include total tables.", "General")]
        [JsonProperty]
        public bool Totals { get; set; }

        [Option("CaloriesCreated", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool CaloriesCreated { get; set; }

        [Option("StressDelta", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool StressDelta { get; set; }

        [Option("DiseaseAdded", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool DiseaseAdded { get; set; }

        [Option("DiseaseStatus", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool DiseaseStatus { get; set; }

        [Option("LevelUp", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool LevelUp { get; set; }

        [Option("ChoreStatus", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool ChoreStatus { get; set; }

        [Option("DomesticatedCritters", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool DomesticatedCritters { get; set; }

        [Option("WildCritters", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool WildCritters { get; set; }

        [Option("WorkTime", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool WorkTime { get; set; }

        [Option("TravelTime", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool TravelTime { get; set; }

        [Option("PersonalTime", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool PersonalTime { get; set; }

        [Option("IdleTime", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool IdleTime { get; set; }

        [Option("OxygenCreated", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool OxygenCreated { get; set; }

        [Option("EnergyCreated", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool EnergyCreated { get; set; }

        [Option("EnergyWasted", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool EnergyWasted { get; set; }

        [Option("ToiletIncident", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool ToiletIncident { get; set; }

        [Option("RocketsInFlight", "Generate data for this report type.", "Report Type")]
        [JsonProperty]
        public bool RocketsInFlight { get; set; }

        public void UpdateTypesDict()
        {
            this.types = new Dictionary<string, bool>()
            {                                                                   
                { "CaloriesCreated",this.CaloriesCreated },                     
                { "StressDelta",this.StressDelta },                            
                { "DiseaseAdded",this.DiseaseAdded },                           
                { "DiseaseStatus",this.DiseaseStatus },                         
                { "LevelUp",this.LevelUp },                                     
                { "ChoreStatus",this.ChoreStatus },                            
                { "DomesticatedCritters",this.DomesticatedCritters },          
                { "WildCritters",this.WildCritters },                           
                { "WorkTime",this.WorkTime },                                   
                { "TravelTime",this.TravelTime },                               
                { "PersonalTime",this.PersonalTime },                          
                { "IdleTime",this.IdleTime },                                   
                { "OxygenCreated",this.OxygenCreated },                         
                { "EnergyCreated",this.EnergyCreated },                         
                { "EnergyWasted",this.EnergyWasted },                          
                { "ToiletIncident",this.ToiletIncident },                       
                { "RocketsInFlight",this.RocketsInFlight },                     
            };
        }
        public DataNotIncludedConfigs()
        {
            //Init 
            this.types = new Dictionary<string, bool>();

            //Format
            this.CSV = true;
            this.JSON = false;
            this.XML = false;            

            //MATH
            this.RoundDigits = 3;
            this.Negatives = true;
            this.KPrefix = false;
            this.Totals = true;

            // Types
            this.CaloriesCreated = true;
            this.StressDelta = true;
            this.DiseaseAdded = true;
            this.DiseaseStatus = true;
            this.LevelUp = true;
            this.ChoreStatus = true;
            this.DomesticatedCritters = true;
            this.WildCritters = true;
            this.WorkTime = true;
            this.TravelTime = true;
            this.PersonalTime = true;
            this.IdleTime = true;
            this.OxygenCreated = true;
            this.EnergyCreated = true;
            this.EnergyWasted = true;
            this.ToiletIncident = false;
            this.RocketsInFlight = false;

        }
    }
}
