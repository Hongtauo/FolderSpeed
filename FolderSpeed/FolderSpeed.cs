using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FolderSpeed
{
    public partial class FolderSpeed : Form
    {
        public string targetPath = string.Empty;
        static public string configFile = "Config.xml";
        public bool flag = false;
        public string configFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, configFile);
        public FolderSpeed()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            // 检查是否存在配置文件，不存在的话先创建
            CreateConfigFile();
            // 从本地的配置文件中加载targetPath
            LoadtargetPath();
            // 打开tagrgetPath对应的文件夹,如果路径为空，则弹出界面让用户选择路径，然后写入到配置文件中
            OpenFolder();

        }
        private void CreateConfigFile()
        {
            //如果该配置文件不存在的话，则在exe运行目录下创建一个Config.xml文件
            // 检查文件是否存在
            if (File.Exists(configFilePath))
            {
                return;
            }
            // 创建一个新的XML文档
            XmlDocument doc = new XmlDocument();
            // 创建XML声明
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);
            // 创建根节点
            XmlElement root = doc.CreateElement("Configuration");
            doc.AppendChild(root);
            // 创建Path节点
            XmlElement path = doc.CreateElement("Path");
            path.InnerText = ""; 
            root.AppendChild(path);
            // 保存文档
            doc.Save(configFilePath);
        }

        public void LoadtargetPath()
        {
            // 检查文件是否存在
            if (File.Exists(configFilePath))
            {
                // 创建XML文档对象
                XmlDocument doc = new XmlDocument();
                // 加载XML文件
                doc.Load(configFilePath);
                // 获取Path节点的值
                XmlNode node = doc.SelectSingleNode("//Path");
                if (node != null)
                {
                    targetPath = node.InnerText;
                }
                else
                {
                    Console.WriteLine("未找到Path");
                }
            }
            else
            {
                Console.WriteLine("Config.xml文件不存在");
            }
        }

        public void OpenFolder()
        {
            if (Directory.Exists(targetPath))
            {
                System.Diagnostics.Process.Start(targetPath);
                this.flag = true; // 如果能正常打开，那就将标志设置为true，然后Load的时候就能通过这个判断是显示窗口还是退出程序
            }          
        }

        public void UpdataConfigFile(string item)
        {
            // 调用一下，以防配置文件不存在的情况下报错
            CreateConfigFile();
            // 创建XML文档对象
            XmlDocument doc = new XmlDocument();
            // 加载XML文件
            doc.Load(configFilePath);
            // 获取Path节点
            XmlNode node = doc.SelectSingleNode("//Path");
            if (node != null)
            {
                // 更新
                node.InnerText = item;
                // 保存
                doc.Save(configFilePath);
                enteredAddress.Text = "路径:"+item; 
            }
            else
            {
                Console.WriteLine("未找到Path节点");
            }
        }

        private void FolderSpeed_Load(object sender, EventArgs e)
        {
            // 如果配置文件为空或者路径不存在，则不退出窗体，显示界面让用户进行修改目标文件夹
            if (!this.flag)
            { 
                return;
            }           
            // 退出主程序
            this.Close();
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                // 显示对话框
                DialogResult result = dialog.ShowDialog();

                // 点击确定按钮
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // 获取选择的文件夹地址
                    string selectedFolderPath = dialog.SelectedPath;
                    // 更新路径
                    UpdataConfigFile(selectedFolderPath);
                }
            }
        }
    }
}
