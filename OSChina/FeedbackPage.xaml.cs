/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Windows;
using Microsoft. Phone. Controls;
using Microsoft. Phone. Tasks;

namespace OSChina
{
    public partial class FeedbackPage : PhoneApplicationPage
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public FeedbackPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. txtOSVersion. Text = string. Format( "版本: {0}", Config. AppVersion );
                };
        }

        #endregion

        #region Private functions
        private void linkFeedback_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask
            {
                To = "oschina.net@gmail.com",
                Subject = "我有话想对OSChina.NET说",
            };
            task.Show();
        }

        private void linkWeibo_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask
            {
                Uri = new Uri( "http://weibo.com/oschina2010", UriKind.Absolute ),
            };
            task. Show( );
        }

        private void btnRatingUS_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask( );
            task. Show( );
        }

        private void linkMobile_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask
            {
                Uri = new Uri( "http://m.oschina.net", UriKind. Absolute )
            };
            task. Show( );
        }

        private void btnCheckVersion_Click(object sender, RoutedEventArgs e)
        {
            //检测更新
            Tool. CheckVersionNeedUpdate( "http://www.oschina.net/MobileAppVersion.xml" );
        }
        #endregion
    }
}