
namespace OSChina. Model
{
    /// <summary>
    /// 搜索列表单元
    /// </summary>
    public sealed class SearchUnit : BaseUnitWithType
    {
        public string title { get; set; }
        public string url { get; set; }
        public string pubDate { get; set; }
        public string author { get; set; }
        public string outline
        {
            get
            {
                return author. IsNullOrWhitespace( ) ? string. Empty : string. Format( "{0} 发表于 {1}", author, Tool. IntervalSinceNow( pubDate ) );
            }
            set { ; }
        }
    }
}
