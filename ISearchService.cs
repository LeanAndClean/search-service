namespace Search
{
    using System.Collections.Generic;

    public class Search
    {
        public int TotalRows {get; set;}
        public int Offset {get; set;}
        public List<Row> Rows {get; set;}
    }

    public class Row
    {
        public string Id {get; set;}
        public string Key {get; set;}
        public Doc Doc {get; set;}
    }

    public class Doc
    {
        public string Id {get; set;}
        public string MbId {get; set;}
        public string Title {get; set;}
        public string Artist {get; set;}
        public float Price {get; set;}
        
        public override string ToString()
        {
            return "Id: " + Id + " MbId: " + MbId + " Title: " + Title + " Artist: " + Artist + " Price: " + Price;
        }
    }

    public interface ISearchService
    {
        IList<dynamic> Find(dynamic filter);
        dynamic Replicate();
    }    
}
