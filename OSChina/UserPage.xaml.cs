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
using Microsoft. Phone. Controls;
using WP7_WebLib. HttpGet;
using OSChina. Model;
using OSChina. Model. AppOnly;
using Microsoft. Phone. Shell;

namespace OSChina
{
    public partial class UserPage : PhoneApplicationPage
    {
        private int hisUID;
        private string hisName;


        public UserPage( )
        {
            InitializeComponent( );
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            if ( this. NavigationContext. QueryString. ContainsKey( "uid" ) )
            {
                this. hisUID = this. NavigationContext. QueryString[ "uid" ]. ToInt32( );
            }
            else if ( this. NavigationContext. QueryString. ContainsKey( "name" ) )
            {
                this. hisName = this. NavigationContext. QueryString[ "name" ];
            }

            //获取他的信息
            this. GetUserInfo( );

            base. OnNavigatedTo( e );
        }

        private void GetUserInfo(  )
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
                {"pageIndex", "0"},
                {"pageSize", "0"},
            };
            if ( this.hisUID != 0 )
                parameters. Add( "hisuid", this. hisUID. ToString( ) );
            if ( this. hisName. IsNotNullOrWhitespace( ) )
                parameters. Add( "hisname", this. hisName );
            WebClient client = Tool. SendWebClient( Config. api_user_info, parameters );
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "获取 user-info 网络错误: {0}", e. Error. Message );
                        return;
                    }
                    else
                    {
                        int pageSize;
                        UserInfo info;
                        Tool. GetUserInfo( e. Result, out pageSize, out info );
                        if ( info != null )
                        {
                            //动态列表开始加载
                            this. activeList. hisUID = info. uid;
                            this. activeList. listBoxHelper_ReloadDelegate( );
                            //博客列表开始加载
                            this. blogList. authoruid = info. uid;
                            this. blogList. listBoxHelper_ReloadDelegate( );
                            //显示字符串
                            this. lblName. Text = info. name;
                            this. hisUID = info. uid;
                            this. hisName = info. name;
                            this. lblExpertise. Text = info. expertise;
                            this. lblFrom. Text = info. from;
                            this. lblJoinTime.Text = info.joinTime;
                            this. lblPlatform. Text = info. devplatform;
                            this. imgGender. Source = new BitmapImage( new Uri( string. Format( "/Resource/{0}.png", info. gender == "男" ? "man" : "woman" ), UriKind. Relative ) );
                            //1:双方互为粉丝  2:你单方面关注他  3:互不关注  4:只有他关注我
                            this. DisplayAppBar( info. relation );
                        }
                    }
                };
        }



        #region 底部按钮功能
        private void DisplayAppBar( int relation )
        {
            switch ( relation )
            {
                case 1:
                    this. ApplicationBar = this. Resources[ "appbar_Minus" ] as ApplicationBar;
                    ( this. ApplicationBar. Buttons[ 3 ] as ApplicationBarIconButton ). Text = "取消互粉";
                    break;
                case 2:
                    this. ApplicationBar = this. Resources[ "appbar_Minus" ] as ApplicationBar;
                    ( this. ApplicationBar. Buttons[ 3 ] as ApplicationBarIconButton ). Text = "取消关注";
                    break;
                case 3:
                    this. ApplicationBar = this. Resources[ "appbar_Plus" ] as ApplicationBar;
                    ( this. ApplicationBar. Buttons[ 3 ] as ApplicationBarIconButton ). Text = "关注 Ta";
                    break;
                case 4:
                    this. ApplicationBar = this. Resources[ "appbar_Plus" ] as ApplicationBar;
                    ( this. ApplicationBar. Buttons[ 3 ] as ApplicationBarIconButton ). Text = "互粉";
                    break;
            }
        }

        private void icon_Refresh1_Click(object sender, EventArgs e)
        {
            if ( this.hisUID != 0 )
            {
                switch ( this.panorama.SelectedIndex )
                {
                    case 0:
                        this. activeList. Refresh( );
                        break;
                    case 2:
                        this. blogList. Refresh( );
                        break;
                }
            }
        }

        private void icon_At_Click(object sender, EventArgs e)
        {
            if ( hisName.IsNotNullOrWhitespace() )
            {
                Tool. To( string. Format( "/PubTweetPage.xaml?at={0}&source=userpage", hisName ) );
            }
        }

        private void icon_Msg_Click(object sender, EventArgs e)
        {
            if ( hisName.IsNotNullOrWhitespace() && hisUID != 0 )
            {
                Tool. To( string. Format( "/PubMsgPage.xaml?receiverID={0}&receiver={1}", hisUID, hisName ) );
            }
        }
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        private void icon_Relation_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
                {"hisuid", this.hisUID.ToString()},
                {"newrelation", "1"},
            };
            WebClient client = Tool. SendWebClient( Config. api_user_updaterelation, parameters );
            client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "user-updaterelation 网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = null;
                        int relation = Tool. UserUpdateRelation( e1. Result, out result );
                        if ( result != null )
                        {
                            switch ( result.errorCode )
                            {
                                case 1:
                                    this. DisplayAppBar( relation );
                                    break;
                                case -1:
                                case -2:
                                case 0:
                                    MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                    break;
                            }
                        }
                    }
                };
        }
        private void icon_Relation_Click1(object sender, EventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
                {"hisuid", this.hisUID.ToString()},
                {"newrelation", "0"},
            };
            WebClient client = Tool. SendWebClient( Config. api_user_updaterelation, parameters );
            client. DownloadStringCompleted += (s, e1) =>
            {
                if ( e1. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "user-updaterelation 网络错误: {0}", e1. Error. Message );
                    return;
                }
                else
                {
                    ApiResult result = null;
                    int relation = Tool. UserUpdateRelation( e1. Result, out result );
                    if ( result != null )
                    {
                        switch ( result. errorCode )
                        {
                            case 1:
                                this. DisplayAppBar( relation );
                                break;
                            case -1:
                            case -2:
                            case 0:
                                MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                break;
                        }
                    }
                }
            };
        }
        #endregion

    }
}