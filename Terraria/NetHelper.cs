using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

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
        /// When called from the server, this sends world data to the clients. Otherwise, this does nothing.
        /// </summary>
        public static void SyncWorldData()
        {
            if (IsServer)
                NetMessage.SendData(MessageID.WorldData);
        }
    }
}
