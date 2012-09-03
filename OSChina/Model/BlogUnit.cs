/*
 * 原作者: 王俊
 * 
 */

namespace OSChina. Model
{
    /// <summary>
    /// 博客列表单元
    /// </summary>
    public sealed class BlogUnit : BaseUnit
    {
        public string url { get; set; }
        public string title { get; set; }
        public string pubDate { get; set; }
        public string authorName { get; set; }
        public int authorUID { get; set; }
        public string outline
        {
            get { return string. Format( "{0} {1} {2} ({3}评)", authorName, documentType ? "原创" : "转载", Tool. IntervalSinceNow( pubDate ), commentCount ); }
            set { ; }
        }
        public int commentCount { get; set; }
        public bool documentType { get; set; }
    }
}
