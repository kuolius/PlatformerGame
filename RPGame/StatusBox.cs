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
    class StatusBox
    {
        ContentManager Content;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        Stats playerStats;

        Texture2D mainBox,hpBar,xpBar,manaBar,header;
        SpriteFont statsFont;

        MouseState mouse,prevMouse;
        KeyboardState keyboard, prevKeyboard;
        Vector2 boxCoord,hpBarCoord;
        Vector2 mouseInBox;
        Point boxSize, hpBarSize,xpBarSize,manaBarSize;
        int hpBarY;
        bool drag,block;
        public bool inBox, isActive;
        float transp;
        

        public StatusBox(ContentManager Content,GraphicsDevice graphics,Stats playerStats)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.playerStats = playerStats;
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            boxCoord = new Vector2(0, 0);

            boxSize = new Point(200, 100);
            hpBarSize = new Point(180, 15);
            xpBarSize = new Point(180, 15);
            manaBarSize = new Point(180, 15);
            hpBarY = 40;

            drag = false;
            block = false;
            inBox = false;
            isActive = true;
            
        }

        public void LoadContent()
        {
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            header = Content.Load<Texture2D>("header");
            hpBar = new Texture2D(graphics, hpBarSize.X, hpBarSize.Y);
            xpBar = new Texture2D(graphics, xpBarSize.X, xpBarSize.Y);
            manaBar = new Texture2D(graphics, manaBarSize.X, manaBarSize.Y);

            statsFont = Content.Load<SpriteFont>("Game/statFont");


           
        }

        public void Update(Stats playerStats,out bool block,bool inBoxIn,bool isFocus)
        {
            
            mouse = Mouse.GetState();
            if (!isFocus)
            {
                keyboard = Keyboard.GetState();
            }
            this.playerStats = playerStats;

            

            
                if (mouse.X > boxCoord.X && mouse.X < boxCoord.X + boxSize.X && mouse.Y > boxCoord.Y && mouse.Y < boxCoord.Y + boxSize.Y)
                {
                    inBox = true;

                    if ((mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released || mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released) && !inBoxIn)
                    {
                        isActive = true;

                    }

                }
                else
                {
                    inBox = false;
                    if ((mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released || mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released))
                    {
                        isActive = false;

                    }
                }

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton==ButtonState.Released && mouse.X > boxCoord.X && mouse.X < boxCoord.X + boxSize.X && mouse.Y > boxCoord.Y && mouse.Y < boxCoord.Y + 20 && isActive)
            {
                drag = true;
                this.block = true;
                if (prevMouse.LeftButton == ButtonState.Released)
                {
                    mouseInBox.X = mouse.X - boxCoord.X;
                    mouseInBox.Y = mouse.Y - boxCoord.Y;
                }
            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                drag = false;
                this.block = false;
            }

            if(drag)
            {
                boxCoord = new Vector2(mouse.X-mouseInBox.X, mouse.Y-mouseInBox.Y);
            }

            hpBarCoord = new Vector2(boxCoord.X + boxSize.X/2-hpBarSize.X/2, boxCoord.Y + hpBarY);

            Color[] data = new Color[hpBarSize.X*hpBarSize.Y];

            for(int i=0;i<data.Length;i++)
            {
                if (i % hpBarSize.X < (hpBarSize.X * playerStats.hp) / playerStats.maxHp)
                    data[i] = Color.Green;
                else
                    data[i] = Color.Red;
            }
            hpBar.SetData(data);

            data = new Color[xpBarSize.X * xpBarSize.Y];

            for(int i=0;i<data.Length;i++)
            {
                if (i % xpBarSize.X < (xpBarSize.X * playerStats.xp) / playerStats.maxXp)
                    data[i] = Color.Yellow;
                else
                    data[i] = Color.White;

            }

            xpBar.SetData(data);

            data = new Color[manaBarSize.X * manaBarSize.Y];

            for (int i = 0; i < data.Length; i++)
            {
                if (i % manaBarSize.X < (manaBarSize.X * playerStats.mana) / playerStats.maxMana)
                    data[i] = Color.Blue;
                else
                    data[i] = Color.White;

            }

            manaBar.SetData(data);

            if (prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U) || prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I) || prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y)|| prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
                isActive = false;

            if (isActive)
                transp = 1;
            else
                transp = .9f;


            

            block = this.block;
            prevMouse = mouse;
            prevKeyboard = keyboard;
        }

        public void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mainBox,destinationRectangle: new Rectangle((int)boxCoord.X,(int)boxCoord.Y,boxSize.X,boxSize.Y), color:Color.White* transp);
            spriteBatch.Draw(header, destinationRectangle: new Rectangle((int)boxCoord.X, (int)boxCoord.Y, boxSize.X, 20), color: Color.White * .7f);
            spriteBatch.Draw(hpBar, position: hpBarCoord, color: Color.White * transp);

            string str = playerStats.hp + "/" + playerStats.maxHp + " HP";
            spriteBatch.DrawString(statsFont,str , new Vector2(hpBarCoord.X + hpBarSize.X/2 - statsFont.MeasureString(str).X/2, hpBarCoord.Y +hpBarSize.Y/2 - statsFont.MeasureString(str).Y/2),Color.Black* transp);

            str = "LvL  " + playerStats.lvl;
            spriteBatch.DrawString(statsFont, str, new Vector2(boxCoord.X + boxSize.X / 2 - statsFont.MeasureString(str).X / 2, hpBarCoord.Y- statsFont.MeasureString(str).Y),Color.Black* transp);

            spriteBatch.Draw(xpBar, new Vector2(boxCoord.X + boxSize.X / 2 - xpBarSize.X / 2,  hpBarCoord.Y+hpBarSize.Y + 5), Color.White * transp);

            //str = playerStats.xp + "/" + playerStats.maxXp + " XP";
            //spriteBatch.DrawString(statsFont, str, new Vector2(boxCoord.X + boxSize.X / 2 - statsFont.MeasureString(str).X / 2, hpBarCoord.Y + hpBarSize.Y + 5 + xpBarSize.Y / 2 - statsFont.MeasureString(str).Y / 2), Color.Black);

            spriteBatch.Draw(manaBar, new Vector2(boxCoord.X + boxSize.X / 2 - manaBarSize.X / 2, hpBarCoord.Y + hpBarSize.Y + 5+xpBarSize.Y+5), Color.White * transp);

            str = playerStats.mana + "/" + playerStats.maxMana + " MP";
            spriteBatch.DrawString(statsFont, str, new Vector2(boxCoord.X + boxSize.X / 2 - statsFont.MeasureString(str).X / 2, hpBarCoord.Y + hpBarSize.Y + 5 + xpBarSize.Y +5+manaBarSize.Y/2 - statsFont.MeasureString(str).Y / 2), Color.Black* transp);


            spriteBatch.End();
        }

    }
}
