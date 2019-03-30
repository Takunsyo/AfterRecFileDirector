using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;

namespace TVRecLiveService
{
    public partial class ServiceMain : ServiceBase
    {
        public ServiceMain()
        {
            InitializeComponent();
        }
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
            int length = GetPrivateProfileString(sectionName, key, "发生错误", buffer, 999, filePath);
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
        private string TVRockPath;
        private string SpinelPath;
        private Process TVRockProcess;//Tvrock's process to monitor.
        private Process SpinelProcess;//Spinel's same shit.
        private int WaitTime = 5000;
        private bool LogToUser = false;
        private string UserName = "";
        private System.Security.SecureString PassWrod;
        //private System.Threading.Thread Checker;// a thread check both process is alive per 5 sec.

        protected override void OnStart(string[] args)
        {
            this.Process(args);
        }

        protected override void OnStop()
        {
            if(!(this.SpinelProcess is null))
            {
                this.SpinelProcess.Exited -= SpinelExitedHandler;
                this.SpinelProcess.Close();
            }
            if (!(this.TVRockProcess is null))
            {
                this.TVRockProcess.Exited -= TVRockExitedHandler;
                this.TVRockProcess.Close();
            }
        }

        public void Process(string[] args=null)
        {
            var SettingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serv.ini");
            if (File.Exists(SettingPath))
            {
                var secAll = GetSectionNames(SettingPath);
                if (secAll.Contains("Main"))
                {
                    this.LogToUser = bool.Parse(GetValue("Main", "LogToUser", SettingPath));
                    if (LogToUser)
                    {
                        this.UserName= GetValue("Main", "UserName", SettingPath);
                        string pw = GetValue("Main", "PassWord", SettingPath);
                        this.PassWrod = new System.Security.SecureString();
                        for (int i = 0; i<pw.Length; i++)
                        {
                            this.PassWrod.AppendChar(pw[i]);
                        }
                    }
                    this.WaitTime = int.Parse(GetValue("Main", "WaitTimeInMS", SettingPath));
                    if (bool.Parse(GetValue("Main", "TVRockEnabled", SettingPath)))
                    {
                        this.TVRockPath = GetValue("Main", "TVRockPath", SettingPath);
                        this.TVRockExitedHandler(null, null);
                    }
                    if (bool.Parse(GetValue("Main", "SpinelEnabled", SettingPath)))
                    {
                        this.SpinelPath = GetValue("Main", "SpinelPath", SettingPath);
                        this.SpinelExitedHandler(null, null);
                    }
                }
            }
            else
            {
                return;
            }
            Task.WaitAll();
        }

        private void TVRockExitedHandler(object sender,EventArgs e)
        {
            System.Threading.ThreadStart hdl = new System.Threading.ThreadStart(() =>
            {
                Task.Delay(WaitTime);
                try
                { //try find if there is a process allready running.
                    string exeName = System.IO.Path.GetFileNameWithoutExtension(this.TVRockPath);
                    this.TVRockProcess = System.Diagnostics.Process.GetProcessesByName(exeName).First(x => !(x is null));
                }
                catch
                { //if not it will occor a error,so create a process and run it.
                    if (this.LogToUser)
                    {
                        this.TVRockProcess = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = this.TVRockPath,
                                UseShellExecute = false,
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Minimized,
                                UserName = this.UserName,
                                Password = this.PassWrod,
                                Domain = Environment.MachineName
                            }
                        };
                        this.TVRockProcess.Start();
                    }
                    else
                    {
                        int pid;
                        ProcessExtensions.StartProcessAsCurrentUser(this.TVRockPath, out pid);
                        this.TVRockProcess = System.Diagnostics.Process.GetProcessById(pid);
                    }
                }
                finally
                { // then attach watch mechanism 
                    this.TVRockProcess.EnableRaisingEvents = true;
                    this.TVRockProcess.Exited += TVRockExitedHandler;
                    Debug.WriteLine(string.Format("[{0}]Process is running. PID = {1}", this.TVRockProcess.ProcessName,this.TVRockProcess.Id));
                    this.TVRockProcess.WaitForExit();
                    Debug.WriteLine("TVRock Process found exited. Wait " + this.WaitTime.ToString() + "ms to restart.");
                }
            });
            System.Threading.Thread mthread = new System.Threading.Thread(hdl);
            mthread.Start();
        }

        private void SpinelExitedHandler(object sender, EventArgs e)
        {
            System.Threading.ThreadStart hdl = new System.Threading.ThreadStart(() =>
            {
                Task.Delay(this.WaitTime);
                try
                {
                    string exeName = System.IO.Path.GetFileNameWithoutExtension(this.SpinelPath);
                    this.SpinelProcess = System.Diagnostics.Process.GetProcessesByName(exeName).First(x => !(x is null));
                }
                catch
                {
                    if (this.LogToUser)
                    {
                        this.SpinelProcess = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = this.SpinelPath,
                                UseShellExecute = false,
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Minimized,
                                UserName=this.UserName,
                                Password=this.PassWrod
                            }
                        };
                        this.SpinelProcess.Start();
                    }
                    else
                    { 
                        int pid;
                        ProcessExtensions.StartProcessAsCurrentUser(this.SpinelPath, out pid);
                        this.SpinelProcess  = System.Diagnostics.Process.GetProcessById(pid);
                    }
                }
                finally
                {
                    this.SpinelProcess.EnableRaisingEvents = true;
                    this.SpinelProcess.Exited += SpinelExitedHandler;
                    Debug.WriteLine(string.Format("[{0}]Process is running. PID = {1}", this.SpinelProcess.ProcessName, this.SpinelProcess.Id));
                    this.SpinelProcess.WaitForExit();
                    Debug.WriteLine("Spinel Process found exited. Wait "+this.WaitTime.ToString()+"ms to restart.");
                }
            });
            System.Threading.Thread mthread = new System.Threading.Thread(hdl);
            mthread.Start();
        }


    }
}
