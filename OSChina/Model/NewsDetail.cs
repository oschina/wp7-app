/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */

namespace OSChina. Model
{
    /// <summary>
    /// 新闻详情
    /// </summary>
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
    /// <summary>
    /// 相关新闻
    /// </summary>
    public sealed class RelativeNews
    {
        public string title;
        public string url;
    }
}
