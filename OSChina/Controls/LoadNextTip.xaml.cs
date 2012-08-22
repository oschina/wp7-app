using System;
using System. Windows. Controls;

namespace OSChina. Controls
{
    public partial class LoadNextTip : UserControl
    {
        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
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

        #endregion

        #region Properties
        /// <summary>
        /// 点击事件
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// 获取/设置 显示的文本
        /// </summary>
        public string Text
        {
            get { return this. tblock_Tip. Text; }
            set
            {
                this. tblock_Tip. Text = value;
            }
        }
        #endregion
    }
}