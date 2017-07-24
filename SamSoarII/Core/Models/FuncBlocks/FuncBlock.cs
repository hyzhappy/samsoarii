using SamSoarII.Core.Simulate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SamSoarII.Core.Models
{
    /// <summary>
    /// 所有元素模型的超级抽象类
    /// </summary>
    abstract public class FuncBlock
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock(FuncBlockModel _model)
        {
            this.model = _model;
            childrens = new LinkedList<FuncBlock>();
            Current = null;
            Namespace = String.Empty;
        }

        #region Numbers

        #region Parent Model
        /// <summary>
        /// 总控模型
        /// </summary>
        protected FuncBlockModel model;
        /// <summary>
        /// 总控模型
        /// </summary>
        public FuncBlockModel Model
        {
            get { return this.model; }
        }
        /// <summary>
        /// 变量索引表
        /// </summary>
        protected Dictionary<string, SortedList<int, FuncBlock_Assignment>> Assigns
        {
            get
            {
                if (model == null) return null;
                return model.Assigns;
            }
        }
        #endregion

        #region Namespace

        /// <summary>
        /// 当前元素所在的命名空间
        /// </summary>
        protected string nspace;
        /// <summary>
        /// 当前元素所在的命名空间
        /// </summary>
        public string Namespace
        {
            get { return this.nspace; }
            set { this.nspace = value; }
        }

        #endregion

        #region Code Position System

        /// <summary>
        /// 这个元素的开始位置
        /// </summary>
        protected int indexstart;
        /// <summary>
        /// 这个元素的开始位置
        /// </summary>
        public int IndexStart
        {
            get { return this.indexstart; }
            set { this.indexstart = value; }
        }
        /// <summary>
        /// 这个元素的结束位置
        /// </summary>
        protected int indexend;
        /// <summary>
        /// 这个元素的结束位置
        /// </summary>
        public int IndexEnd
        {
            get { return this.indexend; }
            set { this.indexend = value; }
        }
        /// <summary>
        /// 这个元素的长度
        /// </summary>
        public int Length
        {
            get { return indexend - indexstart + 1; }
        }
        /// <summary>
        /// 判断是否包含这个位置
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns></returns>
        public bool Contains(int index)
        {
            return index >= IndexStart && index <= IndexEnd;
        }
        /// <summary>
        /// 内部的位移标记，表示这个元素增加或者减少了多少字符单位
        /// </summary>
        /// <remarks>
        /// 一般情况下，当用户向这个元素内输入文本时，会改变宽度和这个标记。
        /// 作了这个标记后，当用户要跳转到后面的元素时，可以用这个标记来对后面部分进行位移。
        /// 但是可以作为代替，沿着结构树向上将经过路径的下一个节点记录外部位移标记，这样内部位移标记就不用保存了。
        /// </remarks>
        public int InnerOffset
        {
            // 内部标记不用保存，始终输出0
            get { return 0; }
            // 设置内部标记
            set
            {
                // 增加当前元素的宽度
                IndexEnd += value;
                // 若存在光标，肯定在近点的位置。若不存在光标，说明不存在子节点。
                if (Current != null)
                {
                    // 光标正好在当前位置的后面
                    if (Current.Value.IndexStart > model.CurrentIndex)
                    {
                        // 设置光标处节点的外部标记
                        Current.Value.OuterOffset += value;
                    }
                    // 光标在当前位置的前面，并且存在光标后面的节点
                    else if (Current.Next != null)
                    {
                        // 设置后面节点的外部标记
                        Current.Next.Value.OuterOffset += value;
                    }
                }
                // 递归向上处理
                if (Parent != null)
                {
                    Parent.InnerOffset += value;
                }
            }
        }
        /// <summary>
        /// 外部的位移标记，表示这个元素【之前】增加或者减少了多少字符单位，需要对坐标进行相应的位移。
        /// </summary>
        protected int outeroffset;
        /// <summary>
        /// 外部的位移标记，表示这个元素【之前】增加或者减少了多少字符单位，需要对坐标进行相应的位移。
        /// </summary>
        /// <remarks>
        /// 设置标记以及坐标即可，剩下的交给之后处理
        /// </remarks>
        public int OuterOffset
        {
            get { return this.outeroffset; }
            set
            {
                this.IndexStart += value - this.outeroffset;
                this.IndexEnd += value - this.outeroffset;
                this.outeroffset = value;
            }
        }
        /// <summary>
        /// 将标记“丢”给所有子节点，同时清空当前节点和所有子节点的外部标记
        /// </summary>
        /// <remarks>
        /// 要求执行这个方法之后，这个节点和子节点的位置正确，所以需要一次运行来确定位置。
        /// 每个子节点若存在子节点，则将标记继续传给第一个子节点，保证之后的传递。
        /// </remarks>
        public void CastOffset()
        {
            // 统计外部标记的和，表示当前扫描到的位置的位移
            int offsetcount = OuterOffset;
            foreach (FuncBlock child in Childrens)
            {
                // 给子节点累加标记和
                child.OuterOffset += offsetcount;
                offsetcount = child.OuterOffset;
                // 存在子节点则继续向下传递
                if (child.Childrens.Count() > 0)
                {
                    child.Childrens.First().OuterOffset += child.OuterOffset;
                }
                // 清空这个子节点的标记，防止二次扫描重复累加
                child.outeroffset = 0;
            }
            // 完成标记下传，清空标记
            this.outeroffset = 0;
        }
        /// <summary>
        /// 当前缩进的高度
        /// </summary>
        protected int height;
        /// <summary>
        /// 当前缩进的高度
        /// </summary>
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        #endregion

        #region Tree Structure System

        /// <summary>
        /// 父节点
        /// </summary>
        protected FuncBlock parent;
        /// <summary>
        /// 父节点
        /// </summary>
        public FuncBlock Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }
        /// <summary>
        /// 所有子节点
        /// </summary>
        protected LinkedList<FuncBlock> childrens;
        /// <summary>
        /// 所有子节点
        /// </summary>
        public IEnumerable<FuncBlock> Childrens
        {
            get { return this.childrens; }
        }
        /// <summary>
        /// 光标节点
        /// </summary>
        protected LinkedListNode<FuncBlock> current;
        /// <summary>
        /// 光标节点
        /// </summary>
        public LinkedListNode<FuncBlock> Current
        {
            get { return this.current; }
            set { this.current = value; }
        }

        #endregion

        #region Breakpoint System
        /// <summary>
        /// 是否是断点
        /// </summary>
        public bool IsBreakpoint { get; set; }

        /// <summary>
        /// 断点地址
        /// </summary>
        public int BPAddress { get; set; }

        /// <summary>
        /// 断点光标
        /// </summary>
        private BreakpointCursor bpCursor;

        /// <summary>
        /// 断点光标
        /// </summary>
        public BreakpointCursor BPCursor
        {
            get
            {
                return this.bpCursor;
            }
            set
            {
                this.bpCursor = value;
            }
        }
        
        /// <summary>
        /// 获得所有局部变量
        /// </summary>
        public IList<FuncBlock_Assignment> LocalVars
        {
            get
            {
                List<FuncBlock> parents = new List<FuncBlock>();
                parents.Add(this);
                FuncBlock _parent = Parent;
                while (_parent != null)
                {
                    parents.Add(_parent);
                    _parent = _parent.Parent;
                }
                List<FuncBlock_Assignment> assigns = new List<FuncBlock_Assignment>();
                for (int i = parents.Count() - 1; i >= 0; i--)
                {
                    _parent = parents[i];
                    foreach (FuncBlock child in _parent.Childrens)
                    {
                        if (child is FuncBlock_Assignment)
                        {
                            assigns.Add((FuncBlock_Assignment)child);
                        }
                        if (child is FuncBlock_AssignmentSeries)
                        {
                            foreach (FuncBlock_Assignment assign in ((FuncBlock_AssignmentSeries)child).Defines)
                            {
                                assigns.Add(assign);
                            }
                        }
                    }
                }
                return assigns;
            }
        }

        /// <summary>
        /// 获得所有函数参数
        /// </summary>
        public IList<FuncBlock_Assignment> Parameters
        {
            get
            {
                FuncBlock _parent = this;
                while (_parent != null)
                {
                    if (_parent is FuncBlock_Local)
                    {
                        FuncBlock_Local fblockl = (FuncBlock_Local)_parent;
                        if (fblockl.VirtualAssigns != null)
                            return fblockl.VirtualAssigns.ToArray();
                    }
                    _parent = _parent.Parent;
                }
                return new FuncBlock_Assignment[0];
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// 添加一个新的子节点
        /// </summary>
        /// <param name="newchild">新的子节点</param>
        public virtual void AddChildren(FuncBlock newchild)
        {
            if (newchild.IndexEnd <= newchild.IndexStart)
                return;
            // 可注释的元素
            IFuncBlock_IsCommentable fbic = null;
            // 函数头元素
            FuncBlock_FuncHeader fbfh = null;
            // 局部区域元素
            FuncBlock_Local fbl = null;
            // 标题注释元素
            FuncBlock_Comment fbc = null;
            // 设置父亲
            newchild.Parent = this;
            // 区域元素需要设置局部的命名空间，用当前的命名空间+ID来命名，并且缩进高度加一
            if (newchild is FuncBlock_Local)
            {
                fbl = (FuncBlock_Local)(newchild);
                fbl.Namespace = String.Format("{0:s}::{1:d}", Namespace, fbl.ID);
                fbl.Height = Height + 1;
            }
            // 保持命名空间和缩进高度
            else
            {
                newchild.Namespace = Namespace;
                newchild.Height = Height;
            }
            // 如果光标为空，则设置新节点为光标
            if (Current == null)
            {
                childrens.AddLast(newchild);
                Current = childrens.Find(newchild);
            }
            // 如果新节点在光标前，插入光标前面并设为新光标
            else if (Current.Value.IndexStart > newchild.IndexEnd)
            {
                childrens.AddBefore(Current, newchild);
                Current = Current.Previous;
            }
            // 如果新节点在光标后，插入光标后面并设为新光标
            else if (Current.Value.IndexEnd < newchild.IndexStart)
            {
                childrens.AddAfter(Current, newchild);
                Current = Current.Next;
            }
            // 存在元素相交情况则报错
            else
            {
                throw new ArgumentException(String.Format("{0:s} interact with {1:s}", newchild, Current.Value));
            }
            fbfh = null;
            fbl = null;
            // 如果新节点为函数头，后面的是区域
            if (Current.Value is FuncBlock_FuncHeader
             && Current.Next != null
             && Current.Next.Value is FuncBlock_Local)
            {
                fbfh = (FuncBlock_FuncHeader)(Current.Value);
                fbl = (FuncBlock_Local)(Current.Next.Value);
            }
            // 如果新节点为区域，前面的是函数头
            if (Current.Value is FuncBlock_Local &&
                Current.Previous != null &&
                Current.Previous.Value is FuncBlock_FuncHeader)
            {
                fbfh = (FuncBlock_FuncHeader)(Current.Previous.Value);
                fbl = (FuncBlock_Local)(Current.Value);
            }
            // 上述两种情况可以组成函数
            if (fbl != null && fbfh != null)
            {
                fbl.Header = fbfh;
                fbfh.Block = fbl;
            }
            fbic = null;
            fbc = null;
            // 如果新节点为一行注释，后面的是可注释的元素
            if (Current.Value is FuncBlock_Comment
             && Current.Next != null
             && Current.Next.Value is IFuncBlock_IsCommentable)
            {
                fbic = (IFuncBlock_IsCommentable)(Current.Next.Value);
                fbc = (FuncBlock_Comment)(Current.Value);
            }
            // 如果新节点为是可注释的元素，前面的是注释
            if (Current.Value is IFuncBlock_IsCommentable
             && Current.Previous != null
             && Current.Previous.Value is FuncBlock_Comment)
            {
                fbic = (IFuncBlock_IsCommentable)(Current.Value);
                fbc = (FuncBlock_Comment)(Current.Previous.Value);
            }
            // 上述两种情况可以组成已注释的元素
            if (fbc != null && fbic != null)
            {
                fbic.Comment = fbc;
            }
        }
        /// <summary>
        /// 删除一个子节点
        /// </summary>
        /// <param name="delchild">要删除的子节点</param>
        public virtual void RemoveChildren(FuncBlock delchild)
        {
            // 若删除的是光标所在位置，优先向前移动
            if (Current.Value == delchild)
            {
                if (Current.Previous == null)
                {
                    Current = Current.Next;
                }
                else
                {
                    Current = Current.Previous;
                }
            }
            // 移除这个节点的逻辑关系
            delchild.Parent = null;
            childrens.Remove(delchild);
            // 如果是总控模型的光标所在位置
            if (model.Current == delchild)
            {
                // 若当前节点的光标非空，则设为这个节点的光标
                if (Current != null)
                {
                    model.CurrentNode = Current;
                }
                // 如果父节点非空，则设为父节点的光标
                else if (Parent != null)
                {
                    model.CurrentNode = Parent.Current;
                }
                // 新建一个该节点的光标链点
                else
                {
                    model.CurrentNode = new LinkedListNode<FuncBlock>(this);
                }
                // 重新移动到当前位置
                model.Move(model.CurrentIndex);
            }
        }
        /// <summary>
        /// 根据给定的文本和范围，重构这个范围
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public virtual void Build(string text, int start, int end, int offset = 0)
        {
            // 设置修改标记
            Model.Parent.InvokeModify(Model);
            // 括号栈
            Stack<char> bracketstack = new Stack<char>();
            // 匹配到的左括号
            char leftbracket = '\0';
            // 当前括号的起点
            int bracketstart = 0;
            // 当前括号的终点
            int bracketend = 0;
            // 上一个分号的位置，默认为范围前一位
            int dividelabel = start - 1;
            // 当前语句的开始
            int stmtstart = 0;
            // 当前语句的结尾
            int stmtend = 0;
            // 当前语句
            string statement = String.Empty;
            // 当前注释的开始
            int commentstart = 0;
            // 当前注释的结尾
            int commentend = 0;
            // 注释节点
            FuncBlock comment = null;
            // 当前节点
            FuncBlock child = null;
            // 若子节点非空，需要清除范围内或者与范围相交的节点
            if (Childrens.Count() > 0)
            {
                // 查找与范围相交的第一个节点
                LinkedListNode<FuncBlock> first = childrens.First;
                // 查找与范围相交的最后一个节点
                LinkedListNode<FuncBlock> last = childrens.Last;
                while (first != null && first.Value.IndexEnd < start)
                    first = first.Next;
                while (last != null && last.Value.IndexStart > end)
                    last = last.Previous;
                // 若第一个节点到达末尾，说明所有节点都在范围前，将光标设为最后一个节点
                if (first == null)
                {
                    Current = childrens.Last;
                }
                // 若最后一个节点到达开头，说明所有节点都在范围后，将光标设为第一个节点
                else if (last == null)
                {
                    Current = childrens.First;
                }
                // 存在与范围相交的节点
                else
                {
                    // 若范围内第一个节点存在之前的节点，将光标设为此处
                    // 同时设置新的范围起点
                    if (first.Previous != null)
                    {
                        Current = first.Previous;
                        start = Current.Value.IndexEnd + 1;
                    }
                    // 否则将光标设为范围后，设新的范围起点为这个节点的开始位置
                    else
                    {
                        Current = last.Next;
                        start = IndexStart;
                    }
                    // 设置新的范围终点
                    if (last.Next != null)
                    {
                        // 注意区域块不包含前括号，需要移到前括号的前面
                        if (last.Next.Value is FuncBlock_Local)
                        {
                            end = last.Next.Value.IndexStart - 2;
                        }
                        else
                        {
                            end = last.Next.Value.IndexStart - 1;
                        }
                    }
                    else
                    {
                        end = IndexEnd - 1;
                    }
                    // 删除范围内的节点
                    List<LinkedListNode<FuncBlock>> removenodes = new List<LinkedListNode<FuncBlock>>();
                    for (LinkedListNode<FuncBlock> node = first; node != null && node.Value.IndexEnd <= last.Value.IndexEnd; node = node.Next)
                    {
                        removenodes.Add(node);
                    }
                    foreach (LinkedListNode<FuncBlock> removenode in removenodes)
                    {
                        FuncBlock fb = removenode.Value;

                        // 需要删除变量赋值语句的信息
                        if (fb is FuncBlock_Assignment)
                        {
                            FuncBlock_Assignment fba = (FuncBlock_Assignment)(fb);
                            fba.Name = String.Empty;
                        }
                        // 需要删除变量多重定义语句的信息
                        if (fb is FuncBlock_AssignmentSeries)
                        {
                            FuncBlock_AssignmentSeries fbas = (FuncBlock_AssignmentSeries)(fb);
                            fbas.ClearDefines();
                        }
                        // 需要删除函数头部的参数信息
                        if (fb is FuncBlock_FuncHeader)
                        {
                            FuncBlock_FuncHeader fbf = (FuncBlock_FuncHeader)(fb);
                            if (fbf.Block != null)
                            {
                                fbf.Block.Header = null;
                            }
                        }
                        childrens.Remove(removenode);
                    }
                }
            }
            // 设置位移标记
            InnerOffset += offset;
            end += offset;
            CastOffset();
            // 从前到后扫描
            for (int i = start; i <= end; i++)
            {
                Match blankMatch = null;
                if (i >= text.Length) break;
                // 注意标志符号
                switch (text[i])
                {
                    // 括号头部设置括号起点，并且增加括号层数
                    case '{':
                        // 如果当前为注释区域，则不处理这个括号符号，下同
                        if (comment != null) break;
                        // 如果之前没有其他括号，则视为区域的头部
                        if (bracketstack.Count() == 0)
                            bracketstart = i;
                        bracketstack.Push('{');
                        break;
                    // 括号尾部设置括号终点，减少括号层数
                    case '}':
                        if (comment != null) break;
                        // 不断从括号栈中弹出括号，直到栈内为空，或者找到匹配的括号为止
                        leftbracket = '\0';
                        while (bracketstack.Count() > 0
                            && (leftbracket = bracketstack.Pop()) != '{') ;
                        // 最外层括号结尾建立新的局部区域
                        if (bracketstack.Count() == 0 && leftbracket == '{')
                        {
                            bracketend = i;
                            child = new FuncBlock_Local(model);
                            child.IndexStart = bracketstart + 1;
                            child.IndexEnd = bracketend;
                            AddChildren(child);
                            if (bracketstart + 1 < bracketend - 1)
                            {
                                child.Build(text, bracketstart + 1, bracketend - 1);
                            }
                            dividelabel = i;
                        }
                        break;
                    // 其他括号的开头
                    case '(':
                    case '[':
                        if (comment != null) break;
                        bracketstack.Push(text[i]);
                        break;
                    // 圆括号结尾可能是函数 & for & while头部
                    case ')':
                        if (comment != null) break;
                        // 不断从括号栈中弹出括号，直到栈内为空，或者找到匹配的括号为止
                        leftbracket = '\0';
                        while (bracketstack.Count() > 0
                            && (leftbracket = bracketstack.Pop()) != '(') ;
                        // 检查是否为函数头部
                        if (this is FuncBlock_Root && bracketstack.Count() == 0 && leftbracket == '(')
                        {
                            int hstart = dividelabel + 1;
                            int hend = i;
                            string htext = text.Substring(hstart, hend - hstart + 1);
                            Match m1 = Regex.Match(htext, @"([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\((.*)\)\s*$");
                            // 如果符合函数头部的正则表达式格式
                            if (m1.Success)
                            {
                                hstart += m1.Index;
                                //hstart = hend - m1.Value.Length + 1;
                                FuncModel func = new FuncModel();
                                func.Offset = hstart;
                                func.ReturnType = Regex.Replace(m1.Groups[1].Value, @"\s*", String.Empty);
                                func.Name = m1.Groups[3].Value;
                                if (m1.Groups[4].Value.Length == 0)
                                {
                                    func.ArgCount = 0;
                                }
                                else
                                {
                                    string[] argtexts = m1.Groups[4].Value.Split(',');
                                    func.ArgCount = argtexts.Length;
                                    // 检查内部的参数是否符合格式
                                    for (int j = 0; j < argtexts.Length; j++)
                                    {
                                        Match m2 = Regex.Match(argtexts[j], @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\s*$");
                                        if (m2.Success)
                                        {
                                            func.SetArgType(j, Regex.Replace(m2.Groups[1].Value, @"\s*", String.Empty));
                                            func.SetArgName(j, m2.Groups[3].Value);
                                        }
                                        else
                                        {
                                            func = null;
                                            break;
                                        }
                                    }
                                }
                                // 建立函数头部
                                if (func != null)
                                {
                                    FuncBlock_FuncHeader newblock = new FuncBlock_FuncHeader(model);
                                    newblock.FuncModel = func;
                                    newblock.IndexStart = hstart;
                                    newblock.IndexEnd = newblock.IndexStart + m1.Value.Length - 1;
                                    AddChildren(newblock);
                                    if (newblock.Comment != null
                                     && newblock.Comment.IndexEnd - newblock.Comment.IndexStart - 2 > 0)
                                    {
                                        newblock.FuncModel.Comment
                                            = text.Substring(
                                                newblock.Comment.IndexStart + 1,
                                                newblock.Comment.IndexEnd - newblock.Comment.IndexStart - 2);
                                    }
                                }
                            }
                        }
                        // 检查是否为for & while循环头部
                        if (this is FuncBlock_Local && bracketstack.Count() == 0 && leftbracket == '(')
                        {
                            int hstart = dividelabel + 1;
                            int hend = i;
                            string htext = text.Substring(hstart, hend - hstart + 1);
                            Match m1 = Regex.Match(htext, @"for\s*\(\s*([^;]*;)\s*([^;]*;)\s*([^\)]*)\s*\)\s*$");
                            Match m2 = Regex.Match(htext, @"while\s*\(\s*([^\)]*)\s*\)\s*$");
                            Match m3 = Regex.Match(htext, @"if\s*\(\s*([^\)]*)\s*\)\s*$");
                            // for循环头部
                            if (m1.Success)
                            {
                                FuncBlock_ForHeader newblock = new FuncBlock_ForHeader(model);
                                newblock.Parent = this;
                                newblock.Namespace = Namespace;
                                newblock.IndexStart = hstart + m1.Index;
                                newblock.IndexEnd = newblock.IndexStart + m1.Value.Length - 1;
                                AddChildren(newblock);
                                if (m1.Groups[1].Length > 0)
                                {
                                    newblock.Start = new FuncBlock_Statement(model);
                                    newblock.Start.Parent = newblock;
                                    newblock.Start.Namespace = Namespace;
                                    newblock.Start.IndexStart = hstart + m1.Groups[1].Index;
                                    newblock.Start.IndexEnd = newblock.Start.IndexStart + m1.Groups[1].Length - 1;
                                }
                                if (m1.Groups[2].Length > 0)
                                {
                                    newblock.Cond = new FuncBlock_Statement(model);
                                    newblock.Cond.Parent = newblock;
                                    newblock.Cond.Namespace = Namespace;
                                    newblock.Cond.IndexStart = hstart + m1.Groups[2].Index;
                                    newblock.Cond.IndexEnd = newblock.Cond.IndexStart + m1.Groups[2].Length - 1;
                                }
                                if (m1.Groups[3].Length > 0)
                                {
                                    newblock.Next = new FuncBlock_Statement(model);
                                    newblock.Next.Parent = newblock;
                                    newblock.Next.Namespace = Namespace;
                                    newblock.Next.IndexStart = hstart + m1.Groups[3].Index;
                                    newblock.Next.IndexEnd = newblock.Next.IndexStart + m1.Groups[3].Length - 1;
                                }
                            }
                            // while循环头部
                            if (m2.Success)
                            {
                                FuncBlock_WhileHeader newblock = new FuncBlock_WhileHeader(model);
                                newblock.IndexStart = hstart + m1.Index;
                                newblock.IndexEnd = newblock.IndexStart + m1.Value.Length - 1;
                                AddChildren(newblock);
                                if (m1.Groups[1].Length > 0)
                                {
                                    newblock.Cond = new FuncBlock_Statement(model);
                                    newblock.Cond.Parent = newblock;
                                    newblock.Cond.Namespace = Namespace;
                                    newblock.Cond.IndexStart = hstart + m1.Groups[1].Index;
                                    newblock.Cond.IndexEnd = newblock.Cond.IndexStart + m1.Groups[1].Length - 1;
                                }
                            }
                            // if循环头部
                            if (m3.Success)
                            {
                                FuncBlock_IfHeader newblock = new FuncBlock_IfHeader(model);
                                newblock.IndexStart = hstart + m1.Index;
                                newblock.IndexEnd = newblock.IndexStart + m1.Value.Length - 1;
                                AddChildren(newblock);
                                if (m1.Groups[1].Length > 0)
                                {
                                    newblock.Cond = new FuncBlock_Statement(model);
                                    newblock.Cond.Parent = newblock;
                                    newblock.Cond.Namespace = Namespace;
                                    newblock.Cond.IndexStart = hstart + m1.Groups[1].Index;
                                    newblock.Cond.IndexEnd = newblock.Cond.IndexStart + m1.Groups[1].Length - 1;
                                }
                            }
                        }
                        //bracketcount--;
                        break;
                    case ']':
                        if (comment != null) break;
                        leftbracket = '\0';
                        // 不断从括号栈中弹出括号，直到栈内为空，或者找到匹配的括号为止
                        while (bracketstack.Count() > 0
                            && (leftbracket = bracketstack.Pop()) != '[') ;
                        break;
                    // 分号表示一条语句的末尾
                    case ';':
                        if (comment != null)
                        {
                            break;
                        }
                        if (bracketstack.Count() == 0)
                        {
                            stmtstart = dividelabel + 1;
                            stmtend = i;
                            statement = text.Substring(stmtstart, stmtend - stmtstart + 1);
                            blankMatch = Regex.Match(statement, @"^\s*");
                            if (blankMatch.Success)
                            {
                                stmtstart += blankMatch.Value.Length;
                                statement = text.Substring(stmtstart, stmtend - stmtstart + 1);
                            }
                            // 检查是否符合变量声明语句的格式，并建立对应的语句
                            if (FuncBlock_Assignment.TextSuit(statement))
                            {
                                child = new FuncBlock_Assignment(model, this, statement);
                            }
                            // 检查是否符合连续变量声明语句的格式，并建立对应的语句
                            else if (FuncBlock_AssignmentSeries.TextSuit(statement))
                            {
                                child = new FuncBlock_AssignmentSeries(model);
                                child.Parent = this;
                                child.Build(text, stmtstart, stmtend);
                            }
                            else
                            {
                                // 检查是否符合while循环尾部的格式，并建立对应的语句
                                Match m1 = Regex.Match(statement, @"^\s*while\s*\(\s*([^\)]*)\s*\)\s*;\s*$");
                                if (m1.Success)
                                {
                                    FuncBlock_WhileEnd childwe = new FuncBlock_WhileEnd(model);
                                    childwe.Parent = this;
                                    childwe.Namespace = Namespace;
                                    if (m1.Groups[1].Length > 0)
                                    {
                                        childwe.Cond = new FuncBlock_Statement(model);
                                        childwe.Cond.Parent = childwe;
                                        childwe.Cond.Namespace = Namespace;
                                        childwe.Cond.IndexStart = stmtstart + m1.Groups[1].Index;
                                        childwe.Cond.IndexEnd = childwe.IndexStart + m1.Groups[1].Length - 1;
                                    }
                                    child = childwe;
                                }
                                else
                                {
                                    child = new FuncBlock_Statement(model);
                                }
                            }
                            child.IndexStart = stmtstart;
                            child.IndexEnd = stmtend;
                            AddChildren(child);
                            dividelabel = i;
                        }
                        break;
                    // 出现斜杠符
                    case '/':
                        // 行注释的开始符(//)
                        if (comment == null)
                        {
                            if (i > 0 && text[i - 1] == '/')
                            {
                                commentstart = i + 1;
                                comment = new FuncBlock_Comment(model);
                            }
                        }
                        // 段注释的结尾符(*/)
                        else if (comment is FuncBlock_CommentParagraph)
                        {
                            if (i > 0 && text[i - 1] == '*')
                            {
                                commentend = i;
                                comment.IndexStart = commentstart;
                                comment.IndexEnd = commentend;
                                AddChildren(comment);
                                comment = null;
                            }
                        }
                        break;
                    // 出现星符
                    case '*':
                        // 段注释的开始符(/*)
                        if (comment == null)
                        {
                            if (i > 0 && text[i - 1] == '/')
                            {
                                commentstart = i + 1;
                                comment = new FuncBlock_CommentParagraph(model);
                            }
                        }
                        break;
                    // 出现换行符
                    case '\n':
                        // 如果在括号内则忽略
                        if (bracketstack.Count() > 0)
                        {
                            break;
                        }
                        // 行注释的结尾
                        if (comment != null &&
                            !(comment is FuncBlock_CommentParagraph))
                        {
                            commentend = i;
                            comment.IndexStart = commentstart;
                            comment.IndexEnd = commentend;
                            AddChildren(comment);
                            comment = null;
                        }
                        break;
                }
            }
            // 如果出现注释缺省，视为结尾符在最后，将剩余部分视为注释块来处理
            if (comment != null)
            {
                commentend = end + 1;
                comment.IndexStart = commentstart;
                comment.IndexEnd = commentend;
                AddChildren(comment);
                comment = null;
                return;
            }
            // 如果出现区域括号的缺省，视为后面已经有匹配的括号，后面的部分视为区域处理
            if (bracketstack.Count() > 0 && bracketstack.Contains('{'))
            {
                bracketend = end + 1;
                child = new FuncBlock_Local(model);
                child.IndexStart = bracketstart;
                child.IndexEnd = bracketend;
                AddChildren(child);
                if (bracketstart + 1 < bracketend)
                {
                    child.Build(text, bracketstart + 1, bracketend - 1);
                }
            }
            // 更新总控的当前节点
            if (Parent != null)
            {
                model.CurrentNode = Parent.Current;
            }
            else
            {
                model.CurrentNode = new LinkedListNode<FuncBlock>(this);
            }
        }
        /// <summary>
        /// 覆盖之前的字符串生成方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0:d}-{1:d}](I={2:d})(O={3:d})", IndexStart, IndexEnd, InnerOffset, OuterOffset);
        }
    }

    /// <summary>
    /// 表示能够注释说明的元素
    /// </summary>
    public interface IFuncBlock_IsCommentable
    {
        FuncBlock_Comment Comment { get; set; }
    }

    /// <summary>
    /// 表示整个代码的根部
    /// </summary>
    public class FuncBlock_Root : FuncBlock
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        /// <param name="text">对应的代码文本</param>
        public FuncBlock_Root(FuncBlockModel _model, string text) : base(_model)
        {
            IndexStart = 0;
            IndexEnd = text.Length;
            Namespace = String.Empty;
            Build(text, 0, text.Length - 1);
        }
        /// <summary>
        /// 覆盖之前的字符串生成方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Root:" + base.ToString();
        }
    }

    /// <summary>
    /// 表示局部区域
    /// </summary>
    public class FuncBlock_Local : FuncBlock
    {
        /// <summary>
        /// 当前局部区域的ID总数
        /// </summary>
        static private int IDCount = 0;
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_Local(FuncBlockModel _model) : base(_model)
        {
            ID = ++IDCount;
        }
        /// <summary>
        /// 区域ID
        /// </summary>
        protected int id;
        /// <summary>
        /// 区域ID
        /// </summary>
        public int ID
        {
            get { return this.id; }
            private set { this.id = value; }
        }
        /// <summary>
        /// 当前区域如果是函数区域，则设置函数头部
        /// </summary>
        protected FuncBlock_FuncHeader header;
        /// <summary>
        /// 当前区域如果是函数区域，则设置函数参数对应的虚拟的声明变量元素
        /// </summary>
        protected List<FuncBlock_Assignment> virtualassigns;
        /// <summary>
        /// 虚拟的声明变量元素
        /// </summary>
        public IEnumerable<FuncBlock_Assignment> VirtualAssigns
        {
            get { return this.virtualassigns; }
        }
        /// <summary>
        /// 当前区域如果是函数区域，则设置函数头部
        /// </summary>
        public FuncBlock_FuncHeader Header
        {
            get { return this.header; }
            set
            {
                if (virtualassigns == null)
                {
                    virtualassigns = new List<FuncBlock_Assignment>();
                }
                // 删除之前的虚拟声明变量
                foreach (FuncBlock_Assignment vassign in virtualassigns)
                {
                    vassign.Name = String.Empty;
                }
                virtualassigns.Clear();
                this.header = value;
                if (value == null)
                {
                    virtualassigns = null;
                    return;
                }
                FuncModel fmodel = value.FuncModel;
                // 设置新的虚拟声明变量
                for (int i = 0; i < fmodel.ArgCount; i++)
                {
                    string argname = fmodel.GetArgName(i);
                    if (argname.Equals(String.Empty)) break;
                    FuncBlock_Assignment vassign = new FuncBlock_Assignment(model);
                    vassign.Namespace = Namespace;
                    vassign.Name = argname;
                    virtualassigns.Add(vassign);
                }
            }
        }
        /// <summary>
        /// 字符串生成方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Local:{0:s}::{1:s}", Namespace, base.ToString());
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static public bool TextSuit(string text)
        {
            // 判断是否有外括号
            return text.First() == '{' && text.Last() == '}';
        }
    }

    /// <summary>
    /// 表示变量声明语句
    /// </summary>
    public class FuncBlock_Assignment : FuncBlock, IFuncBlock_IsCommentable
    {
        /// <summary>
        /// ID总数
        /// </summary>
        static private int IDCount = 0;

        /// <summary>
        /// 对应的注释块
        /// </summary>
        public FuncBlock_Comment Comment { get; set; }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_Assignment(FuncBlockModel _model) : base(_model)
        {
            ID = IDCount++;
        }
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        /// <param name="_parent">父节点</param>
        /// <param name="text">代码文本</param>
        public FuncBlock_Assignment(FuncBlockModel _model, FuncBlock _parent, string text) : base(_model)
        {
            ID = IDCount++;
            Parent = _parent;
            Namespace = Parent.Namespace;
            AnalyzeText(text);
        }
        /// <summary>
        /// 分析对应的代码文本，获得声明变量的信息
        /// </summary>
        /// <param name="text"></param>
        public void AnalyzeText(string text)
        {
            string[] texts = text.Split('=');
            Match m1 = null;
            if (texts.Length == 1)
            {
                m1 = Regex.Match(text, @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\s*;\s*$");
            }
            if (texts.Length == 2)
            {
                m1 = Regex.Match(texts[0], @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\s*$");
            }
            if (m1 != null && m1.Success)
            {
                Name = m1.Groups[3].Value;
            }
            else
            {
                throw new ArgumentException(String.Format("Cannot analyze text {0:s}", text));
            }
        }
        /// <summary>
        /// 字符串生成方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Assignment:{2:s}::{0:s}(Name={1:s})", base.ToString(), Name, Namespace);
        }
        /// <summary>
        /// 变量ID
        /// </summary>
        protected int id;
        /// <summary>
        /// 变量ID
        /// </summary>
        public int ID
        {
            get { return this.id; }
            private set { this.id = value; }
        }
        /// <summary>
        /// 变量类型
        /// </summary>
        protected string type = String.Empty;
        /// <summary>
        /// 变量类型
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
            private set
            {
                this.type = value;
            }
        }
        /// <summary>
        /// 变量名称
        /// </summary>
        protected string name = String.Empty;
        /// <summary>
        /// 变量名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                //if (Parent == null) return;
                // 删除之前在索引表中设置的记录
                string subname = String.Empty;
                SortedList<int, FuncBlock_Assignment> subassigns = null;
                for (int i = 1; i <= name.Length; i++)
                {
                    subname = String.Format("{0:s}::{1:s}", Namespace, name.Substring(0, i));
                    if (Assigns.ContainsKey(subname))
                    {
                        subassigns = Assigns[subname];
                        subassigns.Remove(ID);
                    }
                }
                this.name = value;
                // 增加当前名称的在索引表中设置的记录
                for (int i = 1; i <= name.Length; i++)
                {
                    subname = String.Format("{0:s}::{1:s}", Namespace, name.Substring(0, i));
                    if (Assigns.ContainsKey(subname))
                    {
                        subassigns = Assigns[subname];
                    }
                    else
                    {
                        subassigns = new SortedList<int, FuncBlock_Assignment>();
                        Assigns.Add(subname, subassigns);
                    }
                    subassigns.Add(ID, this);
                }
            }
        }
        /// <summary>
        /// 变量所占用的内存空间
        /// </summary>
        public int Sizeof
        {
            get
            {
                if (type.EndsWith("*"))
                    return 4;
                switch (type)
                {
                    case "BIT":
                        return 4;
                    case "WORD":
                    case "UWORD":
                        return 4;
                    case "DWORD":
                    case "UDWORD":
                        return 8;
                    case "FLOAT":
                    case "UFLOAT":
                        return 8;
                    default:
                        return 4;
                }
            }
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text">代码文本</param>
        /// <returns></returns>
        static public bool TextSuit(string text)
        {
            string[] texts = text.Split('=');
            if (texts.Length == 1)
            {
                Match m1 = Regex.Match(text, @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\s*;\s*$");
                return m1.Success;
            }
            if (texts.Length == 2)
            {
                Match m1 = Regex.Match(texts[0], @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+([a-zA-Z_]\w*)\s*$");
                Match m2 = Regex.Match(texts[1], @"^\s*[^;]*;\s*$");
                //Match m3 = Regex.Match(texts[1], @"^\s*\d*\.\d+\s*;\s*$");
                //Match m4 = Regex.Match(texts[1], @"^\s*0x[\da-fA-F]{1,8}\s*;\s*$");
                bool result = false;
                result |= m2.Success;
                //result |= m3.Success;
                //result |= m4.Success;
                result &= m1.Success;
                return result;
            }
            return false;
        }
    }

    /// <summary>
    /// 表示连续一组变量声明的语句(int i, j...)
    /// </summary>
    public class FuncBlock_AssignmentSeries : FuncBlock, IFuncBlock_IsCommentable
    {
        private List<FuncBlock_Assignment> defines = new List<FuncBlock_Assignment>();

        public IEnumerable<FuncBlock_Assignment> Defines
        {
            get { return this.defines; }
        }

        /// <summary>
        /// 对应的注释块
        /// </summary>
        public FuncBlock_Comment Comment { get; set; }

        public FuncBlock_AssignmentSeries(FuncBlockModel _model) : base(_model)
        {

        }

        public void ClearDefines()
        {
            foreach (FuncBlock_Assignment fba in defines)
            {
                fba.Name = String.Empty;
            }
            defines.Clear();
        }

        public override void Build(string text, int start, int end, int offset = 0)
        {
            ClearDefines();
            Match m1 = Regex.Match(text.Substring(start, end - start + 1), @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+(.*);$");
            if (!m1.Success) return;
            string _text = m1.Groups[3].Value;
            string[] texts = _text.Split(',');
            foreach (string sub in texts)
            {
                Match m2 = Regex.Match(sub, @"^((\s*\*)*)\s*([a-zA-Z_]\w*)\s*");
                if (!m2.Success) return;
                FuncBlock_Assignment fba = new FuncBlock_Assignment(model);
                fba.Namespace = Parent.Namespace;
                fba.Name = m2.Groups[3].Value;
                defines.Add(fba);
            }
        }

        static public bool TextSuit(string text)
        {
            Match m1 = Regex.Match(text, @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s+(.*);$");
            if (!m1.Success) return false;
            string _text = m1.Groups[3].Value;
            string[] texts = _text.Split(',');
            foreach (string sub in texts)
            {
                Match m2 = Regex.Match(sub, @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s*(=\s*[\w\.]*\s*)?$");
                if (!m2.Success) return false;
            }
            return true;
        }

        /// <summary>
        /// 字符串生成方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("AssignmentSeries:{1:s}::{0:s}", base.ToString(), Namespace);
        }

    }

    /// <summary>
    /// 表示一般语句
    /// </summary>
    public class FuncBlock_Statement : FuncBlock
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_Statement(FuncBlockModel _model) : base(_model)
        {

        }
        /// <summary>
        /// 转为字符串的方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Statement::{0:s}:{1:s}", Namespace, base.ToString());
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text">代码文本</param>
        /// <returns></returns>
        static public bool TextSuit(string text)
        {
            return (text.Length > 0 && text.Last() == ';');
        }
    }

    /// <summary>
    /// 表示函数头部
    /// </summary>
    public class FuncBlock_FuncHeader : FuncBlock, IFuncBlock_IsCommentable
    {
        /// <summary>
        /// 对应的函数信息模型
        /// </summary>
        public FuncModel FuncModel { get; set; }
        /// <summary>
        /// 对应的标题注释
        /// </summary>
        public FuncBlock_Comment Comment { get; set; }
        /// <summary>
        /// 对应的函数区域
        /// </summary>
        public FuncBlock_Local Block { get; set; }
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_FuncHeader(FuncBlockModel _model) : base(_model)
        {
            Model.InvokePropertyChanged("Funcs");
        }
        /// <summary>
        /// 转换为字符串的方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("FuncHeader::{0:s}:{1:s}", Namespace, base.ToString());
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text">代码文本</param>
        /// <returns></returns>
        static public bool TextSuit(string text)
        {
            Match m1 = Regex.Match(text, @"^\s*([a-zA-Z_]\w*(\s*\*)*)\s*([a-zA-Z_]\w*)\((\s*([a-zA-Z_]\w*(\s*\*)*)\s*([a-zA-Z_]\w*),?)*\)\s*$");
            return m1.Success;
        }
    }

    /// <summary>
    /// 表示关键字
    /// </summary>
    public class FuncBlock_Keyword : FuncBlock
    {
        public FuncBlock_Keyword(FuncBlockModel _model) : base(_model)
        {
        }
    }

    /// <summary>
    /// 表示for循环的头部
    /// </summary>
    public class FuncBlock_ForHeader : FuncBlock
    {
        public FuncBlock_Statement Start { get; set; }
        public FuncBlock_Statement Cond { get; set; }
        public FuncBlock_Statement Next { get; set; }

        public FuncBlock_ForHeader(FuncBlockModel _model) : base(_model)
        {
        }

    }

    /// <summary>
    /// 表示while循环的头部
    /// </summary>
    public class FuncBlock_WhileHeader : FuncBlock
    {
        public FuncBlock_Statement Cond { get; set; }

        public FuncBlock_WhileHeader(FuncBlockModel _model) : base(_model)
        {
        }
    }

    /// <summary>
    /// 表示while循环的末尾（do { ... } while 格式）
    /// </summary>
    public class FuncBlock_WhileEnd : FuncBlock
    {
        public FuncBlock_Statement Cond { get; set; }

        public FuncBlock_WhileEnd(FuncBlockModel _model) : base(_model)
        {
        }
    }

    /// <summary>
    /// 表示if的开头
    /// </summary>
    public class FuncBlock_IfHeader : FuncBlock
    {
        public FuncBlock_Statement Cond { get; set; }

        public FuncBlock_IfHeader(FuncBlockModel _model) : base(_model)
        {
        }
    }

    /// <summary>
    /// 表示一行注释部分
    /// </summary>
    public class FuncBlock_Comment : FuncBlock
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_Comment(FuncBlockModel _model) : base(_model)
        {

        }
        /// <summary>
        /// 转换为字符串的方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Comment::{0:s}", base.ToString());
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text">代码文本</param>
        /// <returns></returns>
        static public bool TextSuit(string text)
        {
            Match m1 = Regex.Match(text, @"^\/\/.");
            Match m2 = Regex.Match(text, @"^\/\*.\*\/$");
            bool ret = false;
            ret |= m1.Success;
            ret |= m2.Success;
            return ret;
        }
    }

    /// <summary>
    /// 表示一段注释部分
    /// </summary>
    public class FuncBlock_CommentParagraph : FuncBlock_Comment
    {
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_model">总控模型</param>
        public FuncBlock_CommentParagraph(FuncBlockModel _model) : base(_model)
        {

        }
        /// <summary>
        /// 转换为字符串的方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("CommentParagraph::{0:s}", base.ToString());
        }
        /// <summary>
        /// 判断一段代码文本是否符合这个元素
        /// </summary>
        /// <param name="text">代码文本</param>
        /// <returns></returns>
        static new public bool TextSuit(string text)
        {
            Match m2 = Regex.Match(text, @"^\/\*.*\*\/$");
            bool ret = false;
            ret |= m2.Success;
            return ret;
        }
    }

}
