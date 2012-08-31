/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System. Windows;
using Coding4Fun. Phone. Controls;

namespace OSChina. Model
{
    /// <summary>
    /// 评论列表单元
    /// </summary>
    public sealed class CommentUnit : BaseUnit
    {
        private string _portrait;
        public string portrait
        {
            get { return _portrait. IsNullOrWhitespace( ) ? "/Resource/avatar_noimg.jpg" : _portrait; }
            set { _portrait = value; }
        }
        public string author { get; set; }
        public int authorID { get; set; }
        public string content { get; set; }
        public string pubDate { get; set; }
        public int appClientType { get; set; }
        //public int appClientType { get { return 3; } set { ; } }
        public string appClientString
        {
            get { return Tool. GetAppClientString( ( AppOnly. AppClientType ) appClientType ); }
            set { ; }
        }
        public Visibility appClientVisible
        {
            get { return appClientType != 0 && appClientType != 1 ? Visibility. Visible : Visibility. Collapsed; }
            set { ; }
        }
        public CommentReply[ ] replies { get; set; }
        public CommentRefer[ ] refers { get; set; }

        public string source
        {
            get { return string. Format( "{0} 发表于{1}", author, Tool. IntervalSinceNow( pubDate ) ); }
            set { ; }
        }

        public string time
        {
            get { return Tool. IntervalSinceNow( pubDate ); }
            set { ; }
        }

        public double opactiy
        {
            get { return authorID == Config. UID ? 0.75: 1; }
            set { ; }
        }

        public Thickness border
        {
            get { return authorID == Config. UID ? new Thickness( 100, 11, 0, 0 ) : new Thickness( 12,0,0,11 ); }
            set { ; }
        }

        //public Thickness updown
        //{
        //    get { return authorID == Config.UID ? new Thickness() }
        //}

        public ChatBubbleDirection direction
        {
            get { return authorID == Config. UID ? ChatBubbleDirection. LowerRight : ChatBubbleDirection. UpperLeft; }
            set { ; }
        }
    }

    /// <summary>
    /// 评论回复
    /// </summary>
    public sealed class CommentReply
    {
        public string author { get; set; }
        public string pubDate { get; set; }
        public string content { get; set; }
    }

    /// <summary>
    /// 评论引用
    /// </summary>
    public sealed class CommentRefer
    {
        public string title { get; set; }
        public string body { get; set; }
    }
}
