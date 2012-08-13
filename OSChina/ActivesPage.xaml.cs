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
    public partial class ActivesPage : PhoneApplicationPage
    {
        public ActivesPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. activeListControl. listBoxHelper_ReloadDelegate( );
                    //清空user notice
                };
        }

         //1 -- 最新动态  2 -- @我  3 -- 评论  4 -- 我自己
        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            //说明此页来自主页的点击
            if ( this.NavigationContext.QueryString.ContainsKey("catalog") )
            {
                ActiveType ActiveType = ( ActiveType ) Enum. Parse( typeof( ActiveType ), this.NavigationContext.QueryString["catalog"], true);
                this. activeListControl. ActiveType = ActiveType;
                switch ( ActiveType )
                {
                    case ActiveType. All:
                        this. lblTitle. Text = "动态 - 所有";
                        break;
                    case ActiveType. AtMe:
                        this. lblTitle. Text = "动态 - 提到我";
                        break;
                    case ActiveType. Comment:
                        this. lblTitle. Text = "动态 - 对我的评论";
                        break;
                    case ActiveType. MySelf:
                        this. lblTitle. Text = "动态 - 我自己";
                        break;
                }
            }
           
            base. OnNavigatedTo( e );
        }

        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }

        private void icon_Refresh_Click(object sender, EventArgs e)
        {
            this. activeListControl. Refresh( );
        }

    }
}