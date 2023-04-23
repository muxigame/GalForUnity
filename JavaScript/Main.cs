using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Esprima;
using GalForUnity.Core;
using Jint;
using Jint.Native;
using Jint.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// using Jint.CommonJS;

public class Main : MonoBehaviour
{
    public TextAsset jsCode;
    public TextAsset moduleCode;

    public Text console;

    // Start is called before the first frame update
    private void Start()
    {
        // try
        // {
        var dataPath = Application.dataPath;
        var synchronizationContext = SynchronizationContext.Current;
        new Thread(() =>
        {
            var engine = new Engine(cfg =>
            {
                cfg.DebugMode();
                cfg.EnableModules(@"C:\Users\VRcollab\WorkSpace\Android Test\Assets\GalForUnity\JavaScript\Resources\");
                cfg.AllowClr().AllowClr(typeof(Debug).Assembly);
            }).SetValue("log", new Action<object>(x => Debug.Log(x)));
            engine.SetValue("setTimeout", new Action<JsValue, double>(async (x, y) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(y));
                x.Call();
            }));

            var core = new CSCore
                {
                    TextMeshProUGUI = console,
                    Engine = engine,
                    SynchronizationContext = synchronizationContext,
                    dataPath = dataPath
                }
                ;
            engine.AddModule("CSCore", builder => builder
                .ExportType<CSCore>()
            );
            engine.AddModule("UnityEngine", builder => builder
                .ExportType<Resources>()
                .ExportType<GameObject>()
                .ExportType<Transform>()
                .ExportType<Object>()
                .ExportType<Vector4>()
                .ExportType<Vector3>()
                .ExportType<Vector2>()
            );
            try
            {
                engine.ImportModule(@".\JavaScript\main.js");
            }
            catch (UnityException e)
            {
                Debug.LogError(e);
            }
          
        }).Start();
    }

    public class CSCore
    {
        public static CSCore Instance;
        public string dataPath;
        public Engine Engine;
        public SynchronizationContext SynchronizationContext;
        public Text TextMeshProUGUI;

        public CSCore()
        {
            Instance = this;
        }

        public static object Mono(JsValue jsValue)
        {
            return RunInMainThread(jsValue.Call);
        }

        public static object LoadResource(string path)
        {
            return Resources.Load(path);
        }

        public static void SetBackground(Sprite sprite)
        {
            RunInMainThread(() => GalCore.ActiveCore.SetBackground(sprite));
        }

        public static void SetBackground(Texture2D texture)
        {
            RunInMainThread(() => GalCore.ActiveCore.SetBackground(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f))));
        }

        public static void ShowName(object name)
        {
            RunInMainThread(() => Instance.TextMeshProUGUI.text = name.ToString());
        }

        public static void Log(object message, Object context)
        {
            if (Instance.Engine.DebugHandler.CurrentLocation == null)
                RunInMainThread(() => Debug.Log(message, context));
            else
                RunInMainThread(() =>
                {
                    var javaScriptException = new JavaScriptException(JsValue.Undefined).SetJavaScriptCallstack(Instance.Engine, (Location)Instance.Engine.DebugHandler.CurrentLocation);
                    var javaScriptStackTrace = javaScriptException.JavaScriptStackTrace;

                    if (string.IsNullOrEmpty(javaScriptStackTrace))
                    {
                        Debug.Log(message, context);
                        return;
                    }

                    var patternFile = @"(?:\s*at\s.*\s)(?<path>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<name>[\w]+.[\w]+)";
                    var patternLine = @"(?<path>(Assets\\)([\s\.\-\w]+\\)*)(?<name>[\w]+.[\w]+):(?<line>\d*):(?<col>\d*)";

                    foreach (Match match in Regex.Matches(javaScriptStackTrace, patternFile))
                    {
                        var path = match.Result("${path}${name}");
                        var locationSource = Path.Combine("Assets", Path.GetRelativePath(Instance.dataPath, path));
                        javaScriptStackTrace = javaScriptStackTrace.Replace(path, locationSource);
                    }

                    var replacement = "(at <a href=\"${path}${name}\" line=\"${line}\" col=\"${col}\">${path}${name}:${line}:${col}</a>)";
                    Debug.Log(string.Concat(message, "\n", Regex.Replace(javaScriptStackTrace, patternLine, replacement), context));
                });
        }

        private static void RunInMainThread(Action callback)
        {
            var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            Instance.SynchronizationContext.Send(x =>
            {
                var mainThreadEventWaitHandle = (EventWaitHandle)x;
                callback.Invoke();
                mainThreadEventWaitHandle.Set();
            }, eventWaitHandle);
            eventWaitHandle.WaitOne();
        }

        private static T RunInMainThread<T>(Func<T> callback)
        {
            var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            T value = default;
            Instance.SynchronizationContext.Send(x =>
            {
                var mainThreadEventWaitHandle = (EventWaitHandle)x;
                value = callback.Invoke();
                mainThreadEventWaitHandle.Set();
            }, eventWaitHandle);
            eventWaitHandle.WaitOne();
            return value;
        }
    }
}