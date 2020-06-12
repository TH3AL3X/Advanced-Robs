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
using fr34kyn01535.Uconomy;
using System.Threading;
using Rocket.Core;
using Logger = Rocket.Core.Logging.Logger;
using System.Linq;
using System.Diagnostics;

namespace Darkness
{
    public class PlayerComponent : UnturnedPlayerComponent
    {

        public int MaxPolices;
        public uint Reward;
        public float TimeRob;
        public ushort Itemsgive;
        public string TypeRobs;
        public int Cooldown;
        public bool HasBeenStarted = false;

        public static Dictionary<string, DateTime> Cooldowns = new Dictionary<string, DateTime>();
        public static Dictionary<string, string> Robo = new Dictionary<string, string>();
        public void button(UnturnedPlayer player)
        {
            Zone currentZoneFixed = AdvancedZones.Instance.getZoneByName(Atracos.Instance.NombreZona);
            // Easy Fix Copy Pasterino

            foreach (var parameter in currentZoneFixed.GetParameters())
            {
                if (parameter.name.Contains("MaxPolice"))
                {
                    var Policias = parameter.values.FirstOrDefault().ToString();
                    string PoliciasString = Policias;
                    MaxPolices = int.Parse(PoliciasString);
                }

                if (parameter.name.Contains("Reward"))
                {
                    var Recompensa = parameter.values.FirstOrDefault().ToString();
                    string RecompensaString = Recompensa;
                    Reward = uint.Parse(RecompensaString);
                }

                if (parameter.name.Contains("Time"))
                {
                    var Tiempo = parameter.values.FirstOrDefault().ToString();
                    string TiempoString = Tiempo;
                    TimeRob = float.Parse(TiempoString);
                }

                if (parameter.name.Contains("Cooldown"))
                {
                    var Espera = parameter.values.FirstOrDefault().ToString();
                    string EsperaString = Espera;
                    Cooldown = int.Parse(EsperaString);
                }
            }

            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                var untPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                RocketPlayer rocketPlayer = new RocketPlayer(untPlayer.Id);

                if (Provider.clients.Where(p => rocketPlayer.HasPermission(Atracos.Instance.Configuration.Instance.PolicePermission)).Count() < MaxPolices)
                {
                    Atracos.Instance.StopCoroutine(RobarIniciar(player, TimeRob, Reward));
                    {
                        ChatManager.serverSendMessage(Atracos.Instance.Translate("NoPolices", MaxPolices).Replace('(', '<').Replace(')', '>'), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, Atracos.Instance.Configuration.Instance.iconwarn, true);
                        return;
                        //UnturnedChat.Say(player.CSteamID, Atracos.Instance.Translate("NoPolices", MaxPolices), Color.red);
                    }
                }
                else
                {
                    if (Cooldowns.TryGetValue(untPlayer.Id, out DateTime expireDate) && expireDate > DateTime.Now && Robo[Atracos.Instance.NombreZona] == Atracos.Instance.NombreZona)
                    {
                        ChatManager.serverSendMessage(Atracos.Instance.Translate("CooldownRob", System.Math.Truncate((expireDate - DateTime.Now).TotalSeconds)).Replace('(', '<').Replace(')', '>'), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, Atracos.Instance.Configuration.Instance.iconwarn, true);
                        return;
                    }
                    else
                    {

                        if (untPlayer != null)
                        {
                            if (player.IsAdmin || !player.IsAdmin)
                            {
                                if (expireDate != null)
                                {
                                    Cooldowns.Remove(untPlayer.Id);
                                    Robo.Remove(Atracos.Instance.NombreZona);
                                }

                                // Iniciar la serie de comandos 
                                Atracos.Instance.StartCoroutine(RobarIniciar(player, TimeRob, Reward));
                                Cooldowns.Add(untPlayer.Id, DateTime.Now.AddSeconds(Cooldown));
                                Robo.Add(Atracos.Instance.NombreZona, "");
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
                        return;
                    }
                }
            }
        }

        public void desconectado(UnturnedPlayer player)
        {
            ChatManager.serverSendMessage(Atracos.Instance.Translate("leaverob", player.DisplayName, Atracos.Instance.NombreZona).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Atracos.Instance.Configuration.Instance.leavearearobmessageicon, true);
            StopCoroutine(player.GetComponent<PlayerComponent>().RobarIniciar(player, player.GetComponent<PlayerComponent>().TimeRob, player.GetComponent<PlayerComponent>().Reward));
        }

        public IEnumerator TerminarRobo(UnturnedPlayer player)
        {
            RocketPlayer rocketPlayer = new RocketPlayer(player.Id);
            if (rocketPlayer.HasPermission("advancedrobs") && !rocketPlayer.HasPermission("*"))
            {
                yield return new WaitForSeconds(Atracos.Instance.Configuration.Instance.LeaveRobSecondsForGoBack);

                StopCoroutine(player.GetComponent<PlayerComponent>().RobarIniciar(player, player.GetComponent<PlayerComponent>().TimeRob, player.GetComponent<PlayerComponent>().Reward));
                {
                    player.GetComponent<PlayerComponent>().HasBeenStarted = false;
                    // Enviar el mensaje de que el robo a sido cancelado
                    Robo.Remove(Atracos.Instance.NombreZona);
                    ChatManager.serverSendMessage(Atracos.Instance.Translate("leaverob", player.DisplayName, Atracos.Instance.NombreZona).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Atracos.Instance.Configuration.Instance.leavearearobmessageicon, true);
                    // Borrar el UI del reward
                    EffectManager.askEffectClearByID(Atracos.Instance.Configuration.Instance.UIReward, player.CSteamID);
                }
            }
        }

        public IEnumerator RobarIniciar(UnturnedPlayer experience, float TimeRob, uint Reward)
        {
            Zone currentZoneFixed = AdvancedZones.Instance.getZoneByName(Atracos.Instance.NombreZona);
            // Easy Fix Copy Pasterino

            foreach (var parameter in currentZoneFixed.GetParameters())
            {
                if (parameter.name.Contains("Reward"))
                {
                    var Recompensa = parameter.values.FirstOrDefault().ToString();
                    string RecompensaString = Recompensa;
                    Reward = uint.Parse(RecompensaString);
                }

                if (parameter.name.Contains("Time"))
                {
                    var Tiempo = parameter.values.FirstOrDefault().ToString();
                    string TiempoString = Tiempo;
                    TimeRob = float.Parse(TiempoString);
                }

                if (parameter.name.Contains("TypeRob"))
                {
                    var TypeRob = parameter.values.FirstOrDefault().ToString();
                    string TypeRobString = TypeRob;
                    TypeRobs = TypeRobString;
                }
            }

            RocketPlayer rocketPlayer = new RocketPlayer(experience.Id);
            if (rocketPlayer.HasPermission("advancedrobs") && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission(Atracos.Instance.Configuration.Instance.PolicePermission))
            {
                experience.GetComponent<PlayerComponent>().HasBeenStarted = true; // ok boomer
                // Anti Copiar Y Pegar Codigo
                string PlayerTransform = experience.CSteamID.ToString();
                // Enviar el mensaje del robo
                ChatManager.serverSendMessage(Atracos.Instance.Translate("startrob", experience.DisplayName, Atracos.Instance.NombreZona).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Atracos.Instance.Configuration.Instance.startrobmessageicon, true);
                // Segundos del robo para que luego le de el dinero
                yield return new WaitForSeconds(TimeRob);
                // Enviar el mensaje cuando el robo a acabado
                experience.GetComponent<PlayerComponent>().HasBeenStarted = false; // Booolerrr
                Robo.Add(Atracos.Instance.NombreZona, "");
                ChatManager.serverSendMessage(Atracos.Instance.Translate("finishrob", experience.DisplayName, Atracos.Instance.NombreZona).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, Atracos.Instance.Configuration.Instance.finishrobmessageicon, true);
                // Recompensa al terminar el robo
                if (TypeRobs == "item")
                {
                    foreach (var parameter in currentZoneFixed.GetParameters())
                    {

                        if (parameter.name.Contains("items"))
                        {
                            var Items = parameter.values.FirstOrDefault().ToString();
                            string ItemString = Items;
                            Itemsgive = ushort.Parse(ItemString);
                            experience.GiveItem(Itemsgive, 1);
                        }
                    }

                    EffectManager.sendUIEffect(Atracos.Instance.Configuration.Instance.UIReward, 17000, experience.CSteamID, true, "Theft is over");
                    yield return new WaitForSeconds(9f);
                    EffectManager.askEffectClearByID(Atracos.Instance.Configuration.Instance.UIReward, experience.CSteamID);
                }
                else if (TypeRobs == "money")
                {
                    if (Atracos.Instance.Configuration.Instance.UconomyOrXp)
                    {
                        // Eliminar efecto del tiempo
                        EffectManager.askEffectClearByID(16333, experience.CSteamID);
                        // Incrementar el balance en el UECONOMY
                        Uconomy.Instance.Database.IncreaseBalance(PlayerTransform, Reward);
                        // Enviar efecto del dinero reward
                        EffectManager.sendUIEffect(Atracos.Instance.Configuration.Instance.UIReward, 17000, experience.CSteamID, true, "$" + Reward);
                        // Segundos de espera para quitar la UI del reward
                        yield return new WaitForSeconds(9f);
                        // Borrar el UI del reward
                        EffectManager.askEffectClearByID(Atracos.Instance.Configuration.Instance.UIReward, experience.CSteamID);
                    }
                    else
                    {
                        // Eliminar efecto del tiempo 
                        EffectManager.askEffectClearByID(16333, experience.CSteamID);
                        // Incrementar el balance en el xp
                        experience.Experience = experience.Experience + Reward;
                        // Enviar efecto del dinero reward
                        EffectManager.sendUIEffect(Atracos.Instance.Configuration.Instance.UIReward, 17000, experience.CSteamID, true, "$" + Reward);
                        // Segundos de espera para quitar la UI del reward
                        yield return new WaitForSeconds(9f);
                        // Borrar el UI del reward
                        EffectManager.askEffectClearByID(Atracos.Instance.Configuration.Instance.UIReward, experience.CSteamID);
                    }
                }
            }
            else
            {
                UnturnedChat.Say(experience, Atracos.Instance.Translations.Instance.Translate("NoPermissions"), Color.red);
            }
        }
    }
}
