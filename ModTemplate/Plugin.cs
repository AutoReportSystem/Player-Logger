using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Leah_ModTemplate
{
    [BepInPlugin("com.industry.playerlogger", "Player Logger", "1.0.0")]
    internal class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance = null;
        public void Start()
        {
            if (!Directory.Exists("Player Logger"))
            {
                Directory.CreateDirectory("Player Logger");
            }

            if (Instance == null)
            {
                Instance = this;
            }
        }

        static new List<string> PlayersChecked = new List<string>();

        private static string LastRoomName = string.Empty;
        public void Update()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Industry's Player Logger"))
                PhotonNetwork.LocalPlayer.CustomProperties.Add("Industry's Player Logger", true);

            if (PhotonNetwork.InRoom)
            {
                //if (PhotonNetwork.CurrentRoom.Name != LastRoomName)
                {
                    LastRoomName = PhotonNetwork.CurrentRoom.Name;
                    StartCoroutine(CheckServer());
                }
            }
            else
            {
                LastRoomName = string.Empty;
            }
        }

        public static IEnumerator CheckServer()
        {
            yield return new WaitForSeconds(9);
            try
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    if (!Directory.Exists($"Player Logger/{PhotonNetwork.CurrentRoom.Name}"))
                    {
                        Directory.CreateDirectory($"Player Logger/{PhotonNetwork.CurrentRoom.Name}");
                    }
                    foreach (var player in PhotonNetwork.PlayerListOthers)
                    {
                        if (!PlayersChecked.Contains(player.UserId))
                        {
                            string cosmetics = string.Empty;
                            foreach (CosmeticsController.CosmeticItem cosmetic in CosmeticsController.instance.allCosmetics)
                            {
                                if (GorillaGameManager.instance.FindPlayerVRRig(player).rawCosmeticString.Contains(cosmetic.itemName))
                                {
                                    cosmetics += $"{cosmetic.itemName}\n";
                                }
                            }
                            if (cosmetics == string.Empty)
                            {
                                continue;
                            }

                            if (!PlayersChecked.Contains(player.UserId))
                            {
                                File.WriteAllText($"Player Logger/{PhotonNetwork.CurrentRoom.Name}/{player.UserId} - {player.NickName}.txt", $"Name:\n{player.NickName}\n\nUser ID:\n{player.UserId}\n\nTime Found:\n{DateTime.Now.ToString("F")}\n\n\nCustom Properties:\n{ShowPlayerProperties(player)}\n\nCosmetics:\n{cosmetics}");
                                PlayersChecked.Add(player.UserId);
                            }
                        }
                    }
                }
                if (!PhotonNetwork.InRoom && PlayersChecked.Count > 0)
                {
                    PlayersChecked.Clear();
                }
            }
            catch (Exception e) { }
            yield break;
        }

        static string ShowPlayerProperties(Player player)
        {
            string properties = player.CustomProperties.Count > 0
                ? string.Join("\n", player.CustomProperties.Select(kvp => $"{kvp.Key}: {kvp.Value}"))
                : "No Custom Properties.";

            return properties;
        }
    }
}
