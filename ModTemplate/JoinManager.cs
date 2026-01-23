using HarmonyLib;
using Leah_ModTemplate;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PlayerLogger
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
    internal class JoinPatch : MonoBehaviour
    {
        private static void Prefix(Player newPlayer)
        {
            if (newPlayer != oldnewplayer)
            {
                Plugin.Instance.StartCoroutine(Plugin.CheckServer());
                oldnewplayer = newPlayer;
            }
        }

        private static Player oldnewplayer;
    }
}
