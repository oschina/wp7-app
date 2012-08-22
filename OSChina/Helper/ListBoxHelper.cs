using System;
using System. Collections. Generic;
using System. Collections. ObjectModel;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Media;
using OSChina. Controls;
using WP7_ControlsLib. Controls;

namespace OSChina
{
    public class ListBoxHelper
    {
        public bool isLoadOver { get; set; }
        public bool isLoading { get; set; }
        public ObservableCollection<object> datas { get; set; }
        private ScrollViewer listBox_ScrollViewer;
        private double lastListBoxScrollableVerticalOffset = 0;
        public int allCount;
        //刷新验证专用
        public List<int> ids4Refresh = new List<int>( );

        private ListBox listBox { get; set; }

        public ListBoxHelper( ListBox listBox, bool isWhite = false, bool isBlack = false,  bool isNeedFirstTip = true )
        {
            this. listBox = listBox;
            this. datas = new ObservableCollection<object>( );
            if ( isNeedFirstTip )
            {
                LoadNextTip tip = this. GetLoadTip;
                if ( isWhite )
                {
                    tip. Foreground = new SolidColorBrush( Colors. White );
                }
                if ( isBlack )
                {
                    tip. Foreground = new SolidColorBrush( Colors. Black );
                }
                this. datas. Add( tip );
            }
            this. listBox. ItemsSource = this. datas;
            this. loadNextTip. Click += (s, e) =>
            {
                this. Reload( );
            };
            //滚动检测
            this. listBox. LayoutUpdated += new EventHandler( listBox_LayoutUpdated );
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear( )
        {
            this. ids4Refresh. Clear( );
            this. datas. Clear( );
            this. ReloadDelegate = null;
        }

        void listBox_LayoutUpdated(object sender, EventArgs e)
        {
            if ( this. listBox_ScrollViewer != null && isLoadOver == false)
            {
                //是否往下滑动
                bool isGoingDown = this. listBox_ScrollViewer. VerticalOffset > lastListBoxScrollableVerticalOffset;
                this. lastListBoxScrollableVerticalOffset = this. listBox_ScrollViewer. VerticalOffset;
                //是否在监控的最底部区域
                bool isInReloadRegion = this. listBox_ScrollViewer. VerticalOffset >= ( this. listBox_ScrollViewer. ScrollableHeight - Config. ScrollBarOffset );
                if ( isInReloadRegion &&
                    isGoingDown )
                {
                    this. Reload( );
                }
            }
            else
            {
                this. listBox_ScrollViewer = ControlHelper. FindChildOfType<ScrollViewer>( this. listBox );
            }
        }

        /// <summary>
        /// 刷新操作
        /// </summary>
        public void Refresh( )
        {
            isLoading = false;
            isLoadOver = false;
            this. datas. Clear( );
            this. datas. Add( this. GetLoadTip );
            allCount = 0;
            this. Reload( );
        }

        public event Action ReloadDelegate;
        private void Reload()
        {
            if ( this.ReloadDelegate  != null)
            {
                this. ReloadDelegate( );
            }
        }

        /// <summary>
        /// 获取最下面的继续加载提示符
        /// </summary>
        public LoadNextTip GetLoadTip
        {
            get { return isLoadOver ? this. loadOverTip : this. loadNextTip; }
        }
        private LoadNextTip loadNextTip = new LoadNextTip
        {
            Text = "正在刷新",
            HorizontalAlignment = HorizontalAlignment. Center,
            HorizontalContentAlignment = HorizontalAlignment. Center,
            VerticalContentAlignment = System.Windows.VerticalAlignment.Top,
            Width = 480,
            MinWidth = 480,
            Height = 100, 
            MinHeight = 100,
        };
        private LoadNextTip loadOverTip = new LoadNextTip
        {
            Text = "已经加载所有项",
            HorizontalAlignment = HorizontalAlignment. Center,
            HorizontalContentAlignment = HorizontalAlignment. Center,
            VerticalContentAlignment = System. Windows. VerticalAlignment. Top,
            Width = 480,
            MinWidth = 480,
            Height = 100,
            MinHeight = 100,
        };
    }
}
