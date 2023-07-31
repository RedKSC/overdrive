using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordIntegration : MonoBehaviour {
    Discord.Discord discordController;

    private void Start() {
        discordController = new Discord.Discord(1065535447947808798, (System.UInt64)Discord.CreateFlags.Default);
        UserManager user = discordController.GetUserManager();
    }
}
