﻿using lstwoMODS_Core;
using lstwoMODS_Core.UI.TabMenus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.UI;
using UnityExplorer;

namespace lstwoMODS_WobblyLife.Hacks
{
    public class ActionEnterExitInteractModifier : PlayerBasedHack
    {
        public override string Name => "Enter Exit Interact Modifier";
        public override string Description => "";
        public override HacksTab HacksTab => Plugin.VehicleHacksTab;

        public bool ShouldKnockoutIfGoingFast
        {
            get
            {
                if (action)
                {
                    var type = typeof(global::ActionEnterExitInteract);
                    var field = type.GetField("bKnockoutPlayerIfGoingFast", Plugin.Flags);

                    return (bool)field.GetValue(action);
                }

                return false;
            }
            set
            {
                if (action)
                {
                    action.SetShouldKnockoutbPlayerIfGoingFast(value);
                }
            }
        }

        public bool Locked
        {
            get
            {
                if (action && Player != null)
                {
                    return action.IsLocked(Player.Controller);
                }

                return false;
            }
            set
            {
                if (action)
                {
                    action.SetLocked(value);
                }
            }
        }

        public bool Interactable
        {
            get
            {
                if (action)
                {
                    var type = typeof(global::ActionEnterExitInteract);
                    var field = type.GetField("bInteractable", Plugin.Flags);

                    return (bool)field.GetValue(action);
                }

                return false;
            }
            set
            {
                if (action)
                {
                    action.SetInteractable(value);
                }
            }
        }


        private global::ActionEnterExitInteract action;

        private GameObject root;

        private Toggle knockoutToggle;
        private Toggle lockToggle;
        private Toggle interactableToggle;

        public override void ConstructUI(GameObject root)
        {
            this.root = root;
            var ui = new HacksUIHelper(root);

            ui.AddSpacer(6);

            knockoutToggle = ui.CreateToggle("lstwo.ActionEnterExitInteract.KnockoutIfGoingFast", "Should knockout if going fast", (b) => ShouldKnockoutIfGoingFast = b, true);

            ui.AddSpacer(6);

            ui.CreateLBBTrio("Evacuate Players", "lstwo.ActionEnterExitInteract.EvacuatePlayers", () => action.EvacuateAll(), "Evacuate All Players", "lstwo.ActionEnterExitInteract.EvacuateAll",
                () => action.EvacuateAllExceptDriver(), "Evacuate All Except Driver", "lstwo.ActionEnterExitInteract.EvacuateAllExceptDriver");

            ui.AddSpacer(6);

            ui.CreateLBDuo("Evacuate Selected Player", "lstwo.ActionEnterExitInteract.EvacuateSelectedPlayer", () => action.EvacuatePlayer(Player.Controller), "Evacuate");

            ui.AddSpacer(6);

            lockToggle = ui.CreateToggle("lstwo.ActionEnterExitInteract.IsVehicleLocked", "Is vehicle locked", (b) => Locked = b);
            interactableToggle = ui.CreateToggle("lstwo.ActionEnterExitInteract.IsVehicleInteractable", "Is vehicle interactable", (b) => Interactable = b);

            ui.AddSpacer(6);

            ui.CreateButton("Inspect \"Action Enter Exit Interact\" Component", () =>
            {
                if (action)
                {
                    InspectorManager.Inspect(action);
                    UIManager.ShowMenu = true;
                }
            }, "lstwo.ActionEnterExitInteract.Inspect", null, 256 * 3 + 32 * 2, 32);

            ui.AddSpacer(6);
        }

        public override void RefreshUI()
        {
            if (Player == null || !Player.Controller || !Player.Controller.GetPlayerControllerInteractor()) return;

            var action = Player.Controller.GetPlayerControllerInteractor().GetEnteredAction();
            if (action != null)
            {
                if (action is global::ActionEnterExitInteract actionInteract)
                {
                    this.action = actionInteract;
                }

                knockoutToggle.isOn = ShouldKnockoutIfGoingFast;
                lockToggle.isOn = Locked;
                interactableToggle.isOn = Interactable;

                root.SetActive(true);
            }
            else
            {
                root.SetActive(false);
            }
        }

        public override void Update() { }
    }
}
