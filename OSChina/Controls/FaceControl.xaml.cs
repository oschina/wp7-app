using System. Windows. Controls;
using System. Windows. Input;
using System. Windows. Media;

namespace OSChina. Controls
{
    public partial class FaceControl : UserControl
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public FaceControl( )
        {
            InitializeComponent( );
        }
        #endregion

        #region Private functions
        /// <summary>
        /// 点击表情变色
        /// </summary>
        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            sp. Background = new SolidColorBrush( Color. FromArgb( 255, 142, 142, 142 ) );
        }

        /// <summary>
        /// 恢复表情颜色
        /// </summary>
        private void StackPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            sp. Background = new SolidColorBrush( Color. FromArgb( 255, 74, 73, 74 ) );
        }

        /// <summary>
        /// 选择了表情
        /// </summary>
        private void StackPanel_Tap(object sender, GestureEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            if ( sp. Tag != null )
            {
                EventSingleton. Instance. RaiseNewFace( sp. Tag. ToString( ) );
            }
        }
        #endregion
    }
}
