/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System. Windows;
using Microsoft. Phone. Controls;

namespace OSChina
{
    public partial class SettingPage : PhoneApplicationPage
    {
        #region Construct
        public SettingPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {
                this. toggle_ImgVisible. IsChecked = Config. IsImgsVisible;
                this. toggle_NoticeExit. IsChecked = Config. IsNoticeExit;
                this. toggle_UserNotice. IsChecked = Config. IsScheduledTask;
            };
        }
        #endregion
        
        #region Private functions
        private void toggle_ImgVisible_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsImgsVisible = ( bool ) this. toggle_ImgVisible. IsChecked;
        }

        private void toggle_NoticeExit_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsNoticeExit = ( bool ) this. toggle_NoticeExit. IsChecked;
        }

        private void toggle_UserNotice_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsScheduledTask = ( bool ) this. toggle_UserNotice. IsChecked;
        }
        #endregion
    }
}