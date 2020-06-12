using System;
using UnityEngine;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Collections;
using Steamworks;
using SDG.Unturned;
using Game4Freak.AdvancedZones;
using Rocket.Core.Plugins;
using System.Threading;
using Rocket.Core;
using Logger = Rocket.Core.Logging.Logger;
using System.Linq;
using System.Diagnostics;
using Rocket.API.Extensions;

namespace Darkness
{
    class ComandoParametro : IRocketCommand
    {

        public List<string> Aliases
        {
            get
            {
                return new List<string>() { "genrob" };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Help
        {
            get
            {
                return "";
            }
        }

        public string Name
        {
            get
            {
                return "addrob";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "addrobs" };
            }
        }

        public string Syntax
        {
            get
            {
                return "<NameZone>";
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            string Zone = command[0];


            Zone currentZoneFixed = AdvancedZones.Instance.getZoneByName(Zone);

            List<string> MaxPolice = new List<string>() { "3" };
            List<string> RewardRob = new List<string>() { "5000" };
            List<string> TimeRob = new List<string>() { "350" };
            List<string> CooldownRob = new List<string>() { "500" };
            List<string> TitleUI = new List<string>() { "ADVANCEDROBS" };
            List<string> TextUI = new List<string>() { "You want to rob this?" };
            List<string> TypeRob = new List<string>() { "money/item" };
            List<string> ItemsGive = new List<string>() { "334" };

            currentZoneFixed.addParameter("MaxPolice", MaxPolice);
            currentZoneFixed.addParameter("Reward", RewardRob);
            currentZoneFixed.addParameter("Time", TimeRob);
            currentZoneFixed.addParameter("Cooldown", CooldownRob);
            currentZoneFixed.addParameter("TitletUI", TitleUI);
            currentZoneFixed.addParameter("TextUI", TextUI);
            currentZoneFixed.addParameter("TypeRob", TypeRob);
            currentZoneFixed.addParameter("items", ItemsGive);
            currentZoneFixed.addParameter("items", ItemsGive);

            UnturnedChat.Say(player, $"[AdvancedRobs] Has been added the parameters to Advanced Zones to {Zone} , now you can edit the parameters in AdvancedZones config", Color.red);
            AdvancedZones.Instance.Configuration.Save();
        }
    }
}

