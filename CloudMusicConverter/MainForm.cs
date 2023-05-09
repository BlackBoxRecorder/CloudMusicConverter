using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudMusicConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            LabelTotal.Text = "将网易云音乐文件夹拖放到上面，";
            LabelFilename.Text = "然后点击开始转换！";

            LbErrMsg.Text = string.Empty;

            BtnStop.Enabled = false;
            BtnOpen.Enabled = false;

        }


        private bool IsConverting = false;
        private string Dir = "";
        private string ResultDir = "";

        private const string NCM_FILE_EXTENSION = ".ncm";
        readonly List<string> AllNCMFiles = new List<string>();
        readonly List<string> ConvertedFiles = new List<string>();


        private void FilePanel_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (IsConverting) { return; }

                Array aryFiles = ((System.Array)e.Data.GetData(DataFormats.FileDrop));

                if (aryFiles.Length > 1)
                {
                    throw new Exception("请拖放单个文件夹！");
                }

                if (aryFiles.Length == 1)
                {
                    Dir = aryFiles.GetValue(0).ToString();
                    if (!Directory.Exists(Dir))
                    {
                        throw new DirectoryNotFoundException();
                    }

                    AllNCMFiles.Clear();

                    FindFiles(new DirectoryInfo(Dir));

                    LabelTotal.Text = $"找到 {AllNCMFiles.Count} 个ncm文件";
                    LabelFilename.Text = "";
                    LbErrMsg.Text = "";

                    ResultDir = Path.Combine(Dir, "已转换");

                    if (!Directory.Exists(ResultDir))
                    {
                        Directory.CreateDirectory(ResultDir);
                    }
                    else
                    {

                    }
                }
                else
                {
                    throw new InvalidDataException("文件夹拖放错误");
                }

            }
            catch (Exception ex)
            {
                LbErrMsg.Text = ex.Message;
            }
        }

        private void FilePanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && !IsConverting)
                e.Effect = DragDropEffects.Move;
            else e.Effect = DragDropEffects.None;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {

                if (!Directory.Exists(Dir) || AllNCMFiles.Count < 1)
                {
                    throw new DirectoryNotFoundException("未找到.ncm文件");
                }

                BtnStart.Enabled = false;
                BtnStop.Enabled = true;

                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Start();
                    }
                    catch (Exception ex)
                    {
                        LbErrMsg.Text = ex.Message;
                    }
                }));

            }
            catch (Exception ex)
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;

                LbErrMsg.Text = ex.Message;
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                BtnStart.Enabled = true;
                BtnStop.Enabled = false;

                if (Directory.Exists(ResultDir))
                {
                    BtnOpen.Enabled = true;
                }

                IsConverting = false;

            }
            catch (Exception)
            {
                BtnStop.Enabled = true;
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(ResultDir))
                {
                    throw new DirectoryNotFoundException(ResultDir);
                }
                System.Diagnostics.Process.Start("Explorer.exe", ResultDir);
            }
            catch (Exception ex)
            {
                LbErrMsg.Text = ex.Message;
                LbErrMsg.ToolTipText = ex.Message;
            }
        }

        public async void Start()
        {

            string lbTotal = $"找到 {AllNCMFiles.Count} 个ncm文件";
            LabelTotal.Text = lbTotal;
            LabelFilename.Text = "";

            int count = 0;
            IsConverting = true;
            int errCount = 0;
            foreach (string file in AllNCMFiles)
            {
                if (!IsConverting) return;

                var _processing = new FileInfo(file);

                try
                {
                    var fs = _processing.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                    NeteaseCrypto neteaseFile = new NeteaseCrypto(fs)
                    {
                        FileName = Path.GetFileNameWithoutExtension(file),
                        FullFilePath = ResultDir,
                        FullFileName = file
                    };

                    LabelTotal.Text = lbTotal + $"，正在转换第{count}个";
                    LabelFilename.Text = Path.GetFileNameWithoutExtension(file);
                    //skip exist file


                    try
                    {
                        neteaseFile.Dump();
                    }
                    finally
                    {
                        count++;
                        neteaseFile.CloseFile();
                        await Task.Delay(5000);
                    }

                }
                catch (Exception)
                {
                    errCount++;
                    LbErrMsg.Text = $"{errCount}个文件转换失败";
                    LbErrMsg.ToolTipText = file;
                }
            }

            IsConverting = false;
            LabelFilename.Text = "全部转换完成，可以打开文件夹了！";

            BtnStart.Enabled = true;
            BtnStop.Enabled = false;
            BtnOpen.Enabled = true;
        }


        public void FindFiles(DirectoryInfo di)
        {

            try
            {
                FileInfo[] fis = di.GetFiles();

                for (int i = 0; i < fis.Length; i++)
                {
                    if (fis[i].Extension.Contains(NCM_FILE_EXTENSION))
                    {
                        AllNCMFiles.Add(fis[i].FullName);
                        Console.WriteLine("文件：" + fis[i].FullName);
                    }
                }

                DirectoryInfo[] dis = di.GetDirectories();
                for (int j = 0; j < dis.Length; j++)
                {
                    FindFiles(dis[j]);
                }

            }
            catch (Exception ex)
            {
                LbErrMsg.Text = "查找.ncm文件出错";
                LbErrMsg.ToolTipText = ex.Message;
            }

        }

        private void LblWebsite_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://timetickme.com");
            }
            catch (Exception)
            {

            }
        }
    }
}
