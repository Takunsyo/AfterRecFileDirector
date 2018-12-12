using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Reflection;

namespace RVMCore
{
    public static class Logging
    {
        //private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog MainLogger = LogManager.GetLogger("MainLogs");
        private static readonly ILog UploaderLogger = LogManager.GetLogger("UploaderLogs");
        private static readonly ILog OtherLogger = LogManager.GetLogger("OtherLogs");

        private static ILog SelectLogger(string cls)
        {
            switch (cls)
            {
                case "GoogleWarpper":
                case "OneDriveWarpper":
                    return UploaderLogger;
                case "RVMCore":
                    return MainLogger;
                default:
                    return OtherLogger;
            }
        }
        
        public static void InfoLognConsole(this string message)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Namespace;
            SelectLogger(cls).Info(message);
            Console.WriteLine(message);
        }

        public static void InfoLognConsole(this string format, params object[] args)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Namespace;
            SelectLogger(cls).InfoFormat(format, args);
            Console.WriteLine(format, args);
        }

        public static void ErrorLognConsole(this string message)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Namespace;
            SelectLogger(cls).Error(message);
            Console.WriteLine(message);
        }

        public static void ErrorLognConsole(this string format, params object[] args)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Namespace;
            SelectLogger(cls).ErrorFormat(format, args);
            Console.WriteLine(format, args);
        }

        /// <summary>
        /// Log4net config class.
        /// </summary>
        [AttributeUsage(AttributeTargets.Assembly)]
        public class LoggerConfigAttribute : ConfiguratorAttribute
        {
            public LoggerConfigAttribute()
                : base(0)
            {
            }
            /// <summary>
            /// Setup Log4net in code. <para>*Time to get rid of Config.xml hah?</para>
            /// </summary>
            public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
            {
                Hierarchy hierarchy = (Hierarchy)targetRepository;

                //Universal layout pattern.
                PatternLayout patternLayout = new PatternLayout();
                //patternLayout.Header = "[task time=%date{HH:mm:ss,fff}]%newline";
                //patternLayout.Footer = "[/task]%newline";
                patternLayout.ConversionPattern = @"%-5level %date{yyyy/MM/dd_HH:mm:ss,fff} [%thread]] - %message%newline";
                patternLayout.ActivateOptions();

                log4net.Filter.LevelRangeFilter rangeFilter = new log4net.Filter.LevelRangeFilter
                {
                    LevelMax = Level.Fatal,
                    LevelMin = Level.Info
                };
                rangeFilter.ActivateOptions();

                //MainLogger.(in log4net root)
                RollingFileAppender mainRoller = new RollingFileAppender
                {
                    AppendToFile = true, //Save all log to the same file.
                    RollingStyle = RollingFileAppender.RollingMode.Date, //Make a new log file every day.
                    File = @"logs\mainlog.log",
                    Layout = patternLayout,
                    LockingModel = new FileAppender.MinimalLock(),
                    MaxSizeRollBackups = 10,
                    MaximumFileSize = "1000KB",
                    StaticLogFileName = true
                };
                mainRoller.AddFilter(rangeFilter); //don't know what this does. I think I should put it here anyway.
                mainRoller.ActivateOptions();
                hierarchy.Root.AddAppender(mainRoller);
                                                
                hierarchy.Root.Level = Level.All;
                hierarchy.Name = "MainLogs";
                hierarchy.Configured = true;


                //Uploader Logger.
                RollingFileAppender UploadRoller = new RollingFileAppender
                {
                    AppendToFile = true,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    File = @"logs\Uploader.log",
                    Layout = patternLayout,
                    MaxSizeRollBackups = 10,
                    MaximumFileSize = "1GB",
                    StaticLogFileName = true
                };
                UploadRoller.AddFilter(rangeFilter);
                UploadRoller.ActivateOptions();
                //Add make a new logger that use this config.
                ILog _uploadLogger = LogManager.GetLogger("UploaderLogs");
                var uploadLogger = (Logger)_uploadLogger.Logger;
                uploadLogger.Level = Level.All;
                uploadLogger.AddAppender(UploadRoller);
                uploadLogger.Repository.Configured = true;

                //Ohter logs will all go here.
                RollingFileAppender OtherLogs = new RollingFileAppender
                {
                    AppendToFile = true,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    File = @"logs\Others.log",
                    Layout = patternLayout,
                    LockingModel = new FileAppender.MinimalLock(), //this should allow mutiple instances writing to same file.
                    MaxSizeRollBackups = 10,
                    MaximumFileSize = "1GB",
                    StaticLogFileName = true
                };
                OtherLogs.AddFilter(rangeFilter);
                OtherLogs.ActivateOptions();

                ILog _otherLogger = LogManager.GetLogger("OtherLogs");
                var otherLogger = (Logger)_otherLogger.Logger;
                otherLogger.Level = Level.All;
                otherLogger.AddAppender(OtherLogs);
                otherLogger.Repository.Configured = true;
            }

        }
    }
}
