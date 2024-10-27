using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace YaoiLib.Terraria
{
    /// <summary>
    /// Provides utility functions and properties for multiplayer.
    /// </summary>
    public static class NetHelper
    {
        /// <summary>
        /// Gets if a singleplayer world is loaded. (Singleplayer client)
        /// </summary>
        public static bool IsOffline => Main.netMode == NetmodeID.SinglePlayer;
        /// <summary>
        /// Gets if a multiplayer world is loaded. (Multiplayer client or server)
        /// </summary>
        public static bool IsOnline => Main.netMode != NetmodeID.SinglePlayer;

        /// <summary>
        /// Gets if this process is a multiplayer client.
        /// </summary>
        public static bool IsClient => Main.netMode == NetmodeID.MultiplayerClient;
        /// <summary>
        /// Gets if this process is a server.
        /// </summary>
        public static bool IsServer => Main.netMode == NetmodeID.Server;

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatPost(string text, byte r = 255, byte g = 255, byte b = 255)
        {
            if (IsOffline)
                Main.NewText(text, r, g, b);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), new Microsoft.Xna.Framework.Color(r, g, b));
        }

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="color">The color of the message.</param>
        public static void ChatPost(string text, Microsoft.Xna.Framework.Color color)
        {
            if (IsOffline)
                Main.NewTextMultiline(text, c: color);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
        }

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatPost(LocalizedText text, byte r = 255, byte g = 255, byte b = 255)
        {
            if (IsOffline)
                Main.NewText(text.Value, r, g, b);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(text.ToNetworkText(), new Microsoft.Xna.Framework.Color(r, g, b));
        }

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatPost(LocalizedText text, Microsoft.Xna.Framework.Color color)
        {
            if (IsOffline)
                Main.NewTextMultiline(text.Value, c: color);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(text.ToNetworkText(), color);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatLocalPost(string text, byte r = 255, byte g = 255, byte b = 255)
        {
            Main.NewText(text, r, g, b);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatLocalPost(LocalizedText text, byte r = 255, byte g = 255, byte b = 255)
        {
            Main.NewText(text.Value, r, g, b);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="color">The color of the message.</param>
        public static void ChatLocalPost(string text, Microsoft.Xna.Framework.Color color)
        {
            Main.NewTextMultiline(text, c: color);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="text">The message to send.</param>
        /// <param name="color">The color of the message.</param>
        public static void ChatLocalPost(LocalizedText text, Microsoft.Xna.Framework.Color color)
        {
            Main.NewTextMultiline(text.Value, c: color);
        }

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="mod">The mod which the key belongs to.</param>
        /// <param name="key">The key of the message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatPostKey(Mod mod, string key, byte r = 255, byte g = 255, byte b = 255, params object[] args)
        {
            if (IsOffline)
                Main.NewText(Language.GetTextValue(mod.GetLocalizationKey(key), args), r, g, b);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key, args), new Microsoft.Xna.Framework.Color(r, g, b));
        }

        /// <summary>
        /// Broadcasts a message in the chat, similar to an announcement box.
        /// </summary>
        /// <param name="mod">The mod which the key belongs to.</param>
        /// <param name="key">The key of the message to send.</param>
        /// <param name="color">The color of the message.</param>
        public static void ChatPostKey(Mod mod, string key, Microsoft.Xna.Framework.Color color, params object[] args)
        {
            if (IsOffline)
                Main.NewTextMultiline(Language.GetTextValue(mod.GetLocalizationKey(key), args), c: color);
            else if (IsServer)
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key, args), color);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="mod">The mod which the key belongs to.</param>
        /// <param name="key">The key of the message to send.</param>
        /// <param name="r">The red component of the message color.</param>
        /// <param name="g">The green component of the message color.</param>
        /// <param name="b">The blue component of the message color.</param>
        public static void ChatLocalPostKey(Mod mod, string key, byte r = 255, byte g = 255, byte b = 255, params object[] args)
        {
            Main.NewText(Language.GetTextValue(mod.GetLocalizationKey(key), args), r, g, b);
        }

        /// <summary>
        /// Posts a message in chat to the provided player.
        /// </summary>
        /// <param name="mod">The mod which the key belongs to.</param>
        /// <param name="key">The key of the message to send.</param>
        /// <param name="color">The color of the message.</param>
        public static void ChatLocalPostKey(Mod mod, string key, Microsoft.Xna.Framework.Color color, params object[] args)
        {
            Main.NewTextMultiline(Language.GetTextValue(mod.GetLocalizationKey(key), args), c: color);
        }

        /// <summary>
        /// When called from the server, this sends world data to the clients. Otherwise, this does nothing.
        /// </summary>
        public static void SyncWorldData()
        {
            if (IsServer)
                NetMessage.SendData(MessageID.WorldData);
        }
    }
}
