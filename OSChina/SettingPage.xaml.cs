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

namespace OSChina
{
    public partial class SettingPage : PhoneApplicationPage
    {
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

        private void toggle_ImgVisible_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsImgsVisible = (bool)this. toggle_ImgVisible. IsChecked;
        }

        private void toggle_NoticeExit_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsNoticeExit = (bool)this. toggle_NoticeExit. IsChecked;
        }

        private void toggle_UserNotice_Checked(object sender, RoutedEventArgs e)
        {
            Config. IsScheduledTask = ( bool ) this. toggle_UserNotice. IsChecked;
        }
    }
}