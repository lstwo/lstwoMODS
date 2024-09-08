﻿using HawkNetworking;
using Rewired;
using ShadowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

namespace NotAzzamods.UI.TabMenus
{
    public class PropSpawnerTab : BaseTab
    {
        public GameObject SelectedProp
        {
            set
            {
                selectedObject = value;
                spawnBtn.ButtonText.text = $"Spawn Prop ({value.name.Replace(" (UnityEngine.GameObject)", "")})";
            }
        }

        private ScrollPool<PropCell> scrollPool;
        private GameObject selectedObject;
        private PropCellHandler cellHandler;
        private ButtonRef spawnBtn;

        public PropSpawnerTab()
        {
            Name = "Prop Spawner";
        }

        public override void ConstructUI(GameObject root)
        {
            base.ConstructUI(root);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 5, 9999, 0);

            var scrollGroup = UIFactory.CreateHorizontalGroup(root, "group", true, false, true, true, bgColor: new(0, 0, 0, 0));
            UIFactory.SetLayoutElement(scrollGroup, 0, 620, 9999, 0);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", scrollGroup), 5, 0, 0, 9999);

            scrollPool = UIFactory.CreateScrollPool<PropCell>(scrollGroup, "scroll", out var scrollRoot, out var scrollContent, new Color(.114f, .129f, .161f));
            UIFactory.SetLayoutElement(scrollRoot, 0, 0, 9999, 9999);
            UIFactory.SetLayoutElement(scrollContent, 0, 0, 9999, 9999);

            cellHandler = new(this);
            scrollPool.Initialize(cellHandler);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", scrollGroup), 5, 0, 0, 9999);
            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 5, 9999, 0);

            spawnBtn = UIFactory.CreateButton(root, "SpawnBtn", "Spawn Prop");
            spawnBtn.OnClick = () =>
            {
                var player = PlayerUtils.GetMyPlayer();
                if (player == null || !selectedObject) return;

                var character = player.GetPlayerCharacter();
                var pos = character.GetPlayerPosition() + character.GetPlayerForward();
                HawkNetworkManager.DefaultInstance.InstantiateNetworkPrefab(selectedObject, pos);
            };
            UIFactory.SetLayoutElement(spawnBtn.GameObject, 0, 32, 9999, 0);
        }

        public override void RefreshUI()
        {
            base.RefreshUI();

            cellHandler.Refresh();
        }
    }

    public class PropCell : ICell
    {
        private bool enabled;

        public bool Enabled => enabled;
        public RectTransform Rect { get; set; }
        public GameObject UIRoot { get; set; }
        public float DefaultHeight => 32;

        public GameObject prop;
        public PropSpawnerTab parentTab;

        private Text text;
        private ButtonRef button;

        public GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateUIObject("PropCell", parent);
            UIFactory.SetLayoutElement(UIRoot, 0, 32, 9999, 0);
            Rect = UIRoot.GetComponent<RectTransform>();

            button = UIFactory.CreateButton(UIRoot, "button", "");
            button.OnClick = OnCellButtonClicked;

            RectTransform buttonRect = button.GameObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 0);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;

            text = button.ButtonText;

            return UIRoot;
        }

        public void OnCellButtonClicked()
        {
            parentTab.SelectedProp = prop;
        }

        public void Disable()
        {
            enabled = false;
            UIRoot.SetActive(false);
        }

        public void Enable()
        {
            enabled = true;
            UIRoot.SetActive(true);
        }

        public void ConfigureCell(GameObject prop)
        {
            this.prop = prop;
            text.text = prop.ToString();
        }
    }

    public class PropCellHandler : ICellPoolDataSource<PropCell>
    {
        public int ItemCount => propList.Count;
        public PropSpawnerTab parentTab;

        private List<GameObject> propList = new();

        public PropCellHandler(PropSpawnerTab parentTab)
        {
            this.parentTab = parentTab;
        }

        public void OnCellBorrowed(PropCell cell)
        {
        }

        public void SetCell(PropCell cell, int index)
        {
            if(index < propList.Count)
            {
                cell.parentTab = parentTab;
                cell.ConfigureCell(propList[index]);
                cell.Enable();
            }
            else
            {
                cell.Disable();
            }
        }

        public void Refresh()
        {
            propList.Clear();

            foreach(var behavior in UnityEngine.Object.FindObjectsOfType<HawkNetworkBehaviour>())
            {
                var obj = behavior.gameObject;
                if(obj && !obj.name.Contains("(Clone)"))
                {
                    propList.Add(obj);
                }
            }
        }

        private string[] ArtifactPrefabs = { "Ancient Fossils/Artifact_AncientFossils_Ammonite", "Ancient Fossils/Artifact_AncientFossils_Fossilised_Plant", "Ancient Fossils/Artifact_AncientFossils_Giant_Shell", "Ancient Fossils/Artifact_AncientFossils_Shark Tooth", "Ancient Fossils/Artifact_AncientFossils_Trilobite", "Cave/Artifact Cave Amethyst", "Cave/Artifact Cave Jade", "Cave/Artifact Cave Ruby", "Cave/Artifact Cave Saphire", "Dinosaur Fossils/Artifact_Dino Egg", "Dinosaur Fossils/Artifact_Dino Footprint", "Dinosaur Fossils/Artifact_Dino_Claw", "Dinosaur Fossils/Artifact_Dino_Dropping", "Dinosaur Fossils/Artifact_Dino_Mosquito_in_Amber_resin", "Early Wobbly/Artifact_EarlyWobbly_EarlyWobblySpear", "Early Wobbly/Artifact_EarlyWobbly_First_Pizza", "Early Wobbly/Artifact_EarlyWobbly_FirstWheel", "Early Wobbly/Artifact_EarlyWobbly_Fossilised_Present", "Early Wobbly/Artifact_EarlyWobbly_Wobbly_Club", "Gold/Artifact_Gold_Gold_Pants", "Gold/Artifact_Gold_Gold_Ring", "Gold/Artifact_Gold_Wobbly_Amulet", "Gold/Artifact_Gold_Wobbly_Sceptre", "Ice Cave/Artifact_IceCave_Mamoth Tusk", "Ice Cave/Artifact_IceCave_Penguin Hockey Stick", "Ice Cave/Artifact_IceCave_Penguin Ice Sculpture", "Ice Cave/Artifact_IceCave_Yeti Fur", "Jungle/Artifact_Jungle_Ancient_Wobbly_Bomb", "Jungle/Artifact_Jungle_Ancient_Wobbly_Mask", "Jungle/Artifact_Jungle_Venus_FlyTrap", "Jungle/Artifact_Jungle_Wobbly_Stone", "Ocean/Artifact_Ocean_ClamShell", "Ocean/Artifact_Ocean_Coral", "Ocean/Artifact_Ocean_Pearl", "Ocean/Artifact_Ocean_Turtle Shell", "ParadiseIsland/Artifact_ParadiseIsland_Cocktail", "ParadiseIsland/Artifact_ParadiseIsland_Flower", "ParadiseIsland/Artifact_ParadiseIsland_Golf_Ball_Emerald", "ParadiseIsland/Artifact_ParadiseIsland_GolfClub_Gold", "Random/Artifact_Random_Ancient_Jelly", "Random/Artifact_Random_Ghost_Anchor", "Sewer/Artifact_Sewer_Baby Tooth", "Sewer/Artifact_Sewer_Blueprints", "Sewer/Artifact_Sewer_Goldern Toilet", "Sewer/Artifact_Sewer_Lipstick", "Treasure/Artifact_Treasure_Ancient_Crown", "Treasure/Artifact_Treasure_Ancient_Scroll", "Treasure/Artifact_Treasure_Goblet", "Treasure/Artifact_Treasure_UraniumGems", "TrexParts/Artifact_Trex_ArmPart", "TrexParts/Artifact_Trex_JawPart", "TrexParts/Artifact_Trex_LegPart", "TrexParts/Artifact_Trex_TailPart", "Wizard/Artifact_Wizard_Hat", "Wizard/Artifact_Wizard_Magic Book", "Wizard/Artifact_Wizard_Robe", "Wizard/Artifact_Wizard_Staff" };
        private string[] HouseInteriorPrefabs = { "Grannys Sofa", "Prop_Big_Chair_01", "Prop_Big_Chair_02", "Prop_Big_Chair_03", "Prop_Big_Clock", "Prop_CChair_01", "Prop_Chair_02", "Prop_Chair_03", "Prop_Chair_04", "Prop_Chair_05", "Prop_Chair_06", "Prop_Chair_07", "Prop_Chair_08", "Prop_Chair_09", "Prop_Couch_01", "Prop_Couch_02", "Prop_Couch_03", "Prop_Couch_04", "Prop_Couch_05", "Prop_Couch_06", "Prop_Couch_07", "Prop_Couch_08", "Prop_Couch_09", "Prop_Cushion_01", "Prop_Cushion_02", "Prop_Desk_Chair", "Prop_Draw_01", "Prop_Draw_02", "Prop_Draw_03", "Prop_Draw_04", "Prop_Draw_05", "Prop_Draw_06", "Prop_Dryer", "Prop_Flower_Pot_01", "Prop_Foot_Rest_01", "Prop_Foot_Rest_02", "Prop_Fridge_01", "Prop_FruitBowl", "Prop_Shelf_07_Dynamic", "Prop_Small_Table_01", "Prop_Small_Table_02", "Prop_Small_Table_03", "Prop_Small_Table_04", "Prop_Small_Table_05", "Prop_Small_Table_06", "Prop_Small_Table_07", "Prop_Small_Table_08", "Prop_Speaker_01", "Prop_Speaker_02", "Prop_Speaker_03", "Prop_Table_01", "Prop_Table_02", "Prop_Table_03", "Prop_Tv_01", "Prop_Tv_02", "Prop_Tv_03", "Prop_Tv_04", "Prop_Tv_06", "Prop_Tv_07", "Prop_Washing_Machine", "Prop_Cupboard_01", "Prop_Cupboard_02", "Prop_Cupboard_03", "Prop_Cupboard_04", "Prop_Cupboard_07", "Prop_Lamp_01", "Prop_Lamp_02", "Prop_Lamp_03", "Prop_Lamp_04", "Prop_Lamp_05", "Prop_Lamp_06", "Prop_Microwave", "Prop_Oven_Blue", "Prop_Oven_Grey", "Prop_Oven_White" };
        private string[] GoldMinePrefabs = { "PickAxe", "Rocks/Diamond_Rock_Large", "Rocks/Diamond_Rock_Normal", "Rocks/Diamond_Rock_Small", "Rocks/Ghost_GraveStone", "Rocks/Gold_Rock_Large", "Rocks/Gold_Rock_Normal", "Rocks/Gold_Rock_Small", "Rocks/Iron_Rock_Large", "Rocks/Iron_Rock_Normal", "Rocks/Iron_Rock_Small", "Rocks/Uranium_Rock_Large", "Rocks/Uranium_Rock_Normal", "Rocks/Uranium_Rock_Small", "TNT" };
        private string[] HospitalPrefabs = { "HospitalBloodPresure", "HospitalMachine", "HospitalStand" };
        private string[] UncategorizedPrefabs = { "AmericanFridge", "Axe_FireFighter", "Axe_Woodland", "Banana_L", "Banana_M", "Banana_S", "Banana_To_Peel", "Bar Bell", "Barrel", "BaseBallBat_Blue", "BaseBallBat_Green", "BaseBallBat_Yellow", "Bongos", "BobSled", "Cannon_Ball", "Chair_BeachHut", "Floaty_Bed", "Floaty_Crocodile", "Floaty_Duck", "FireExtinguisher", "Garden Gnome", "GarbageBag", "GasBottle", "Jelly", "KnightSword", "LampPost_Dynamic", "Lantern_Blue", "Lawn Flamingo Broken", "Mayor_Key", "Mop", "NewsPaper", "Outlet Monster System", "PizzaBox", "PizzaBoxLarge", "Patio Chair", "Patio Table", "Pallet", "Picnicbench", "Plank", "Portable_HosePipe", "PoolNoodle", "Prop_Umbrella_01", "Prop_Umbrella_02", "Prop_Umbrella_03", "RecordPlayer", "RemoteExplosive", "RubberDucky", "RubberDucky_Giant", "RubberDucky_Huge", "RubberDucky_Large", "RubberDucky_Mid", "RubberDucky_Small", "Rubber_Malet", "Small_HokeyPuck", "ShoppingTrolly", "SurfBoard_1", "SurfBoard_2", "Table_BeachHut", "Tennis Ball", "Tennis Racket", "Torch", "Trash Can", "ToolBox", "WickerBasket", "WickerBasket_1", "WorkLight", "WrongSword_1", "WrongSword_2", "WrongSword_3", "WrongSword_4", "WrongSword_5", "WrongSword_6" };
        private string[] GolfPrefabs = { "Golf Bag", "GolfBall", "GolfBall_Green", "GolfBall_Orange", "GolfBall_Yellow", "GolfClub", "GolfClub_Driver", "GolfClub_Iron" };
        private string[] PropShopPrefabs = { "PropShop_Food/PropShop_Apple", "PropShop_Food/PropShop_Baguette", "PropShop_Food/PropShop_Banana", "PropShop_Food/PropShop_Barrito", "PropShop_Food/PropShop_Bread", "PropShop_Food/PropShop_BottleWater", "PropShop_Food/PropShop_CakeSlice", "PropShop_Food/PropShop_Carrot", "PropShop_Food/PropShop_CauliFlower", "PropShop_Food/PropShop_Cereal", "PropShop_Food/PropShop_Cheese", "PropShop_Food/PropShop_Chicken", "PropShop_Food/PropShop_Chocolate", "PropShop_Food/PropShop_ChocolateLolly", "PropShop_Food/PropShop_CrispyCrisps", "PropShop_Food/PropShop_Croissant", "PropShop_Food/PropShop_Cup", "PropShop_Food/PropShop_Donut", "PropShop_Food/PropShop_Eclair", "PropShop_Food/PropShop_Fish", "PropShop_Food/PropShop_Ham", "PropShop_Food/PropShop_HotDog", "PropShop_Food/PropShop_IceCreamCone", "PropShop_Food/PropShop_IceCreamSoft", "PropShop_Food/PropShop_IceCreamTub", "PropShop_Food/PropShop_IceLolly", "PropShop_Food/PropShop_IceSlice", "PropShop_Food/PropShop_LargeCake", "PropShop_Food/PropShop_Lettuce", "PropShop_Food/PropShop_Milk", "PropShop_Food/PropShop_Pizza", "PropShop_Food/PropShop_Sandwich", "PropShop_Food/PropShop_SodaPop", "PropShop_Food/PropShop_Steak", "PropShop_Food/PropShop_SweetCorn", "PropShop_Food/PropShop_Tomato", "PropShop_Food/PropShop_TwoStickIceLolly", "PropShop_Food/PropShop_VeggieSausage", "PropShop_Food/PropShop_WaterMelon", "PropShop_GreenScreen/PropShop_Anchor", "PropShop_GreenScreen/PropShop_Cactus", "PropShop_GreenScreen/PropShop_Cactus_1", "PropShop_GreenScreen/PropShop_CampFire", "PropShop_GreenScreen/PropShop_CattleSkull", "PropShop_GreenScreen/PropShop_ClapBoard", "PropShop_GreenScreen/PropShop_Crab", "PropShop_GreenScreen/PropShop_Flag", "PropShop_GreenScreen/PropShop_MoonBuggy", "PropShop_GreenScreen/PropShop_MoonRock", "PropShop_GreenScreen/PropShop_MoonRock_1", "PropShop_GreenScreen/PropShop_MessageBottle", "PropShop_GreenScreen/PropShop_Prop_Cactus", "PropShop_GreenScreen/PropShop_Prop_Cactus_1", "PropShop_GreenScreen/PropShop_Prop_CampFire", "PropShop_GreenScreen/PropShop_Prop_CattleSkull", "PropShop_GreenScreen/PropShop_Prop_Crab", "PropShop_GreenScreen/PropShop_Prop_Flag", "PropShop_GreenScreen/PropShop_Prop_MoonBuggy", "PropShop_GreenScreen/PropShop_Prop_Rock", "PropShop_GreenScreen/PropShop_Prop_Rock_1", "PropShop_GreenScreen/PropShop_Prop_SeaWeed", "PropShop_GreenScreen/PropShop_Prop_Wagon", "PropShop_GreenScreen/PropShop_Robot", "PropShop_GreenScreen/PropShop_Rover", "PropShop_GreenScreen/PropShop_ScienceBox", "PropShop_GreenScreen/PropShop_ShipWreck", "PropShop_GreenScreen/PropShop_TresureChest", "PropShop_GreenScreen/PropShop_TresureChest_Open", "PropShop_GreenScreen/PropShop_TumbleWeed" };
        private string[] JunglePrefabs = { "Game/Cave_Maze_Key_Orb", "Game/Moving_Block", "Game/Temple_Maze_Key_Orb", "Ruins/NaughtsAndCrosses_O", "Ruins/NaughtsAndCrosses_X", "Temple/Crate", "Temple/FloorTorch", "Temple/Temple_TallVase", "Temple/Temple_Vase", "TrialRooms/Trial Hat Prop" };
        private string[] SewersPrefabs = { "Sewer Barrel Blue", "Sewer Barrel Toxic Waste", "Sewer Lanturn Handheld", "Sewer Map/Sewer_Map_V1", "Sewer Map/Sewer_Map_V2", "Sewer Map/Sewer_Map_V3", "Sewer Map/Sewer_Map_V4", "Sewer Map/Sewer_Map_V5", "Sewer Map/Sewer_Map_V6", "Sewer Map/Sewer_Map_V7", "Sewer Map/Sewer_Map_V8", "Sewer Map/Sewer_Map_V9", "Sewer Old/Queen Frog Crown", "Sewer Puzzle/Sewer_Mountain/Coal", "Sewer Puzzle/Sewer_Mountain/Plaque_Left", "Sewer Puzzle/Sewer_Mountain/Plaque_Right" };
        private string[] MissionsPrefabs = { "Detective Series/HauntedHouse/HauntedHouseLunchBox", "Detective Series/LostMagnifyingGlass/MagnifyingGlass", "Detective Series/VigilanteJelly/CleaningProduct", "Detective Series/VigilanteJelly/HandBag_Jelly", "Diserted Island/Bouncy Ball With Face", "Dress As Clown Party/Baloon Inflator", "Dress As Clown Party/Baloon Snake", "Dress As Clown Party/Baloon Sword", "Dress As Clown Party/Clown Cream Pie", "Golden Bowling/Bowling_Ball_Golden", "JungleGorilla/Banana_JungleGorillaMission", "LostBeesMission/Bee Keeper Net", "LostBinoculars/Binoculars", "Stolen Sandwich/Sandwich_MissionItem", "Unrobbery Bank/Bank Card_Blue", "Unrobbery Bank/Bank Card_Red", "Unrobbery Bank/Bank Diamond", "Unrobbery Bank/BankFuseBox_Key", "Unrobbery Bank/BankVase", "UnderGroundMixup/FirstAidKit", "UnderGroundMixup/FloatyRescueDuck", "UnderGroundMixup/MonkeyWrench" };
        private string[] MountainBasePrefabs = { "Alien Proof", "GiantBoxes/GiantBoxes.008", "GiantBoxes/GiantBoxes.009", "GiantBoxes/GiantBoxes.010", "GiantBoxes/GiantBoxes.012", "ScienceSmallContainer" };
        private string[] LumberPrefabs = { "LumberTree_Dynamic" };
        private string[] LabPrefabs = { "LargeLab/Baby Spider", "LargeLab/BluePiece", "LargeLab/CheckersRed", "LargeLab/CheckersWhite", "LargeLab/Dice", "LargeLab/GreenPiece", "LargeLab/PaperClip", "LargeLab/PushPin", "LargeLab/RedPiece", "LargeLab/SI_Prop_CarboardBox_02", "LargeLab/Straw", "LargeLab/YellowPiece", "UFO Part Battery", "UFO Part Fuse", "UFO Part Keys" };
        private string[] MuseumPrefabs = { "MuseumBasement/TrexBone", "MuseumBasement/Trilobite" };
        private string[] ObservatoryPrefabs = { "Observatory_Moon Boots Box" };
        private string[] PlayGroundPrefabs = { "BeachBall", "BeachBall_Large", "BeachBall_Mid", "BouncyCastle", "FootBall", "Horse on stick", "Rocking horse", "Toy_Helicopter", "Trampoline" };
        private string[] PowerPlantPrefabs = { "ToxicWaste_Drum" };
        private string[] JellyManPrefabs = { "JackHammer", "JellyBasementKey", "JellyCarSteeringWheel", "JellyCar_Wheel", "JellyChair", "JellyLamp", "JellySofa", "NoJellyCarEngineDynamic" };
        private string[] IceCreamPrefabs = { "Ice Cream Bucket", "Ice Cream Scoop" };
        private string[] DreamPrefabs = { "Burger_Cloud", "Castle/AlarmClock", "Castle/Bedroom/StoryCounters/StoryCounter_1", "Castle/Bedroom/StoryCounters/StoryCounter_2", "Castle/Bedroom/StoryCounters/StoryCounter_3", "Castle/Bedroom/StoryCounters/StoryCounter_4", "Castle/Bedroom/StoryCounters/StoryCounter_5", "Castle/ClockKey", "Castle/IdeaBulb_SofaUnlock", "Castle/Kitchen/Dream_Kitchen_Apple", "Castle/Kitchen/Dream_Kitchen_Bread", "Castle/Kitchen/Dream_Kitchen_Carrot", "Castle/Kitchen/Dream_Kitchen_Cheese", "Castle/Kitchen/Dream_Kitchen_Fish_1", "Castle/Kitchen/Dream_Kitchen_Ham", "Castle/Kitchen/Dream_Kitchen_LargeCake", "Castle/Kitchen/Dream_Kitchen_Lettuce", "Diorama Props/Dream Puzzle Golf Club_Dynamic", "Diorama Props/Dream Puzzle Ham_Dynamic", "Diorama Props/Dream Puzzle Shovel_Dynamic", "Diorama Props/Dream Puzzle Sloth_Dynamic", "Diorama Props/Dream Puzzle TowerBlock_Dynamic", "Diorama Props/Dream Puzzle TrexBone_Dynamic", "DuckyCloud", "Ideas/Dream_Idea_Prop_Burger", "Ideas/Dream_Idea_Prop_Duck", "Ideas/Dream_Idea_Prop_Piano", "Ideas/Dream_Idea_Prop_Plane", "KingsInvitation", "Piano_Cloud" };
        private string[] WeatherStationPrefabs = { "Job/WeatherBalloon_Device_ZigZag" };
        private string[] TreasurePrefabs = { "Money/Treasure_Bronze_Ingot", "Money/Treasure_Bronze_Ring", "Money/Treasure_Gold_Coin", "Money/Treasure_Gold_Ingot", "Money/Treasure_Silver_Coin", "Money/Treasure_Silver_Ingot", "Money/Treasure_Silver_Ring" };
        private string[] BoardGamesPrefabs = { "BluePiece", "CheckersRed", "CheckersWhite", "Chess Pieces/Chess Bishop Black", "Chess Pieces/Chess Bishop White", "Chess Pieces/Chess King Black", "Chess Pieces/Chess King White", "Chess Pieces/Chess Knight Black", "Chess Pieces/Chess Knight White", "Chess Pieces/Chess Pawn Black", "Chess Pieces/Chess Pawn White", "Chess Pieces/Chess Queen Black", "Chess Pieces/Chess Queen White", "Chess Pieces/Chess Rook Black", "Chess Pieces/Chess Rook White", "Dice", "GreenPiece", "RedPiece", "YellowPiece" };
        private string[] ConstructionPrefabs = { "Props/ClawHammer", "Props/LumpHammer", "Props/ResourcesBag_Dynamic" };
        private string[] TvStudioPrefabs = { "Chair", "DirectorsChair", "DynamicWobblyQuiz_Poster", "Wig_Hair_Dynamic" };
        private string[] ShopInteriorPrefabs = { "SI_Prop_Bench_01", "SI_Prop_CarboardBox_01", "SI_Prop_CarboardBox_02", "SI_Prop_CarboardBox_03", "SI_Prop_Chair_01", "SI_Prop_FireExtinguisher_01", "SI_Prop_HighChair_01", "SI_Prop_Instrument_Guitar_01", "SI_Prop_Instrument_Guitar_02", "SI_Prop_Instrument_Guitar_03", "SI_Prop_Instrument_Guitar_04", "SI_Prop_Instrument_Guitar_06", "SI_Prop_Instrument_Keyboard", "SI_Prop_Instrument_Keyboard (1)", "SI_Prop_Ladder_01", "SI_Prop_SignCaution_01", "SI_Prop_Stereo", "SI_Prop_Stereo_03", "SI_Prop_Toy_Alien_1", "SI_Prop_Toy_Banana", "SI_Prop_Toy_Bear_01", "SI_Prop_Toy_Bear_02", "SI_Prop_Toy_Caterpillar", "SI_Prop_Toy_Chick_01", "SI_Prop_Toy_Dog_01", "SI_Prop_Toy_Leak" };
        private string[] ShoppingBasketsPrefabs = { "Green_ShoppingBasket" };
        private string[] CavesPrefabs = { "Cave_CoinRoom/Coins/Coin_Moon", "Cave_CoinRoom/Coins/Coin_MoonStar", "Cave_CoinRoom/Coins/Coin_Sun", "Cave_CoinRoom/Coins/Coin_Sun_1", "Cave_CoinRoom/Coins/Coin_Stars_Moon", "HauntedTunnel/FlashLight", "IceCave/GongHammer", "IceCave/Hockey Stick", "IceCave/IceChisel", "IceCave/IcePick", "IceCave/Yeti Photo Frame" };
        private string[] BurgerPrefabs = { "BottomBun", "BurgerCooked", "BurgerRaw", "Cheese", "CheeseMelted", "Lettuce", "Tomato", "TopBun" };
        private string[] BowlingPrefabs = { "Bowling_Aid", "Bowling Ball", "Bowling Pins Set" };
        private string[] FishingPropsPrefabs = { "Fish/Dynamic/Angel Fish_Dynamic", "Fish/Dynamic/Angler Fish_Dynamic", "Fish/Dynamic/Arctic Char_Dynamic", "Fish/Dynamic/Arctic Cod_Dynamic", "Fish/Dynamic/Boot_Dynamic", "Fish/Dynamic/Bream_Dynamic", "Fish/Dynamic/Blue Dragon_Dynamic", "Fish/Dynamic/Carp_Dynamic", "Fish/Dynamic/CatFish_Dynamic", "Fish/Dynamic/Cockle_Dynamic", "Fish/Dynamic/ClownFish_Dynamic", "Fish/Dynamic/Cod_Dynamic", "Fish/Dynamic/Eel_Dynamic", "Fish/Dynamic/FlyingFish_Dynamic", "Fish/Dynamic/Flounder_Dynamic", "Fish/Dynamic/Frozen Boot", "Fish/Dynamic/Giant Crab_Dynamic", "Fish/Dynamic/Grouper_Dynamic", "Fish/Dynamic/Haddock_Dynamic", "Fish/Dynamic/Herring_Dynamic", "Fish/Dynamic/Ice Cube Fish_Dynamic", "Fish/Dynamic/Ice Cube_Dynamic", "Fish/Dynamic/Ice Fish_Dynamic", "Fish/Dynamic/JellyFish_Dynamic", "Fish/Dynamic/LionFish_Dynamic", "Fish/Dynamic/Lobster_Dynamic", "Fish/Dynamic/Marlin_Dynamic", "Fish/Dynamic/Oyster_Dynamic", "Fish/Dynamic/Pike_Dynamic", "Fish/Dynamic/Prawn_Dynamic", "Fish/Dynamic/Pufferfish_Dynamic", "Fish/Dynamic/Rainbow Trout_Dynamic", "Fish/Dynamic/Salmon_Dynamic", "Fish/Dynamic/Salmon_Pink_Dynamic", "Fish/Dynamic/Sardine_Dynamic", "Fish/Dynamic/Sea Cucumber_Dynamic", "Fish/Dynamic/Sea Horse_Green_Dynamic", "Fish/Dynamic/SeaHorse_Yellow_Dynamic", "Fish/Dynamic/Sea Spider_Dynamic", "Fish/Dynamic/SeaUrchin_Dynamic", "Fish/Dynamic/Seaweed Ball_Dynamic", "Fish/Dynamic/Squid_Dynamic", "Fish/Dynamic/Starfish_Dynamic", "Fish/Dynamic/StingRay_Dynamic", "Fish/Dynamic/Trout_Dynamic", "Fish/Dynamic/Wobbly Fish_Dynamic", "Fish/Dynamic/Worm_Dynamic" };
        private string[] FarmPrefabs = { "Bean", "CauliFlower", "Cow_Dynamic", "SweetCorn", "Tomato", "Tomato_Cannon" };
        private string[] WizardPrefabs = { "CloudHouse", "Wizard Prop Shop/MagicWand_Attract", "Wizard Prop Shop/MagicWand_Repel", "Wizard Prop Shop/WizardPotion_BigHead", "Wizard Prop Shop/WizardPotion_BigNose", "Wizard Prop Shop/WizardPotion_Fart", "Wizard Prop Shop/WizardPotion_FlyAway", "Wizard Prop Shop/WizardPotion_Jump", "Wizard Prop Shop/WizardPotion_LongArms", "Wizard Prop Shop/WizardPotion_RainbowSkin", "Wizard Prop Shop/WizardPotion_Sleep", "Wizard Prop Shop/WizardPotion_SmallHead", "Wizard Prop Shop/WizardPotion_Speed", "Wizard Prop Shop/WizardPotionSack", "WizardTent" };

    }
}
