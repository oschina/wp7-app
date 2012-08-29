/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using Microsoft. Phone. Controls;
using OSChina. Model. AppOnly;

namespace OSChina
{
    public partial class PostsPage : PhoneApplicationPage
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public PostsPage( )
        {
            InitializeComponent( );
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            #region 如果是问答分类
            if ( this.NavigationContext.QueryString.ContainsKey("catalog") )
            {
                int catalog = this. NavigationContext. QueryString[ "catalog" ]. ToInt32( );
                this. postsControl. catalog = catalog;
                switch ( (PostType)catalog )
                {
                    case PostType. Answer:
                        this. lblTitle. Text = "讨论区 - 问答";
                        break;
                    case PostType. Share:
                        this. lblTitle. Text = "讨论区 - 分享";
                        break;
                    case PostType. Standard:
                        this. lblTitle. Text = "讨论区 - 综合";
                        break;
                    case PostType. Career:
                        this. lblTitle. Text = "讨论区 - 职业";
                        break;
                    case PostType. Website:
                        this. lblTitle. Text = "讨论区 - 站务";
                        break;
                }
                if ( this. postsControl. listBoxHelper == null )
                {
                    return;
                }
                if ( this. postsControl. listBoxHelper. allCount > 2 )
                {
                    return;
                }
                this. postsControl. listBoxHelper. Refresh( );
            }
            #endregion

            #region 如果是问答标签
            else if ( this.NavigationContext.QueryString.ContainsKey("tag") )
            {
                this. postsControl. tag = this. NavigationContext. QueryString[ "tag" ];
                this. lblTitle. Text = string. Format( "标签 - {0}", this. postsControl. tag );
                if ( this. postsControl. listBoxHelper == null )
                {
                    return;
                }
                if ( this. postsControl. listBoxHelper. allCount > 2 )
                {
                    return;
                }
                this. postsControl. listBoxHelper. Refresh( );
            }
            #endregion

            base. OnNavigatedTo( e );
        }
        #endregion

        #region Private functions
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }

        protected override void OnBackKeyPress(System. ComponentModel. CancelEventArgs e)
        {
            base. OnBackKeyPress( e );
        }

        private void icon_Refresh_Click(object sender, EventArgs e)
        {
            this. postsControl. listBoxHelper. Refresh( );
        }

        private void icon_Post_Click(object sender, EventArgs e)
        {
            if ( Tool. CheckLogin( ) )
            {
                Tool. To( "/PubPostPage.xaml" );
            }
        }

        private void icon_Back_Click(object sender, EventArgs e)
        {
            if ( this. postsControl. tag. IsNotNullOrWhitespace( ) )
            {
                Tool. To( "/MainPage.xaml" );
            }
            else 
            {
                if ( this.NavigationService.CanGoBack )
                {
                    this. NavigationService. GoBack( );
                }
            }
        }
        #endregion

       
    }
}
