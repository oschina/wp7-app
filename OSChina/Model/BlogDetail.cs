
namespace OSChina. Model
{
    /// <summary>
    /// 博客详情
    /// </summary>
    public sealed class BlogDetail
    {
        public int id;
        public string title;
        public string url;
        public string where;
        public int commentCount;
        public string body;
        public string author;
        public int authorID;
        public int documentType;
        public string pubDate;
        public bool favorite;
    }
}
