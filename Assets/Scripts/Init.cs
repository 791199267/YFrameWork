using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame.UI;

public class Init : MonoBehaviour
{
    private List<int> dataList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowPanel<MainPanel>();
    }

    // Update is called once per frame
    void Update()
    {

    }
   
}
