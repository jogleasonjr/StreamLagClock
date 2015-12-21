using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if SDL2
using SDL2;
#else
using Forms = System.Windows.Forms;
#endif

namespace StreamLagClock
{
    public class GameHost : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _bigFont, _miniFont;
        private bool _backgroundToggle;
        private readonly FrameCounter _frameCounter = new FrameCounter();
        private int _frameNumber;

        public GameHost()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
            Window.Title = "Stream Lag Clock";
            this.IsFixedTimeStep = false;

            //maximize window
#if SDL2
            SDL.SDL_MaximizeWindow(Window.Handle);
            /* Some platforms may like having ApplyChanges called after this.
             * It depends on system window manager events though...
             * The fun with replacing WinForms!
             * -flibit
             */
            Rectangle bounds = Window.ClientBounds;
            _graphics.PreferredBackBufferWidth = bounds.Width;
            _graphics.PreferredBackBufferHeight = bounds.Height;
#else
            var form = (Forms.Form)Forms.Form.FromHandle(Window.Handle);
            form.WindowState = Forms.FormWindowState.Maximized;
#endif
        }


        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bigFont = Content.Load<SpriteFont>("Consolas");
            _miniFont = Content.Load<SpriteFont>("Courier New");
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        private static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }

            else return val;
        }

        protected override void Draw(GameTime gameTime)
        {
            var windowCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            // change background color to force streamer to update the whole screen
            var bgColor = Color.Azure;
            _backgroundToggle = !_backgroundToggle;
            if (_backgroundToggle)
            {
                bgColor = Color.LightCyan;
            }

            GraphicsDevice.Clear(bgColor);

            _spriteBatch.Begin();

            var centerSplit = new Vector2(0, 52); 

            // frame text
            string frameText = string.Format("F{0}", _frameNumber++.ToString("0000"));
            Vector2 frameTextCenter = (_bigFont.MeasureString(frameText) / 2) + centerSplit;
            if (_frameNumber == 1000)
                _frameNumber = 0;

            // time text
            string msText = string.Format("T{0}", DateTime.Now.Millisecond.ToString("0000"));
            Vector2 msTextCenter = (_bigFont.MeasureString(msText) / 2) - centerSplit ;

            // fps text
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fpsText = string.Format("FPS: {0:0.00}", Clamp(_frameCounter.AverageFramesPerSecond, 0, 999));

            // Draw the frame string
            _spriteBatch.DrawString(spriteFont: _bigFont,
                                    text: frameText,
                                    position: windowCenter,
                                    color: Color.DarkBlue,
                                    rotation: 0,
                                    origin: frameTextCenter,
                                    scale: 4.0f,
                                    effects: SpriteEffects.None,
                                    layerDepth: 0.5f);

            // Draw the time string
            _spriteBatch.DrawString(spriteFont: _bigFont,
                              text: msText,
                              position: windowCenter,
                              color: Color.DarkBlue,
                              rotation: 0,
                              origin: msTextCenter,
                              scale: 4.0f,
                              effects: SpriteEffects.None,
                              layerDepth: 0.5f);

            // Draw the FPS string
            _spriteBatch.DrawString(spriteFont: _miniFont,
                              text: fpsText,
                              position: Vector2.Zero,
                              color: Color.Red,
                              rotation: 0,
                              origin: Vector2.Zero,
                              scale: 4.0f,
                              effects: SpriteEffects.None,
                              layerDepth: 0.5f);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
