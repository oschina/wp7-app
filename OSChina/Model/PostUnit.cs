/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */

namespace OSChina. Model
{
    /// <summary>
    /// 问答列表单元
    /// </summary>
    public sealed class PostUnit : BaseUnit
    {
        private string _portrait;
        public string portrait
        {
            get { return this. _portrait; }
            set 
            {
                if ( value. IsNullOrWhitespace( ) )
                {
                    _portrait = "/Resource/avatar_noimg.jpg";
                }
                else
                    _portrait = value;
            }
        }
        public string author { get; set; }
        public int authorID { get; set; }
        public string title { get; set; }
        public int answerCount { get; set; }
        public int viewCount { get; set; }
        public string pubDate { get; set; }
        public AnswerUnit answer { get; set; }

        public string outline
        {
            get { return string. Format( "{0} 发布于{1}", author, Tool. IntervalSinceNow( pubDate ) ); }
            set { ; }
        }
    }

    public sealed class AnswerUnit
    {
        public string name { get; set; }
        public string time { get; set; }
    }
}
