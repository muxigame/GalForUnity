using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Esprima;
using Jint;
// using Jint.CommonJS;
using Jint.Native;
using Jint.Native.Error;
using Jint.Runtime;
using Jint.Runtime.CallStack;
using Jint.Runtime.Debugger;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class Main : MonoBehaviour
{
    public TextAsset jsCode;
    public TextAsset moduleCode;

    public Text console;
    // Start is called before the first frame update
    void Start()
    {
    
        // try
        // {
            var engine = new Engine(cfg =>
                {
                    cfg.DebugMode();
                    // cfg.AllowDebuggerStatement();
                    cfg.EnableModules(@"C:\Users\Roc\GalForUnityCreator\Assets\GalForUnity\JavaScript\Resources\");
                    cfg.CatchClrExceptions();
                    cfg.AllowClr().AllowClr(typeof(Debug).Assembly);
                }).SetValue("log", new Action<object>(x => console.text = x.ToString()));
            // engine.
            // engine.Execute(moduleCode.text);
            //
            // engine.Step += (object sender, DebugInformation e) =>
            // {
            //     JintCallStack callStack = engine.GetType().GetField("CallStack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(engine)  as JintCallStack;
            //     Debug.Log("callStack count"+callStack.Count());
            //     var javaScriptException = new JavaScriptException(ErrorConstructor.CreateErrorConstructor(engine,"test")).SetCallstack(engine);
            //     Debug.Log(javaScriptException.CallStack);
            //     Debug.Log(javaScriptException.LineNumber);
            //     return StepMode.Into;
            // };
            var core = new CSCore()
                {
                    TextMeshProUGUI = console,
                    Engine = engine
                }
            ;
            engine.AddModule("CSCore", builder => builder
                .ExportType<CSCore>()
            );
            // engine.Execute()
            engine.ImportModule( @".\JavaScript\main.js");
            //     .RegisterInternalModule("CSCore", typeof(Core))
            //     .RegisterInternalModule("console", typeof(Debug))
            //     .RunMain("JavaScript/main.js");
            // engine.Evaluate("function hello() {};var text=1;hello();");
        // }
        // catch (Exception e)
        // {
        //     console.text=e.ToString();
        //     throw;
        // }
    }
    public class CSCore
    {
        public Text TextMeshProUGUI;
        public Engine Engine;

        public CSCore() => Instance = this;
        public static CSCore Instance;

        public static void ShowName(string name) => Instance.TextMeshProUGUI.text = name;
        public static void Log(string message, Object context)
        {
            var javaScriptException = new JavaScriptException(JsValue.Undefined).SetJavaScriptCallstack(Instance.Engine, (Location)Instance.Engine.DebugHandler.CurrentLocation);
            var javaScriptStackTrace = javaScriptException.JavaScriptStackTrace;

            if (string.IsNullOrEmpty(javaScriptStackTrace))
            {
                Debug.Log(message,context);
                return;
            }
            string patternFile = @"(?:\s*at\s.*\s)(?<path>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<name>[\w]+.[\w]+)";
            string patternLine = @"(?<path>(Assets\\)([\s\.\-\w]+\\)*)(?<name>[\w]+.[\w]+):(?<line>\d*):(?<col>\d*)";

            foreach (Match match in Regex.Matches(javaScriptStackTrace, patternFile))
            {
                var path = match.Result("${path}${name}");
                var locationSource = Path.Combine("Assets",Path.GetRelativePath(Application.dataPath, path));
                javaScriptStackTrace = javaScriptStackTrace.Replace(path, locationSource);
            }
            string replacement = "(at <a href=\"${path}${name}\" line=\"${line}\">${path}${name}:${line}:${col}</a>)";
            Debug.Log(string.Concat(message,"\n",Regex.Replace(javaScriptStackTrace, patternLine, replacement),context));
        }
    }
    
}
