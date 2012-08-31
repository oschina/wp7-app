/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System. Collections. Generic;
using System. Windows;
using OSChina. Model;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public partial class LoginPage : WP7_ControlsLib. Controls. ProgressTrayPage
    {
        #region Fields
        /// <summary>
        /// 登陆成功后是否需要回退
        /// </summary>
        private bool isNeedBack;
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public LoginPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {
                this. txt_UserName. Focus( );
            };
        }
        /// <summary>
        /// 页面初始化
        /// </summary>
        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            this. txt_UserName. Text = Config. LoginName. EnsureNotNull( );
            this. txt_Password. Password = Config. Password. EnsureNotNull( );
            //检查是否需要回退
            if ( this. NavigationContext. QueryString. ContainsKey( "tag" ) )
            {
                this. isNeedBack = this. NavigationContext. QueryString[ "tag" ] == "needback";
            }
            base. OnNavigatedTo( e );
        }
        #endregion

        #region Private function
        /// <summary>
        /// 登陆处理
        /// </summary>
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string name = txt_UserName. Text. Trim( );
            string password = txt_Password. Password. Trim( );
            if ( name.Length == 0 )
            {
                MessageBox. Show( "用户名不能为空" );
                return;
            }
            if ( password.Length == 0 )
            {
                MessageBox. Show( "登陆密码不能为空" );
                return;
            }
            //开始登陆
            this. LoadingText = "登陆中";
            this. ProgressIndicatorIsVisible = true;
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"username", name},
                {"pwd", password},
                {"keep_login", "1"},
            };
            PostClient client = Tool. SendPostClient( Config. api_login_validate, parameters );
            client. DownloadStringCompleted += (s, e1) =>
                {
                    this. ProgressIndicatorIsVisible = false;
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "登陆时网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                         MyInfo myInfo;
                         ApiResult result = Tool. GetLoginResult( e1. Result, out myInfo );
                         if ( result != null )
                         {
                             switch ( result.errorCode )
                             {
                                 case 1:
                                     if ( ( bool ) check_RememberMe. IsChecked == true )
                                     {
                                         Config. LoginName = txt_UserName. Text. Trim( );
                                         Config. Password = txt_Password. Password. Trim( );
                                     }
                                     else
                                     {
                                         Config. LoginName = Config. Password = null;
                                     }
                                     Config. UID = myInfo. uid;
                                     Config. MyInfo = myInfo;
                                     EventSingleton. Instance. RaiseLoginOrLogout( );
                                     //登陆成功后可能需要回退
                                     if ( this.isNeedBack && this.NavigationService.CanGoBack )
                                     {
                                         this. NavigationService. GoBack( );
                                     }
                                     break;
                                 case 0:
                                 case -1:
                                 case -2:
                                     MessageBox. Show( ( result. errorMessage ). TrimEnd('\n','\t'), "登陆失败", MessageBoxButton. OK );
                                     break;
                             }
                         }
                    }
                };

        }

        #endregion
    }
}