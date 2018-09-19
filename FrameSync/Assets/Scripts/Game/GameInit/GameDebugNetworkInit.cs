﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using GameData;

namespace Game
{
    public class GameDebugNetworkInit : StateBase
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            BattleInfo.Clear();
            BattleInfo.userId = 1;
            BattleInfo.levelId = GameConst.Instance.GetInt("default_level_id");
            var levelResInfo = ResCfgSys.Instance.GetCfg<ResLevel>(BattleInfo.levelId);
            BattleInfo.sceneId = levelResInfo.scene_id;
            BattleInfo.standAlone = false;
            BattleInfo.ip = "127.0.0.1";
            BattleInfo.port = 8080;
            this.ParentSwitchState((int)GameStateType.GameIn);
        }
    }
}

