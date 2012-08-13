namespace OSChina. Model
{
    public sealed class ActiveUnit : BaseUnit
    {
        private string _portrait;
        public string portrait
        {
            get { return _portrait. IsNullOrWhitespace( ) ? "/Resource/avatar_noimg.jpg" : _portrait; }
            set { _portrait = value; }
        }
        public string author { get; set; }
        public int authorID { get; set; }
        public int catalog { get; set; }
        public int objID { get; set; }
        public int objType { get; set; }
        public int objCatalog { get; set; }
        public string objTitle { get; set; }
        public string objReplyName { get; set; }
        public string objReplyBody { get; set; }
        public int appClient { get; set; }
        public string message { get; set; }
        public int commentCount { get; set; }
        public string pubDate { get; set; }
        public string tweetImage { get; set; }
        public string url { get; set; }
    }
}
