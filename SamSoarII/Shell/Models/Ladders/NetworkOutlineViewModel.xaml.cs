using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.Global;
using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;
using System.Windows.Threading;
using System.Threading;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// NetworkOutlineViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkOutlineViewModel : UserControl, IDisposable
    {
        public NetworkOutlineViewModel()
        {
            InitializeComponent();
            LadderScale.ScaleX = GlobalSetting.LadderOriginScaleX / 1.7;
            LadderScale.ScaleY = GlobalSetting.LadderOriginScaleY / 1.7;
            Loaded += OnLoaded;
        }
        
        public void Dispose()
        {
            Loaded -= OnLoaded;
            Core = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IsViewModified = true;
        }
        
        #region Number

        private LadderNetworkModel core;
        public LadderNetworkModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                if (core?.View != null)
                    DynamicDispose();
                this.core = value;
                if (core?.View != null)
                {
                    LadderCanvas.Children.Clear();
                    LadderCanvas.Width = core.View.LadderCanvas.Width;
                    LadderCanvas.Height = core.RowCount * core.View.HeightUnit;
                    Scroll.ScrollToVerticalOffset(0);
                    loadedrowstart = 0;
                    loadedrowend = -1;
                    oldscrolloffset = 0;
                }
            }
        }

        #endregion

        #region Shell

        private bool isviewmodified;
        public bool IsViewModified
        {
            get { return this.isviewmodified; }
            set { this.isviewmodified = value; }
        }

        private int loadedrowstart;
        public int LoadedRowStart { get { return this.loadedrowstart; } }

        private int loadedrowend;
        public int LoadedRowEnd { get { return this.loadedrowend; } }

        private double oldscrolloffset;

        public void DynamicUpdate()
        {
            if (Core?.View == null) return;
            double scaleY = 0;
            Point p = new Point();
            double newscrolloffset = 0;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                scaleY = LadderScale.ScaleY;
                p = LadderCanvas.TranslatePoint(new Point(0, 0), Scroll);
                newscrolloffset = Scroll.VerticalOffset;
            });
            if (core?.View == null) return;
            int _loadedrowstart = 0;
            int _loadedrowend = core.RowCount - 1;
            _loadedrowstart = Math.Max(_loadedrowstart, (int)(-p.Y / (core.View.HeightUnit * scaleY)) - 3);
            _loadedrowend = Math.Min(_loadedrowend, (int)((-p.Y + Scroll.ViewportHeight) / (core.View.HeightUnit * scaleY)) + 3);
            if (_loadedrowstart > _loadedrowend)
            {
                if (loadedrowstart <= loadedrowend)
                {
                    if (newscrolloffset > oldscrolloffset)
                        DisposeRange(loadedrowstart, loadedrowend);
                    else
                        DisposeRange(loadedrowend, loadedrowstart);
                }
            }
            else if (loadedrowstart > _loadedrowend)
            {
                if (newscrolloffset > oldscrolloffset)
                    CreateRange(_loadedrowstart, _loadedrowend);
                else
                    CreateRange(_loadedrowend, _loadedrowstart);
            }
            else
            {
                if (newscrolloffset > oldscrolloffset)
                {
                    if (_loadedrowstart < loadedrowstart)
                        CreateRange(_loadedrowstart, Math.Min(_loadedrowend, loadedrowstart - 1));
                    if (_loadedrowstart > loadedrowstart)
                        DisposeRange(loadedrowstart, _loadedrowstart - 1);
                    if (loadedrowend < _loadedrowend)
                        CreateRange(Math.Max(_loadedrowstart, loadedrowend + 1), _loadedrowend);
                    if (loadedrowend > _loadedrowend)
                        DisposeRange(_loadedrowend + 1, loadedrowend);
                }
                else
                {
                    if (_loadedrowstart < loadedrowstart)
                        CreateRange(Math.Min(_loadedrowend, loadedrowstart - 1), _loadedrowstart);
                    if (_loadedrowstart > loadedrowstart)
                        DisposeRange(_loadedrowstart - 1, loadedrowstart);
                    if (loadedrowend < _loadedrowend)
                        CreateRange(_loadedrowend, Math.Max(_loadedrowstart, loadedrowend + 1));
                    if (loadedrowend > _loadedrowend)
                        DisposeRange(loadedrowend, _loadedrowend + 1);
                }
            }
            loadedrowstart = _loadedrowstart;
            loadedrowend = _loadedrowend;
            oldscrolloffset = newscrolloffset;
        }
        
        public void DynamicDispose()
        {
            if (Core?.View == null) return;
            if (loadedrowstart <= loadedrowend)
            {
                DisposeRange(loadedrowstart, loadedrowend);
                loadedrowstart = 0;
                loadedrowend = -1;
            }
        }
        
        private void CreateRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    IEnumerable<LadderUnitModel> units = Core.Children.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y);
                    units = units.Concat(Core.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y));
                    foreach (LadderUnitModel unit in units)
                    {
                        if (unit.Visual == null)
                        {
                            unit.Visual = BaseVisualUnitModel.Create(unit);
                            foreach (var kvPair in unit.Visual.Visuals)
                            {
                                for (int i = 0; i < kvPair.Value.Length; i++)
                                {
                                    if (kvPair.Value[i] != null)
                                        LadderCanvas.AddVisual(kvPair.Value[i]);
                                }
                            }
                        }
                    }
                });
            }
        }

        private void DisposeRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    if (Core == null) return;
                    IEnumerable<LadderUnitModel> units = Core.Children.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y);
                    units = units.Concat(Core.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y));
                    foreach (LadderUnitModel unit in units)
                    {
                        if (unit.Visual != null)
                        {
                            unit.Visual.Dispose();
                        }
                    }
                });
            }
        }

        #endregion
    }
}
