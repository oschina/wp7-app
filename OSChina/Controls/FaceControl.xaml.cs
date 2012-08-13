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

namespace OSChina. Controls
{
    public partial class FaceControl : UserControl
    {
        public FaceControl( )
        {
            InitializeComponent( );
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            sp. Background = new SolidColorBrush( Color. FromArgb( 255, 142, 142, 142 ) );
        }


        private void StackPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            sp. Background = new SolidColorBrush( Color. FromArgb( 255, 74, 73, 74 ) );
           
        }

        private void StackPanel_Tap(object sender, GestureEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            if ( sp. Tag != null )
            {
                EventSingleton. Instance. RaiseNewFace( sp. Tag. ToString( ) );
            }
        }

     
    }
}
