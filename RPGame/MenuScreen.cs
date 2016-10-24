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
    class MenuScreen
    {
        public ContentManager Content;
        public GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        public GameWindow Window;
        Cursor cursor;

        public List<Texture2D> menuItems;
        public int menuSpace;
        int magnifyIndex;
        public MouseState mouseState;
        
        public State state;
        public float scaleIndex;
        

        public MenuScreen(ContentManager Content,GraphicsDevice graphicsDevice,GameWindow Window,GraphicsDeviceManager graphics)
        {
            

        }

        public virtual void Initialize()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            menuSpace = 10;
            menuItems = new List<Texture2D>();
            magnifyIndex = -1;
            scaleIndex = .65f;
            cursor = new Cursor(Content, graphicsDevice);
            cursor.Initialize();

        }

        public virtual void LoadContent()
        {

            cursor.LoadContent();
        }

        public virtual void Update(out State state,MainMenuScreen mainMenuScreen,ResolutionScreen resolutionScreen)
        {
            mouseState = Mouse.GetState();
            magnifyIndex = -1;

            for(int i=0;i<menuItems.Count;i++)
            {
                if (mouseState.X > Window.ClientBounds.Width / 2 - (float)(150*scaleIndex) && mouseState.X < Window.ClientBounds.Width / 2 + 150*scaleIndex && mouseState.Y > Window.ClientBounds.Height / 2 - menuItems.Count * (50*scaleIndex + menuSpace) / 2 + i * (50*scaleIndex + menuSpace) && mouseState.Y < Window.ClientBounds.Height / 2 - menuItems.Count * (50*scaleIndex + menuSpace) / 2 + i * (50*scaleIndex + menuSpace) + 50*scaleIndex)
                    magnifyIndex = i;

            }

            state = this.state;

            cursor.Update();
        }

        public void Draw()
        {
            graphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            for(int i=0;i<menuItems.Count; i++)
            {
                if(i==magnifyIndex)
                    spriteBatch.Draw(menuItems[i], new Vector2(graphicsDevice.Viewport.Width / 2 - 150, graphicsDevice.Viewport.Height / 2 - menuItems.Count * (float)(50*scaleIndex + menuSpace) / 2 + i * (float)(50*scaleIndex + menuSpace)-(float)(50*.1/2)),new Rectangle(0,0,300,50), Color.White,0,Vector2.Zero,1f,SpriteEffects.None,1);
                else
                    spriteBatch.Draw(menuItems[i], new Vector2(graphicsDevice.Viewport.Width / 2 - (float)(150*scaleIndex), graphicsDevice.Viewport.Height / 2 - menuItems.Count * (float)((50*scaleIndex + menuSpace)) / 2 + i * (float)((50*scaleIndex + menuSpace))), new Rectangle(0, 0, 300, 50), Color.White, 0, Vector2.Zero, scaleIndex, SpriteEffects.None, 1);
            }

            spriteBatch.End();
            cursor.Draw();
        }
    }
}
