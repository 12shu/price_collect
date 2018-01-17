using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;   // 调试
using System.IO;            // 读写
/// phantomjs需要的
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System.Collections.ObjectModel; // ReadOnlyCollection 的命名空间


namespace price_collect
{
    public partial class Form1 : Form
    {
        public Encoding StandardErrorEncoding { get; set; }
        public string info_csv = "./查询列表.csv";
        public string[] title = { "商品", "单位", "链接", "价格xpath" };
        public string exe_path = "价格采集.exe";

        public color_log log = new color_log(); // 富文本的日志输出

        public Form1()
        {
            InitializeComponent();
        }

        // FormClosingEventArgs 是关闭窗口
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(
                    "是否要退出?",
                    "提示",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) == DialogResult.OK)
            {
                e.Cancel = false;
            }
            else { e.Cancel = true; }
        }

        // 保存采集信息
        private void save_info(object sender, EventArgs e)
        {

            string[] content = { textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text };
            write2csv writer = new write2csv();
            writer.start(info_csv,content,title);
            string Message_str = String.Format("刚才输入的xpath信息已保存至\"{0}\"", info_csv);
            log.LogMessage(display_box,Message_str);

        }

        // 开始爬取数据
        public void start_craw(object sender, EventArgs e)
        {
            string goods_name, goods_unit, goods_url, price_xpath;
            goods_name = textBox1.Text;
            goods_unit = textBox2.Text;
            goods_url = textBox3.Text;
            price_xpath = textBox4.Text;

            one_crawl(goods_name, goods_unit, goods_url, price_xpath);
            log.LogMessage(display_box,"价格数据已保存在'商品价格'文件夹里");
        }

        /// <summary>
        /// 用于调用外部exe
        /// </summary>
        /// <param name="runFilePath">exe的路径</param>
        /// <param name="args">参数数组</param>
        public string StartProcess(string runFilePath, params string[] args)
        {
            try
            {
                string s = "";
                foreach (string arg in args)
                {
                    s = s + arg + " ";
                }
                s = s.Trim();
                Process process = new Process();//创建进程对象    
                ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s); // 括号里是(程序名,参数)
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;       //是否使用操作系统的shell启动,设为false后才能捕捉错误
                startInfo.RedirectStandardOutput = true;         //由调用程序获取输出信息
                startInfo.CreateNoWindow = true;                 //不显示调用程序的窗口
                process.StartInfo.RedirectStandardError = true;  //重定向错误流
                //startInfo.StandardErrorEncoding = ASCIIEncoding.UTF8; // 设置编码
                //startInfo.StandardOutputEncoding = ASCIIEncoding.GetEncoding(936);// gbk只能用代码页来设置
                process.Start();
                StreamReader out_reader = process.StandardOutput;     
                log.LogMessage(display_box, out_reader.ReadToEnd()); // 将输出内容写入日志
                /// 调试
                StreamReader Error_Reader = process.StandardError;   // 读取错误流
                string info_error = Error_Reader.ReadToEnd();        // 正确的姿势
                string feedback = Error_Reader.ReadLine();        // ReadLine()只读取第一行
                if (info_error != "")
                {
                    info_error = "参数:\n" + s +"\n捕捉到外部程序的错误:\n" + info_error;
                    log.LogError(display_box,info_error);
                }
                int exit_code = process.ExitCode;                    // 获取返回码   
                process.WaitForExit();
                return exit_code.ToString();
            }
            catch (System.SystemException e)
            {
                string info_C = "捕捉到C#的错误:\n" + e.ToString();
                log.LogMessage(display_box, info_C);
                return "C#代码错误";
            }

        }


        /// <summary>
        /// 爬取一条数据,先调用"价格采集.exe",无效时,调用phantomjs_crawl.start
        /// 前者速度快,后者通用性强
        /// </summary>
        /// <param name="goods_name">商品名</param>
        /// <param name="goods_unit">单位</param>
        /// <param name="goods_url">链接</param>
        /// <param name="price_xpath">价格的xpath</param>
        public void one_crawl(string goods_name, string goods_unit, string goods_url, string price_xpath)
        {
            // 调用
            string[] args = {goods_name,goods_unit,goods_url,price_xpath };
            string status_code = StartProcess(exe_path, args);  // 调用外部的exe
            // 判断错误原因
            string status_info = "";
            if ((status_code == "0") || (status_code == "1"))
            {
                if (status_code == "0") { status_info = "成功获取数据"; }
                else { status_info = "'采集数据.exe'获取数据失败,该数据位于渲染后的网页上"; }
            }
            else
            { status_info = "网址链接打开失败,返回码:"+ status_code; }

            string price_csv = "./商品价格/" + goods_name + ".csv";
            // python没有获取到数据时
            if (status_code != "0")
            {
                log.LogNormal(display_box, status_info);
                log.LogNormal(display_box, "开始使用C#的phantomjs爬虫");
                phantomjs_crawl crawl = new phantomjs_crawl();
                string price = crawl.xpath_crwal(goods_url, price_xpath); // 网址,xpath
                if (price == "NULL")
                {
                    log.LogError(display_box, "C#的phantomjs爬虫也未成功得到"+ goods_name+"的数据");
                    return; // phantomjs.exe也失败了,就退出当前函数
                }
                string[] price_title = {"日期","价格","单位" };
                ///判断是否有最新价格
                bool is_exist_newest=false; // 初始化,"最新价格不存在"
                if (File.Exists(price_csv)) // 存在该文件时
                {
                    //获取已有最新价格
                    string[] lines = File.ReadAllLines(price_csv);
                    string[] data0 = lines[lines.Length - 1].Split('\"');                             // 拆分
                    List<string> data_1 = new List<string>(data0);
                    string[] data = data_1.Where(p => (p != ",") & (p != " ") & (p != "")).ToArray(); // 去除数组中的逗号,空格,空值
                    if (price == data[1]) { is_exist_newest = true; }
                }

                if (price != "")
                {
                    log.LogNormal(display_box, "成功获取渲染后的网页数据,价格为:");
                    if (is_exist_newest == false)
                    {
                        DateTime dt = DateTime.Now;
                        string date_str = String.Format("{0:d}", dt);
                        string[] content = { date_str, price, goods_unit };   // 日期,价格,单位
                        write2csv writer = new write2csv();
                        writer.start("./商品价格/" + goods_name + ".csv", content, price_title);
                        log.LogMessage(display_box,"今日价格为:"+price+",已保存到'商品价格'文件夹");
                    }
                    else { log.LogMessage(display_box, "最新价格已存在,无需重复写入.今日价格为:" + price + goods_unit); }
                }
                else
                {
                    log.LogError(display_box, "获取失败");
                }
            }
        }

        public void test(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines("./商品价格/牙膏.csv");
            string[] data0 = lines[lines.Length-1].Split('\"');
            // 去除数组中的逗号,空格,空值
            List<string> data_1 = new List<string>(data0);
            string[] data = data_1.Where(p => (p != ",") & (p != " ") & (p != "")).ToArray();
            MessageBox.Show(data[1]);
        }


        /// <summary>
        /// 打开"查询列表.csv",批量采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void batch_crawl(object sender, EventArgs e)
        {
            string[] lines =File.ReadAllLines(@info_csv,Encoding.UTF8);// 读取
            int m =lines.Count();
            for (int i = 1; i <= m - 1; i = i + 1) // 第1行是表头,跳过
            {
                log.LogWarning(display_box, "\n\n\n找到第" + i.ToString() + "个!,内容为:");
                log.LogNormal(display_box, lines[i].ToString());

                string[] data0 = lines[i].Split('\"');
                /// 去除数组中的逗号,空格,空值
                List<string> data_1 = new List<string>(data0);
                string[] data = data_1.Where( p => (p != ",")&(p != " ") & (p != "")).ToArray();

                /// 判断参数是否切割正确
                int args_num = data.Length;
                if (args_num != 4)
                {
                    string Message_info = String.Format("参数个数应该为4,现有{0}个!\n", args_num.ToString());
                    for (int j = 0; j < args_num; j = j + 1)
                    {
                        Message_info += String.Format("{0})", j + 1) + String.Format("{0}\n", data[j]);
                    }
                    log.LogMessage(display_box, Message_info);
                }
                else
                {
                    one_crawl(data[0],data[1],data[2],data[3]);
                }
            }
            log.LogWarning(display_box, "已完成数据的批量获取!\n价格数据保存在'商品价格'文件夹里");
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class color_log
    {
        #region 日志记录、支持其他线程访问  

        public delegate void LogAppendDelegate(RichTextBox RichTextBox0, Color color, string text);

        public void LogAppendMethod(RichTextBox RichTextBox0,Color color, string text)
        {
            if (!RichTextBox0.ReadOnly)
            { RichTextBox0.ReadOnly = true; }

            RichTextBox0.AppendText("\n");
            RichTextBox0.SelectionColor = color;
            RichTextBox0.AppendText(text);
            RichTextBox0.ScrollToCaret(); // 保持滚动条在底部
            RichTextBox0.Refresh();       // 刷新文本框显示
        }

        public void LogError(RichTextBox RichTextBox0,string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Red, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }

        public void LogWarning(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Blue, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }

        public void LogMessage(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Green, text);
        }

        public void LogNormal(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Black,text);
        }
        #endregion
    }

    public class phantomjs_crawl
    {
        public string xpath_crwal(string url,string xpath0)
        {
            /// <summary>
            /// 利用phantomjs.exe来完成爬虫
            /// 需要在工具--NuGet 程序包管理器--安装几个包
            /// Selenium.PhantomJS.WebDriver和Selenium.WebDriver
            /// </summary>
            /// <param name="url">数据的网址</param>
            /// <param name="xpath0">数据的xpath</param>
            /// <returns></returns>
            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            var options = new PhantomJSOptions();
            options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0");
            service.HideCommandPromptWindow = true; // 隐藏dos窗口
            var driver1 = new PhantomJSDriver(service,options);
            driver1.Navigate().GoToUrl(url);
            

            ReadOnlyCollection<IWebElement> res = driver1.FindElementsByXPath(xpath0); // 搜索嘛,结果肯定是一个数组
            string res_text;
            if (res.Count != 0)
            {
                res_text = res[0].Text;
                driver1.Quit();
            }
            else
            {
                res_text = "NULL";
            }
            return res_text;
        }
    }

    public class write2csv
    {
        public void start(string file_name,string[] content,string[] title)
        {
            string str_content= content[0];
            string str_title= title[0];
            for (int i = 1; i < content.Length; i = i + 1)
            {
                str_content += String.Format(",\"{0}\"", content[i]);
                str_title += String.Format(",\"{0}\"", title[i]);
            }
            str_content += "\n";
            str_title += "\n";
            if (File.Exists(file_name))
            {
                File.AppendAllText(file_name, str_content, ASCIIEncoding.UTF8);  // 添加模式,utf-8
            }
            else
            {
                File.WriteAllText(file_name, str_title, ASCIIEncoding.UTF8);     // 写表头
                File.AppendAllText(file_name, str_content, ASCIIEncoding.UTF8);  // 添加
            };
        }
    }
}
