using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EditNET.Helpers;

namespace EditNET
{
    public static partial class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (Thread.CurrentThread == _mainThread)
                    return;

                if (args.ExceptionObject is CrashAppException)
                {
                    Debug.WriteLine("Application is going to crash");
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    return;
                }

                _ = App.ShowApplicationError((Exception)args.ExceptionObject);

#if HANDLE_CRASH
                // preventing process from crash in RELEASE
                Thread.CurrentThread.Join();
#endif
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                if (args.Exception.InnerException is CrashAppException)
                    ThreadPool.QueueUserWorkItem(_ => throw new CrashAppException(args.Exception));

                args.SetObserved();

                if (args.Exception.InnerException!.Data.Contains(App.HandledExceptionHackMark))
                    return;

                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("An Unhandled exception was thrown.");
                    Debugger.Break();
                }

                _ = App.ShowApplicationError(args.Exception);
            };
        }
    }
}