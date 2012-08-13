
using System. Windows;
using OSChina. Model. AppOnly;

namespace OSChina. Model
{
    public sealed class TweetUnit : BaseUnit
    {
        private string _portrait;
        public string portrait
        {
            get { return _portrait. IsNullOrWhitespace( ) ? "/Resource/avatar_noimg.jpg" : _portrait; }
            set { _portrait = value; }
        }
        public string author { get; set; }
        public string authorFormat 
        {
            get { return string. Format( "{0}:", author ); }
            set { ; }
        }
        public int authorID { get; set; }
        public string body { get; set; }
        public int commentCount { get; set; }
        public int appClient { get; set; }
        public string pubDate { get; set; }
        public string pubDateFormat
        {
            get { return string. Format( "{0} {1}", Tool. IntervalSinceNow( pubDate ), Tool. GetAppClientString( (AppClientType)appClient ) ); }
            set { ; }
        }
        public string imgSmall { get; set; }
        public Visibility imgSmallVisible
        {
            get { return imgSmall. IsNullOrWhitespace( ) ? Visibility. Collapsed : Visibility. Visible; }
            set { ; }
        }
        public string imgBig { get; set; }
    }
}
