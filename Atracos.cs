using Game4Freak.AdvancedZones;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Darkness
{
    public class Atracos : RocketPlugin<Configuracion>
    {
        public static Atracos Instance;

        public static List<UnturnedPlayer> Jugador = new List<UnturnedPlayer>();

        public const string robar = "robs";

        public string NombreZona;

        public string Titulo;

        public string Texto;


        protected override void Load()
        {
            Instance = this;
            // Starterino Pepperoni
            AdvancedZones.Instance.addCustomFlag("robs", 102, "Rob any region made by Zombie");

            // Cargar botones
            EffectManager.onEffectButtonClicked += new EffectManager.EffectButtonClickedHandler(this.onEffectButtonClicked);

            // Cargar cuando sales de la zona
            AdvancedZones.onZoneLeave += onZoneLeft;

            // Cargar cuando entras a una zona
            AdvancedZones.onZoneEnter += onZoneEnter;

            U.Events.OnPlayerDisconnected += disconnect;
        }


        private void disconnect(UnturnedPlayer player)
        {
            if (player.GetComponent<PlayerComponent>().HasBeenStarted)
            {
                player.GetComponent<PlayerComponent>().desconectado(player);
            }
        }

        protected override void Unload()
        {
            // Quitar boton
            EffectManager.onEffectButtonClicked -= new EffectManager.EffectButtonClickedHandler(onEffectButtonClicked);

            // Quitar zona
            AdvancedZones.onZoneLeave -= onZoneLeft;

            // Quitar zona
            AdvancedZones.onZoneEnter -= onZoneEnter;

            U.Events.OnPlayerDisconnected -= disconnect;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"startrob", "(color=red)[AdvancedRobs] {0} IS TRYING TO ROB THE {1}(/color)"},
                    {"finishrob", "(color=red)[AdvancedRobs] {0} HAS STOLEN CORRECTLY THE {1} THEFT(/color)"},
                    {"leaverob", "(color=red)[AdvancedRobs] {0} IS OUT OF THE {1} THEFT AREA(/color)"},
                    {"CooldownRob", "(color=yellow)[AdvancedRobs](/color) (color=red)Cooldown for rob again {0}(/color)"},
                    {"NoPermissions", "[AdvancedRobs] You don't have permissions to rob this"},
                    {"NoPolices", "(color=yellow)[AdvancedRobs](/color) (color=red)To be able to theft, there must be a maximum of {0} polices(/color)"},
                    {"EnterPolice", "(color=yellow)[AdvancedRobs](/color) (color=red)You're a cop, you just got into the robbery(/color)"}
                };
            }
        }

        private void onEffectButtonClicked(Player player, string buttonName)
        {

            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromPlayer(player);

            // Boton de aceptar
            if (buttonName == "AdRob") { player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false); player.GetComponent<PlayerComponent>().button(unturnedPlayer); EffectManager.askEffectClearByID(16777, unturnedPlayer.CSteamID); EffectManager.askEffectClearByID(16444, unturnedPlayer.CSteamID); }

            // Boton de cancelar
            if (buttonName == "AdRobCancel") { player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false); EffectManager.askEffectClearByID(16777, unturnedPlayer.CSteamID); }

        }

        // Al salir de la zona se quite la UI y le quite los permisos de robar
        private void onZoneLeft(UnturnedPlayer player, Zone zone, Vector3 lastPos)
        {
            // Si tiene la flag
            if (zone.hasFlag(robar))
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);


                if (player.GetComponent<PlayerComponent>().HasBeenStarted)
                    StartCoroutine(player.GetComponent<PlayerComponent>().TerminarRobo(player));
            }
        }

        // Cuando entre a la zona del robo que se ejecute los comandos
        private void onZoneEnter(UnturnedPlayer player, Zone zone, Vector3 lastPos)
        {
            RocketPlayer rocketPlayer = new RocketPlayer(player.Id);
            if (zone.hasFlag(robar))
            {
                Zone currentzone = AdvancedZones.Instance.getZoneByName(NombreZona = $"{zone.getName()}");

                Zone currentZoneFixed = AdvancedZones.Instance.getZoneByName(Atracos.Instance.NombreZona);

                foreach (var parameter in currentZoneFixed.GetParameters())
                {
                    if (parameter.name.Contains("TitletUI"))
                    {
                        var TitleUI = parameter.values.FirstOrDefault().ToString();
                        string TitleUIString = TitleUI;
                        Titulo = TitleUIString;
                    }
                    if (parameter.name.Contains("TextUI"))
                    {
                        var TextUI = parameter.values.FirstOrDefault().ToString();
                        string TextUIString = TextUI;
                        Texto = TextUIString;
                    }
                }

                if (!player.GetComponent<PlayerComponent>().HasBeenStarted)
                {
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                    EffectManager.sendUIEffect(16777, 16778, player.CSteamID, true, Titulo, Texto);
                }

                if (player.GetComponent<PlayerComponent>().HasBeenStarted)
                    StopCoroutine(player.GetComponent<PlayerComponent>().TerminarRobo(player));

            }
            else
            {
                if (rocketPlayer.HasPermission(Atracos.Instance.Configuration.Instance.PolicePermission) && !rocketPlayer.HasPermission("*"))
                {
                    ChatManager.serverSendMessage(Atracos.Instance.Translate("EnterPolice").Replace('(', '<').Replace(')', '>'), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, Atracos.Instance.Configuration.Instance.iconwarn, true);
                    return;
                }
            }
        }
    }
}