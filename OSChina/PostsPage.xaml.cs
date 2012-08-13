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

namespace OSChina
{
    public partial class PostsPage : PhoneApplicationPage
    {
        public PostsPage( )
        {
            InitializeComponent( );
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
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
            base. OnNavigatedTo( e );
        }
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


    }
}