using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CryptoReaper
{
    class ContentStore
    {
        private static Dictionary<string, object> _content = new Dictionary<string, object>();

        public static T Get<T>(string key) => (T)_content[key];

        private readonly Game _game;

        public ContentStore(Game game)
        {
            _game = game;
            _game.Content.RootDirectory = "Content";
        }        

        public void LoadContent()
        {
            Load<Texture2D>("Sprites/title");
            Load<Texture2D>("Sprites/OpenSpace");
            Load<Texture2D>("Sprites/SoulSpire");
            Load<Texture2D>("Sprites/StraightHellFirePipe");
            Load<Texture2D>("Sprites/StraightSoulPipe");
            Load<Texture2D>("Sprites/Wall");
            Load<Texture2D>("Sprites/AngledHellFirePipe");
            Load<Texture2D>("Sprites/AngledSoulPipe");
            Load<Texture2D>("Sprites/HellFireSpire");
            Load<Texture2D>("Sprites/Player");
            Load<Texture2D>("Sprites/PlayerSelect");
        }

        private void Load<T>(string key)
        {
            _content[key] = _game.Content.Load<T>(key);
        }
    }
}
