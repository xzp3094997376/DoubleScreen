using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public OperationItemGroup group;
    public OperationBaseItem z1;
    public OperationBaseItem z2;
    // Start is called before the first frame update
    void Start()
    {
        z1._Tran = z2.transform;
       // group._Diffuse = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.A))
        {
            group._Diffuse = true;
            Debug.Log("GetKeyDown A");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            group._Diffuse = false;
            Debug.Log("GetKeyDown B");
        }
       
    }
}
