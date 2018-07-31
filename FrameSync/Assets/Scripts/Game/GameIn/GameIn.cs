﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;

namespace Game
{
    public class GameIn : StateBase
    {
        private CommandSequence m_cJoinSequence;
        protected override void OnEnter()
        {
            FrameSyncSys.Instance.StopRun();
            BattleInfo.Clear();
            BattleInfo.userId = 1;
            BattleInfo.sceneId = GameConst.Instance.GetInt("default_scene_id");
            SceneEffectPool.Instance.Clear();
            SceneGOPool.Instance.Clear();
            ViewSys.Instance.Open("LoadingView");
            m_cJoinSequence = new CommandSequence();
            var cmdConnectBattleServer = new Cmd_ConnectBattleServer();
            var cmdLoadScene = new Cmd_LoadScene();
            var cmdPreload = new Cmd_Preload();
            cmdLoadScene.On_Done += OnLoadSceneDone;
            cmdPreload.On_Done += OnPreLoad;
            m_cJoinSequence.AddSubCommand(cmdConnectBattleServer);
            m_cJoinSequence.AddSubCommand(cmdLoadScene);
            m_cJoinSequence.AddSubCommand(cmdPreload);
            m_cJoinSequence.On_Done += OnJoinScene;
            m_cJoinSequence.Execute();
        }

        protected override void OnUpdate()
        {
            if(m_cJoinSequence != null)
            {
                m_cJoinSequence.OnUpdate();
            }
        }

        private void OnLoadSceneDone(CommandBase obj)
        {
        }

        private void OnPreLoad(CommandBase obj)
        {
            //出事化战斗场景数据
            BattleScene.Instance.Init(BattleInfo.sceneId);
            CameraSys.Instance.Init();
        }

        private void OnJoinScene(CommandBase obj)
        {
            m_cJoinSequence = null;
            CLog.Log("进入场景成功");
            //初始化一些数据
            PvpPlayerMgr.Instance.Init();

            GlobalEventDispatcher.Instance.AddEvent(GameEvent.StartBattle, OnStartBattle);
            GlobalEventDispatcher.Instance.AddEvent(GameEvent.PvpPlayerCreate, OnPlayerCreate);
            //向服务器发送准备完成消息
            C2S_GameReady_Data data = new C2S_GameReady_Data();
            NetSys.Instance.SendMsg(NetChannelType.Game, (short)PacketOpcode.C2S_GameReady, data);
        }

        private void OnStartBattle(object args)
        {
            CLog.Log("开始战斗");
            FrameSyncSys.Instance.StartRun();//开始帧同步
        }

        private void OnPlayerCreate(object args)
        {
            PvpPlayer player = (PvpPlayer)args;
            //初始化其他数据
         
            if (BattleInfo.userId == player.id)
            {
                player.CreateUnit((int)CampType.Camp1);
                PvpPlayerMgr.Instance.SetMainPlayer(player);
                ViewSys.Instance.Close("LoadingView");
                ViewSys.Instance.Open("FightView");
            }
            else
            {
                player.CreateUnit((int)CampType.Camp2);
            }
            CLog.Log("初始化其他战斗的数据");
        }

        protected override void OnExit()
        {
            ViewSys.Instance.Close("FightView");
            CameraSys.Instance.Clear();
            PvpPlayerMgr.Instance.Clear();
            SceneEffectPool.Instance.Clear();
            SceneGOPool.Instance.Clear();
            if (m_cJoinSequence != null)
            {
                m_cJoinSequence.OnDestroy();
                m_cJoinSequence = null;
            }

            //断开连接操作
        }
    }
}
