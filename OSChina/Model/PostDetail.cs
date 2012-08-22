
namespace OSChina. Model
{
    /// <summary>
    /// 问答详情
    /// </summary>
    public sealed class PostDetail
    {
        public int id;
        public string title;
        public string url;
        public string portrait;
        public string body;
        public string author;
        public int authorID;
        public int answerCount;
        public int viewCount;
        public string pubDate;
        public bool favorite;

        public string[ ] tags;
    }
}
