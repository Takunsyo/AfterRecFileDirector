using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Reflection;

namespace AfterRecFileDirector
{
    class Program
    {
        #region"ConsoleWindowControl"
        //Dll import
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        //RVMCore dll
        #endregion
        #region "IniReading"
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string sectionName, string key, string defaultValue, byte[] returnBuffer, int size, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string sectionName, string key, string value, string filePath);
        /// <summary>
        /// 根据Key读取Value
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="key">key的名称</param>
        /// <param name="filePath">文件路径</param>
        private static string GetValue(string sectionName, string key, string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(sectionName, key, "", buffer, 999, filePath);
            string rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length);
            return rs;
        }
        /// <summary>
        /// 获取ini文件内所有的section名称
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回一个包含section名称的集合</returns>
        private static List<string> GetSectionNames(string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(null, "", "", buffer, 999, filePath);
            String[] rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
            return rs.ToList();
        }
        /// <summary>
        /// 获取指定section内的所有key
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回一个包含key名称的集合</returns>
        public static List<string> GetKeys(string sectionName, string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(sectionName, null, "", buffer, 999, filePath);
            String[] rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
            return rs.ToList();
        }
        /// <summary>
        /// 保存内容到ini文件
        /// <para>若存在相同的key，就覆盖，否则就增加</para>
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="key">key的名称</param>
        /// <param name="value">存储的值</param>
        /// <param name="filePath">文件路径</param>
        public static bool SetValue(string sectionName, string key, string value, string filePath)
        {
            int rs = (int)WritePrivateProfileString(sectionName, key, value, filePath);
            return rs > 0;
        }
        /// <summary>
        /// 移除指定的section
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool RemoveSection(string sectionName, string filePath)
        {
            int rs = (int)WritePrivateProfileString(sectionName, null, "", filePath);
            return rs > 0;
        }
        /// <summary>
        /// 移除指定的key
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool Removekey(string sectionName, string key, string filePath)
        {
            int rs = (int)WritePrivateProfileString(sectionName, key, null, filePath);
            return rs > 0;
        }
        #endregion

        //end import
        
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr mHwnd = GetConsoleWindow();
            ShowWindow(mHwnd, SW_HIDE);
            Assembly RVMCore = null;
            System.Type SettingObj = null;
            //System.Type StreamFile = null;
            //System.Type Share = null;
            System.Type TVAFT = null;
            dynamic mySetting = null;
            //Check if running as service
            if (args.Length >= 1 && args[0].ToUpper() == "-SERVICE")
            {//Running as windows service

                //Code to start Service
                string fPath = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "TVRecLiveService.dll");
                if (System.IO.File.Exists(fPath))
                {
                    var Service = Assembly.LoadFile(fPath);
                    var Program = Service.GetType("Program");
                    Program.InvokeMember("Main", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static, null, null, new string[] { });
                    return;
                }
                else
                {
                    ShowWindow(mHwnd, SW_SHOW);
                    Console.WriteLine("Missing DLL file!! \n \"TVRecLiveService.dll\" is missing.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(609); //exit app with error code "ERROR_BAD_DLL_ENTRYPOINT"
                }
            }
            else
            {//Load RVMCore
                string fPath = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "RVMCore.dll");
                if (System.IO.File.Exists(fPath)) {
                    RVMCore = Assembly.LoadFile(fPath);
                    SettingObj = RVMCore.GetType("RVMCore.SettingObj");
                    //StreamFile = RVMCore.GetType("RVMCore.StreamFile");
                    //Share = RVMCore.GetType("RVMCore.Share");
                    TVAFT = RVMCore.GetType("RVMCore.TVAFT");
                }
                else
                {
                    ShowWindow(mHwnd, SW_SHOW);
                    Console.WriteLine("Missing DLL file!! \n \"RVMCore.dll\" is missing.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(609);//exit app with error code "ERROR_BAD_DLL_ENTRYPOINT"
                }
            }

            // Read setting
            bool a = false;
            while (!a)
            {
                Console.WriteLine("Reading settings.");

                try
                {
                    //mySetting = SettingObj.Read();
                    mySetting = (dynamic)SettingObj.InvokeMember("Read", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static, null, null, new string[] { });
                    //mySetting = ((dynamic)Activator.CreateInstance(SettingObj)).Read();
                    a = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error : {0}", ex.Message);
                    Console.WriteLine("Sleep 10 sec...");
                    System.Threading.Thread.Sleep(10000);
                }
            }
            // TestCodes()
#if DEBUG
            //ShowWindow(mHwnd, SW_SHOW);
            //Upload(@"I:\0.SuperBird\[20180506]ミュージカル『刀剣乱舞』～結びの響き、始まりの音～.ts", true,false);
            //Debugger.Break();
#endif
            if (args.Length <= 0)
            {
                Console.WriteLine("No argument found, App will run on Winform Mode.");
                if (mySetting.AllowBeep)
                    Console.Beep(550, 200);
                FreeConsole();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new MainWindow());
                System.Type MainWindow = RVMCore.GetType("RVMCore.MainWindow");
                Application.Run((Form)Activator.CreateInstance(MainWindow));
                return;
            }
            if (args.Length >= 1 && args[0].ToUpper() == "5S")
            {
                Show5S();
            }
            if (!(args.Length >= 1 && args[0].StartsWith("-")))
            {
                FreeConsole();
                ShowWindow(mHwnd, SW_SHOW);
            }
            // AllocConsole()
            Console.WriteLine("App is running in Console Mode.");
            // if in console mode
            //Start verify process
            string[] margs = args;
            bool isVerify = false;
            try
            {
                string tmp = args.First(x => x.ToLower().Equals("-verify"));
                if (tmp != null) isVerify = true;
            }
            catch
            {
                isVerify = false;
            }
            if (isVerify)
            {
                bool doit = false;
                for (int i = 0; i < 3; i++)
                {
                    string mpath = args[1];
                    if (System.IO.File.Exists(mpath))
                    {
                        Console.WriteLine("Check file exitens \"{0}\" Attempt {1} time.", mpath, i.ToString());
                        if (i == 2) { doit = true; break; }
                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {
                        doit = false;
                        break;
                    }
                }
                if (doit)
                {
                    Console.WriteLine("File checked, proceed to currect settings parameters.");
                    string logpath = (args.Length >= 3) ? System.IO.Path.Combine(args[2], "tvrock.log2") : System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tvrock", "tvrock.log2");
                    if (!System.IO.File.Exists(logpath))
                    {
                        Console.WriteLine("Failed to attach! Log file not exsited!");
                        Console.WriteLine("Log Path : {0}", logpath);
                        Environment.Exit(2);//exit app with error code "ERROR_FILE_NOT_FOUND"
                    }
                    string lcPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    try
                    {
                        // only for debug:
                        //lcPath = "E:\\AfterRecFileDirector.exe";
                        string line = System.IO.File.ReadAllLines(logpath, Encoding.GetEncoding("Shift_JIS")).
                                         First(x => x.Contains(args[1]) && !x.Contains("-verify") && x.Contains(lcPath));
                        int index = line.IndexOf(lcPath);
                        line = System.Text.RegularExpressions.Regex.Replace(line.Substring(index), "[\"”]", "", System.Text.RegularExpressions.RegexOptions.None);
                        string[] tmpStr = line.Split(' ');
                        margs = tmpStr;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to attach! Exited with error: {0}!", e.Message);
                        Environment.Exit(12);//exit app with error code "ERROR_INVALID_ACCESS"
                    }
                }
                else
                {
                    Console.WriteLine("File not exists, either early procedure was a success action, or a faulty parameter.");
                    Console.WriteLine("Check your parameters and try again. This action will leave no logs.");
                    Environment.Exit(50);//exit app with error code "ERROR_NOT_SUPPORTED"
                }
            }
            MethodInfo SortFile = TVAFT.GetMethod("SortFile");
            bool paras = (bool)SortFile.Invoke(null, new object[] { margs });
            if (paras)
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        [Obsolete("This method is deprecated, no one want to see 5s stuff anymore.",false)]
        private static void Show5S()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.AppendLine("改善生产现场环境、提升生产效率、保障产品品质、营造快乐的工作环境，今天你实施5S了吗？");
            s.AppendLine("");
            s.AppendLine("什么是5S？");
            s.AppendLine("5S就是整理（SEIRI）、整顿（SEITON）、清扫（SEISO）、清洁（SEIKETSU）、素养（SHITSUKE）五个项目。");
            s.AppendLine("没有实施5S的计算机，触目可及地就可感受到计算机的脏乱，到处可见的不知何用的文件，未经管理的大量数据。即使再高速的计算机也无法处理大量的无意义的数据，最终只会拖慢处理速度。");
            s.AppendLine("1S-整理");
            s.AppendLine("定义：         区分要与不要的东西， 计算机内除了要用的东西以外， 一切都不下载、不生成。一个概略的判定原则， 是将未来30天内， 用不着的任何东西都可移出现场。");
            s.AppendLine("目的： 将硬盘腾出来活用。");
            s.AppendLine("2S-整顿");
            s.AppendLine("定义：         要的东西依规定定位、定方法摆放整齐，明确数量，明确标示，既实现\"三定\"：定名、定量、定位。");
            s.AppendLine("目的：不浪费\"时间\"找东西。");
            s.AppendLine("3S-清扫");
            s.AppendLine("定义：         清除计算机内的垃圾文件， 用过的东西无用即删。");
            s.AppendLine("目的： 消除\"脏污\"， 保持硬盘没有碎片、运转畅快。");
            s.AppendLine("4S-清洁");
            s.AppendLine("定义：         将上面3S实施的做法制度化， 规范化， 维持其成果。");
            s.AppendLine("目的： 通过制度化来维持成果。");
            s.AppendLine("5S-素养");
            s.AppendLine("定义：         培养文明礼貌习惯， 按规定行事， 养成良好的工作习惯。目的： 南无阿弥佗佛， 成为对任何工作都讲究认真的人。");
            s.AppendLine("");
            s.AppendLine("认真的你， 今天有做到5S吗？");
            var title = "系统例行推销政策！请读完。";
            while (true)
            {
                var box = MessageBox.Show(s.ToString(), title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, (MessageBoxOptions)0x40000);
                if (box == DialogResult.Yes)
                    System.Environment.Exit(0);
                else
                    MessageBox.Show("请立刻清扫!");
            }
        }
    }
}
