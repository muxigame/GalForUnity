//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveThreadTool.cs
//
//        Created by 半世癫(Roc) at 2021-12-10 22:43:31
//
//======================================================================

using System.Threading;

namespace GalForUnity.System.Archive{
    public class ArchiveThreadTool:GfuInstanceManager<ArchiveThreadTool>{
        public delegate void ArchiveThreadAction<T1>(out T1 arg1);
        private readonly int _waitTime = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadWaitingTime;
        private readonly int _waitMaxSecond = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadWaitingMaxSecond;

        /// <summary>
        /// 期间线程处于等待状态，直到action将引用值修改为true线程继续运行,如果一直得不到结果线程会一直等待,直到超时
        /// </summary>
        /// <param name="action">线程函数</param>
        /// <param name="always">是否持续调用</param>
        public void Wait(ArchiveThreadAction<bool> action,bool always=false){
            bool isExecuted = false;
            action.Invoke(out isExecuted);
            if (Thread.CurrentThread.Name == ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName){
                int canWaitTime = _waitMaxSecond;
                while (!isExecuted && canWaitTime > 0){
                    Thread.Sleep(_waitTime);
                    canWaitTime -= _waitTime;
                    if(always) action.Invoke(out isExecuted);
                } 
            }
        }
        /// <summary>
        /// 通过Mono代理，在Mono环境中运行函数，期间线程处于等待状态，直到action将引用值修改为true线程继续运行,如果一直得不到结果线程会一直等待,直到超时
        /// </summary>
        /// <param name="action">线程函数</param>
        /// <param name="always">是否持续调用</param>
        public void WaitForMono(ArchiveThreadAction<bool> action,bool always=false){
            bool isExecuted = false;
            GfuRunOnMono.Update(() => {
                action.Invoke(out isExecuted);
            });
            if (Thread.CurrentThread.Name == ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName){
                int canWaitTime = _waitMaxSecond*1000;
                while (!isExecuted && canWaitTime > 0){
                    Thread.Sleep(_waitTime);
                    canWaitTime -= _waitTime;
                    if(always) GfuRunOnMono.Update(() => {
                        action.Invoke(out isExecuted);
                    });
                } 
            }
        }
        /// <summary>
        /// 通过Mono代理，在Mono环境中运行函数，期间线程处于等待状态，直到action将引用值修改为true线程继续运行,如果一直得不到结果线程会一直等待,直到超时
        /// </summary>
        /// <param name="action">线程函数</param>
        /// <param name="always">是否持续调用</param>
        public void WaitForMonoAsync(ArchiveThreadAction<bool> action,bool always=false){
            new Thread(() => {
                bool isExecuted = false;
                GfuRunOnMono.Update(() => {
                    action.Invoke(out isExecuted);
                });
                int canWaitTime = _waitMaxSecond * 1000;
                while (!isExecuted && canWaitTime > 0){
                    Thread.Sleep(_waitTime);
                    canWaitTime -= _waitTime;
                    if(always) GfuRunOnMono.Update(() => {
                        action.Invoke(out isExecuted);
                    });
                }
            }){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        }
    }
}
