
using OSChina. Model. AppOnly;

namespace OSChina. Model
{
    public sealed class TweetDetail
    {
        public int id;
        public string body;
        public string author;
        public int authorID;
        public int commentCount;
        public string portrait;
        public string pubDate;
        public string imgSmall;
        public string imBig;
        public AppClientType appClient;
    }
}
