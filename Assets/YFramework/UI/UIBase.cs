using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.YFramework.UI
{
    public enum EAssetsLoadState
    {
        /// <summary>
        /// 等待加载
        /// </summary>
        Wait = 1,

        /// <summary>
        /// 正在加载
        /// </summary>
        Doing = 2,

        /// <summary>
        /// 加载完成
        /// </summary>
        Over = 3,
    }

    public enum EPanelLayerType
    {
        /// <summary>
        /// 普通固定面板
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 弹框层级面板
        /// </summary>
        DialogLayer = 2,

        /// <summary>
        /// 全局通用弹窗
        /// </summary>
        OverLayer = 3,

        /// <summary>
        /// 遮挡操作层级
        /// </summary>
        BlockLayer = 4,

        /// <summary>
        /// 加载界面
        /// </summary>
        LoadingLayer = 5,
    }

    public enum EPanelVisibleType
    {
        Hide =0,
        
        Show = 1,
    }

    public class UIBase
    {
        /// <summary>
        /// 事件注册标志
        /// 只在面板展示期间才响应消息
        /// </summary>
        private bool eventRegFlag = false;

        /// <summary>
        /// 资源加载状态
        /// </summary>
        protected EAssetsLoadState loadState = EAssetsLoadState.Wait;

        protected EPanelLayerType panelLayer = EPanelLayerType.Normal;

        protected EPanelVisibleType panelVisibleType = EPanelVisibleType.Hide;

        public void ShowPanel()
        {
            switch (loadState)
            {
                case EAssetsLoadState.Wait:
                    loadAssest();
                    break;
                case EAssetsLoadState.Doing:
                    break;
                case EAssetsLoadState.Over:
                    tryShowPanel();
                    break;
                default:
                    break;
            }
        }

        public void HidePanel()
        {
            if (panelVisibleType == EPanelVisibleType.Show)
            {
                panelVisibleType = EPanelVisibleType.Hide;
                onHide();
            }
        }

        protected virtual void loadAssest()
        {
            loadState = EAssetsLoadState.Doing;
        }

        protected virtual void loadAssetOver()
        {
            loadState = EAssetsLoadState.Over;
            onInit();
            tryShowPanel();
        }

        protected virtual void onDestory()
        {
            loadState = EAssetsLoadState.Wait;
        }
        
        protected virtual void onInit()
        {
            
        }

        protected virtual void tryShowPanel()
        {
            if (panelVisibleType == EPanelVisibleType.Hide)
            {
                panelVisibleType = EPanelVisibleType.Show;
                onShow();
            }
        }

        protected virtual void onShow()
        {
            if (!eventRegFlag)
            {
                onEvent();
            }

            setVisble(true);
        }

        protected virtual void onHide()
        {
            if (eventRegFlag)
            {
                offEvent();
            }

            onDestory();
            setVisble(false);
        }

        protected virtual void onEvent()
        {
            eventRegFlag = true;
        }

        protected virtual void offEvent()
        {
            eventRegFlag = false;
        }

        protected virtual void setVisble(bool visble)
        {
        }

        protected Transform getParent()
        {
            Transform parent = null;
            switch (panelLayer)
            {
                case EPanelLayerType.Normal:
                    parent = UIManager.Instance.NormalLayerParent.transform;
                    break;
                case EPanelLayerType.DialogLayer:
                    parent = UIManager.Instance.DialogLayerParent.transform;
                    break;
                case EPanelLayerType.OverLayer:
                    parent = UIManager.Instance.OverLayerParent.transform;
                    break;
                case EPanelLayerType.BlockLayer:
                    parent = UIManager.Instance.BlockLayerParent.transform;
                    break;
                case EPanelLayerType.LoadingLayer:
                    parent = UIManager.Instance.LoadingLayerParent.transform;
                    break;
                default:
                    Debug.LogErrorFormat("未处理的UI层级{0}", panelLayer);
                    break;
            }

            return parent;
        }
    }
}