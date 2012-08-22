
namespace OSChina. Model
{
    /// <summary>
    /// 留言列表单元
    /// </summary>
    public sealed class MsgUnit : BaseUnit
    {
        private string _portrait;
        public string portrait
        {
            get { return _portrait. IsNullOrWhitespace( ) ? "/Resource/avatar_noimg.jpg" : _portrait; }
            set { _portrait = value; }
        }
        public int friendID { get; set; }
        public string friendName { get; set; }
        public string sender { get; set; }
        public int senderID { get; set; }
        public string content { get; set; }
        public int messageCount { get; set; }
        public string pubDate { get; set; }

        public string date
        {
            get { return Tool. IntervalSinceNow( pubDate ); }
            set { ; }
        }

        public string source
        {
            get { return string. Format( "{0} {1}", senderID == Config. UID ? "发给" : "来自", friendName ); }
            set { ; }
        }
    }
}
