using System. Collections. Generic;
using System. Windows;
using System. Windows. Documents;
using System. Windows. Media;
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public partial class PubCommentPage : WP7_ControlsLib. Controls. ProgressTrayPage
    {
        #region Properties
        private int id { get; set; }
        private CommentType catalog { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public PubCommentPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {
                this. txtContent. Focus( );
            };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            this. checkResendToZone. IsChecked = Config. IsPostToZone;
            //friendid
            if ( this. NavigationContext. QueryString. ContainsKey( "catalog" ) )
            {
                //给 新闻，博客 (catalog = 5)， 帖子，动弹 
                if ( this. NavigationContext. QueryString. ContainsKey( "id" ) )
                {
                    this. catalog = ( CommentType ) this. NavigationContext. QueryString[ "catalog" ]. ToInt32( );
                    this. id = this. NavigationContext. QueryString[ "id" ]. ToInt32( );
                    //显示按钮与转发选项
                    if ( this. catalog == CommentType. Tweet )
                    {
                        this. checkResendToZone. Visibility = System. Windows. Visibility. Visible;
                    }
                    //设定标题
                    if ( this. NavigationContext. QueryString. ContainsKey( "title" ) )
                    {
                        this. lblTitle. Inlines. Add( new Run { Text = "在 ", FontSize = 24 } );
                        this. lblTitle. Inlines. Add( new Run { Text = this. NavigationContext. QueryString[ "title" ], FontSize = 24, Foreground = Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush } );
                        this. lblTitle. Inlines. Add( new Run { Text = " 发表评论", FontSize = 24 } );
                    }
                    else
                    {
                        this. lblTitle. Text = "发表评论";
                    }
                }
                //给消息中心发
                else if ( this. NavigationContext. QueryString. ContainsKey( "friendid" ) )
                {
                }
            }

            base. OnNavigatedTo( e );
        }
        #endregion

        #region Private functions
        private void checkResendToZone_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsPostToZone = ( bool ) this. checkResendToZone. IsChecked;
        }

        private void iconSend_Click(object sender, System. EventArgs e)
        {
            string content = txtContent. Text. Trim( );
            if ( content. Length == 0 )
            {
                MessageBox. Show( "发表评论不能为空" );
                return;
            }
            this. LoadingText = "正在提交";
            this. ProgressIndicatorIsVisible = true;
            Dictionary<string, object> parameters = null;
            if ( this. catalog != CommentType. Blog )
            {
                parameters = new Dictionary<string, object>
                    {
                        {"catalog", (int)this.catalog},
                        {"id", this.id},
                        {"uid", Config.UID},
                        {"content", content},
                        {"isPostToMyZone", this.catalog == CommentType.Tweet ? ((bool)this.checkResendToZone.IsChecked ? "1" : "0" ): "0"},
                    };
            }
            else
            {
                parameters = new Dictionary<string, object>
                {
                    {"blog", this.id},
                    {"uid", Config.UID},
                    {"content", content},
                };
            }
            PostClient client = Tool. SendPostClient( this. catalog != CommentType. Blog ? Config. api_comment_pub : Config. api_blogcomment_pub, parameters );
            client. DownloadStringCompleted += (s, e1) =>
            {
                this. ProgressIndicatorIsVisible = false;
                if ( e1. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "发表评论时网络错误: {0}", e1. Error. Message );
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
                                if ( this.NavigationService.CanGoBack )
                                {
                                    this. NavigationService. GoBack( );
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
        #endregion
    }
}