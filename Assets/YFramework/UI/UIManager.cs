using Assets.YFramework.UI;
using GameFramwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
    private List<UIBase> uiPanelList = new List<UIBase>();

    private GameObject normalLayerParent;
    public GameObject NormalLayerParent { get { return normalLayerParent; } }

    private GameObject dialogLayerParent;
    public GameObject DialogLayerParent { get { return dialogLayerParent; } }

    private GameObject overLayerParent;
    public GameObject OverLayerParent { get { return overLayerParent; } }

    private GameObject blockLayerParent;
    public GameObject BlockLayerParent { get { return blockLayerParent; } }

    private GameObject loadingLayerParent;
    public GameObject LoadingLayerParent { get { return loadingLayerParent; } }

    private void Awake()
    {
        normalLayerParent = GameObject.Find("normalLayerParent");
        dialogLayerParent = GameObject.Find("dialogLayerParent");
        overLayerParent = GameObject.Find("overLayerParent");
        blockLayerParent = GameObject.Find("blockLayerParent");
        loadingLayerParent = GameObject.Find("loadingLayerParent");
    }

    public void ShowPanel<T>() where T : UIBase, new()
    {
        UIBase uiPanel = null;
        foreach (var item in uiPanelList)
        {
            if (item.GetType() == typeof(T))
            {
                uiPanel = item;
                break;
            }
        }

        if (uiPanel == null)
        {
            uiPanel = new T();
            uiPanelList.Add(uiPanel);
        }
        uiPanel.ShowPanel();
    }


    public void HidePanel<T>() where T : UIBase, new()
    {
        foreach (var item in uiPanelList)
        {
            if (item.GetType() == typeof(T))
            {
                item.HidePanel();
            }
        }
    }

    public void HideAllPanel()
    {
        foreach (var item in uiPanelList)
        {
            item.HidePanel();
        }
    }
}
