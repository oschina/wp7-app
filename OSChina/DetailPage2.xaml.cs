using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Documents;
using System. Windows. Input;
using System. Windows. Media;
using System. Windows. Media. Animation;
using System. Windows. Shapes;
using Microsoft. Phone. Controls;
using WP7_WebLib. HttpGet;
using OSChina. Model. AppOnly;
using OSChina. Model;
using Microsoft. Phone. Shell;
using Microsoft. Phone. Tasks;
using Coding4Fun. Phone. Controls;
using WP7_WebLib. HttpPost;
using WP7_ControlsLib. Controls;
using System. Windows. Navigation;
using OSChina. Controls;

namespace OSChina
{
    public partial class DetailPage2 : PhoneApplicationPage
    {
          #region 属性
        private DetailType DetailType { get; set; }
        private NewsDetail newsDetail;
        private PostDetail postDetail;
        private BlogDetail blogDetail;
        private SoftwareDetail softwareDetail;
        private TweetDetail tweetDetail;
        private PopUpImage pop;
        #endregion

        public DetailPage2( )
        {
            InitializeComponent( );
            this. browser. NavigateToString( ControlHelper. ConvertExtendedASCII( string. Format( "<div style='font-size:100px;text-align:center;'>正在加载 . . .</div>" ) ) );
            //点击网页链接的处理
            this. browser. Navigating += new EventHandler<NavigatingEventArgs>( (s, e) =>
                {
                    switch ( e. Uri. AbsoluteUri )
                    {
                        case "http://www.wangjuntom.com/":
                            if ( this. NavigationService. BackStack. Select( b => b. Source. OriginalString. Contains( "DetailPage2.xaml" ) ).Count() > 1 )
                            {
                                Tool. To( "/MainPage.xaml" );
                            }
                            else
                            {
                                if ( this. NavigationService. CanGoBack )
                                {
                                    this. NavigationService. GoBack( );
                                }
                                else
                                {
                                    Tool. To( "/MainPage.xaml" );
                                }
                            }
                            e. Cancel = true;
                            break;
                        //大图展示
                        case "http://www.wangjuntomtweetimg.com/":
                            //查看大图
                            this. pop = new PopUpImage( );
                            this. pop. Create( this. tweetDetail. imBig, 730 );
                            this. IsEnabled = false;
                            this. pop. pop. Closed += (s1, e1) =>
                            {
                                this. IsEnabled = true;
                            };
                            e. Cancel = true;
                            break;
                        default:
                            Tool. ProcessAppLink( e. Uri. AbsoluteUri );
                            break;
                    }
                } );
        }



        protected override void OnBackKeyPress(System. ComponentModel. CancelEventArgs e)
        {
            if ( this. pop != null && this. pop. pop. IsOpen )
            {
                this. pop. pop. IsOpen = false;
                this. IsEnabled = true;
                e. Cancel = true;
            }
            else
            {
                base. OnBackKeyPress( e );
            }
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            /*
             * 因为所有的 文章详情都是 httpGet 操作，所以传递的 e.uri 我们拆分出 uri 就可以获取数据了
             * 当然还需要 type 参数代表文章的类型 这个用于 xml 反序列化的结果分析
             */
            if ( this.NavigationContext.QueryString.ContainsKey("id") && this.NavigationContext.QueryString.ContainsKey("type") )
            {
                this. DetailType = ( DetailType ) this. NavigationContext. QueryString[ "type" ]. ToInt32( );
                //如果是软件类型 则去除掉一些东西
                if ( this.DetailType == Model.AppOnly.DetailType.Software )
                {
                    this. LayoutRoot. Children. Remove( this. pivot );
                    this. browser = new WebBrowser( ) { Height=772, VerticalAlignment= System.Windows.VerticalAlignment.Top };
                    this. LayoutRoot. Children. Add( this. browser );
                }
                string url = this. GetDetailUrl( this. NavigationContext. QueryString[ "id" ], this. DetailType );
                url = string. Format( "{0}&guid={1}", url, Guid. NewGuid( ). ToString( ) );
                //下载文章
                this. DownloadDocument( url );
                //下载评论列表
                this. DownloadComments( this. NavigationContext. QueryString[ "id" ]. ToInt32( ) );
                //显示标题
                this. DisplayPivotItem( );
            }
            base. OnNavigatedTo( e );
        }

        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        /// <summary>
        /// 下载文章详情
        /// </summary>
        private void DownloadDocument(string uri)
        {
            WebClient proxy = Tool. SendWebClient( uri, new Dictionary<string, string>( ) );
            proxy. DownloadStringCompleted += (s, e) =>
                {
                    if ( e. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "获取 {0} 网络错误: {0}", this. DetailType. ToString( ), e. Error. Message );
                        return;
                    }
                };
            switch ( this.DetailType )
            {
                case DetailType. News:
                    proxy.DownloadStringCompleted += (s, e)=>
                        {
                            NewsDetail n = Tool. GetNewsDetail( e. Result );
                            if ( n != null )
                            {
                                this. newsDetail = n;
                                this. browser. NavigateToString( Tool. ProcessHTMLString( Tool. HtmlProcess_News( n ) ) );
                                this. CheckApplicationBarByDetailType( );
                                this. lblCommentCount. Text = string. Format( "评论({0})", this. newsDetail. commentCount );
                            }
                        };
                    break;
                case DetailType. Post:
                    proxy. DownloadStringCompleted += (s, e) =>
                        {
                            PostDetail p = Tool. GetPostDetail( e. Result );
                            if ( p != null )
                            {
                                this. postDetail = p;
                                this. browser. NavigateToString( Tool. ProcessHTMLString( Tool. HtmlProcess_Post( p ) ) );
                                this. CheckApplicationBarByDetailType( );
                                this. lblCommentCount. Text = string. Format( "评论({0})", this. postDetail. answerCount );
                            }
                        };
                    break;
                case DetailType. Blog:
                    proxy. DownloadStringCompleted += (s, e) =>
                        {
                            BlogDetail b = Tool. GetBlogDetail( e. Result );
                            if ( b != null )
                            {
                                this. blogDetail = b;
                                this. browser. NavigateToString( Tool. ProcessHTMLString( Tool. HtmlProcess_Blog( b ) ) );
                                this. CheckApplicationBarByDetailType( );
                                this. lblCommentCount. Text = string. Format( "评论({0})", this. blogDetail. commentCount );
                                //这里特别注意 要获取一下owneruid
                                this. commentListControl. owneruid = this. blogDetail. authorID. ToString( );
                            }
                        };
                    break;
                case DetailType. Software:
                    proxy. DownloadStringCompleted += (s, e) =>
                        {
                            SoftwareDetail soft = Tool. GetSoftwareDetail( e. Result );
                            if ( soft != null )
                            {
                                this. softwareDetail = soft;
                                this. browser. NavigateToString( Tool. ProcessHTMLString( Tool. HtmlProcess_Software( soft ) ) );
                                this. CheckApplicationBarByDetailType( );
                            }
                        };
                    break;
                case DetailType. Tweet:
                    proxy. DownloadStringCompleted += (s, e) =>
                        {
                            TweetDetail t = Tool. GetTweetDetail( e. Result );
                            if ( t != null )
                            {
                                this. tweetDetail = t;
                                this. browser. NavigateToString( Tool. ProcessHTMLString( Tool. HtmlProcess_Tweet( t )) );
                                this. CheckApplicationBarByDetailType( );
                                this. lblCommentCount. Text = string. Format( "评论({0})", this. tweetDetail. commentCount );
                            }
                        };
                    break;
            }
            GC. Collect( );
        }
        /// <summary>
        /// 下载所有评论列表
        /// </summary>
        /// <param name="id">评论的id</param>
        private void DownloadComments( int id )
        {
            this. commentListControl. id = id;
            switch ( this.DetailType )
            {
                case DetailType. News:
                    this. commentListControl. catalog = ( int ) CommentType. News;
                    break;
                case DetailType. Post:
                    this. commentListControl. catalog = ( int ) CommentType. Post;
                    break;
                case DetailType. Tweet:
                    this. commentListControl. catalog = ( int ) CommentType. Tweet;
                    break;
                case DetailType. Blog:
                    this. commentListControl. catalog = ( int ) CommentType. Blog;
                    break;
            }
            //开始下载
            this. commentListControl. listBoxHelper_ReloadDelegate( );
        }

        /// <summary>
        /// 显示正确的 pivot 标题
        /// </summary>
        private void DisplayPivotItem( )
        {
            switch ( this.DetailType )
            {
                case DetailType. News:
                    this. lblDocumentTitle. Text = "资讯详情";
                    break;
                case DetailType. Post:
                    this. lblDocumentTitle. Text = "问答详情";
                    break;
                case DetailType. Blog:
                    this. lblDocumentTitle. Text = "博客详情";
                    break;
                case DetailType. Tweet:
                    this. lblDocumentTitle. Text = "动弹详情";
                    break;
            }
        }

        #region 底部按钮处理
        private void icon_Pin1_Click(object sender, EventArgs e)
        {
            string title, author,  url ,id;
            int commentCount;
            if (GetUrl_Title_Author_CommentCount(out id, out url, out title, out author, out commentCount))
	        {
                Tool. CreateTile( id, this. DetailType, title, author, commentCount );
	        }
        }

        private void icon_Comment1_Click(object sender, EventArgs e)
        {
            if ( this.IsEnabled == false )
            {
                return;
            }
            if ( this. DetailType != Model. AppOnly. DetailType. News && Tool. CheckLogin( ) == false )
            {
                //无法发表评论
            }
            else
            {
                string title, author, url, id;
                int commentCount;
                if ( GetUrl_Title_Author_CommentCount( out id, out url, out title, out author, out commentCount ) )
                {
                    InputPrompt input = new InputPrompt
                    {
                        Title = "发表评论",
                        Message = string. Format( "对 {0}", title ),
                        IsCancelVisible = true, 
                         IsSubmitOnEnterKey = false,
                    };
                    //验证缓存
                    Dictionary<string, string> cacheCommentPub = Config. Cache_CommentPub;
                    input. Value = cacheCommentPub. ContainsKey( string. Format( "{0}_{1}", this. DetailType, id ) ) ? cacheCommentPub[ string. Format( "{0}_{1}", this. DetailType, id ) ] : string. Empty;
                    input. OnValueChanged += (s, e1) =>
                        {
                            Dictionary<string, string> cacheCommentsPub = Config. Cache_CommentPub;
                            cacheCommentsPub[ string. Format( "{0}_{1}", this. DetailType, id ) ] = e1. Text;
                            Config. Cache_CommentPub = cacheCommentsPub;
                        };
                    if ( this.DetailType == Model.AppOnly.DetailType.Tweet )
                    {
                        input. Message = string. Empty;
                    }
                    input. Completed += (s, e1) =>
                        {
                            if ( e1.PopUpResult == PopUpResult.Ok )
                            {
                                if ( e1.Result.Length == 0 )
                                {
                                    MessageBox. Show( "发表的评论内容不能为空" );
                                    return;
                                }
                                Dictionary<string, object> parameters = null;
                                if ( this. DetailType != Model. AppOnly. DetailType. Blog )
                                {
                                    parameters = new Dictionary<string, object>
                                        {
                                            {"catalog", this.GetPubCommentType},
                                            {"id", id},
                                            {"uid", Config.UID},
                                            {"content", e1.Result},
                                            //{"isPostToMyZone", this.DetailType == CommentType.Tweet ? ((bool)this.checkResendToZone.IsChecked ? "1" : "0" ): "0"},
                                            {"isPostToMyZone","1"},
                                        };
                                }
                                else
                                {
                                    parameters = new Dictionary<string, object>
                                        {
                                            {"blog", id},
                                            {"uid", Config.UID},
                                            {"content", e1.Result},
                                        };
                                }
                                PostClient client = Tool. SendPostClient( this. DetailType == Model. AppOnly. DetailType. Blog ? Config. api_blogcomment_pub : Config. api_comment_pub, parameters );
                                client. DownloadStringCompleted += (s1, e2) =>
                                    {
                                        if ( e2. Error != null )
                                        {
                                            System. Diagnostics. Debug. WriteLine( "发表评论时网络错误: {0}", e2. Error. Message );
                                            return;
                                        }
                                        ApiResult result = Tool. GetApiResult( e2. Result );
                                        if ( result != null )
                                        {
                                            switch ( result. errorCode )
                                            {
                                                case 1:
                                                    //进入评论列表页  改变 pivot 的 selectIndex
                                                    if ( Config.Cache_CommentPub.ContainsKey(string.Format("{0}_{1}", this.DetailType, id)) )
                                                    {
                                                        Config. Cache_CommentPub. Remove( string. Format( "{0}_{1}", this. DetailType, id ) );
                                                    }
                                                    this. icon_Refresh_Click( null, null );
                                                    if ( this.pivot.SelectedIndex != 1 )
                                                    {
                                                        this. pivot. SelectedIndex = 1;
                                                    }
                                                    break;
                                                case 0:
                                                case -1:
                                                case -2:
                                                    MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                                    break;
                                            }
                                        }
                                    };
                            }
                        };
                    input. Show( );
                }
            }
        }

        private void icon_Refresh_Click(object sender, EventArgs e)
        {
            if ( this.IsEnabled == false )
            {
                return;
            }
            if ( this. commentListControl != null )
            {
                this. commentListControl. listBoxHelper. Refresh( );
            }
        }

        private void icon_Favorite1_Click(object sender, EventArgs e)
        {
            if ( Tool.CheckLogin() == false)
            {
                return;
            }
            string id, url, title, author;
            int commentCount;
            if ( GetUrl_Title_Author_CommentCount(out id, out url, out title, out author, out commentCount) )
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"objid", id},
                    {"type", GetFavoriteOperationType()},
                };
                WebClient client = Tool. SendWebClient( Config. api_fav_add, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                    {
                        if ( e1. Error != null )
                        {
                            System. Diagnostics. Debug. WriteLine( "favorite-add 网络错误: {0}", e1. Error. Message );
                            return;
                        }
                        else
                        {
                            ApiResult result = Tool. GetApiResult( e1. Result );
                            switch ( result.errorCode )
                            {
                                case 1:
                                    switch (this.DetailType)
	                                {
                                        case DetailType. News:
                                        case DetailType. Post:
                                        case DetailType. Blog:
                                            this. ApplicationBar = this. Resources[ "bar_news_nofavorite" ] as ApplicationBar;
                                            break;
                                        case DetailType. Software:
                                            this. ApplicationBar = this. Resources[ "bar_software_nofavorite" ] as ApplicationBar;
                                            break;
	                                }
                                    break;
                                case 0:
                                case -1:
                                case -2:
                                    MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                    break;
                            }
                        }
                    };
            }
        }

        private void icon_NoFavorite2_Click(object sender, EventArgs e)
        {
            if ( Tool. CheckLogin( ) == false )
            {
                return;
            }
            string id, url, title, author;
            int commentCount;
            if ( GetUrl_Title_Author_CommentCount( out id, out url, out title, out author, out commentCount ) )
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"objid", id},
                    {"type", GetFavoriteOperationType()},
                };
                WebClient client = Tool. SendWebClient( Config. api_fav_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "favorite-delete 网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e1. Result );
                        switch ( result. errorCode )
                        {
                            case 1:
                                switch ( this. DetailType )
                                {
                                    case DetailType. News:
                                    case DetailType. Post:
                                    case DetailType. Blog:
                                        this. ApplicationBar = this. Resources[ "bar_news_favorite" ] as ApplicationBar;
                                        break;
                                    case DetailType. Software:
                                        this. ApplicationBar = this. Resources[ "bar_software_favorite" ] as ApplicationBar;
                                        break;
                                }
                                break;
                            case 0:
                            case -1:
                            case -2:
                                MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                break;
                        }
                    }
                };
            }
        }

        private void menuItem_ShareSina1_Click(object sender, EventArgs e)
        {
            string title, author, url, id;
            int commentCount;
            if ( GetUrl_Title_Author_CommentCount( out id, out url, out title, out author, out commentCount ) )
            {
                string share_url = "http://v.t.sina.com.cn/share/share.php";
                string all = string. Format( "{0}?appkey={1}&title={2}&&url={3}", share_url, Config. SinaKey, title, url );
                WebBrowserTask task = new WebBrowserTask
                {
                    Uri = new Uri( all, UriKind. Absolute )
                };
                task. Show( );
            }
        }

        private void menuItem_ShareQQ1_Click(object sender, EventArgs e)
        {
            string title, author, url, id;
            int commentCount;
            if ( GetUrl_Title_Author_CommentCount( out id, out url, out title, out author, out commentCount ) )
            {
                string share_url = "http://share.v.t.qq.com/index.php?c=share&a=index";
                string all = string. Format( "{0}&title={1}&url={2}&appkey={3}&source={4}&site={5}", share_url, title, url, Config. TencentKey, "OSChina", "OSChina.NET" );
                WebBrowserTask task = new WebBrowserTask
                {
                    Uri = new Uri( all, UriKind. Absolute )
                };
                task. Show( );
            }
        }

        private bool GetUrl_Title_Author_CommentCount( out string id, out string url, out string title, out string author, out int commentCount )
        {
            commentCount = 0;
            url = title = author = id = null;
            switch ( this.DetailType )
            {
                case DetailType. News:
                    if ( this. newsDetail != null )
                    {
                        url = this. newsDetail. url;
                        title = this. newsDetail. title;
                        author = this. newsDetail. author;
                        commentCount = this. newsDetail. commentCount;
                        id = this. newsDetail. id. ToString( );
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DetailType. Post:
                    if ( this. postDetail != null )
                    {
                        url = this. postDetail. url;
                        title = this. postDetail. title;
                        author = this. postDetail. author;
                        commentCount = this. postDetail. answerCount;
                        id = this. postDetail. id. ToString( );
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DetailType. Blog:
                    if ( this. blogDetail != null )
                    {
                        url = this. blogDetail. url;
                        title = this. blogDetail. title;
                        author = this. blogDetail. author;
                        commentCount = this. blogDetail. commentCount;
                        id = this. blogDetail. id. ToString( );
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DetailType. Software:
                    if ( this. softwareDetail != null )
                    {
                        id = this. softwareDetail. id. ToString( );
                        url = this. softwareDetail. url;
                        title = this. softwareDetail. title;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case DetailType. Tweet:
                    if ( this. tweetDetail != null )
                    {
                        id = this. tweetDetail. id. ToString( );
                        author = this. tweetDetail. author;
                        commentCount = this. tweetDetail. commentCount;
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }

        private string GetFavoriteOperationType( )
        {
            switch ( this.DetailType )
            {
                case DetailType. News:
                    return "4";
                case DetailType. Post:
                    return "2";
                case DetailType. Blog:
                    return "3";
                case DetailType. Software:
                    return "1";
                default:
                    return string. Empty;
            }
        }

        private string GetPubCommentType
        {
            get 
            {
                switch ( this.DetailType )
                {
                    case DetailType. News:
                        return "1";
                    case DetailType. Post:
                        return "2";
                    case DetailType. Tweet:
                        return "3";
                    case DetailType. Blog:
                    case DetailType. Software:
                    default:
                        return "0";
                }
            }
        }

        private string GetDetailUrl(string id, DetailType type)
        {
            switch ( type )
            {
                case DetailType. News:
                    return string. Format( "{0}?id={1}", Config. api_news_detail, id );
                case DetailType. Post:
                    return string. Format( "{0}?id={1}", Config. api_post_detail, id );
                case DetailType. Blog:
                    return string. Format( "{0}?id={1}", Config. api_blog_detail, id );
                case DetailType. Software:
                    return string. Format( "{0}?ident={1}", Config. api_software_detail, id );
                case DetailType. Tweet:
                    return string. Format( "{0}?id={1}", Config. api_tweet_detail, id );
            }
            return null;
        }

        private void CheckApplicationBarByDetailType( )
        {
            switch ( this. DetailType )
            {
                case DetailType. News:
                    this. ApplicationBar = this. newsDetail. favorite ? this. Resources[ "bar_news_nofavorite" ] as ApplicationBar : this. Resources[ "bar_news_favorite" ] as ApplicationBar;
                    break;
                case DetailType. Post:
                    this. ApplicationBar = this. postDetail. favorite ? this. Resources[ "bar_news_nofavorite" ] as ApplicationBar : this. Resources[ "bar_news_favorite" ] as ApplicationBar;
                    break;
                case DetailType. Blog:
                    this. ApplicationBar = this. blogDetail. favorite ? this. Resources[ "bar_news_nofavorite" ] as ApplicationBar : this. Resources[ "bar_news_favorite" ] as ApplicationBar;
                    break;
                case DetailType. Software:
                    this. ApplicationBar = this. softwareDetail. favorite ? this. Resources[ "bar_software_nofavorite" ] as ApplicationBar : this. Resources[ "bar_software_favorite" ] as ApplicationBar;
                    break;
                case DetailType. Tweet:
                    this. ApplicationBar = this. Resources[ "bar_tweet" ] as ApplicationBar;
                    break;
            }
        }
        #endregion
    }
}