﻿using HawkNetworking;
using lstwoMODS_WobblyLife.UI.TabMenus;
using ShadowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using lstwoMODS_Core;
using lstwoMODS_Core.UI.TabMenus;
using lstwoMODS_Core.Hacks;

namespace lstwoMODS_WobblyLife.Hacks
{
    public class ChangePlayerName : PlayerBasedHack
    {
        public override string Name => "Change Player Name";

        public override string Description => "Allows you to Change the Players Name!";

        public override HacksTab HacksTab => Plugin.PlayerHacksTab;

        private InputFieldRef nameInput;

        public void execute(string newName)
        {
            if (Player == null) return;

            Player.Controller.SetServerPlayerName(newName);
        }

        public override void ConstructUI(GameObject root)
        {
            var ui = new HacksUIHelper(root);

            ui.AddSpacer(6);

            var lib = ui.CreateLIBTrio("Player Name", "Player Name", "Name");
            lib.Button.OnClick = () => execute(lib.Input.Text);
            nameInput = lib.Input;

            ui.AddSpacer(6);
        }

        public override void RefreshUI()
        {
            if (Player != null)
            {
                nameInput.Text = Player.Controller.GetPlayerName();
            }
        }

        public override void Update()
        {

        }
    }
}
