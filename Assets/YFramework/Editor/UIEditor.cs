using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Assets.Framework.Editor
{
    public class UIEditor : MonoBehaviour
    {
        /// <summary>
        /// 给UI组件打标记
        /// </summary>
        [MenuItem("GameObject/BindUIMark", false, -10)]
        public static void BindUIMark()
        {
            var slectGameObjects = Selection.gameObjects;
            foreach (var sGameObject in slectGameObjects)
            {
                if (sGameObject.GetComponent<UIMark>() == null)
                {
                    sGameObject.AddComponent<UIMark>();
                }
            }
        }

        /// <summary>
        /// 解除UI组件的标记
        /// </summary>
        [MenuItem("GameObject/UnBindUIMark", false, -9)]
        public static void UnBindUIMark()
        {
            var slectGameObjects = Selection.gameObjects;
            foreach (var sGameObject in slectGameObjects)
            {
                var uiMark = sGameObject.GetComponent<UIMark>();
                if (uiMark != null)
                {
                    DestroyImmediate(uiMark);
                }
            }
        }

        /// <summary>
        /// 生成Prefab的UI脚本
        /// </summary>
        [MenuItem("GameObject/CreatUIViewCode", false, -8)]
        public static void AutoCreatUIViewCode()
        {
            var slectGameObjects = Selection.gameObjects;
            foreach (var sGameObject in slectGameObjects)
            {
                //Debug.Log(sGameObject.name);
                CreatUIViewCode(sGameObject);
            }
        }

        /// <summary>
        /// 生成Prefab的UI脚本
        /// </summary>
        [MenuItem("GameObject/CreatUIPanelCode", false, -7)]
        public static void AutoCreatUIPanelCode()
        {
            var slectGameObjects = Selection.gameObjects;
            foreach (var sGameObject in slectGameObjects)
            {
                //Debug.Log(sGameObject.name);
                CreatUIPanelCode(sGameObject);
            }
        }

        /// <summary>
        /// 关联组件到脚本的属性
        /// </summary>
        [MenuItem("GameObject/BindUICodeField", false, -6)]
        public static void BindUICodeField()
        {
            var selectGameObjects = Selection.gameObjects;
            foreach (var v in selectGameObjects)
            {
                var className = EditorUtil.GetGameObjectViewClassName(v);
                var type = EditorUtil.GetType(className, logFlag: false);
                if (type != null)
                {
                    var com = v.GetComponent(type);
                    if (com != null)
                    {
                        var sObj = new SerializedObject(com);
                        v.GetComponentsInChildren<UIMark>(true).ToList().ForEach(elementMark =>
                        {
                            //给对象添加引用
                            var propertyName = elementMark.transform.name;
                            var mobj = sObj.FindProperty(propertyName);
                            mobj.objectReferenceValue = elementMark.transform.gameObject;
                            Debug.Log("add sth from " + mobj.name + "? " + elementMark.name);

                        });
                        sObj.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
            }
        }

        /// <summary>
        /// 生成Prefab的UI脚本
        /// </summary>
        [MenuItem("GameObject/SavePrefab", false, -5)]
        public static void SavePrefab()
        {
            var slectGameObjects = Selection.gameObjects;
            foreach (var sGameObject in slectGameObjects)
            {
                EditorUtil.SavePrefab(sGameObject);
            }
        }

        /// <summary>
        /// 生成UIprefab对应的视图类，并挂载到Prefab上
        /// 只关联打了UIMark标记的组件
        /// </summary>
        /// <param name="gameObject">被操作的Prefab</param>
        /// <param name="autoBind">自动绑定会关联Prefab下所有的UI组件</param>
        private static void CreatUIViewCode(GameObject gameObject, bool autoBind = false)
        {
            List<IField> uIMarks = new List<IField>();
            var uITrans = gameObject.GetComponentsInChildren(typeof(Transform));
            foreach (var uiTran in uITrans)
            {
                var uiMark = uiTran.gameObject.GetComponent<UIMark>();

                if (uiMark == null && autoBind)
                {
                    uiMark = uiTran.gameObject.AddComponent<UIMark>();
                }

                if (uiMark != null)
                {
                    uIMarks.Add(uiMark);
                }
            }

            EditorUtil.CreatUIViewCode(gameObject, uIMarks);

            EditorPrefs.SetBool("ScriptGenerator", true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成面板对应的控制类
        /// </summary>
        /// <param name="gameObject"></param>
        private static void CreatUIPanelCode(GameObject gameObject)
        {
            EditorUtil.CreatUIPanelCode(gameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 脚本改动后的回调,挂载脚本并关联组件
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void AddComponent2GameObject()
        {
            if (!EditorPrefs.GetBool("ScriptGenerator"))
            {
                return;
            }

            EditorPrefs.SetBool("ScriptGenerator", false);

            var slectGameObjects = Selection.gameObjects;
            foreach (var v in slectGameObjects)
            {
                var className = EditorUtil.GetGameObjectViewClassName(v);
                Type type = EditorUtil.GetType(className);
                if (type == null)
                {
                    Debug.Log("编译失败");
                    return;
                }
                else
                {
                    var scriptComponent = v.GetComponent(type);
                    if (!scriptComponent)
                    {
                        scriptComponent = v.AddComponent(type);
                        BindUICodeField();
                    }
                }
            }
        }
    }
}

