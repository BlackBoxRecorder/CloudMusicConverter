﻿using System;
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
        private const string MP3_FILE_EXTENSION = ".mp3";
        private const string FLAC_FILE_EXTENSION = ".flac";

        List<string> AllNCMFiles = new List<string>();
        List<string> ConvertedFiles = new List<string>();


        private void FilePanel_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (IsConverting) { return; }

                Array aryFiles = ((System.Array)e.Data.GetData(DataFormats.FileDrop));

                if (aryFiles.Length > 1)
                {
                    throw new InvalidDataException("请拖放单个文件夹！");
                }

                if (aryFiles.Length != 1)
                {
                    throw new InvalidDataException("文件夹拖放错误");
                }

                Dir = aryFiles.GetValue(0).ToString();
                if (!Directory.Exists(Dir))
                {
                    throw new DirectoryNotFoundException("文件夹不存在");
                }

                AllNCMFiles.Clear();
                ConvertedFiles.Clear();

                FindFiles(new DirectoryInfo(Dir), ref AllNCMFiles, new List<string>() { NCM_FILE_EXTENSION });

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
                    //查找目录下所有文件
                    FindFiles(new DirectoryInfo(ResultDir), ref ConvertedFiles,
                        new List<string>() { MP3_FILE_EXTENSION, FLAC_FILE_EXTENSION });
                    LabelFilename.Text = $"已有{ConvertedFiles.Count}个转换完成。";

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

            int count = 1;
            IsConverting = true;
            int errCount = 0;
            foreach (string file in AllNCMFiles)
            {
                if (!IsConverting) return;
                await Task.Delay(50);
                try
                {
                    var filename = Path.GetFileNameWithoutExtension(file);
                    if (ConvertedFiles.Exists(c => c.Contains(filename)))
                    {
                        count++;
                        continue;
                    }

                    var _processing = new FileInfo(file);
                    var fullpath = _processing.DirectoryName;
                    var subDir = fullpath.Replace(Dir, "");


                    var fs = _processing.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                    var resdir = ResultDir + subDir;
                    if (!Directory.Exists(resdir))
                    {
                        Directory.CreateDirectory(resdir);
                    }
                    NeteaseCrypto neteaseFile = new NeteaseCrypto(fs)
                    {
                        FileName = filename,
                        FullFilePath = resdir,
                        FullFileName = file
                    };

                    LabelTotal.Text = lbTotal + $"，正在转换第{count}个";
                    LabelFilename.Text = Path.GetFileNameWithoutExtension(file);

                    try
                    {
                        neteaseFile.Dump();
                    }
                    finally
                    {
                        count++;
                        neteaseFile.CloseFile();
                    }
                }
                catch (Exception ex)
                {
                    errCount++;
                    LbErrMsg.Text = $"{errCount}个文件转换失败";
                    LbErrMsg.ToolTipText = ex.Message;
                }
            }

            IsConverting = false;

            var stat = GetDirStat(ResultDir);
            LabelTotal.Text = stat;

            LabelFilename.Text = "全部转换完成，可以打开文件夹了！";

            BtnStart.Enabled = true;
            BtnStop.Enabled = false;
            BtnOpen.Enabled = true;
        }

        public void FindFiles(DirectoryInfo di, ref List<string> result, List<string> extensions)
        {
            try
            {
                FileInfo[] fis = di.GetFiles();

                for (int i = 0; i < fis.Length; i++)
                {
                    if (extensions.Contains(fis[i].Extension))
                    {
                        result.Add(fis[i].FullName);
                        Console.WriteLine("文件：" + fis[i].FullName);
                    }
                }

                DirectoryInfo[] dis = di.GetDirectories();
                for (int j = 0; j < dis.Length; j++)
                {
                    FindFiles(dis[j], ref result, extensions);
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


        /// <summary>
        /// 获取文件个数和文件夹大小
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private string GetDirStat(string dir)
        {
            try
            {
                var files = new List<string>();
                FindFiles(new DirectoryInfo(dir), ref files,
                    new List<string> { MP3_FILE_EXTENSION, FLAC_FILE_EXTENSION });

                long totalSize = 0;

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    totalSize += fi.Length;
                }

                return $"已转换文件共{files.Count}个，占用{totalSize / 1024 / 1024}Mb";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
        }
    }
}
