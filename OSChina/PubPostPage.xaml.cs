/*
 * 原作者: 王俊
 * 
 */
using System;
using System. Collections. Generic;
using System. Windows;
using System. Windows. Controls;
using OSChina. Model;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public partial class PubPostPage : WP7_ControlsLib. Controls. ProgressTrayPage
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public PubPostPage( )
        {
            InitializeComponent( );
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            //登陆验证
            this. checkEmailNoticeMe. IsChecked = Config. IsEmailNoticeMe;
            txtTitle. Text = Config. Cache_Question_Title;
            txtContent. Text = Config. Cache_Question_Content;
            this. pickerPostType. SelectedIndex = Config. Cache_Question_Index;

            base. OnNavigatedTo( e );
        }
        #endregion

        #region Private functions
        private void checkEmailNoticeMe_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsEmailNoticeMe = ( bool ) this. checkEmailNoticeMe. IsChecked;
        }

        private void pickerPostType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( this.pickerPostType == null )
            {
                return;
            }
            Config. Cache_Question_Index = this. pickerPostType. SelectedIndex;
            switch ( this.pickerPostType.SelectedIndex )
            {
                case 0:
                case 2:
                    this. txtTitle. Watermark = "你有什么技术问题，请在此输入标题";
                    break;
                case 1:
                    this. txtTitle. Watermark = "你有要跟大家分享的呢，请在此输入标题";
                    break;
                case 3:
                    this. txtTitle. Watermark = "[城市]我想要应聘一个 xxxx 职位";
                    break;
                case 4:
                    this. txtTitle. Watermark = "你对OSChina有什么建议，请在此输入标题";
                    break;
            }
        }

        private void iconSend_Click(object sender, EventArgs e)
        {
            string title = txtTitle. Text. Trim( );
            string content = txtContent. Text. Trim( );
            if ( title. Length == 0 )
            {
                MessageBox. Show( "标题请不要为空" );
                return;
            }
            if ( content. Length == 0 )
            {
                MessageBox. Show( "问题内容请不要为空" );
                return;
            }
            ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). LoadingText = "正在提交";
            ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = true;
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"uid", Config.UID},
                {"title",title},
                {"catalog", this.pickerPostType.SelectedIndex+1},
                {"content",content},
                {"isNoticeMe", (bool)this.checkEmailNoticeMe.IsChecked ? "1" : "0"},
            };
            PostClient client = Tool. SendPostClient( Config. api_post_pub, parameters );
            client. DownloadStringCompleted += (s, e1) =>
            {
                ( this as WP7_ControlsLib. Controls. ProgressTrayPage ). ProgressIndicatorIsVisible = false;
                if ( e1. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "发表问题时网络错误: {0}", e1. Error. Message );
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
                                Config. ClearCacheQuestion( );
                                MessageBox. Show( "问答发布成功" );
                                Tool. To( string. Format( "/PostsPage.xaml?catalog={0}", this. pickerPostType. SelectedIndex + 1 ) );
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

        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config. Cache_Question_Title = txtTitle. Text;
            Config. Cache_Question_Content = txtContent. Text;
        }
        #endregion
    }
}