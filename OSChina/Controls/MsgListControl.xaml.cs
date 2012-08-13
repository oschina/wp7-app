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
using OSChina. Model;
using OSChina. Model. AppOnly;
using Microsoft. Phone. Controls;

namespace OSChina. Controls
{
    public partial class MsgListControl : UserControl
    {
        private ListBoxHelper listBoxHelper { get; set; }

        public MsgListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Msg );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                    this. listBoxHelper_ReloadDelegate( );
                };
           
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }

        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid",Config.UID.ToString()},
                {"pageIndex",(this.listBoxHelper.allCount/20).ToString()},
                {"pageSize", "20"},
            };
            WebClient client = Tool. SendWebClient( Config.api_msg_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 MsgList 网络错误: {0}", e. Error. Message );
                    return;
                }
                Tool. ClearUserNotice( 2 );
                int pageSize;
                MsgUnit[ ] newMsgs = Tool. GetMsgList( e. Result, out pageSize );
                if ( newMsgs != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<MsgUnit>( this. listBoxHelper. datas );
                    newMsgs = newMsgs. Where( m => ids. Contains( m. id ) == false ). ToArray( );
                    //去除最后的提示文字
                    //this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                    ////添加刚才的元素
                    //newMsgs. ForAll( m => this. listBoxHelper. datas. Add( m ) );

                    if ( newMsgs. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newMsgs[ 0 ];
                            for ( int i = 1 ; i < newMsgs. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newMsgs[ i ] );
                            }
                        }
                        else
                        {
                            newMsgs. ForAll( n => this. listBoxHelper. datas. Add( n ) );
                        }
                    }
                    else
                    {
                        //去除最后项
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                        }
                    }

                    //添加最后的提示文字
                    if ( this. listBoxHelper. isLoadOver == false || this. listBoxHelper. datas. Count <= 0 )
                    {
                        this. listBoxHelper. datas. Add( this. listBoxHelper. GetLoadTip );
                    }
                }
            };
        }


        private void list_Msg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MsgUnit m = this. list_Msg. SelectedItem as MsgUnit;
            this. list_Msg. SelectedItem = null;
            if ( m != null )
            {
                Tool. To( string. Format( "/WordsPage.xaml?friendid={0}&friendname={1}", m. friendID, m. friendName ) );
            }
        }

        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            MsgUnit m = item. DataContext as MsgUnit;
            if ( m != null )
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"friendid", m.friendID.ToString()},
                };
                WebClient client = Tool. SendWebClient( Config.api_msg_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "删除消息中心时网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e1. Result );
                        switch ( result. errorCode )
                        {
                            case 1:
                                this. listBoxHelper. Refresh( );
                                break;
                            case 0:
                            case -1:
                            case -2:
                                MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                break;
                            default:
                                break;
                        }
                    }
                };
            }
        }

        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MsgUnit m = ( sender as Image ). DataContext as MsgUnit;
            if ( m != null )
            {
                //进入用户专页
                Tool. ToUserPage( m.friendID );
                e. Handled = true;
            }
        }
    }
}
