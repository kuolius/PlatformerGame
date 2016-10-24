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
    class FlashScreen
    {
        ContentManager Content;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;
        GameWindow Window;

        Texture2D logo;
        State state;
        Color color;
        int colorIndex;
        bool fadeOut;
        bool fadeIn;
        KeyboardState keyboardState,prevKeyboardState;

        public FlashScreen(ContentManager Content,GraphicsDevice graphicsDevice,GameWindow Window)
        {
            this.Content = Content;
            
            this.graphicsDevice = graphicsDevice;
            this.Window = Window;
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            state = State.flashScreen;
            color = new Color(255, 255, 255);
            colorIndex = 0;
            fadeOut = true;
            fadeIn = false;
        }

        public void LoadContent()
        {
            logo = Content.Load<Texture2D>("logo");
        }

        public void Update(out State state)
        {
            keyboardState = Keyboard.GetState();
            color = new Color(colorIndex, colorIndex, colorIndex);

            if(keyboardState!=prevKeyboardState && prevKeyboardState!=null && fadeOut)
            {
                fadeOut = false;
                fadeIn = true;
                colorIndex = 100;
            }

            if (fadeOut)
            {
                if (colorIndex == 255)
                {
                    fadeIn = true;
                    fadeOut = false;
                }
                colorIndex++;
            }
            if (fadeIn)
            {
                if(colorIndex==0)
                {
                    this.state = State.mainMenuScreen;
                }
                colorIndex--;
            }

            state = this.state;
            prevKeyboardState = keyboardState;
        }

        public void Draw()
        {
            graphicsDevice.Clear(color);
            spriteBatch.Begin();
            spriteBatch.Draw(logo, new Rectangle(new Point(Window.ClientBounds.Width / 2 - 100, Window.ClientBounds.Height / 2 -100),new Point(200,200)), color);
            spriteBatch.End();
        }
    }
}
