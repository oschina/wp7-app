/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Input;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina. Controls
{
    public partial class ActiveListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        public ListBoxHelper listBoxHelper { get; private set; }

        /// <summary>
        /// 此动态页的动态类型
        /// </summary>
        private ActiveType activeType;
        public ActiveType ActiveType
        {
            get { return this. activeType; }
            set 
            {
                bool isNeedRefresh = activeType != value;
                activeType = value;
                if ( isNeedRefresh )
                {
                    if ( this.listBoxHelper != null )
                    {
                        this. listBoxHelper. Refresh( );
                    }
                }
            }
        }

        /// <summary>
        /// 用户专页专用动态的属性
        /// </summary>
        private int _hisUID;
        public int hisUID
        {
            get { return _hisUID; }
            set 
            {
                _hisUID = value;
                if ( value != 0 )
                {
                    this. list_Activies. ItemTemplate = this. Resources[ "template_User" ] as DataTemplate;
                }
            }
        }

        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public ActiveListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Activies );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                };
            this. Unloaded += (s, e) =>
                {
                    this. listBoxHelper. Clear( );
                };
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 处理继续加载
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = null;
            parameters = this. hisUID == 0 ? new Dictionary<string, string>
                {
                    {"catalog", ((int)this.ActiveType).ToString()},
                    {"uid", Config.UID.ToString()},
                    {"pageSize", "20"},
                    {"pageIndex", (this.listBoxHelper.allCount / 20).ToString()},
                } : new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"hisuid", this.hisUID.ToString()},
                    {"pageSize","20"},
                    {"pageIndex", (this.listBoxHelper.allCount / 20).ToString()},
                };
            WebClient client = Tool. SendWebClient( this. hisUID == 0 ? Config. api_active_list : Config. api_user_info, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 ActiveList或userInfo 网络错误: {0}", e. Error. Message );
                    return;
                }
                //清空 UserNotice
                this. ClearUserNotice( );

                int pageSize;
                ActiveUnit[ ] newActives = Tool. GetActiveList( e. Result, out pageSize );
                if ( newActives != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    //这里需要特殊处理 因为热门动弹总共就20条
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //如果是刷新则提示有数据 注意必须在筛选前就处理好
                    if ( this. listBoxHelper. ids4Refresh. Count > 0 )
                    {
                        int count = newActives. Where( a => this. listBoxHelper. ids4Refresh. Contains( a. id ) == false ). Count( );
                        if ( count > 0 )
                        {
                            EventSingleton. Instance. ToastMessage( null, string. Format( "{0} 条新动态", count ) );
                        }
                        this. listBoxHelper. ids4Refresh. Clear( );
                    }
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<ActiveUnit>( this. listBoxHelper. datas );
                    newActives = newActives. Where( a => ids. Contains( a. id ) == false ). ToArray( );

                    if ( newActives. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newActives[ 0 ];
                            for ( int i = 1 ; i < newActives. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newActives[ i ] );
                            }
                        }
                        else
                        {
                            newActives. ForAll( n => this. listBoxHelper. datas. Add( n ) );
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

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void Refresh( )
        {
            //记住现有的 id 集合
            this. listBoxHelper. ids4Refresh = Tool. GetIDS_ID<ActiveUnit>( this. listBoxHelper. datas ). Take( 20 ). ToList( );
            this. listBoxHelper. Refresh( );
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 清除用户通知
        /// </summary>
        private void ClearUserNotice( )
        {
            switch ( this.ActiveType )
            {
                case ActiveType. AtMe:
                    Tool. ClearUserNotice( 1 );
                    break;
                case ActiveType. Comment:
                    Tool. ClearUserNotice( 3 );
                    break;
            }
        }

        /// <summary>
        /// 点击后进入文章详情页
        /// </summary>
        private void list_Activies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActiveUnit a = this. list_Activies. SelectedItem as ActiveUnit;
            this. list_Activies. SelectedItem = null;
            if ( a != null )
            {
                if ( a. url. IsNullOrWhitespace( ) )
                {
                    if ( a. catalog <= 0 || a. catalog >= 5 )
                        return;
                    switch ( a. catalog )
                    {
                        case 1:
                            Tool. ToDetailPage( a. objID. ToString( ), DetailType. News );
                            break;
                        case 2:
                            Tool. ToDetailPage( a. objID. ToString( ), DetailType. Post );
                            break;
                        case 3:
                            Tool. ToDetailPage( a. objID. ToString( ), DetailType. Tweet );
                            break;
                        case 4:
                            Tool. ToDetailPage( a. objID. ToString( ), DetailType. Blog );
                            break;
                    }
                }
                else
                {
                    Tool. ProcessAppLink( a. url );
                }
            }
        }

        /// <summary>
        /// 点击头像后进入用户个人专页
        /// </summary>
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ActiveUnit a = ( ( sender as Image ). DataContext as ActiveUnit );
            if ( a != null )
            {
                Tool. ToUserPage( a. authorID );
                e. Handled = true;
            }
        }

        /// <summary>
        /// 文字显示辅助处理
        /// </summary>
        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock txt = sender as TextBlock;
            ActiveUnit a = txt. DataContext as ActiveUnit;
            if ( a != null )
            {
                Tool. ProcessActiveUnit( a, txt );

                this. list_Activies. UpdateLayout( );
            }
        }

        /*
         * 动弹的唯一图像加载
         * 因为动弹很多都不带图像，如果没有图像  则删除此Image控件以节约性能
         */
        private void imgTweet_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            ActiveUnit a = img. DataContext as ActiveUnit;
            if ( a != null && a.tweetImage.IsNotNullOrWhitespace() )
            {
            }
            else
            {
                Panel panel = img. Parent as Panel;
                if ( panel != null )
                {
                    panel. Children. Remove( img );
                }
            }
        }

        #endregion
    }
}
