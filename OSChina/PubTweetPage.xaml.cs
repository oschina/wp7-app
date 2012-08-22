using System;
using System. Collections. Generic;
using System. IO;
using System. Windows;
using System. Windows. Media. Imaging;
using Microsoft. Phone. Shell;
using Microsoft. Phone. Tasks;
using OSChina. Model;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public partial class PubTweetPage : WP7_ControlsLib. Controls. ProgressTrayPage
    {
        #region Fields
        private Stream g_stream;
        #endregion

        #region Construct
        public PubTweetPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. txtContent. Focus( );
                    this. ApplicationBar = this. Resources[ "appbar_Face" ] as ApplicationBar;
                };
            this. txtContent. GotFocus += (s, e) =>
                {
                    this. ApplicationBar = this. Resources[ "appbar_Face" ] as ApplicationBar;
                    this. facePanel. Visibility = System. Windows. Visibility. Collapsed;
                };
            EventSingleton. Instance. OnNewFace += (s, e) =>
                {
                    this. txtContent.Text = this.txtContent.Text.Insert(this.txtContent.SelectionStart, e.Tag as string);
                };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            //验证登陆
            if ( this.NavigationContext.QueryString.ContainsKey("at") )
            {
                this. txtContent. Text = string. Format( " @{0}", this. NavigationContext. QueryString[ "at" ] );
            }
            if ( this.NavigationContext.QueryString.ContainsKey("source") == false  )
            {
                //缓存赋值
                this. txtContent. Text = Config. Cache_Tweet;
                if ( Config. Cache_TweetPic != null )
                {
                    BitmapImage bmpSource = new BitmapImage( );
                    bmpSource. SetSource( Config. Cache_TweetPic );
                    this. img. Source = bmpSource;
                }
            }
          
            base. OnNavigatedTo( e );
        }

        #endregion

        #region 底部按钮事件
        private void iconCamera_Click(object sender, EventArgs e)
        {
            CameraCaptureTask task = new CameraCaptureTask( );
            task. Completed += (s, e1) =>
                {
                    if ( e1. ChosenPhoto != null )
                    {
                        BitmapImage bmpSource = new BitmapImage( );
                        bmpSource. SetSource( e1. ChosenPhoto );
                        this. img. Source = bmpSource;
                        BitmapImage g_bmp = new BitmapImage( );
                        g_bmp. CreateOptions = BitmapCreateOptions. DelayCreation;
                        g_bmp. SetSource( e1. ChosenPhoto );
                        g_stream = ReduceSize( g_bmp );
                        Config. Cache_TweetPic = g_stream;
                    }
                };
            try
            {
                task. Show( );
            }
            catch { }
        }

        private void iconPic_Click(object sender, EventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask
            {
                ShowCamera = true,
            };
            task. Completed += (s, e1) =>
            {
                if ( e1. ChosenPhoto != null )
                {
                    BitmapImage bmpSource = new BitmapImage( );
                    bmpSource. SetSource( e1. ChosenPhoto );
                    this. img. Source = bmpSource;
                    BitmapImage g_bmp = new BitmapImage( );
                    g_bmp. CreateOptions = BitmapCreateOptions. DelayCreation;
                    g_bmp. SetSource( e1. ChosenPhoto );
                    g_stream = ReduceSize( g_bmp );
                    Config. Cache_TweetPic = g_stream;
                }
            };
            try
            {
                task. Show( );
            }
            catch { }
        }

        private Stream ReduceSize(BitmapImage g_bmp)
        {
            WriteableBitmap wb = new WriteableBitmap( g_bmp );
            MemoryStream g_MS = new MemoryStream( );
            System. Windows. Media. Imaging. Extensions. SaveJpeg( wb, g_MS, 800, 640, 0, 82 );
            g_MS. Seek( 0, SeekOrigin. Begin );
            return g_MS;
        }

        private void iconSend_Click(object sender, EventArgs e)
        {
            string tweet = txtContent. Text. Trim( );
            if ( tweet. Length == 0 )
            {
                MessageBox. Show( "动弹内容请不要为空" );
                return;
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"uid", Config.UID},
                {"msg", tweet},
            };
            #region 如果不需要传递图片
            this. g_stream = Config. Cache_TweetPic;
            if ( this.g_stream == null )
            {
                ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). LoadingText = "正在提交";
                ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = true;
                PostClient client = Tool. SendPostClient( Config. api_tweet_pub, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = false;
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "发布动弹时网络错误: {0}", e1. Error. Message );
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
                                    Config. ClearCacheTweet( );
                                    Tool. To( "/TweetsPage.xaml?tag=latest" );
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

            #region 如果需要传递图片
            else
            {
                //Tool. AsyncPubTweetWithImage( this. g_fileName, this. g_stream, parameters );
                Tool. AsyncPubTweetWithImage( "1.jpg", this. g_stream, parameters );
                //退回
                EventSingleton. Instance. ToastMessage(null, "后台正在发送此动弹" );
            }
            #endregion
        }

        #endregion

        #region 修改动弹文本
        private void txtContent_TextChanged(object sender, System. Windows. Controls. TextChangedEventArgs e)
        {
            Config. Cache_Tweet = this. txtContent. Text;
            this. lblStringlength. Text = ( 160 - this. txtContent. Text. Length ). ToString( );
        }
        #endregion

        #region 表情
        private void iconFace_Click(object sender, EventArgs e)
        {
            switch ( (sender as ApplicationBarIconButton).Text )
            {
                case "表情":
                    this. ApplicationBar = this. Resources[ "appbar_Keyboard" ] as ApplicationBar;
                    this. Focus( );
                    this. facePanel. Visibility = Visibility. Visible;
                    break;
                case "键盘":
                    this. ApplicationBar = this. Resources[ "appbar_Face" ] as ApplicationBar;
                    this. txtContent. Focus( );
                    this. facePanel. Visibility = Visibility. Collapsed;
                    break;
            }
        }

        #endregion
    }
}