using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestTriggerEnter : NetworkBehaviour
{
    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //        Deactivate(other.gameObject);
    //}

    void Deactivate(GameObject _colObj)
    {
        NetworkIdentity objNetId = _colObj.GetComponent<NetworkIdentity>();
        CmdDeactivate(objNetId);
    }

    [Command]
    public void CmdDeactivate(NetworkIdentity _objNetId)
    {
        RpcDeactivate(_objNetId.gameObject);
        _objNetId.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcDeactivate(GameObject _colObj)
    {
        _colObj.SetActive(false);
    }
}
