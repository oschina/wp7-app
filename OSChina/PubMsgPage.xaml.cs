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
using WP7_WebLib. HttpPost;
using OSChina. Model;

namespace OSChina
{
    public partial class PubMsgPage : WP7_ControlsLib. Controls. ProgressTrayPage
    {
        private int receiverID { get; set; }
        private string receiver { get; set; }

        public PubMsgPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. txtContent. Focus( );
                };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            //登陆验证
            IDictionary<string, string> queryString = this. NavigationContext. QueryString;
            if ( queryString.ContainsKey("receiverID") && queryString.ContainsKey("receiver") )
            {
                this. receiverID = queryString[ "receiverID" ]. ToInt32( );
                this. receiver = queryString[ "receiver" ];
                this. lblTitle. Inlines. Add( new Run { Text = "发给 ", FontSize = 28 } );
                this. lblTitle. Inlines. Add( new Run
                {
                    Text = queryString[ "receiver" ],
                    FontSize = 28,
                    Foreground = Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush,
                    FontStyle = FontStyles. Italic
                } );

                //验证缓存
                Dictionary<int, string> cacheMessage = Config. Cache_Message;
                this. txtContent. Text = cacheMessage. ContainsKey( this. receiverID ) ? cacheMessage[ this. receiverID ] : string. Empty;
            }
            base. OnNavigatedTo( e );
        }

        private void iconSend_Click(object sender, EventArgs e)
        {
            string content = txtContent. Text. Trim( );
            if ( content. Length == 0 )
            {
                MessageBox. Show( "留言内容不能为空" );
                return;
            }
            else
            {
                ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). LoadingText = "正在提交";
                ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = true;
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"uid", Config.UID},
                    {"receiver", this.receiverID},
                    {"content", content},
                };
                PostClient client = Tool. SendPostClient( Config. api_msg_pub, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = false;
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "发送留言时网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e1. Result );
                        if ( result != null )
                        {
                            switch ( result. errorCode )
                            {
                                case 1:
                                    if ( this.NavigationService.BackStack.FirstOrDefault() != null && this.NavigationService.BackStack.FirstOrDefault().Source.OriginalString.Contains("UserPage"))
                                    {
                                        Tool. To( string. Format( "/WordsPage.xaml?friendid={0}&friendname={1}", this. receiverID, this. receiver ) );
                                    }
                                    else if(this.NavigationService.CanGoBack)
                                    {
                                        this. NavigationService. GoBack( );
                                    }
                                    if ( Config.Cache_Message.ContainsKey(this.receiverID) )
                                    {
                                        Config. Cache_Message. Remove( this. receiverID );
                                    }
                                    break;
                                case 0:
                                case -1:
                                case -2:
                                    MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                    break;
                            }
                        }
                    }

                };
            }
        }

        #region 发送文字的缓存
        private void txtContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dictionary<int, string> cacheMessage = Config. Cache_Message;
            cacheMessage[ this. receiverID ] = txtContent. Text;
            Config. Cache_Message = cacheMessage;
        }
        #endregion
    }
}