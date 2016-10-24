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
    class MainMenuScreen:MenuScreen
    {
        MouseState prevMouseState;
        GameScreen gameScreen;
        

        public MainMenuScreen(ContentManager Content,GraphicsDevice graphicsDevice,GameWindow Window,GraphicsDeviceManager graphics) :base(Content,graphicsDevice,Window,graphics)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            this.Window=Window;
            
        }

        public override void Initialize()
        {
            base.Initialize();
            initialize(prevMouseState);
           
           
        }

        public void initialize(MouseState prevMouseState)
        {
            state = State.mainMenuScreen;
            this.prevMouseState = prevMouseState;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            menuItems.Add(Content.Load<Texture2D>("startGame"));
            menuItems.Add(Content.Load<Texture2D>("Resolution"));
            menuItems.Add(Content.Load<Texture2D>("exit"));
            
        }

        public void getGameScreen(GameScreen gameScreen)
        {
            this.gameScreen = gameScreen;

        }


        public override void Update(out State state,MainMenuScreen mainMenuScreen, ResolutionScreen resolutionScreen)
        {
            
            base.Update(out state,mainMenuScreen, resolutionScreen);

            for (int i = 0; i < menuItems.Count; i++)
            {
                if (mouseState.LeftButton==ButtonState.Pressed && prevMouseState.LeftButton==ButtonState.Released &&  mouseState.X > Window.ClientBounds.Width / 2 - 150*scaleIndex && mouseState.X < Window.ClientBounds.Width / 2 + 150*scaleIndex && mouseState.Y > Window.ClientBounds.Height / 2 - menuItems.Count * (50*scaleIndex + menuSpace) / 2 + i * (50*scaleIndex + menuSpace) && mouseState.Y < Window.ClientBounds.Height / 2 - menuItems.Count * (50*scaleIndex + menuSpace) / 2 + i * (50*scaleIndex + menuSpace) + 50*scaleIndex)
                    switch (i)
                    {
                        case 0:
                            this.state = State.gameScreen;
                            gameScreen.Initialize();
                            gameScreen.LoadContent();
                            break;
                        case 1:
                            this.state = State.resolutionScreen;
                            resolutionScreen.initialize(mouseState);
                            break;
                        case 2:
                            this.state = State.exit;
                            break;

                    }


            }
            prevMouseState = mouseState;



        }



    }
}
