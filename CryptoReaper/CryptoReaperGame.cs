using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using static CryptoReaper.Functions;

namespace CryptoReaper
{
    class CryptoReaperGame : Game
    {
        private const int TileDim = 36;
        public readonly Vector2 TileScalar = new Vector2(TileDim, TileDim);
        private readonly GraphicsDeviceManager _graphics;
        private readonly InputState _inputState;
        private SpriteFont _font;
        private SpriteBatch _spriteBatch;
        private Crypt _crypt;
        private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private List<Button> _buttons = new List<Button>();
        private Cursor _cursor;

        public CryptoReaperGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;

            _crypt = new Crypt();
            _inputState = new InputState();
            _cursor = new Cursor(this, _crypt);
        }

        protected override void Initialize()
        {
            base.Initialize();

            var i = 0;

            var soulReceiverButton = new Button(new Rectangle((i++ * 46) + 10, 700, TileDim, TileDim), _textures["Receiver"], Color.CornflowerBlue);
            soulReceiverButton.Clicked += SoulReceiverButton_Clicked;
            _buttons.Add(soulReceiverButton);

            var hfReceiverButton = new Button(new Rectangle((i++ * 46) + 10, 700, TileDim, TileDim), _textures["Receiver"], Color.IndianRed);
            hfReceiverButton.Clicked += HfReceiverButton_Clicked;
            _buttons.Add(hfReceiverButton);

            var hellFirePipe = new Button(new Rectangle((i++ * 46) + 10, 700, TileDim, TileDim), _textures["HellFirePipe"], Color.White);
            hellFirePipe.Clicked += PipeButton_Clicked; ;
            _buttons.Add(hellFirePipe);

            var engineButton = new Button(new Rectangle((i++ * 46) + 10, 700, TileDim, TileDim), _textures["Engine"], Color.White);
            engineButton.Clicked += EngineButton_Clicked;
            _buttons.Add(engineButton);
        }

        protected override void LoadContent()
        {
            void LoadTexture(string name) { _textures[name] = Content.Load<Texture2D>($"Sprites/{name}"); };
            void LoadTextures(params string[] names) => names.ToList().ForEach(LoadTexture);

            _font = Content.Load<SpriteFont>("HudFont");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures(
                "HardRock",
                "SoftRock",
                "Floor",
                "Spire",
                "Receiver",
                "HellFirePipe",
                "SoulPipe",
                "Engine",
                "Exchange"
            );
        }

        private double _stepTimer = 0;

        protected override void Update(GameTime gameTime)
        {
            _inputState.Update(gameTime);

            if (_inputState.KeyPress(Microsoft.Xna.Framework.Input.Keys.Left))
                _crypt.GenerateCryptSection(0, 1);

            _buttons.ForEach(b => b.Update(_inputState));
            _cursor.Update(_inputState);

            _stepTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_stepTimer < 10) return;
            _stepTimer = 0;
            _crypt.Step();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var xs = _crypt.CryptSections.Select(s => s.Position.X);
            var minCol = xs.Min();
            var maxCol = xs.Max() + 1;
            var colCount = maxCol - minCol;
            var ys = _crypt.CryptSections.Select(s => s.Position.X);
            var minRow = ys.Min();
            var maxRow = ys.Max() + 1;
            var rowCount = maxRow - minRow;

            _spriteBatch.Begin();

            for (var sectionRow = minRow; sectionRow < rowCount; sectionRow++)
                for (var sectionCol = minCol; sectionCol < colCount; sectionCol++)
                {
                    var section = _crypt.GetSection(sectionRow, sectionCol);
                    if (section == null)
                        continue;

                    for (int row = 0; row < Crypt.CryptFeaturesDimPerSection; row++)
                        for (int col = 0; col < Crypt.CryptFeaturesDimPerSection; col++)
                        {
                            var featurePos = section.SectionPositionToCryptPosition(row, col);
                            var feature = _crypt[featurePos];

                            var drawPos = featurePos.ToVector2() * TileScalar;

                            var textureKey =
                                feature is CryptFeature.Floor ? "Floor" :
                                feature is CryptFeature.HardRock ? "HardRock" :
                                feature is CryptFeature.SoftRock ? "SoftRock" :
                                feature is CryptFeature.Spire ? "Spire" :
                                null;

                            var texture = _textures[textureKey];

                            _spriteBatch.Draw(texture,
                                drawPos,
                                feature is CryptFeature.SoulSpire ? Color.CornflowerBlue :
                                feature is CryptFeature.HellFireSpire ? Color.IndianRed :
                                feature is CryptFeature.CryptCoinSpire ? Color.Yellow :
                                Color.White);
                             
                            if (feature.Device == null) continue;

                            var cap = (float)(feature.Device.ResourceCapacity < 1 ? 1 : feature.Device.ResourceCapacity);
                            var color = new Color(
                                feature.Device.FireQuantity / cap,
                                0,
                                feature.Device.SoulQuantity / cap
                            );

                            _spriteBatch.Draw(
                                _textures[GetDeviceTexture(feature.Device)],
                                drawPos,
                                color);
                        }
                }

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"Crypt Coins: {_crypt.CryptCoinBalance}", new Vector2(900, 600), Color.Lime);
            _spriteBatch.DrawString(_font, $"Crypt Coin Waste: {_crypt.WastedCryptCoins}", new Vector2(900, 620), Color.Lime);
            _spriteBatch.DrawString(_font, $"Hell Fire Waste: {_crypt.WastedHellFire}", new Vector2(900, 640), Color.Lime);
            _spriteBatch.DrawString(_font, $"Soul Waste: {_crypt.WastedSouls}", new Vector2(900, 660), Color.Lime);

            _buttons.ForEach(b => b.Draw(_spriteBatch));
            _cursor.Draw(_spriteBatch, _textures);
            _spriteBatch.End();
        }

        private void HfReceiverButton_Clicked(object sender, EventArgs e)
        {
            var d = new CryptDevice.HellFireReceiver(_crypt);
            _cursor.PlaceDevice = d;
            _cursor.Next = () => new CryptDevice.HellFireReceiver(_crypt);
        }

        private void SoulReceiverButton_Clicked(object sender, EventArgs e)
        {
            var d = new CryptDevice.SoulReceiver(_crypt);
            _cursor.PlaceDevice = d;
            _cursor.Next = () => new CryptDevice.SoulReceiver(_crypt);
        }

        private void PipeButton_Clicked(object sender, EventArgs e)
        {
            var d = new CryptDevice.Pipe(_crypt);
            _cursor.PlaceDevice = d;
            _cursor.Next = () => new CryptDevice.Pipe(_crypt);
        }

        private void EngineButton_Clicked(object sender, EventArgs e)
        {
            var d = new CryptDevice.CryptCoinEngine(_crypt);
            _cursor.PlaceDevice = d;
            _cursor.Next = () => new CryptDevice.CryptCoinEngine(_crypt);
        }

        [STAThread]
        static void Main()
        {
            using (var g = new CryptoReaperGame())
                g.Run();
        }
    }
}
