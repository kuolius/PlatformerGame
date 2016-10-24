using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGame
{
    class ResolutionScreen:MenuScreen
    {
        MouseState prevMouseState;
        GraphicsDeviceManager graphics;
       

        public ResolutionScreen(ContentManager Content,GraphicsDevice graphicsDevice,GameWindow Window,GraphicsDeviceManager graphics):base(Content,graphicsDevice,Window,graphics)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            this.Window = Window;
            this.graphics = graphics;
           
        }

        public override void Initialize()
        {
            base.Initialize();
            initialize(prevMouseState);
          
        }

        public void initialize(MouseState prevMouseState)
        {
            state = State.resolutionScreen;
            this.prevMouseState = prevMouseState;
        }


        public override void LoadContent()
        {
            base.LoadContent();
            menuItems.Add(Content.Load<Texture2D>("Resolutions/800x600"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1024x768"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1152x864"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1280x720"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1280x768"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1280x800"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1280x1024"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1360x768"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1366x768"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1440x900"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/1600x1200"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/fullscreen"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/windowed"));
            menuItems.Add(Content.Load<Texture2D>("Resolutions/back"));
            
        }

        public override void Update(out State state, MainMenuScreen mainMenuScreen,ResolutionScreen resolutionScreen)
        {
            
            base.Update(out state,mainMenuScreen,resolutionScreen);
            
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released && mouseState.X > Window.ClientBounds.Width / 2 - 150 * scaleIndex && mouseState.X < Window.ClientBounds.Width / 2 + 150 * scaleIndex && mouseState.Y > Window.ClientBounds.Height / 2 - menuItems.Count * (50 * scaleIndex + menuSpace) / 2 + i * (50 * scaleIndex + menuSpace) && mouseState.Y < Window.ClientBounds.Height / 2 - menuItems.Count * (50 * scaleIndex + menuSpace) / 2 + i * (50 * scaleIndex + menuSpace) + 50 * scaleIndex)
                {
                    switch (i)
                    {
                        case 0:
                            graphics.PreferredBackBufferWidth = 800;
                            graphics.PreferredBackBufferHeight = 600;
                            

                            break;
                        case 1:
                            graphics.PreferredBackBufferWidth = 1024;
                            graphics.PreferredBackBufferHeight = 768;
                            
                            break;
                        case 2:
                            graphics.PreferredBackBufferWidth = 1152;
                            graphics.PreferredBackBufferHeight = 864;
                            
                            break;
                        case 3:
                            graphics.PreferredBackBufferWidth = 1280;
                            graphics.PreferredBackBufferHeight = 720;
                            
                            break;
                        case 4:
                            graphics.PreferredBackBufferWidth = 1280;
                            graphics.PreferredBackBufferHeight = 768;
                           
                            break;
                        case 5:
                            graphics.PreferredBackBufferWidth = 1280;
                            graphics.PreferredBackBufferHeight = 800;
                           
                            break;
                        case 6:
                            graphics.PreferredBackBufferWidth = 1280;
                            graphics.PreferredBackBufferHeight = 1024;
                         
                            break;
                        case 7:
                            graphics.PreferredBackBufferWidth = 1360;
                            graphics.PreferredBackBufferHeight = 768;
                           
                            break;
                        case 8:
                            graphics.PreferredBackBufferWidth = 1366;
                            graphics.PreferredBackBufferHeight = 768;
                            
                            break;
                        case 9:
                            graphics.PreferredBackBufferWidth = 1440;
                            graphics.PreferredBackBufferHeight = 900;
                            
                            break;
                        case 10:
                            graphics.PreferredBackBufferWidth = 1600;
                            graphics.PreferredBackBufferHeight = 1200;
                           
                            break;
                        case 11:
                            graphics.IsFullScreen = true;
                            graphics.ApplyChanges();
                            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                            graphics.ApplyChanges();
                            break;
                        case 12:
                            
                            graphics.IsFullScreen = false;
                           
                            break;
                        case 13:
                            state = State.mainMenuScreen;
                            mainMenuScreen.initialize(mouseState);
                            break;
                    }
                    if (i >= 0 && i < 13 && i!=11 )
                    {
                        graphics.IsFullScreen = true;
                        graphics.ApplyChanges();
                        int width = Window.ClientBounds.Width;
                        int height = Window.ClientBounds.Height;
                        graphics.IsFullScreen = false;
                        graphics.ApplyChanges();
                        Window.Position = new Point(width / 2 - Window.ClientBounds.Width / 2, height / 2 - Window.ClientBounds.Height / 2);
                    }
                }

            }
            prevMouseState = mouseState;

        }

    }
}
