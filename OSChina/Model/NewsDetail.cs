
namespace OSChina. Model
{
    public sealed class NewsDetail
    {
        public int id;
        public string title;
        public string url;
        public string body;
        public string author;
        public int authorID;
        public int commentCount;
        public string pubDate;
        public string softwareLink;
        public string softwareName;
        public bool favorite;
        public RelativeNews[ ] relativies;
    }

    public sealed class RelativeNews
    {
        public string title;
        public string url;
    }
}
