using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.InstructionViewModel;
using SamSoarII.Utility;
using SamSoarII.PLCCompiler;
using SamSoarII.InstructionModel;
using System.Windows;
using System.ComponentModel;
namespace SamSoarII.Project
{
    public class LadderDiagramModel : IComparable, INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public bool IsMainLadder { get; set; }
        private SortedDictionary<IntPoint, BaseViewModel> _ladderElements = new SortedDictionary<IntPoint, BaseViewModel>();
        private SortedDictionary<IntPoint, VerticalLineViewModel> _ladderVerticalLines = new SortedDictionary<IntPoint, VerticalLineViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public LadderDiagramModel(string name)
        {
            Name = name;
            IsMainLadder = false;
        }

        public IEnumerable<BaseViewModel> GetElements()
        {
            return _ladderElements.Values;
        }

        public IEnumerable<VerticalLineViewModel> GetVerticalLines()
        {
            return _ladderVerticalLines.Values;
        }

        public BaseViewModel SearchElemnt(int x, int y)
        {
            return _ladderElements[new IntPoint() { X = x, Y = y }];
        }

        public VerticalLineViewModel SearchVerticalLine(int x, int y)
        {
            return _ladderVerticalLines[new IntPoint() { X = x, Y = y }];
        }

        public BaseViewModel AddElement(BaseViewModel element)
        {
            BaseViewModel old = null;
            IntPoint p = new IntPoint() { X = element.X, Y = element.Y };
            if(_ladderElements.ContainsKey(p))
            {
                old = _ladderElements[p];
                _ladderElements.Remove(p);
            }
            _ladderElements.Add(p, element);
            return old;
        }

        public bool AddVerticalLine(VerticalLineViewModel verticalLine)
        {
            IntPoint p = new IntPoint() { X = verticalLine.X, Y = verticalLine.Y };
            if(_ladderVerticalLines.ContainsKey(p))
            {
                return false;
            }
            _ladderVerticalLines.Add(p, verticalLine);
            return true;
            
        }

        public void ClearElements()
        {
            _ladderElements.Clear();
        }

        public void ClearVerticalLines()
        {
            _ladderVerticalLines.Clear();
        }

        public void Clear()
        {
            ClearElements();
            ClearVerticalLines();
        }

        public void ClearSearchedFlag()
        {            
            foreach (var model in _ladderElements.Values)
            {
                model.IsSearched = false;
            }
            foreach (var model in _ladderVerticalLines.Values)
            {
                model.IsSearched = false;
            }
        }

        public void PreCompile()
        {
            ClearSearchedFlag();
            var rootElements = _ladderElements.Values.Where(x => { return x.Type == ElementType.Output; });
            Queue<BaseViewModel> tempQueue = new Queue<BaseViewModel>(rootElements);
            while(tempQueue.Count > 0)
            {
                var tempElement = tempQueue.Dequeue();
                if(tempElement.Type != ElementType.Null)
                {
                    var templist = SearchFrom(tempElement);
                    foreach (var ele in templist)
                    {
                        tempQueue.Enqueue(ele);
                    }
                }
            }
        }

        public string GenerateCode()
        {
            PreCompile();
            var graph = ConvertToGraph();
            graph.Convert();
            var tree = graph.ConvertToTree();
            tree.TreeName = "network1";
            return tree.GenerateCode();
        }

        private List<BaseViewModel> SearchFrom(BaseViewModel viewmodel)
        {
            if(viewmodel.IsSearched)
            {
                return viewmodel.NextElemnets;
            }
            List<BaseViewModel> result = new List<BaseViewModel>();
            if(viewmodel.X == 0)
            {
                viewmodel.IsSearched = true;
                result.Add(BaseViewModel.Null);
                viewmodel.NextElemnets = result;
                return result;
            }
            else
            {
                int x = viewmodel.X;
                int y = viewmodel.Y;
                var relativePoints = GetRelativePoint(x - 1, y);
                foreach(var p in relativePoints)
                {
                    BaseViewModel leftmodel;
                    if (_ladderElements.TryGetValue(p, out leftmodel))
                    {
                        if (leftmodel.Type == ElementType.HLine)
                        {
                            var _leftresult = SearchFrom(leftmodel);
                            foreach(var temp in SearchFrom(leftmodel))
                            {
                                result.Add(temp);
                            }
                        }
                        else
                        {
                            result.Add(leftmodel);
                        }
                    }
                }
                viewmodel.IsSearched = true;
                viewmodel.NextElemnets = result;
                return result;
            }

        }

        private IEnumerable<IntPoint> GetRelativePoint(int x, int y)
        {
            List<IntPoint> result = new List<IntPoint>();
            Stack<IntPoint> tempStack = new Stack<IntPoint>();
            var p = new IntPoint() { X = x, Y = y };
            
            var q1 = new IntPoint() { X = x, Y = y };
            var q2 = new IntPoint() { X = x, Y = y };
            while(SearchUpVLine(q1))
            {
                q1 = new IntPoint() { X = q1.X, Y = q1.Y - 1 };
                tempStack.Push(q1);
            }
            while(tempStack.Count > 0)
            {
                result.Add(tempStack.Pop());
            }
            result.Add(p);
            while (SearchDownVLine(q2))
            {
                q2 = new IntPoint() { X = q2.X, Y = q2.Y + 1 };
                result.Add(q2);
            }
            return result;           
        } 

        private bool SearchUpVLine(IntPoint p)
        {
            return _ladderVerticalLines.Values.Any(l=> { return (l.X == p.X) && (l.Y == p.Y - 1); });
        }

        private bool SearchDownVLine(IntPoint p)
        {
            return _ladderVerticalLines.Values.Any(l => { return (l.X == p.X) && (l.Y == p.Y); });
        }

        public bool Assert()
        {
            return _ladderElements.Values.All(x => x.Assert());
        }

        public ExpGraph ConvertToGraph()
        {
            return LadderHelper.ConvertGraph(_ladderElements.Values);
        }

        public int CompareTo(object obj)
        {
            var ldmodel = obj as LadderDiagramModel;
            if(IsMainLadder)
            {
                return -1;
            }
            return 0;
        }
    }
}
