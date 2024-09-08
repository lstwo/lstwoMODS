﻿using System;
using System.Collections.Generic;
using NotAzzamods.Hacks.Custom.JobManager;
using UnityEngine;

namespace NotAzzamods.Hacks.Custom
{
    public class JobHacks : BaseHack
    {
        public override string Name => "Job Mods";

        public override string Description => "";

        public override void ConstructUI(GameObject root)
        {
            var ui = new HacksUIHelper(root);

            ui.AddSpacer(6);

            foreach (BaseJobManager manager in jobManagers)
            {
                try
                {
                    var newRoot = ui.CreateVerticalGroup(manager.GetType().Name, false, true, false, true, bgColor: new(0, 0, 0, 0));
                    var newUI = new HacksUIHelper(newRoot);
                    manager.ui = newUI;
                    manager.root = newRoot;
                    manager.ConstructUI();
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while Constructing Job Manager " + manager.GetType().Name + ": " + e.Message + e.StackTrace);
                }
            }

            ui.AddSpacer(6);
        }

        public override void RefreshUI()
        {
            if (Player == null) return;

            if (Player.Employment.GetActiveJob())
                currentJobMission = Player.Employment.GetActiveJob();
            else
                currentJobMission = null;

            foreach (BaseJobManager manager in jobManagers)
            {
                try
                {
                    manager.SetPlayerController(Player.Controller);
                    manager.RefreshUI();
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while Refreshing Job Manager: " + e.Message + e.StackTrace);
                }
            }
        }

        public override void Update()
        {
        }

        public static List<BaseJobManager> jobManagers = new();

        public Dictionary<Type, GameObject[]> jobControls = new();
        public JobMission currentJobMission;
    }
}