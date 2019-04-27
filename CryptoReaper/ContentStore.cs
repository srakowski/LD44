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
        }

        private void Load<T>(string key)
        {
            _content[key] = _game.Content.Load<T>(key);
        }
    }
}
