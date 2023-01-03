using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.YFramework.UI
{
    public class UGUIBase : UIBase
    {
        private string assetsPath = "";
        protected GameObject panelGameObject;
        private ResourceRequest requestLoad;

        protected void setAssetsPath(string assetsPath)
        {
            this.assetsPath = assetsPath;
        }

        protected override void loadAssest()
        {
            base.loadAssest();
            //loadAssetsSync();
            loadAssetsAsync();
        }

        private void loadAssetsSync()
        {
            var temp = Resources.Load<GameObject>(assetsPath);
            panelGameObject = GameObject.Instantiate<GameObject>(temp);
            loadAssetOver();
        }

        private void loadAssetsAsync()
        {
            requestLoad = Resources.LoadAsync<GameObject>(assetsPath);
            requestLoad.completed += onAsyncOver;
        }

        private void onAsyncOver(AsyncOperation obj)
        {
            panelGameObject = GameObject.Instantiate<GameObject>(requestLoad.asset as GameObject);
            loadAssetOver();
        }

        protected override void loadAssetOver()
        {
            var parent = getParent();
            panelGameObject.transform.SetParent(parent, false);
            base.loadAssetOver();
        }

        protected override void onShow()
        {
            base.onShow();
        }

        protected override void onHide()
        {
            base.onHide();
        }

        protected override void setVisble(bool visble)
        {
            if (panelGameObject != null)
            {
                panelGameObject.SetActive(visble);
            }
        }
    }
}
