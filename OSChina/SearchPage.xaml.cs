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
using Microsoft. Phone. Controls;
using OSChina. Model;

namespace OSChina
{
    public partial class SearchPage : PhoneApplicationPage
    {
        #region Properties
        private ListBoxHelper listBoxHelper { get; set; }
        #endregion

        #region Construct
        public SearchPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Result,false, false, false );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                    this. iconSearch_Click( null, null );
                };
          
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }
        #endregion

        #region Private functions
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }

        private void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"catalog",this.GetSearchCatalog},
                {"content",this.txtSearch.Text.Trim()},
                {"pageIndex", (this.listBoxHelper.allCount / 20).ToString()},
                {"pageSize","20"},
            };
            WebClient client = Tool. SendWebClient( Config.api_search_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 SearchList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                SearchUnit[ ] newResults = Tool. GetSearchList( e. Result, out pageSize );
                if ( newResults != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //筛选
                    string[ ] ids = Tool. GetIDS_ID_Type<SearchUnit>( this. listBoxHelper. datas );
                    newResults = newResults. Where( r => ids. Contains( string. Format( "{0}_{1}", r. id, r. type ) ) == false ). ToArray( );

                    if ( newResults. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newResults[ 0 ];
                            for ( int i = 1 ; i < newResults. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newResults[ i ] );
                            }
                        }
                        else
                        {
                            newResults. ForAll( n => this. listBoxHelper. datas. Add( n ) );
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

        private void iconSearch_Click(object sender, EventArgs e)
        {
            if ( this.list_Result != null )
            {
                this. list_Result. Visibility = System. Windows. Visibility. Visible;
            }
            //关闭键盘
            this. Focus( );
            if ( this.txtSearch != null && this. txtSearch. Text. Trim( ) == "" )
            {
                return;
            }
            if ( this.listBoxHelper != null )
            {
                //清空数据
                this. listBoxHelper. Refresh( );
                listBoxHelper_ReloadDelegate( );
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.Key == Key.Enter)
            {
                this. iconSearch_Click( null, null );
            }
            base. OnKeyDown( e );
        }

        private void radio_Software_Checked(object sender, RoutedEventArgs e)
        {
            //相当于执行 搜索按钮 点击
            iconSearch_Click( null, null );
        }

        private string GetSearchCatalog
        {
            get 
            {
                if ( (bool)this.radio_Software.IsChecked )
                {
                    return "software";
                }
                else if ( (bool)this.radio_Post.IsChecked )
                {
                    return "post";
                }
                else if ( (bool)this.radio_Blog.IsChecked )
                {
                    return "blog";
                }
                else if ( (bool)this.radio_News.IsChecked )
                {
                    return "news";
                }
                else 
                {
                    return "software";
                }
            }
        }

        private void list_Result_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchUnit s = this. list_Result. SelectedItem as SearchUnit;
            this. list_Result. SelectedItem = null;
            if ( s != null )
            {
                Tool. ProcessAppLink( s. url );
            }
        }
        #endregion
    }
}