using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskScheduler;

namespace CATC.FIDS.Utils
{
    public class TaskSchedulerHelper
    {
        /// <summary>
        /// 创建任务计划
        /// </summary>
        /// <param name="user">系统账号</param>
        /// <param name="pwd">系统密码</param>
        /// <param name="creator">创建者</param>
        /// <param name="taskName">计划名</param>
        /// <param name="path">exe位置</param>
        /// <param name="interval">PT1H1M 1小时1分钟</param>
        /// <returns></returns>
        public static int CreaTaskScheduler(string user,string pwd,string creator,string taskName,string path,string interval="PT1H1M")
        {
            try
            {
                if (IsExists(taskName))
                {
                    DeleteTask(taskName);
                }
                TaskSchedulerClass scheduler = new TaskSchedulerClass();
                scheduler.Connect(null, null, null, null);
                ITaskFolder folder = scheduler.GetFolder("\\");
                ITaskDefinition task = scheduler.NewTask(0);
                task.RegistrationInfo.Author = "航显客户端定时计划任务-民航电信";
                task.RegistrationInfo.Description = "http://www.catc.net.cn/";
                task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;

                IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Path =path;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.ExecutionTimeLimit = "PT0S";
                task.Settings.RunOnlyIfIdle = false;
                
                IRegisteredTask regTask = folder.RegisterTaskDefinition(
                                          "CATC.FIDS.Client.UI",task,(int)_TASK_CREATION.TASK_CREATE
                                          ,"","",_TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,"");
                IRunningTask runTask = regTask.Run(null);
                return (int)runTask.State;
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(ex.Message);
            }
            return (int)_TASK_STATE.TASK_STATE_UNKNOWN;
            
        }

        public static bool IsExists(string taskName)
        {
            bool isExists = false;
            IRegisteredTaskCollection task_exists = GetAllTasks();
            for (int i = 1; i <=task_exists.Count; i++)
            {
                IRegisteredTask t = task_exists[i];
                if (t.Name.Contains(taskName))
                {
                    isExists = true;
                    break;
                }
            }
            return isExists;
        }

        public static IRegisteredTaskCollection GetAllTasks()
        {
            TaskSchedulerClass scheduler = new TaskSchedulerClass();
            scheduler.Connect(null, null, null, null);
            ITaskFolder folder = scheduler.GetFolder("\\");
            IRegisteredTaskCollection task_exists = folder.GetTasks(1);
            return task_exists;
        }
        public static void DeleteTask(string taskName)
        {
            TaskSchedulerClass scheduler = new TaskSchedulerClass();
            scheduler.Connect(null, null, null, null);
            ITaskFolder folder = scheduler.GetFolder("\\");
            folder.DeleteTask(taskName,0);

        }

        public static void RunTask(string taskName)
        {
            try
            {
                TaskSchedulerClass scheduler = new TaskSchedulerClass();
                scheduler.Connect(null, null, null, null);
                ITaskFolder folder = scheduler.GetFolder("\\");
                IRegisteredTask runTask = folder.GetTask(taskName);
                runTask.Run(null);
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(ex.Message);
            }
        }
    }
}
