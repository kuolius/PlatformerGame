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
    class Cursor
    {
        ContentManager Content;
        GraphicsDevice graphics;
        MouseState mouse;
        Texture2D cursorTexture;
        SpriteBatch spriteBatch;

        public Cursor(ContentManager Content,GraphicsDevice graphics)
        {
            this.Content = Content;
            this.graphics = graphics;
        }
    
        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
        }

        public void LoadContent()
        {
            cursorTexture = Content.Load<Texture2D>("Game/cursor");
        }

        public void Update()
        {
            mouse = Mouse.GetState();
        }

        public void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(cursorTexture, new Vector2(mouse.X, mouse.Y), Color.White);
            spriteBatch.End();
        }
    }
}
