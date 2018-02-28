﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;

public class TestNet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.AddComponentOnce<NetSys>();
        gameObject.AddComponentOnce<FrameSyncSys>();

        NetSys.Instance.BeginConnect(NetChannelType.Game, "127.0.0.1", 8080, OnCallback);
        NetSys.Instance.AddMsgCallback(NetChannelType.Game, (short)Proto.PacketOpcode.Msg_Test, OnCallback, true);
	}

    private void OnCallback(object netObj)
    {
        var data = netObj as Proto.Msg_Test_Data;
        CLog.Log("recv:"+data.msg);
    }

    private void OnCallback(bool succ, NetChannelType channel)
    {
        CLog.Log("返回连接服务器结果:"+succ);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,50),"发送测试包"))
        {
            Proto.Msg_Test_Data data = new Proto.Msg_Test_Data();
            data.msg = "发送测试包";
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)Proto.PacketOpcode.Msg_Test,data);
        }
    }
}
