using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System. Windows. Media. Imaging; 
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using OSChina. Model. AppOnly;
using OSChina. Model;
using Microsoft. Phone. Shell;
using Coding4Fun. Phone. Controls;
using OSChina. Controls;
using System. Windows. Threading;

namespace OSChina
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            Tool. StartUserNoticeThread( );
            //弹出消息提示
            this. Loaded += (s, e) =>
                {
                    //登陆或注销用户的检测
                    this. Instance_OnLoginOrLogout( null, null );
                    Tool. AsyncGetUserNotice( );
                    //我的动弹那里获取自己的头像
                    this. DisplayMyTweetPortrait( );
                    
                    EventSingleton. Instance. OnLoginOrLogout += new EventHandler<TagEventArgs>( Instance_OnLoginOrLogout );
                    EventSingleton. Instance. OnGetUserNotice += new EventHandler<TagEventArgs>( Instance_OnGetUserNotice );
                    //
                    Console. Write( ScheduledUnit. Instance );
                };
            this. Unloaded += (s, e) =>
                {
                    GC. Collect( );
                    EventSingleton. Instance. OnLoginOrLogout -= Instance_OnLoginOrLogout;
                    EventSingleton. Instance. OnGetUserNotice -= Instance_OnGetUserNotice;
                };
            //启动 UserNotice 的获取
        }

        #region 初始化
        //用户登陆或注销
        void Instance_OnLoginOrLogout(object sender, TagEventArgs e)
        {
            bool isLogin = Config. IsLogin;
            this. activeItem. Visibility = isLogin ? Visibility. Visible : Visibility. Collapsed;
            this. panel4Login. Visibility = isLogin ? Visibility. Visible : Visibility. Collapsed;
            this. panel4Logout. Visibility = isLogin ? Visibility. Collapsed : Visibility. Visible;
        }
        //页面初始化
        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            base. OnNavigatedTo( e );
        }
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        //获取到 UserNotice 后的处理
        void Instance_OnGetUserNotice(object sender, TagEventArgs e)
        {
            UserNotice notice = e. Tag as UserNotice;
            if ( notice != null )
            {
                if ( notice. atMeCount != 0 )
                {
                    this. notice_AtMe. Visibility = System. Windows. Visibility. Visible;
                    this. notice_AtMe. Content = notice. atMeCount. ToString( );
                }
                else
                {
                    this. notice_AtMe. Visibility = System. Windows. Visibility. Collapsed;
                }
                if ( notice. reviewCount != 0 )
                {
                    this. notice_Review. Visibility = System. Windows. Visibility. Visible;
                    this. notice_Review. Content = notice. reviewCount. ToString( );
                }
                else
                {
                    this. notice_Review. Visibility = System. Windows. Visibility. Collapsed;
                }
                if ( notice. msgCount != 0 )
                {
                    this. notice_Msg. Content = new TextBlock
                    {
                        Text = string. Format( "{0} 未读消息", notice. msgCount ),
                        Foreground = new SolidColorBrush( Colors.Black ),
                        FontWeight = FontWeights. Bold,
                        FontFamily = new FontFamily( "Arial" ),
                    };
                }
                else
                {
                    this. notice_Msg. Content = new TextBlock
                    {
                        Text = "无未读消息",
                        Foreground = new SolidColorBrush( Colors.Black )
                    };
                }
            }
        }
        //改变全景Item
        private void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( trexStoryboard. GetCurrentState( ) == ClockState. Stopped )
            {
                if ( this. panorama. SelectedIndex == 3 && Config. IsLogin )
                {
                    trexStoryboard. Begin( );
                }
            }
        }
        #endregion

        #region 资讯区
        //进入博客页  测试用
        private void btnToDetailPage_Click(object sender, RoutedEventArgs e)
        {
            switch ( (sender as Button).Name )
            {
                case "btnToDetailPage1":
                    Tool. ToDetailPage( "29421", DetailType. News );
                    break;
                case "btnToDetailPage2":
                    Tool. ToDetailPage( "55638", DetailType. Post );
                    break;
                case "btnToDetailPage3":
                    Tool. ToDetailPage( "59678", DetailType. Blog );
                    break;
                case "btnToDetailPage4":
                    Tool. ToDetailPage( "casperjs", DetailType. Software );
                    break;
                case "btnToDetailPage5":
                    Tool. ToDetailPage( "771923", DetailType. Tweet );
                    break;
            }
        }
        private void btnNews_Click(object sender, RoutedEventArgs e)
        {
            RoundButton btn = sender as RoundButton;
            if ( btn != null )
            {
                btn. Background = Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush;
                switch ( btn.Name )
                {
                    case "btnNews":
                        this. btnBlogs.Background = new SolidColorBrush( Colors. Black );
                        this. btnHotBlogs. Background = new SolidColorBrush( Colors. Black );
                        this. newsList. NewsType = Controls. NewsType. News;
                        this. lblTitleNews. Text = "资讯";
                        break;
                    case "btnBlogs":
                        this. btnNews. Background = new SolidColorBrush( Colors. Black );
                        this. btnHotBlogs. Background = new SolidColorBrush( Colors. Black );
                        this. newsList. NewsType = Controls. NewsType. Blogs;
                        this. lblTitleNews. Text = "博客";
                        break;
                    case "btnHotBlogs":
                        this. btnNews. Background = new SolidColorBrush( Colors. Black );
                        this. btnBlogs. Background = new SolidColorBrush( Colors. Black );
                        this. newsList. NewsType = Controls. NewsType. RecommendBlogs;
                        this. lblTitleNews. Text = "推荐";
                        break;
                }
            }
        }
        #endregion

        #region 问答区
        //进入问答列表
        private void btnPost1_Click(object sender, RoutedEventArgs e)
        {
            switch ( ( sender as HubTile ). Name )
            {
                case "btnPost1":
                    Tool. To( "/PostsPage.xaml?catalog=1" );
                    break;
                case "btnPost2":
                    Tool. To( "/PostsPage.xaml?catalog=2" );
                    break;
                case "btnPost3":
                    Tool. To( "/PostsPage.xaml?catalog=3" );
                    break;
                case "btnPost4":
                    Tool. To( "/PostsPage.xaml?catalog=4" );
                    break;
                case "btnPost5":
                    Tool. To( "/PostsPage.xaml?catalog=5" );
                    break;
            }
        }
        #endregion

        #region 动弹区

        private void tweetPortal_Latest_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch ( ( sender as Control ). Name )
            {
                case "tweetPortal_Latest":
                    Tool. To( "/TweetsPage.xaml?tag=latest" );
                    break;
                case "tweetPortal_Hottest":
                    Tool. To( "/TweetsPage.xaml?tag=hottest" );
                    break;
                case "tweetPortal_My":
                    if ( Tool.CheckLogin() )
                    {
                        Tool. To( "/TweetsPage.xaml?tag=my" );
                    }
                    break;
            }
        }
        private void DisplayMyTweetPortrait( )
        {
            MyInfo myInfo = Config. MyInfo;
            if ( myInfo != null && myInfo. portrait. IsNotNullOrWhitespace( ) )
            {
                this. tweetPortal_My. Source = new BitmapImage( new Uri( myInfo. portrait, UriKind. Absolute ) );
            }
            else
            {
                //使用默认头像

            }
        }
        #endregion

        #region 我的区
        //消息中心图片效果
        //发送留言
        private void btnPostMsg_Click(object sender, RoutedEventArgs e)
        {
            //登陆提示
            if ( Tool. CheckLogin( ) == false )
            {
                return;
            }
            else
            {
                Tool. To( "/MsgsPage.xaml" );
            }
        }
        //进入动态页
        private void btnActive_1_Click(object sender, RoutedEventArgs e)
        {
            //登陆提示
            if ( Tool.CheckLogin() == false)
            {
                return;
            }
            switch ( (sender as Tile).Name )
            {
                case "btnActive_1":
                    Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. All ) );
                    break;
                case "btnActive_2":
                    Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. AtMe ) );
                    break;
                case "btnActive_3":
                    Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. Comment ) );
                    break;
                case "btnActive_4":
                    Tool. To( string. Format( "/ActivesPage.xaml?catalog={0}", ( int ) ActiveType. MySelf ) );
                    break;
            }
        }

        #endregion

        #region 更多区
        private void panelLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Tool. To( "/LoginPage.xaml?tag=needback" );
        }
        private void btnMyInfo_Click(object sender, RoutedEventArgs e)
        {
            Tool. To( "/MyPage.xaml" );
        }
        private void btnFavorite_Click(object sender, RoutedEventArgs e)
        {
            Tool. To( "/FavPage.xaml" );
        }
        private void btnFans_Click(object sender, RoutedEventArgs e)
        {
            Tool. To( "/FriendPage.xaml?fans=1" );
        }
        private void btnFollow_Click(object sender, RoutedEventArgs e)
        {
            Tool. To( "/FriendPage.xaml?fans=0" );
        }
        private void btnAboutUs_Click(object sender, RoutedEventArgs e)
        {
            Tool. To( "/FeedbackPage.xaml" );
        }
        private void panelLogout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Config. Cookie = null;
            EventSingleton. Instance. RaiseLoginOrLogout( );
            MessageBox. Show( "您的账户注销成功", "温馨提示", MessageBoxButton. OK );
        }
        #endregion

        #region ApplicationBar 按钮事件
        //如果当前是新闻，动弹，或者我的  都可以执行刷新操作
        private void icon_Refresh1_Click(object sender, EventArgs e)
        {
            switch ( this.panorama.SelectedIndex )
            {
                    //新闻列表刷新
                case 0:
                    this. newsList. Refresh( );
                    break;
                    //动弹列表刷新
                    //注意这里只需要刷新
                case 2:
                    TweetPortal. AnsycGetHottestTweets( true );
                    TweetPortal. AnsycGetHottestTweets( false );
                    break;
                    //我的刷新，注意不是列表刷新，而是 userNotice 刷新显示
                case 3:
                    //动态页只有登陆了才有效
                    if ( Config.IsLogin )
                    {
                        Tool. AsyncGetUserNotice( );
                    }
                    break;
            }
        }
        //发表动弹
        private void icon_Tweet_Click(object sender, EventArgs e)
        {
            if ( Tool.CheckLogin() )
            {
                Tool. To( "/PubTweetPage.xaml" );
            }
        }
        //发表问题
        private void icon_Post_Click(object sender, EventArgs e)
        {
            if ( Tool. CheckLogin( ) )
            {
                Tool. To( "/PubPostPage.xaml" );
            }
        }
        //搜索
        private void icon_Search_Click(object sender, EventArgs e)
        {
            Tool. To( "/SearchPage.xaml" );
        }
        //关于我们
        private void menu_AboutUS_Click(object sender, EventArgs e)
        {
            Tool. To( "/FeedbackPage.xaml" );
        }
        //软件页
        private void menu_Software_Click(object sender, EventArgs e)
        {
            Tool. To( "/SoftwarePage.xaml" );
        }
        private void menu_Setting_Click(object sender, EventArgs e)
        {
            Tool. To( "/SettingPage.xaml" );
        }
        #endregion

        #region Back键处理
        protected override void OnBackKeyPress(System. ComponentModel. CancelEventArgs e)
        {
            if ( Config. IsNoticeExit )
            {
                if ( this. NavigationService. CanGoBack == false )
                {
                    if ( MessageBox. Show( "您确认要退出 OSChina 客户端吗?", "温馨提示", MessageBoxButton. OKCancel ) == MessageBoxResult. OK )
                    {
                        base. OnBackKeyPress( e );
                    }
                    else
                    {
                        e. Cancel = true;
                    }
                }
                base. OnBackKeyPress( e );
            }
            else
            {
                base. OnBackKeyPress( e );
            }
        }

        #endregion

        


    }
}