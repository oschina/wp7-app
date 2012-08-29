/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */

//#define local
#define oschina

using System;
using System. Collections. Generic;
using System. IO;
using System. IO. IsolatedStorage;
using Microsoft. Phone. Info;
using Microsoft. Phone. Shell;
using OSChina. Model;

namespace OSChina
{
    /// <summary>
    /// App 配置信息静态类
    /// </summary>
    public static class Config
    {
        #region Api 地址
#if oschina
        public const string api_news_list = "http://www.oschina.net/action/api/news_list";
        public const string api_news_detail = "http://www.oschina.net/action/api/news_detail";
        public const string api_software_detail = "http://www.oschina.net/action/api/software_detail";
        public const string api_blog_detail = "http://www.oschina.net/action/api/blog_detail";
        public const string api_post_list = "http://www.oschina.net/action/api/post_list";
        public const string api_post_detail = "http://www.oschina.net/action/api/post_detail";
        public const string api_post_pub = "http://www.oschina.net/action/api/post_pub";
        public const string api_tweet_list = "http://www.oschina.net/action/api/tweet_list";
        public const string api_tweet_detail = "http://www.oschina.net/action/api/tweet_detail";
        public const string api_tweet_pub = "http://www.oschina.net/action/api/tweet_pub";
        public const string api_tweet_delete = "http://www.oschina.net/action/api/tweet_delete";
        public const string api_active_list = "http://www.oschina.net/action/api/active_list";
        public const string api_msg_list = "http://www.oschina.net/action/api/message_list";
        public const string api_msg_delete = "http://www.oschina.net/action/api/message_delete";
        public const string api_msg_pub = "http://www.oschina.net/action/api/message_pub";
        public const string api_comment_list = "http://www.oschina.net/action/api/comment_list";
        public const string api_comment_pub = "http://www.oschina.net/action/api/comment_pub";
        public const string api_comment_reply = "http://www.oschina.net/action/api/comment_reply";
        public const string api_comment_delete = "http://www.oschina.net/action/api/comment_delete";
        public const string api_login_validate = "https://www.oschina.net/action/api/login_validate";
        public const string api_user_info = "http://www.oschina.net/action/api/user_information";
        public const string api_user_updaterelation = "http://www.oschina.net/action/api/user_updaterelation";
        public const string api_notice_clear = "http://www.oschina.net/action/api/notice_clear";
        public const string api_fav_list = "http://www.oschina.net/action/api/favorite_list";
        public const string api_fav_add = "http://www.oschina.net/action/api/favorite_add";
        public const string api_fav_delete = "http://www.oschina.net/action/api/favorite_delete";
        public const string api_user_notice = "http://www.oschina.net/action/api/user_notice";
        public const string api_search_list = "http://www.oschina.net/action/api/search_list";
        public const string api_friends_list = "http://www.oschina.net/action/api/friends_list";
        public const string api_softwarecatalog_list = "http://www.oschina.net/action/api/softwarecatalog_list";
        public const string api_softwaretag_list = "http://www.oschina.net/action/api/softwaretag_list";
        public const string api_software_list = "http://www.oschina.net/action/api/software_list";
        public const string api_blogcomment_list = "http://www.oschina.net/action/api/blogcomment_list";
        public const string api_blogcomment_pub = "http://www.oschina.net/action/api/blogcomment_pub";
        public const string api_blogcomment_delete = "http://www.oschina.net/action/api/blogcomment_delete";
        public const string api_my_info = "http://www.oschina.net/action/api/my_information";
        public const string api_userblog_list = "http://www.oschina.net/action/api/userblog_list";
        public const string api_userblog_delete = "http://www.oschina.net/action/api/userblog_delete";
        public const string api_blog_list = "http://www.oschina.net/action/api/blog_list";
#elif local
        public const string api_news_list = "http://192.168.1.213/action/api/news_list";
        public const string api_news_detail = "http://192.168.1.213/action/api/news_detail";
        public const string api_software_detail = "http://192.168.1.213/action/api/software_detail";
        public const string api_blog_detail = "http://192.168.1.213/action/api/blog_detail";
        public const string api_post_list = "http://192.168.1.213/action/api/post_list";
        public const string api_post_detail = "http://192.168.1.213/action/api/post_detail";
        public const string api_post_pub = "http://192.168.1.213/action/api/post_pub";
        public const string api_tweet_list = "http://192.168.1.213/action/api/tweet_list";
        public const string api_tweet_detail = "http://192.168.1.213/action/api/tweet_detail";
        public const string api_tweet_pub = "http://192.168.1.213/action/api/tweet_pub";
        public const string api_tweet_delete = "http://192.168.1.213/action/api/tweet_delete";
        public const string api_active_list = "http://192.168.1.213/action/api/active_list";
        public const string api_msg_list = "http://192.168.1.213/action/api/message_list";
        public const string api_msg_delete = "http://192.168.1.213/action/api/message_delete";
        public const string api_msg_pub = "http://192.168.1.213/action/api/message_pub";
        public const string api_comment_list = "http://192.168.1.213/action/api/comment_list";
        public const string api_comment_pub = "http://192.168.1.213/action/api/comment_pub";
        public const string api_comment_reply = "http://192.168.1.213/action/api/comment_reply";
        public const string api_comment_delete = "http://192.168.1.213/action/api/comment_delete";
        public const string api_login_validate = "http://192.168.1.213/action/api/login_validate";
        public const string api_user_info = "http://192.168.1.213/action/api/user_information";
        public const string api_user_updaterelation = "http://192.168.1.213/action/api/user_updaterelation";
        public const string api_notice_clear = "http://192.168.1.213/action/api/notice_clear";
        public const string api_fav_list = "http://192.168.1.213/action/api/favorite_list";
        public const string api_fav_add = "http://192.168.1.213/action/api/favorite_add";
        public const string api_fav_delete = "http://192.168.1.213/action/api/favorite_delete";
        public const string api_user_notice = "http://192.168.1.213/action/api/user_notice";
        public const string api_search_list = "http://192.168.1.213/action/api/search_list";
        public const string api_friends_list = "http://192.168.1.213/action/api/friends_list";
        public const string api_softwarecatalog_list = "http://192.168.1.213/action/api/softwarecatalog_list";
        public const string api_softwaretag_list = "http://192.168.1.213/action/api/softwaretag_list";
        public const string api_software_list = "http://192.168.1.213/action/api/software_list";
        public const string api_blogcomment_list = "http://192.168.1.213/action/api/blogcomment_list";
        public const string api_blogcomment_pub = "http://192.168.1.213/action/api/blogcomment_pub";
        public const string api_blogcomment_delete = "http://192.168.1.213/action/api/blogcomment_delete";
        public const string api_my_info = "http://192.168.1.213/action/api/my_information";
        public const string api_userblog_list = "http://192.168.1.213/action/api/userblog_list";
        public const string api_userblog_delete = "http://192.168.1.213/action/api/userblog_delete";
        public const string api_blog_list = "http://192.168.1.213/action/api/blog_list";
#endif
        #endregion

        #region App唯一元素
        /// <summary>
        /// App第一次生成的Guid 识别符
        /// </summary>
        public static string Guid
        {
            get 
            {
                if ( IsolatedStorageSettings. ApplicationSettings. Contains( "Guid" ) )
                {
                    return IsolatedStorageSettings. ApplicationSettings[ "Guid" ] as string;
                }
                else
                {
                    Guid guid = System. Guid. NewGuid( );
                    IsolatedStorageSettings. ApplicationSettings[ "Guid" ] = guid. ToString( );
                    IsolatedStorageSettings. ApplicationSettings. Save( );
                    return guid. ToString( );
                }
            }
        }

        /// <summary>
        /// 表示当前是否具有网路
        /// </summary>
        public static bool IsNetworkRunning
        {
            get 
            {
                return PhoneApplicationService. Current. State. ContainsKey( "IsNetworkRunning" ) ?
                    ( bool ) PhoneApplicationService. Current. State[ "IsNetworkRunning" ] : false;
            }
            set { PhoneApplicationService. Current. State[ "IsNetworkRunning" ] = value; }
        }

        /// <summary>
        /// 用户UID
        /// </summary>
        public static int UID
        {
            get 
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("uid") ?
                (int)IsolatedStorageSettings.ApplicationSettings["uid"] : 0; 
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["uid"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// 登陆用户名
        /// </summary>
        public static string LoginName
        {
            get 
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("LoginName") ?
                    IsolatedStorageSettings.ApplicationSettings["LoginName"] as string : null;
            }
            set 
            {
                IsolatedStorageSettings.ApplicationSettings["LoginName"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("Password") ?
                    IsolatedStorageSettings.ApplicationSettings["Password"] as string : null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["Password"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// Cookie
        /// </summary>
        public static string Cookie
        {
            get
            {
                return IsolatedStorageSettings.ApplicationSettings.Contains("Cookie") ?
                    IsolatedStorageSettings.ApplicationSettings["Cookie"] as string : null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["Cookie"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// 表示当前是否登陆
        /// </summary>
        public static bool IsLogin
        {
            get { return !string. IsNullOrWhiteSpace( Cookie ); }
        }

        /// <summary>
        /// UserAgent
        /// </summary>
        public static string UserAgent
        {
            get 
            {
                if ( PhoneApplicationService. Current. State. ContainsKey( "UserAgent" ) )
                {
                    return PhoneApplicationService. Current. State[ "UserAgent" ] as string;
                }
                else
                {
                    string agent = string. Format( "OSChina.NET/{0}/WP7/{1}/{2}/{3}", AppVersion, "Mango", DeviceStatus. DeviceName, Guid );
                    PhoneApplicationService. Current. State[ "UserAgent" ] = agent;
                    return agent;
                }
            }
        }

        /// <summary>
        /// 发评论时是否转发到个人空间（仅在针对动弹评论时才生效）
        /// </summary>
        public static bool IsPostToZone
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "IsPostToZone" ) ?
                    ( bool ) IsolatedStorageSettings. ApplicationSettings[ "IsPostToZone" ] : false;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "IsPostToZone" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 发帖时 是否有人回答时被邮件通知
        /// </summary>
        public static bool IsEmailNoticeMe
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "IsEmailNoticeMe" ) ?
                    ( bool ) IsolatedStorageSettings. ApplicationSettings[ "IsEmailNoticeMe" ] : false;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "IsEmailNoticeMe" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 保存的 20 个最新动弹
        /// </summary>
        public static TweetUnit[ ] tweetsLatest
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "tweetsLatest" ) ?
                    IsolatedStorageSettings. ApplicationSettings[ "tweetsLatest" ] as TweetUnit[ ] : null;
            }
            set 
            {
                IsolatedStorageSettings. ApplicationSettings[ "tweetsLatest" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 保存的 20 个热门动弹
        /// </summary>
        public static TweetUnit[ ] tweetsHottest
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "tweetsHottest" ) ?
                    IsolatedStorageSettings. ApplicationSettings[ "tweetsHottest" ] as TweetUnit[ ] : null;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "tweetsHottest" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 存储的当前用户通知
        /// </summary>
        public static UserNotice UserNoticeInPhone
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "UserNoticeInPhone" ) ?
                    PhoneApplicationService. Current. State[ "UserNoticeInPhone" ] as UserNotice : new UserNotice( );
            }
            set
            {
                PhoneApplicationService. Current. State[ "UserNoticeInPhone" ] = value;
            }
        }

        /// <summary>
        /// 存储的自我信息
        /// </summary>
        public static MyInfo MyInfo
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "MyInfo" ) ?
                    IsolatedStorageSettings. ApplicationSettings[ "MyInfo" ] as MyInfo : null;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "MyInfo" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 表示在 WebBrowser 控件是是否显示图片 与流量相关
        /// </summary>
        public static bool IsImgsVisible
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "IsImgsVisible" ) ?
                            (bool)IsolatedStorageSettings. ApplicationSettings[ "IsImgsVisible" ] : true;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "IsImgsVisible" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 表示退出App 时是否弹出退出提示框
        /// </summary>
        public static bool IsNoticeExit
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "IsNoticeExit" ) ?
                            ( bool ) IsolatedStorageSettings. ApplicationSettings[ "IsNoticeExit" ] : true;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "IsNoticeExit" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }

        /// <summary>
        /// 表示App 是否具备后台拉取的功能
        /// </summary>
        public static bool IsScheduledTask
        {
            get
            {
                return IsolatedStorageSettings. ApplicationSettings. Contains( "IsScheduledTask" ) ?
                            ( bool ) IsolatedStorageSettings. ApplicationSettings[ "IsScheduledTask" ] : true;
            }
            set
            {
                IsolatedStorageSettings. ApplicationSettings[ "IsScheduledTask" ] = value;
                IsolatedStorageSettings. ApplicationSettings. Save( );
            }
        }
        #endregion

        #region App缓存
        /// <summary>
        /// 缓存写好但没发送的动弹文字
        /// </summary>
        public static string Cache_Tweet
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_Tweet" ) ?
                    PhoneApplicationService. Current. State[ "Cache_Tweet" ]. ToString( ) : string. Empty;
            }
            set 
            {
                PhoneApplicationService. Current. State[ "Cache_Tweet" ] = value;
            }
        }

        /// <summary>
        /// 清空动弹的缓存 一般在动弹发送成功后执行
        /// </summary>
        public static void ClearCacheTweet( )
        {
            Cache_Tweet = string. Empty;
            Cache_TweetPic = null;
        }
       
        /// <summary>
        /// 缓存准备好但没法送的动弹图片
        /// </summary>
        public static Stream Cache_TweetPic
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_TweetPic" ) ?
                    PhoneApplicationService. Current. State[ "Cache_TweetPic" ] as Stream : null;
            }
            set { PhoneApplicationService. Current. State[ "Cache_TweetPic" ] = value; }
        }

        /// <summary>
        /// 缓存发布问答时 写好但没发布的标题
        /// </summary>
        public static string Cache_Question_Title
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_Question_Title" ) ?
                    PhoneApplicationService. Current. State[ "Cache_Question_Title" ]. ToString( ) : string. Empty;
            }
            set 
            {
                PhoneApplicationService. Current. State[ "Cache_Question_Title" ] = value;
            }
        }

        /// <summary>
        /// 缓存发布问答时 写好但没发布的问答类型
        /// </summary>
        public static int Cache_Question_Index
        {
            get 
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_Question_Index" ) ?
                    PhoneApplicationService. Current. State[ "Cache_Question_Index" ]. ToString( ). ToInt32( ) : 0;
            }
            set { PhoneApplicationService. Current. State[ "Cache_Question_Index" ] = value; }
        }

        /// <summary>
        /// 缓存发布问答时 写好但没发布的内容
        /// </summary>
        public static string Cache_Question_Content
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_Question_Content" ) ?
                    PhoneApplicationService. Current. State[ "Cache_Question_Content" ]. ToString( ) : string. Empty;
            }
            set 
            {
                PhoneApplicationService. Current. State[ "Cache_Question_Content" ] = value;
            }
        }

        /// <summary>
        /// 清除发布问答的缓存 一般在问答发布成功后执行
        /// </summary>
        public static void ClearCacheQuestion( )
        {
            Cache_Question_Title = string. Empty;
            Cache_Question_Index = 0;
            Cache_Question_Content = string. Empty;
        }

        /// <summary>
        /// 发布留言的缓存 针对不同的人（uid）进行保存
        /// </summary>
        public static Dictionary<int, string> Cache_Message
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_Message" ) ?
                    PhoneApplicationService. Current. State[ "Cache_Message" ] as Dictionary<int, string> : new Dictionary<int, string>( );
            }
            set { PhoneApplicationService. Current. State[ "Cache_Message" ] = value; }
        }

        /// <summary>
        /// 缓存发表评论的文字内容
        /// </summary>
        public static Dictionary<string, string> Cache_CommentPub
        {
            get
            {
                return PhoneApplicationService. Current. State. ContainsKey( "Cache_CommentPub" ) ?
                    PhoneApplicationService. Current. State[ "Cache_CommentPub" ] as Dictionary<string, string> : new Dictionary<string, string>( );
            }
            set { PhoneApplicationService. Current. State[ "Cache_CommentPub" ] = value; }
        }

        /// <summary>
        /// 存储回复评论的缓存
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="commentID">评论ID</param>
        /// <param name="replyID">评论回复的ID</param>
        public static void SaveCache_CommentReply( string text,  int commentID, int replyID )
        {
            PhoneApplicationService. Current. State[ string. Format( "CommentReply_{0}_{1}", commentID, replyID ) ] = text;
        }

        /// <summary>
        /// 获取回复评论的文本
        /// </summary>
        /// <param name="commentID">评论ID</param>
        /// <param name="replyID">评论回复的ID</param>
        /// <returns>缓存的文本</returns>
        public static string GetCache_CommentReply( int commentID, int replyID )
        {
            string key = string. Format( "CommentReply_{0}_{1}", commentID, replyID );
            return PhoneApplicationService. Current. State. ContainsKey( key ) ? PhoneApplicationService. Current. State[ key ]. ToString( ) : string. Empty;
        }
        #endregion

        #region 初始化必须执行的方法
        /// <summary>
        /// 检查网络状态
        /// </summary>
        public static void CheckNetworkState( )
        {
            IsNetworkRunning = Microsoft. Phone. Net. NetworkInformation. NetworkInterface. GetIsNetworkAvailable( );
        }

        #endregion

        #region 固定字符串
        /// <summary>
        /// HTML组装字符串的 头部
        /// </summary>
        public const string HTML_Style = "<style>#oschina_title {color: #000000; margin: 20px 0px 10px 6px; font-weight:bold;font-size:20px;}#oschina_title img{vertical-align:middle;margin-right:6px;}#oschina_title a{color:#0D6DA8;}#oschina_outline {color: #707070; padding:0 0 12px 10px; font-size: 16px;line-height:30px}#oschina_outline a{color:#0D6DA8;}#oschina_software{color:#808080;font-size:14px}#oschina_body img {max-width: 480px;}#oschina_body {font-size:18px;max-width:300px;line-height:28px;} #oschina_body table{max-width:300px;}#oschina_body pre { font-size:34pt;font-family:Courier New,Arial;border:1px solid #ddd;border-left:10px solid #6CE26C;background:#f6f6f6;padding:20px;line-height:42px;}</style>";

        /// <summary>
        /// HTML组装字符串的 尾部
        /// </summary>
        public const string HTML_Bottom = "<div style='margin-bottom:200px'/>";

        /// <summary>
        /// App 版本号
        /// </summary>
        public const string AppVersion = "1.8";

        /// <summary>
        /// 文字提示符
        /// </summary>
        public const string LoadNext20 = "下面 20 项 . . .";
        public const string LoadingTip = "正在加载 . . .";
        public const string NoNetworkTip = "网络无连接";

        /// <summary>
        /// 微博分享需要的Key
        /// </summary>
        public const string SinaKey = "3616966952";
        public const string TencentKey = "96f54f97c4de46e393c4835a266207f4";

        /// <summary>
        /// 检测滚动落差值
        /// </summary>
        public const double ScrollBarOffset = 2;

        #endregion
    }
}
