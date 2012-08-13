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
    public partial class LoadNextTip : UserControl
    {
        public LoadNextTip( )
        {
            InitializeComponent( );
            this. MouseLeftButtonDown += (s, e) =>
            {
                if ( this. Click != null )
                {
                    this. Click( this, null );
                }
            };
        }

        public event EventHandler Click;

        public string Text
        {
            get { return this. tblock_Tip. Text; }
            set
            {
                this. tblock_Tip. Text = value;
            }
        }
    }
}