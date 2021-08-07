using System.Collections.Generic;


namespace DataNotIncluded
{
    class CycleReport
    {
        private DataOject gameData;

        public void ExtractData()
        {
            // Handling Headers
            //this.gameData.dataHeader;
            //this.gameData.dataRows;

        }
        public CycleReport(DataOject gameData)
        {
            this.gameData = gameData;
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
        public DataHeader dataHeader;
        public List<DataRow> dataRows;


        public DataOject(DataHeader dataHeader, List<DataRow> dataRows)
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
