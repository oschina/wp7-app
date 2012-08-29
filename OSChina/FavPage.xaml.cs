/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using Microsoft. Phone. Controls;
using OSChina. Model. AppOnly;

namespace OSChina
{
    public partial class FavPage : PhoneApplicationPage
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public FavPage( )
        {
            InitializeComponent( );

            this. fav_Software. FavType = FavType. Software;
            this. fav_Post. FavType = FavType. Post;
            this. fav_Code. FavType = FavType. Code;
            this. fav_Blog. FavType = FavType. Blog;
            this. fav_News. FavType = FavType. News;
            this. Loaded += (s, e) =>
                {
                    this. fav_Software. listBoxHelper_ReloadDelegate( );
                    this. fav_Post. listBoxHelper_ReloadDelegate( );
                    this. fav_Code. listBoxHelper_ReloadDelegate( );
                    this. fav_Blog. listBoxHelper_ReloadDelegate( );
                    this. fav_News. listBoxHelper_ReloadDelegate( );
                };
        }
        #endregion

        #region Private functions
        /// <summary>
        /// 离开页面时触发
        /// </summary>
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        #endregion
    }
}