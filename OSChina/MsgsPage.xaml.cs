/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using Microsoft. Phone. Controls;

namespace OSChina
{
    public partial class MsgsPage : PhoneApplicationPage
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public MsgsPage( )
        {
            InitializeComponent( );
        }
        #endregion

        #region Private functions
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        #endregion
    }
}