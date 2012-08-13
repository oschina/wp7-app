
namespace OSChina. Model
{
    public sealed class NewsUnit : BaseUnit
    {
        public string title { get; set; }
        public int commentCount { get; set; }
        public string author { get; set; }
        public int authorID { get; set; }
        public string pubDate { get; set; }
        public string url { get; set; }
        public NewsType newsType { get; set; }

        public string outline
        {
            get { return string. Format( "{0} 发布于 {1} ({2}评)", author, Tool. IntervalSinceNow( pubDate ), commentCount ); }
            set { ;}
        }
    }
    public sealed class NewsType
    {
        public int type { get; set; }
        public string attachment { get; set; }
        public int authorID { get; set; }
    }
}
