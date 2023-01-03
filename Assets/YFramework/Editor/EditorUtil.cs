using GameFramwork;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Framework.Editor
{
    static class EditorUtil
    {
        public static readonly string uIViewCodeCreatPath = Application.dataPath + "/Scripts/UIScripts/ViewScripts";
        public static readonly string uIPanelCodeCreatPath = Application.dataPath + "/Scripts/UIScripts/PanelScripts";

        public static readonly string uIPrefabPath = Application.dataPath + "/Resources/Prefabs/UI";
        public static readonly string uINameSapce = "XGame.UI";


        public static Type GetType(string className, string nameSpace = "XGame.UI", bool logFlag = true)
        {
            //var asmb = AppDomain.CurrentDomain.GetAssemblies().First(v => v.FullName.StartsWith("Assembly-CSharp,"));

            //生成的UIViewCode在不同的程序集中
            //所以需要先加载程序集再使用反射取到类型
            var dataPath = Application.dataPath;
            var asmbPath =  dataPath.Remove(dataPath.Length - 6, 6) + @"Library\ScriptAssemblies\Assembly-CSharp.dll";
            Assembly asmb = Assembly.LoadFrom(asmbPath);
            var typeName = nameSpace + "." + className;
            try
            {
                Type type = asmb.GetType(typeName, true, true);
                return type;
            }
            catch (Exception e)
            {
                if (logFlag)
                {
                    UnityEngine.Debug.LogWarningFormat("{0}类型不存在:{1}", typeName , e);
                }
                return null;
            }
        }

        public static string GetGameObjectViewClassName(GameObject gameObject)
        {
            return gameObject.name + "View";
        }

        public static string GetGameObjectPanelClassName(GameObject gameObject)
        {
            return gameObject.name + "Panel";
        }

        /// <summary>
        /// 生成Prefab到工程中
        /// </summary>
        /// <param name="gameObject"></param>
        public static void SavePrefab(GameObject gameObject)
        {
            if (!Directory.Exists(uIPrefabPath))
            {
                Directory.CreateDirectory(uIPrefabPath);
            }
            var filePath = uIPrefabPath + "/" + gameObject.name + ".prefab";

            Action saveAction = () =>
            {
                bool succ = false;
                PrefabUtility.SaveAsPrefabAsset(gameObject, filePath, out succ);
                if (!succ)
                {
                    Debug.LogFormat("保存{0}Prefab失败", gameObject.name);
                }
                else
                {
                    Debug.LogFormat("生成Prefab成功，存放路径{0}", filePath);
                }
            };


            if (File.Exists(filePath))
            {
                EditorAffirmView.ShowWindow(saveAction);
            }
            else
            {
                saveAction?.Invoke();
            }
        }

        /// <summary>
        /// 动态创建Ui视图的关联类
        /// </summary>
        /// <param name="className"></param>
        /// <param name="fieldList"></param>
        public static void CreatUIViewCode(GameObject gameObject, List<IField> fieldList)
        {
            var className = EditorUtil.GetGameObjectViewClassName(gameObject);

            //代码编译单元
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            //命名空间
            CodeNamespace codeNamespace = new CodeNamespace(uINameSapce);
            //导入引用
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine.UI"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("TMPro"));

            //定义类
            CodeTypeDeclaration classUnit = new CodeTypeDeclaration(className);
            classUnit.IsClass = true;
            classUnit.TypeAttributes = TypeAttributes.Public;
            classUnit.BaseTypes.Add(new CodeTypeReference("MonoBehaviour"));

            //类，命名空间，编译单元相关联
            codeNamespace.Types.Add(classUnit);
            codeUnit.Namespaces.Add(codeNamespace);

            //字段
            foreach (var fieldItem in fieldList)
            {
                CodeMemberField codeMemberField = new CodeMemberField();
                codeMemberField.Name = fieldItem.FieldEntityName;
                codeMemberField.Attributes = MemberAttributes.Public;
                codeMemberField.Type = new CodeTypeReference(fieldItem.FieldTypeName);
                classUnit.Members.Add(codeMemberField);
            }

            //构造方法
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            classUnit.Members.Add(constructor);

            YFileUtil.Instance.CreatCodeFile(uIViewCodeCreatPath, className + ".cs", codeUnit);
        }

        /// <summary>
        /// 动态创建Ui视图的控制类
        /// </summary>
        /// <param name="className"></param>
        /// <param name="fieldList"></param>
        public static void CreatUIPanelCode(GameObject gameObject)
        {
            var className = EditorUtil.GetGameObjectPanelClassName(gameObject);

            var viewClassName = EditorUtil.GetGameObjectViewClassName(gameObject);

            //代码编译单元
            CodeCompileUnit codeUnit = new CodeCompileUnit();

            //命名空间
            CodeNamespace codeNamespace = new CodeNamespace(uINameSapce);
            //导入引用
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine.UI"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("Assets.YFramework.UI"));

            //定义类
            CodeTypeDeclaration classUnit = new CodeTypeDeclaration(className);
            classUnit.IsClass = true;
            classUnit.TypeAttributes = TypeAttributes.Public;
            classUnit.BaseTypes.Add(new CodeTypeReference("UGUIBase"));

            //类，命名空间，编译单元相关联
            codeNamespace.Types.Add(classUnit);
            codeUnit.Namespaces.Add(codeNamespace);

            //字段
            CodeMemberField codeMemberField = new CodeMemberField();
            codeMemberField.Name = "root";
            codeMemberField.Attributes = MemberAttributes.Public;
            codeMemberField.Type = new CodeTypeReference(viewClassName);
            classUnit.Members.Add(codeMemberField);

            //方法
            string statementStr = "";

            //构造方法
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            statementStr = string.Format(@"            setAssetsPath(""Prefabs/UI/{0}"");", gameObject.name);
            CodeSnippetStatement ass = new CodeSnippetStatement(statementStr);
            constructor.Statements.Add(ass);
            classUnit.Members.Add(constructor);

            statementStr = string.Format(@"            root = panelGameObject.GetComponent<{0}>();", viewClassName);
            statementStr += "\n";
            statementStr += "            base.onInit();";
            var method2 = creatCodeMethod("onInit", statementStr, MemberAttributes.Family | MemberAttributes.Override);
            classUnit.Members.Add(method2);

            statementStr = string.Format(@"            base.onShow();");
            var method3 = creatCodeMethod("onShow", statementStr, MemberAttributes.Family | MemberAttributes.Override);
            classUnit.Members.Add(method3);

            statementStr = string.Format(@"            base.onHide();");
            var method4 = creatCodeMethod("onHide", statementStr, MemberAttributes.Family | MemberAttributes.Override);
            classUnit.Members.Add(method4);

            //List<string> onOffEventStrList = getOnOffEventStrList();
            //bool autoEvent = true;

            //事件注册 反注册 方法
            statementStr = string.Format(@"            base.onEvent();");
            //if (autoEvent && onOffEventStrList[0] != "") statementStr += "\r\n" + onOffEventStrList[0];
            var method5 = creatCodeMethod("onEvent", statementStr, MemberAttributes.Family | MemberAttributes.Override);
            classUnit.Members.Add(method5);

            statementStr = string.Format(@"            base.offEvent();");
            //if (autoEvent && onOffEventStrList[1] != "") statementStr += "\r\n" + onOffEventStrList[1];
            var method6 = creatCodeMethod("offEvent", statementStr, MemberAttributes.Family | MemberAttributes.Override);
            classUnit.Members.Add(method6);

            //var btnNameList = getBtnNameList();
            //if (btnNameList.Count > 0 && autoEvent)
            //{
            //    statementStr = "";
            //    statementStr += "GObject sender = (GObject)context.sender;";
            //    statementStr += "\r\n";
            //    for (int i = 0; i < btnNameList.Count; i++)
            //    {
            //        if (i == 0)
            //        {
            //            statementStr += "if (sender == _root." + btnNameList[i] + ")";
            //        }
            //        else
            //        {
            //            statementStr += "else if (sender ==  _root." + btnNameList[i] + ")";
            //        }
            //        statementStr += "\r\n{\r\n}\r\n";
            //    }
            //    var method7 = creatCodeMethod("onClickbtn", statementStr, MemberAttributes.Public);
            //    CodeParameterDeclarationExpression expression = new CodeParameterDeclarationExpression("EventContext", "context");
            //    method7.Parameters.Add(expression);
            //    classUnit.Members.Add(method7);
            //}

            YFileUtil.Instance.CreatCodeFile(uIPanelCodeCreatPath, className + ".cs", codeUnit);
        }


        private static CodeMemberMethod creatCodeMethod(string methodName, string statementStr, MemberAttributes memberAttributes = MemberAttributes.Public | MemberAttributes.Final)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = methodName;
            method.Attributes = memberAttributes;
            CodeSnippetStatement ass = new CodeSnippetStatement(statementStr);
            method.Statements.Add(ass);
            return method;
        }

        private static List<string> getOnOffEventStrList(/*List<string> ComponentList*/)
        {
            List<string> resList = new List<string>();
            //var btnComponentList = getBtnNameList(ComponentList);
            //string onEvent = "";
            //string offEvent = "";
            //for (int i = 0; i < btnComponentList.Count; i++)
            //{
            //    if (i != btnComponentList.Count - 1)
            //    {
            //        onEvent += "_root." + btnComponentList[i] + ".onClick.Add(onClickbtn);\r\n";
            //        offEvent += "_root." + btnComponentList[i] + ".onClick.Remove(onClickbtn);\r\n";
            //    }
            //    else
            //    {
            //        onEvent += "_root." + btnComponentList[i] + ".onClick.Add(onClickbtn);";
            //        offEvent += "_root." + btnComponentList[i] + ".onClick.Remove(onClickbtn);";
            //    }
            //}
            //resList.Add(onEvent);
            //resList.Add(offEvent);
            return resList;
        }

        private static List<string> getBtnNameList(List<string> ComponentList)
        {
            List<string> btnComponentList = new List<string>();
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].Length > 3)
                {
                    var prefixStr = ComponentList[i][0].ToString() + ComponentList[i][1].ToString() + ComponentList[i][2].ToString();
                    if (prefixStr == "btn")
                    {
                        btnComponentList.Add(ComponentList[i]);
                    }
                }
            }
            return btnComponentList;
        }
    }
}
