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
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_WebLib. HttpGet;

namespace OSChina
{
    public partial class FriendPage : PhoneApplicationPage
    {
        public FriendPage( )
        {
            InitializeComponent( );
            this. myFans. IsFans = true;
            this. myFollow. IsFans = false;

            //控制 myFans 与 myFollow 开始加载
            this. myFollow. listBoxHelper_ReloadDelegate( );
            this. myFans. listBoxHelper_ReloadDelegate( );
            //显示朋友个数
            this. DisplayFriendsCount( );
        }

        private void DisplayFriendsCount( )
        {
            //先立即显示
            MyInfo _info = Config. MyInfo;
            if ( _info != null )
            {
                this. lblFollowers. Text = string. Format( "我关注的({0})", _info. followersCount );
                this. lblFans. Text = string. Format( "我的粉丝({0})", _info. fansCount );
            }

            //然后网络获取
            Dictionary<string, string> parameters = new Dictionary<string, string>
                    {
                        {"uid", Config.UID.ToString()},
                    };
            WebClient client = Tool. SendWebClient( Config. api_my_info, parameters );
            client. DownloadStringCompleted += (s1, e1) =>
            {
                if ( e1. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取my-info 时网络错误: {0}", e1. Error. Message );
                    return;
                }
                else
                {
                    MyInfo info = Tool. GetMyInfo( e1. Result );
                    if ( info != null )
                    {
                        this. lblFollowers. Text = string. Format( "我关注的({0})", info. followersCount );
                        this. lblFans. Text = string. Format( "我的粉丝({0})", info. fansCount );
                        Config. MyInfo. favCount = info. favCount;
                        Config. MyInfo. fansCount = info. fansCount;
                    }
                }
            };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            if ( this.NavigationContext.QueryString.ContainsKey("fans") )
            {
                if ( this.NavigationContext.QueryString["fans"] == "1" )
                {
                    this. pivot. SelectedIndex = 1;
                }
            }
            base. OnNavigatedTo( e );
        }
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
    }
}