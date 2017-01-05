using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.Project;
using SamSoarII.UserInterface;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private ProjectModel _currentProject = null;
        private ProjectModel CurrentProject
        {
            get
            {
                return _currentProject;
            }
            set
            {
                _currentProject = value;
                if(_currentProject != null)
                {
                    HasProject = true;
                }
                else
                {
                    HasProject = false;
                }
            }
        }
        private bool _hasProject = false;
        public bool HasProject
        {
            get
            {
                return _hasProject;
            }
            set
            {
                _hasProject = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HasProject"));
                }
            }
        }

        private string projectFullFileName;
        private ScaleTransform _scaleTransform = new ScaleTransform();
        private ProjectTreeView _projcetTreeView;


        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += MainWindow_SizeChanged;
            LDTabControl.Scale = _scaleTransform;
            LDTabControl.SelectionChanged += (sender, args) =>
            {
                var selectedTab = LDTabControl.SelectedItem as BaseTabItem;
                if(selectedTab != null)
                {
                    selectedTab.OnItemSelected();
                }
            };          
        }

        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HasProject;
        }

        private void AddNewRoutineCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HasProject;
        }

        private void AddNewRoutineCommandExecute(object sender, RoutedEventArgs e)
        {
            AddNewRoutineWindow window;
            using (window = new AddNewRoutineWindow())
            {
                window.EnsureButtonClick += (sender1, e1) =>
                {
                    AddSubRoutine(window.NameContent);
                    window.Close();
                };
                window.ShowDialog();
            }
        }

        private void AddFuncBlockCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = HasProject;
        }

        private void AddFuncBlockCommandExecute(object sender, RoutedEventArgs e)
        {
            AddNewRoutineWindow window;
            using (window = new AddNewRoutineWindow())
            {
                window.EnsureButtonClick += (sender1, e1) =>
                {
                    AddFuncBlock(window.NameContent);
                    window.Close();
                };
                window.ShowDialog();
            }
        }

        private void CutCommandExecute(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("cut");
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _scaleTransform.ScaleX = LDTabControl.ActualWidth / 3000;
            _scaleTransform.ScaleY = LDTabControl.ActualWidth / 3000;         
        }

        private void AddSubRoutine(string name)
        {
            LadderDiagramModel ldmodel = new LadderDiagramModel(name);
            CurrentProject.AddSubRoutine(ldmodel);
            _projcetTreeView.AddSubRoutineTreeViewItem(ldmodel);
            LDTabControl.ShowItem(ldmodel);
        }

        private void AddFuncBlock(string name)
        {
            FuncBlockModel fbmodel = new FuncBlockModel(name);
            CurrentProject.AddFuncBlock(fbmodel);
            _projcetTreeView.AddFuncBlockTreeViewItem(fbmodel);
            LDTabControl.ShowItem(fbmodel);
        }

        private void CreateMainRoutine(string name)
        {
            LadderDiagramModel ldmodel = new LadderDiagramModel(name);
            CurrentProject.SetMainRoutine(ldmodel);
            LDTabControl.CurrentProject = CurrentProject;
            LDTabControl.ShowItem(ldmodel);
        }

        private void CreateProject(string name)
        {
            //复位TabControl
            LDTabControl.Items.Clear();
            //创建工程模型
            CurrentProject = new ProjectModel(name);
            CreateMainRoutine("Main");
            CurrentProject.Save(projectFullFileName);
            //创建工程树形视图
            _projcetTreeView = new ProjectTreeView(CurrentProject, LDTabControl);
            TreeViewGrid.Children.Add(_projcetTreeView);
        }

        private void OpenProject(string fullFileName)
        {
            //复位TabControl
            LDTabControl.Items.Clear();
            //创建新的工程
            CurrentProject = new ProjectModel();
            CurrentProject.Open(fullFileName);
            projectFullFileName = fullFileName;
            //创建工程树形视图
            _projcetTreeView = new ProjectTreeView(CurrentProject, LDTabControl);
            TreeViewGrid.Children.Add(_projcetTreeView);
            //清除当前所有TabItem
            LDTabControl.Items.Clear();
            //为LDTabControl设置当前工程上下文
            LDTabControl.CurrentProject = CurrentProject;
            //显示主程序
            LDTabControl.ShowItem(CurrentProject.MainRoutine);
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            NewProjectDialog newProjectDialog;
            using (newProjectDialog = new NewProjectDialog())
            {
                newProjectDialog.EnsureButtonClick += (sender1, e1) =>
                {
                    string name = newProjectDialog.NameContent;
                    string dir = newProjectDialog.PathContent;
                    if (!Directory.Exists(dir))
                    {
                        MessageBox.Show("指定路径不存在");
                        return;
                    }
                    string fullFileName = string.Format(@"{0}\{1}.ssp", dir, name);
                    if (File.Exists(fullFileName))
                    {
                        MessageBox.Show("指定路径已存在同名文件");
                        return;
                    }
                    projectFullFileName = fullFileName;
                    CreateProject(name);
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }

        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssp文件|*.ssp";
            if(openFileDialog.ShowDialog() == true)
            {
                try
                {
                    OpenProject(openFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.Message);
                    MessageBox.Show("不正确的工程文件，工程文件已损坏!");
                }
            }
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {
            CurrentProject.Save(projectFullFileName);
        }

        private void SaveAsProject(object sender, RoutedEventArgs e)
        {

        }

        private void CompileProject(object sender, RoutedEventArgs e)
        {
            //var stream = File.Open(@"\SamSoar\hardware\FGs-16\src\PLCLadder.c", FileMode.OpenOrCreate);
            string code = string.Format("#include<stdint.h>\r\n void RunLadder()\r\n{{\r\n {0} \r\n}}\r\n", CurrentProject.MainRoutine.GenerateCode());
            //File.WriteAllText(@"hardware\FGs-16\src\PLCLadder.c", code);
            // Step 1, write a code file
            string makeDir = Directory.GetCurrentDirectory() + @"\hardware\FGs-16";
            string codeDir = Directory.GetCurrentDirectory() + @"\hardware\FGs-16\src";
            string codeFileName = codeDir + @"\PLCLadder.c";
            if (!File.Exists(codeFileName))
            {
                File.Create(codeFileName);
            }
            File.WriteAllText(codeFileName, code);
            // step 2, compile
            Process cmd = new Process();
            cmd.StartInfo.FileName = "make";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.Arguments = string.Format(" -C {0}", makeDir);
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();
            cmd.WaitForExit();
            if(cmd.ExitCode == 0)
            {
                MessageBox.Show("编译成功");
            }
            else
            {
                MessageBox.Show("编译失败");
            }
        }

        private void ProcessExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            _scaleTransform.ScaleX *= 1.1;
            _scaleTransform.ScaleY *= 1.1;
        }

        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            _scaleTransform.ScaleX /= 1.1;
            _scaleTransform.ScaleY /= 1.1;
        }
    }
}
