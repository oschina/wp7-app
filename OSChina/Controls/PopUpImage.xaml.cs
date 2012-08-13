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
using System. Windows. Media. Imaging;
using System. Windows. Shapes;
using Microsoft. Phone. Controls;
using System. Windows. Controls. Primitives;

namespace OSChina. Controls
{
    public partial class PopUpImage : UserControl
    {
        public Popup pop { get; private set; }

        public PopUpImage(  )
        {
            InitializeComponent( );
        }

        public void Create( string imgUrl, double height = 768)
        {
            this. img. Height = this. grid. Height = height;
            this. img. Source = new BitmapImage( new Uri( imgUrl, UriKind. Absolute ) );
            this. pop = new Popup
            {
                Child = this,
                IsOpen = true,
            };
        }


    }
}
