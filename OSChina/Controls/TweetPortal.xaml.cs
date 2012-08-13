using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Documents;
using System. Windows. Input;
using System. Windows. Media;
using System. Windows. Media. Imaging;
using System. Windows. Media. Animation;
using System. Windows. Shapes;
using System. Threading;
using System. Windows. Threading;
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_WebLib. HttpGet;

namespace OSChina. Controls
{
    public partial class TweetPortal : UserControl
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 当前显示的动弹 id
        /// </summary>
        public int tweetID { get; set; }
        private TweetUnit currentTweet { get; set; }

        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        public TweetPortal( )
        {
            InitializeComponent( );
            this. LayoutRoot. Loaded += (s, e) =>
                {
                    //启动动态刷新当前显示动弹的线程
                    if ( this.timer1 == null )
                    {
                        this. timer1 = new DispatcherTimer { Interval = TimeSpan. FromSeconds( this.Type == "Latest" ? 4.2 : 6.9 ) };
                        this. timer1. Tick += new EventHandler( Display );
                        timer1. Start( );
                        this. Display( null, null );
                    }
                    switch ( this.Type )
                    {
                        case "Latest":
                            this. LayoutRoot. Background = new SolidColorBrush( Color. FromArgb( 255, 100, 149, 237 ) );
                            this. lblComment. Foreground = this. lblContent. Foreground = this. lblName. Foreground = new SolidColorBrush( Colors. White );
                            //this. lblTweetType. Text = "最新动弹";
                            break;
                        case "Hottest":
                            this. LayoutRoot. Background = new SolidColorBrush( Color. FromArgb( 255, 153, 204, 50 ) );
                            this. lblComment. Foreground = this. lblContent. Foreground = this. lblName. Foreground = new SolidColorBrush( Colors. Black );
                            //this. lblTweetType. Text = "热门动弹";
                            //this. gridTweetType. Background = new SolidColorBrush( Color. FromArgb( 255, 100, 149, 237 ) );
                            break;
                    }
                    //启动获取动弹集合的线程
                    if ( this.timer2 == null )
                    {
                        this. timer2 = new DispatcherTimer { Interval = this. Type == "Latest" ? TimeSpan. FromMinutes( 2 ) : TimeSpan. FromMinutes( 20 ) };
                        this. timer2. Tick += (s1, e1) =>
                            {
                                AnsycGetHottestTweets( this. Type == "Latest" ? true : false );
                            };
                        this. timer2. Start( );
                        AnsycGetHottestTweets( this. Type == "Latest" ? true : false );
                    }
                };
        }

        private void Display(object sender, EventArgs e)
        {
            switch ( this. Type )
            {
                case "Latest":
                    this. GetNextTweetUnit( Config. tweetsLatest );
                    break;
                case "Hottest":
                    this. GetNextTweetUnit( Config. tweetsHottest );
                    break;
            }
        }

        private void GetNextTweetUnit( TweetUnit[ ] tweets )
        {
            if ( tweets != null && tweets.Length > 0 )
            {
                TweetUnit t = tweets[ 0 ];
                for ( int i=0 ;i < tweets.Length ; i++ )
                {
                    if ( tweets[i]. id == tweetID )
                    {
                        //返回下一项
                        t = i < tweets. Length - 1 ? tweets[ i + 1 ] : tweets[ 0 ];
                        break;
                    }
                }
                //获取下一项
                this. tweetID = t. id;
                
                this. DisplayTweet( t );
            }
        }

        private void DisplayTweet( TweetUnit t )
        {
            this. currentTweet = t;
            //开始关闭动画 (建议 Fade)
            this. fadeOut. Begin( );
            this. fadeOut. Completed += (s, e) =>
                {
                    //然后控件转变 
                    this. lblName. Text = string. Format( "{0}:", t. author );
                    this. lblContent. Text = t. body;
                    this. lblComment. Text = string. Format( "{0}  {1}评", t. pubDateFormat, t. commentCount );
                    this. imgPortrait. Source = new BitmapImage( new Uri( t. portrait. IsNotNullOrWhitespace( ) ? t. portrait : "/Resource/avatar_noimg.jpg", UriKind. RelativeOrAbsolute ) );
                    //最后执行上移动画
                    this. fadeIn. Begin( );
                };
        }



        #region 异步获取动弹
        public static void AnsycGetHottestTweets( bool isLatest)
        {
            WebClient client = Tool. SendWebClient( Config. api_tweet_list, new Dictionary<string, string>( )
            {
                {"uid", isLatest ?  "0" : "-1"},
                {"pageIndex", "0"},
                {"pageSize", "20"},
            } );
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "首次获取动弹时网络错误: {0}", e. Error. Message );
                        return;
                    }
                    else
                    {
                        int pageSize, tweetCount;
                        TweetUnit[ ] newTweets = Tool. GetTweetList( e. Result, out pageSize, out tweetCount );
                        if ( isLatest )
                        {
                            Config. tweetsLatest = newTweets;
                        }
                        else
                        {
                            Config. tweetsHottest = newTweets;
                        }
                    }
                };
        }
        #endregion

        #region 点击头像后进入个人专页
        private void imgPortrait_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ( this.currentTweet != null )
            {
                Tool. ToUserPage( this. currentTweet. authorID );
            }
        }
        #endregion
    }
}
