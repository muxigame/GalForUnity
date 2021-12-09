using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GalForUnity.Attributes;

// ReSharper disable all InconsistentNaming
namespace GalForUnity.System{
    public class GfuLanguage{
        public static GfuLanguage GfuLanguageInstance=new GfuLanguage();
        private GfuLanguage(){
            
        }

        private static Dictionary<string,LanguageItem> LogLanguageItem=new Dictionary<string, LanguageItem>() {
            {"The GfuInstanceID was not found and cannot be added, and this is not a local resource object, meaning the object is not saved",
                new LanguageItem("The GfuInstanceID was not found and cannot be added, and this is not a local resource object, meaning the object is not saved",
                    "没有找到GfuInstanceID，且无法添加,而且这不是一个本地资源对象，意味着该对象没有被保存")},
            {"The node failed to save itself, which means the data may not have been saved",
                new LanguageItem("The node failed to save itself, which means the data may not have been saved",
                    "节点无法保存自身，这意味着数据可能没有被保存")},
            {"The node failed to be resolved, which means there may be an unsupported type in the node",
                new LanguageItem("The node failed to be resolved, which means there may be an unsupported type in the node",
                    "无法解析节点，这意味着节点中可能存在不支持的类型")},
            {"This is not a GfuNode object",
                new LanguageItem("This is not a GfuNode object",
                    "这不是一个GfuNode对象")},
            {"Resolving the port failed, which means that there may be an unsupported type in the default data for the port",
                new LanguageItem("Resolving the port failed, which means that there may be an unsupported type in the default data for the port",
                    "解析端口失败，这意味着该端口的默认数据中可能存在不支持的类型")},
            {"There is an exception when saving the file, please contact the developer as soon as possible to avoid loss",
                new LanguageItem("There is an exception when saving the file, please contact the developer as soon as possible to avoid loss",
                    "在保存文件的时候出现了一个异常，请尽快与开发者联系，避免损失")},
            {"Do not save, or because the scene switch caused the object lost, cannot get the value of the object from the ID, you can try to open the original scene and try again, do not save!",
                new LanguageItem("Do not save, or because the scene switch caused the object lost, cannot get the value of the object from the ID, you can try to open the original scene and try again, do not save!",
                    "请勿保存场景图，或因为场景切换导致对象丢失,亦或者您手动删除了相关引用，无法从ID获得对象的值，您可以尝试打开原有场景重试")},
            {"Failed to get a value",
                new LanguageItem("Failed to get a value",
                    "获取值失败")},
            {"Failed to initialize node data",
                new LanguageItem("Failed to initialize node data",
                    "初始化节点数据出错")},
            {"This is not a GfuPort",
                new LanguageItem("This is not a GfuPort",
                    "这不是一个GfuPort")},
            {"Parsing the port failed, which means that the port data was not saved",
                new LanguageItem("Parsing the port failed, which means that the port data was not saved",
                    "解析端口失败，这意味着端口数据没有被保存")},
            {"Parse connection failure",
                new LanguageItem("Parse connection failure",
                    "解析连接失败")},
            {"It is not allowed to execute the diagram without playing, please run the scene first",
                new LanguageItem("It is not allowed to execute the diagram without playing, please run the scene first",
                    "不允许在没有播放的时候执行图，请先运行场景")},
            {"This Resource object is not saved in the Resource directory and may not be loaded in the game:",
                new LanguageItem("This Resource object is not saved in the Resource directory and may not be loaded in the game:",
                    "这个资源对象没有被保存在Resource目录下，可能无法在游戏中加载:")},
            {"The role node has no corresponding operation role",
                new LanguageItem("The role node has no corresponding operation role",
                    "角色节点没有对应的操作角色,故角色节点无效")},
            {"The current node has no output port",
                new LanguageItem("The current node has no output port",
                    "当前节点没有输出端口")},
            {"The current port is not connected",
                new LanguageItem("The current port is not connected",
                    "当前端口未连接")},
            {"The current connection has no input node",
                new LanguageItem("The current connection has no input node",
                    "当前连接没有输入节点")},
            {"In addition to the abnormal 0",
                new LanguageItem("In addition to the abnormal 0",
                    "除0异常")},
        };

        public static string ParseLog(string name){
            return LogLanguageItem[name].Value;
        }

        private static Regex isDir = new Regex("(^[a-zA-Z]+/?)+");
        private static Regex isName = new Regex("[a-zA-Z0-9_]+");
        public static string Parse(string name){
            if(string.IsNullOrEmpty(name)) return name;
            if (isDir.IsMatch(name)){
                var newString = new Regex("[^\\w/]").Replace(name, "").ToUpper();
                if (newString.Contains("/")){
                    var strings = newString.Split('/');
                    string returnString = "";
                    foreach (var s in strings){
                        try{
                            returnString += (typeof(GfuLanguage).GetField(s).GetValue(GfuLanguageInstance) as LanguageItem).Value+"/";
                        }catch{
                            throw new NullReferenceException($"language item cannot find string:{name} and want find:{s}");
                        }
                    }
                    if(!string.IsNullOrEmpty(returnString?.Trim())) return returnString.Substring(0,returnString.Length-1);
                }
            }
            if(isName.IsMatch(name)){
                // Debug.Log(name);
                var value = (string) (typeof(GfuLanguage).GetField(name.ToUpper())?.GetValue(GfuLanguageInstance) as LanguageItem)?.Value;
                if (!string.IsNullOrEmpty(value?.Trim())) return value;
            }
            return name;
        }
        
        /// <summary>
        /// 获取字符串的英文长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }


        public enum LanguageEnum{
            [Rename(nameof(English))]
            English,
            [Rename(nameof(Chinese))]
            Chinese,
            // [Rename(nameof(Japanese))]
            // Japanese,
        }
        
        public readonly LanguageItem LANGUAGE = new LanguageItem("Language","语言");     
        public readonly LanguageItem ENGLISH = new LanguageItem("English","英语");       
        public readonly LanguageItem CHINESE = new LanguageItem("Chinese","简体中文");
        public readonly LanguageItem JAPANESE = new LanguageItem("Japanese","日本语");

        public readonly LanguageItem ROLEDATA = new LanguageItem("Role Data","角色数据");
        public readonly LanguageItem ROLEMODEL = new LanguageItem("Role Model","角色模型");
        public readonly LanguageItem PLOTMODEL = new LanguageItem("Plot Model","剧情模型");
        public readonly LanguageItem SCENEMODEL = new LanguageItem("Plot Model","场景模型");
        public readonly LanguageItem SCENEMODELS = new LanguageItem("Scene Models","场景模型集");
        public readonly LanguageItem PLOTFLOW = new LanguageItem("Plot Flow","剧情流");
        public readonly LanguageItem PLOTITEM = new LanguageItem("Plot Item","剧情项");
        
        public readonly LanguageItem PLOTREQUIRE = new LanguageItem("Plot Require","剧情要求");
        public readonly LanguageItem SCENEREQUIRE = new LanguageItem("Scene Require","场景要求");
        public readonly LanguageItem DURATION = new LanguageItem("Duration","持续时间");
        public readonly LanguageItem ROLEDATAREQUIRE = new LanguageItem("Role Data Require","角色数据要求");
        public readonly LanguageItem TRIGGERPLOT = new LanguageItem("Trigger Plot","触发剧情");
        public readonly LanguageItem REPLACEPLOT = new LanguageItem("Replace Plot","替换剧情");
        public readonly LanguageItem REPLACECOUNT = new LanguageItem("Replace Count","替换次数");
        public readonly LanguageItem PLOTPROBABILITY = new LanguageItem("Plot Probability","剧情触发率(百分之)");
        public readonly LanguageItem PLOTTYPE = new LanguageItem("Plot Type", "剧情类型");
        public readonly LanguageItem NORMAL = new LanguageItem("Normal", "正常");
        public readonly LanguageItem FIXED = new LanguageItem("Fixed", "固定");
        public readonly LanguageItem SPECIAL = new LanguageItem("Special", "特殊");
        public readonly LanguageItem REPLACE = new LanguageItem("Replace", "替换");
        public readonly LanguageItem OVERDAY = new LanguageItem("Over Day","结束本日");
        public readonly LanguageItem PLOTGRAPH = new LanguageItem("Plot Graph","剧情图");      
        public readonly LanguageItem ROLEINDEX = new LanguageItem("Role Index ", "角色索引 ");    
        public readonly LanguageItem _STARTINDEX = new LanguageItem("Start Index", "开始索引 ");
        public readonly LanguageItem _PLOTFLOWTYPE = new LanguageItem("Plot Flow Type", "剧情流模式");

        public readonly LanguageItem ACTIONMODEL = new LanguageItem("Action Model", "活动模型");    
        public readonly LanguageItem ROLE = new LanguageItem("Role", "角色");    
        public readonly LanguageItem PLOTITEMS = new LanguageItem("Plot Items", "剧情项集");
        public readonly LanguageItem BEFORE = new LanguageItem("Before", "之前");
        public readonly LanguageItem AFTER = new LanguageItem("After", "之后");
        public readonly LanguageItem TRIGGERTYPE = new LanguageItem("Trigger Type", "触发类型");
        public readonly LanguageItem PLOT = new LanguageItem("Plot", "剧情");
        public readonly LanguageItem DATE = new LanguageItem("Date", "日期");

        public readonly LanguageItem ACTIONMODELTYPE = new LanguageItem("Action Model Type", "活动模型类型");
        public readonly LanguageItem DIRECTIONSCENEMODEL = new LanguageItem("Direction Scene Model", "目标场景模型");
        public readonly LanguageItem DIRECTIONPLOTMODEL = new LanguageItem("Direction Plot Model", "目标剧情模型");
        public readonly LanguageItem CUSTOMEVENT = new LanguageItem("Custom Event", "自定义事件");
        public readonly LanguageItem CUSTOMEVENTNOPARAM = new LanguageItem("Custom Event No Param", "自定义无参事件");
        public readonly LanguageItem CHANGEPROBABILITY = new LanguageItem("Change To Probability", "要增减的触发率(百分之)");
        public readonly LanguageItem GROWUP = new LanguageItem("GrowUp", "变更数值");
        public readonly LanguageItem JUMPPLOT = new LanguageItem("Jump Plot", "跳转剧情");
        public readonly LanguageItem GOTOSCENE = new LanguageItem("Go to Scene", "前往场景");
        public readonly LanguageItem CHANGEPLOTPROBABILITY = new LanguageItem("Change Plot Probability", "变更剧情触发率");

        public readonly LanguageItem ROLESET = new LanguageItem("Role Set", "角色集合");
        public readonly LanguageItem CURRENTTIME = new LanguageItem("Current Time","当前时间");
        public readonly LanguageItem CURRENTROLE = new LanguageItem("Current Role","当前角色");
        public readonly LanguageItem CURRENTPLOTMODEL = new LanguageItem("Current Plot Model","当前剧情模型");
        public readonly LanguageItem CURRENTROLEMODEL = new LanguageItem("Current Role Model","当前角色模型");
        public readonly LanguageItem CURRENTSCENEMODEL = new LanguageItem("Current Scene Model","当前场景模型");
        public readonly LanguageItem CURRENTINSTANCEIDSTORAGE = new LanguageItem("Current Instance ID Storage","当前ID存储器");
        public readonly LanguageItem SCENECONTROLLER = new LanguageItem("Scene Controller","场景控制器");
        public readonly LanguageItem ROLECONTROLLER = new LanguageItem("Role Controller","角色控制器");
        public readonly LanguageItem OPTIONCONTROLLER = new LanguageItem("Option Controller","选项控制器");
        public readonly LanguageItem PLOTFLOWCONTROLLER = new LanguageItem("Plot Flow Controller","剧情流控制器");
        public readonly LanguageItem CUSTOMVIEW = new LanguageItem("Custom View","自定义视图");
        public readonly LanguageItem OPTIONVIEWTYPE = new LanguageItem("Option View Type", "选项的视图类型");
        public readonly LanguageItem OPTION = new LanguageItem("Option", "选项");
        public readonly LanguageItem OPTIONSPACE = new LanguageItem("Option Space", "选项空间");
        public readonly LanguageItem AUTOSIZE = new LanguageItem("Auto Size", "自动计算选项大小");
        public readonly LanguageItem OPTIONSPOOL = new LanguageItem("Options Object Pool", "选项的对象池");
        public readonly LanguageItem SHOWPLOTVIEW = new LanguageItem("Show Plot View","视图容器");
        public readonly LanguageItem GRAPHSYSTEM = new LanguageItem("Graph System","图系统");
        public readonly LanguageItem SYSTEMDATA = new LanguageItem("System Data","游戏数据");
        public readonly LanguageItem CURRENTGFUPLOTFLOWEDITORWINDOW = new LanguageItem("Current Gfu Plot Flow Editor Window","当前剧情流编辑窗口");
        public readonly LanguageItem PLOTFLOWEDITORWINDOW = new LanguageItem("Plot Flow Editor Window","剧情流编辑窗口");
        public readonly LanguageItem PLOTITEMEDITORWINDOW = new LanguageItem("Plot Item Editor Window","剧情项编辑窗口");
        public readonly LanguageItem CURRENTGFUPLOTITEMEDITORWINDOW = new LanguageItem("Current Gfu Item Flow Editor Window","当前剧情项编辑窗口");
        public readonly LanguageItem CURRENTPLOTFLOWGRAPH = new LanguageItem("Current Plot Flow Graph","当前剧情流图");
        public readonly LanguageItem CURRENTPLOTITEMGRAPH = new LanguageItem("Current Plot Item Graph","当前剧情项图");
        public readonly LanguageItem CURRENTPLOTITEMGRAPHDATA = new LanguageItem("Current Plot Item Graph Data","当前剧情项图数据");
        public readonly LanguageItem CURRENTPLOTFLOWGRAPHDATA = new LanguageItem("Current Plot Flow Graph Data","当前剧情流图数据");
        public readonly LanguageItem CURRENTMONOPROXY = new LanguageItem("Current Mono Proxy","当前Mono代理");
        public readonly LanguageItem GRAPHDATA = new LanguageItem("Graph Data","图数据");
        public readonly LanguageItem NODE = new LanguageItem("Node","节点");
        public readonly LanguageItem LOGIC = new LanguageItem("Logic","逻辑");
        public readonly LanguageItem SCRIPT = new LanguageItem("Script","脚本");
        public readonly LanguageItem PLOTSCRIPT = new LanguageItem("Plot Script","剧情脚本");
        public readonly LanguageItem OPERATION = new LanguageItem("Operation","操作");
        public readonly LanguageItem HORIZONTAL = new LanguageItem("Horizontal","水平");
        public readonly LanguageItem VERTICAL = new LanguageItem("Vertical","垂直");
        public readonly LanguageItem CONDITION = new LanguageItem("Condition","条件");
        public readonly LanguageItem EVENT = new LanguageItem("Event","事件");
        public readonly LanguageItem PRIORITY = new LanguageItem("Priority","优先级");  
        public readonly LanguageItem PROBABILITY = new LanguageItem("Probability","概率");  
        public readonly LanguageItem STARTTIME = new LanguageItem("Start Time","开始时间");
        public readonly LanguageItem REPETITIONCOUNT = new LanguageItem("Repetition Count","触发次数");
        public readonly LanguageItem OVERTIME = new LanguageItem("Over Time","结束时间");
        public readonly LanguageItem YEAR = new LanguageItem("Year","年");
        public readonly LanguageItem MONTH = new LanguageItem("Month","月");
        public readonly LanguageItem DAY = new LanguageItem("Day","日");
        public readonly LanguageItem GRAPH = new LanguageItem("Graph","图");
        public readonly LanguageItem VALUENAME = new LanguageItem("Value Name","数值名称");
        public readonly LanguageItem BACKGROUNDIMAGE = new LanguageItem("Background Image","背景图片");
        public readonly LanguageItem BACKGROUNDAUDIO = new LanguageItem("Background Audio","背景音乐");
        public readonly LanguageItem SCENEGRAPH = new LanguageItem("Scene Graph", "场景图");
        public readonly LanguageItem PLOTMODE = new LanguageItem("Plot Mode", "剧情模式");
        public readonly LanguageItem OPERATIONTYPE = new LanguageItem("Operation Type","操作类型");
        public readonly LanguageItem PLOTFLOWNODE = new LanguageItem("Plot Flow Node","剧情流节点");
        public readonly LanguageItem PLOTJUMPNODE = new LanguageItem("Plot Jump Node","剧情跳转节点");
        public readonly LanguageItem PLOTITEMNODE = new LanguageItem("Plot Item Node","剧情项节点");
        public readonly LanguageItem SCENENODE = new LanguageItem("Scene Node","场景节点");
        public readonly LanguageItem PLOTITEMGRAPH = new LanguageItem("Plot Item Graph","剧情项图");
        public readonly LanguageItem PLOTGRAPHNODE = new LanguageItem("Plot Graph Node","剧情图节点");
        public readonly LanguageItem ENDNODE = new LanguageItem("End Node","结束节点");
        public readonly LanguageItem EXIT = new LanguageItem("Exit","剧情出口");
        public readonly LanguageItem EXIT1 = new LanguageItem("(First)Exit","(一)剧情出口");
        public readonly LanguageItem EXIT2 = new LanguageItem("(Second)Exit","(二)剧情出口");
        public readonly LanguageItem EXIT3 = new LanguageItem("(third)Exit","(三)剧情出口");
        public readonly LanguageItem EXIT4 = new LanguageItem("(fourth)Exit","(四)剧情出口");
        public readonly LanguageItem EXIT5 = new LanguageItem("(fifth)Exit","(五)剧情出口");
        public readonly LanguageItem ENTER = new LanguageItem("Enter","剧情入口");
        public readonly LanguageItem SATISFYEXIT = new LanguageItem("Satisfying Exit","(满足)剧情出口");
        public readonly LanguageItem DISSATISFYEXIT = new LanguageItem("Dissatisfying Exit","(不满足)剧情出口");
        public readonly LanguageItem ROLEDATACHECKNODE = new LanguageItem("Role Data Check Node","角色数值检查节点");
        public readonly LanguageItem PROBABILITYNODE = new LanguageItem("Probability Node","概率分支节点");
        public readonly LanguageItem TIMECHECKNODE = new LanguageItem("Time Check Node","时间检查节点");
        public readonly LanguageItem EXITONEPROBABILITY = new LanguageItem("First Exit Probability","出口一概率");
        public readonly LanguageItem CHANGEROLEDATANODE = new LanguageItem("Change Role Data Node","变更角色数值节点");
        public readonly LanguageItem PLOTGRAPHSOURCE = new LanguageItem("Plot Graph Source","剧情图来源");
        public readonly LanguageItem MAINNODE = new LanguageItem("Main Node","主节点");
        
        
        public readonly LanguageItem NAMEVIEW = new LanguageItem("Name View","姓名视图容器");
        public readonly LanguageItem SPEAKVIEW = new LanguageItem("Speak View","说话视图容器");
        public readonly LanguageItem NAME = new LanguageItem("Name","姓名");
        public readonly LanguageItem SPEAK = new LanguageItem("Speak","语句");
        public readonly LanguageItem BACKGROUNDVIEW = new LanguageItem("Background View","背景视图容器");
        public readonly LanguageItem AUDIOSOURCE = new LanguageItem("Audio Source","音源");
        public readonly LanguageItem PARENTCANVAS = new LanguageItem("Parent Canvas","父画布");
        public readonly LanguageItem BACKGROUNDDISTANCE = new LanguageItem("Background Distance","背景距离");
        public readonly LanguageItem INTERVALTIME = new LanguageItem("Interval Time","间隔时间");
        public readonly LanguageItem RENDERINGMODE = new LanguageItem("Rendering Mode","渲染模式");
        public readonly LanguageItem TILED = new LanguageItem("Tiled","平铺");
        public readonly LanguageItem GEOMETRICSCALING = new LanguageItem("Geometric Scaling","等比缩放");
        public readonly LanguageItem CUSTOM = new LanguageItem("Custom","自定义");
        public readonly LanguageItem CUSTOMMETHOD = new LanguageItem("Custom Method","自定义方法");    
        public readonly LanguageItem NONE = new LanguageItem("None","无");
        public readonly LanguageItem DEFAULT = new LanguageItem("Default","默认");
        
        public readonly LanguageItem TRANSFORMNODE = new LanguageItem("Transform Node","变换节点");
        public readonly LanguageItem TRANSFORMOPERATION = new LanguageItem("Transform Operation","变换操作");
        public readonly LanguageItem POSITION = new LanguageItem("Position","位置");
        public readonly LanguageItem ROTATION = new LanguageItem("Rotation","旋转");    
        public readonly LanguageItem SCALE = new LanguageItem("Scale","缩放");
        public readonly LanguageItem COLOR = new LanguageItem("Color","颜色");
        public readonly LanguageItem ANIMATION = new LanguageItem("Animation","动画");
        public readonly LanguageItem ANIMATIONCLIP = new LanguageItem("Animation Clip","动画剪辑");
        public readonly LanguageItem ROLENODE = new LanguageItem("Role Node","角色节点");
        public readonly LanguageItem OPTIONNODE = new LanguageItem("Option Node","选项节点");
        public readonly LanguageItem ADDNODE = new LanguageItem("Add Node","相加节点");
        public readonly LanguageItem SUBTRACTNODE = new LanguageItem("Subtract Node","相减节点");
        public readonly LanguageItem DIVISIONNODE = new LanguageItem("Division Node","相除节点");
        public readonly LanguageItem MULTIPLYNODE = new LanguageItem("Multiply Node","相乘节点");
        public readonly LanguageItem COMBINENODE = new LanguageItem("Combine Node","组合节点");
        public readonly LanguageItem SPLITNODE = new LanguageItem("Split Node","拆分节点");
        public readonly LanguageItem ANIMATIONNODE = new LanguageItem("Animation Node","动画节点");
        public readonly LanguageItem ROLEOPERATIONTYPE = new LanguageItem("Role Operation Type","登场下场操作");
        public readonly LanguageItem LINEARNODE = new LanguageItem("Linear Operation Node","线性操作节点");
        public readonly LanguageItem VECTOR4NODE = new LanguageItem("Vector4 Node","4维向量节点");
        public readonly LanguageItem OPACITY = new LanguageItem("Opacity","不透明度");
        public readonly LanguageItem VECTOR4 = new LanguageItem("Vector4","4维向量");
        public readonly LanguageItem VECTOR3 = new LanguageItem("Vector3","3维向量");
        public readonly LanguageItem VECTOR2 = new LanguageItem("Vector2","2维向量");
        public readonly LanguageItem VECTOR1 = new LanguageItem("Vector1","1维向量");
        public readonly LanguageItem TIMENODE = new LanguageItem("Time Node","时间节点");
        public readonly LanguageItem CHANNEL = new LanguageItem("Channel","通道");
        public readonly LanguageItem TIME = new LanguageItem("Time","时间");
        public readonly LanguageItem SINETIME = new LanguageItem("Sine Time","时间正弦值");
        public readonly LanguageItem COSINETIME = new LanguageItem("Cosine Time","时间余弦值");
        public readonly LanguageItem DELTATIME = new LanguageItem("Delta Time","增量时间");
        public readonly LanguageItem SMOOTHDELTATIME = new LanguageItem("Smooth Delta Time","平滑增量时间");
        public readonly LanguageItem FLOAT = new LanguageItem("Float","浮点数");
        public readonly LanguageItem SCENE = new LanguageItem("Scene","场景");
        public readonly LanguageItem BOOLEANNODE = new LanguageItem("Boolean Node","布尔节点");
        public readonly LanguageItem POWERNODE = new LanguageItem("Power Node","幂指节点");
        public readonly LanguageItem SQUAREROOTNODE  = new LanguageItem("Square Root Node","平方根节点");
        public readonly LanguageItem BOOLEAN = new LanguageItem("Boolean","布尔");
        public readonly LanguageItem LOGICOPERATION  = new LanguageItem("Logic Operation","逻辑操作");
        public readonly LanguageItem OBJECT = new LanguageItem("Object","对象");
        public readonly LanguageItem COMPARENODE = new LanguageItem("Compare Node","比较节点");
        public readonly LanguageItem GEOMETRY = new LanguageItem("Geometry","几何");
        public readonly LanguageItem SINENODE = new LanguageItem("Sine Node","正弦节点");
        public readonly LanguageItem TANGENTNODE = new LanguageItem("Tangent Node","正切节点");
        public readonly LanguageItem COSINENODE = new LanguageItem("Cosine Node","余弦节点");
        public readonly LanguageItem ARCSINENODE = new LanguageItem("Arcsine Node","反正弦节点");
        public readonly LanguageItem ARCCOSINENODE = new LanguageItem("Arccosine Node","反余弦节点");
        public readonly LanguageItem ARCTANGENTNODE = new LanguageItem("Arctangent Node","反正切节点");
        public readonly LanguageItem VALUE = new LanguageItem("Value","值");
        public readonly LanguageItem VALUE1 = new LanguageItem("Value1","值1");
        public readonly LanguageItem VALUE2 = new LanguageItem("Value1","值2");
        public readonly LanguageItem TRUE = new LanguageItem("True","真");
        public readonly LanguageItem FALSE = new LanguageItem("False", "假");
        public readonly LanguageItem FROM = new LanguageItem("From","从");
        public readonly LanguageItem SUBTRACT = new LanguageItem("Subtract","减少");
        public readonly LanguageItem ADD = new LanguageItem("Add","添加");
        public readonly LanguageItem IN = new LanguageItem("In","输入");
        public readonly LanguageItem OUT = new LanguageItem("Out","输出");
        public readonly LanguageItem EXECUTE = new LanguageItem("Execute","执行");
        public readonly LanguageItem SAVE = new LanguageItem("Sace","保存");
        public readonly LanguageItem HINT = new LanguageItem("hint","提示");
        public readonly LanguageItem MATH = new LanguageItem("Math","数学");
        public readonly LanguageItem LOOP = new LanguageItem("Loop","循环");
        public readonly LanguageItem DURATIONTIME = new LanguageItem("Duration time","持续时间");
        public readonly LanguageItem TO = new LanguageItem("To","到");
        public readonly LanguageItem X = new LanguageItem("X","X");
        public readonly LanguageItem YES = new LanguageItem("Yes","是滴");
        public readonly LanguageItem OK = new LanguageItem("OK","好的");
        public readonly LanguageItem NO = new LanguageItem("No,Thinks","不了，谢谢");
        public readonly LanguageItem Y = new LanguageItem("Y","Y");
        public readonly LanguageItem Z = new LanguageItem("Z","Z");
        public readonly LanguageItem W = new LanguageItem("W","W");
        
        
        public readonly LanguageItem ISCANJUMP = new LanguageItem("Is Can Jump","是否可跳过");
        public readonly LanguageItem REPEATABILITY = new LanguageItem("Repeatability","可重复");
        public readonly LanguageItem ISLOOPING = new LanguageItem("Is Looping","是否循环");
        public readonly LanguageItem ISAUTOHIGHLIGHT = new LanguageItem("Is Auto High Light","是否自动高光");
        public readonly LanguageItem INITIALIZETHEGAMESYSTEM = new LanguageItem("Initialize the game system","初始化游戏系统");
        public readonly LanguageItem ROLESPRITEMAP = new LanguageItem("Role Sprite Map","角色精灵贴图");
        public readonly LanguageItem INITIALIZEHIERARCHY = new LanguageItem("Initialize all role model","初始化Hierarchy中所有角色");
        public readonly LanguageItem INITIALIZEGAMEVIEW = new LanguageItem("Initialize the game view system","初始化游戏视图系统");
        public readonly LanguageItem ADDALLSCENEMODEL = new LanguageItem("Add all SceneModel","添加Hierarchy中所有场景");
        public readonly LanguageItem INITIALIZEALLPLOTMODEL = new LanguageItem("Initialize all PlotModel","初始化Hierarchy中所有剧情");
        public readonly LanguageItem ADDTYPE = new LanguageItem("Add Type","添加类型");
        public readonly LanguageItem REMOVETYPE = new LanguageItem("Remove Type","移除类型");
        public readonly LanguageItem CHANGETYPE = new LanguageItem("Change Type","选择类型");
        public readonly LanguageItem CURRENTSAVABLECONFIG = new LanguageItem("Current Savable Config", "当前可保存类型配置");
        public readonly LanguageItem INITIALIZEAOTHER = new LanguageItem("Do I need to initialize additional dependencies for you?","需要为您初始化其他依赖项吗");
        public readonly LanguageItem HASEXISTS = new LanguageItem("The same item already exists","已经存在相同项");
    }
    public class LanguageItem{
        public LanguageItem(Dictionary<string,string> value){
            foreach (var keyValuePair in value){
                GetType().GetField(keyValuePair.Key).SetValue(this,keyValuePair.Value);
            }
        }
        public LanguageItem(params string[] value){
            var fieldInfos = GetType().GetFields();
            for (var i = 0; i < value.Length; i++){
                fieldInfos[i].SetValue(this,value[i]);
            }
        }
        public string Value{
            get{
                if (GameSystem.StaticData==null) return English;
                switch (GameSystem.StaticData.Language){
                    case GfuLanguage.LanguageEnum.Chinese: return Chinese;
                    case GfuLanguage.LanguageEnum.English: return English;
                    // case GfuLanguage.LanguageEnum.Japanese: return Japanese;
                }
                return English;
            }
        }
        public readonly string English = "None";
        public readonly string Chinese = "无";
        public readonly string Japanese = "无";
    }
}
