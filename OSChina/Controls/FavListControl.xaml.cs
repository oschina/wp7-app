/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina. Controls
{
    public partial class FavListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        private ListBoxHelper listBoxHelper { get; set; }

        /// <summary>
        /// 收藏类型
        /// </summary>
        public FavType FavType { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public FavListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Favs );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                    this. listBoxHelper_ReloadDelegate( );
                };
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 继续加载收藏列表
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper == null )
            {
                return;
            }
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
                {"type", ((int)FavType).ToString()},
                {"pageIndex",(this.listBoxHelper.allCount/20).ToString()},
                {"pageSize", "20"},
            };
            WebClient client = Tool. SendWebClient( Config. api_fav_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 FavList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                FavUnit[ ] newFavs = Tool. GetFavoriteList( e. Result, out pageSize );
                if ( newFavs != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //筛选
                    string[ ] ids = Tool. GetIDS_ID_Type<FavUnit>( this. listBoxHelper. datas );
                    newFavs = newFavs. Where( f => ids. Contains( string. Format( "{0}_{1}", f. id, f. type ) ) == false ). ToArray( );

                    if ( newFavs. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newFavs[ 0 ];
                            for ( int i = 1 ; i < newFavs. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newFavs[ i ] );
                            }
                        }
                        else
                        {
                            newFavs. ForAll( n => this. listBoxHelper. datas. Add( n ) );
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

        #endregion

        #region Private functions
        /// <summary>
        /// 点击选择某项收藏
        /// </summary>
        private void list_Favs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FavUnit f = this. list_Favs. SelectedItem as FavUnit;
            this. list_Favs. SelectedItem = null;
            if ( f != null )
            {
                Tool. ProcessAppLink( f. url );
            }
        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            FavUnit f = item. DataContext as FavUnit;
            if ( f != null )
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"objid", f.id.ToString()},
                    {"type",f.type.ToString()},
                };
                WebClient client = Tool. SendWebClient( Config. api_fav_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                    {
                        if ( e1. Error != null )
                        {
                            System. Diagnostics. Debug. WriteLine( "删除收藏时网络错误: {0}", e1. Error. Message );
                            return;
                        }
                        else
                        {
                            ApiResult result = Tool. GetApiResult( e1. Result );
                            switch ( result.errorCode )
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
        #endregion
    }
}
