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
    class StatsBox
    {
        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;

        Texture2D mainBox,header, xButton;
        Vector2 mainBoxCoord, mouseInBox;
        SpriteFont font;
        Stats playerStats;

        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;

        public bool display,isActive,isActivated, inBox;
        bool drag, block;
        float transp;

        public StatsBox(ContentManager Content, GraphicsDevice graphics, GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
        }

        public void Initialize()
        {
            display = false;
            drag = false;
            block = false;
            isActive = false;
            isActivated = false;
            mainBoxCoord = new Vector2(Window.ClientBounds.Width - 300, Window.ClientBounds.Height / 2 - 150);
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics);
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            font = Content.Load<SpriteFont>("Game/statFont");
            header = Content.Load<Texture2D>("header");
            xButton = Content.Load<Texture2D>("x");
        }

        public void Update(Stats playerStats,out bool block,bool inBoxIn,bool isFocus)
        {
            mouse = Mouse.GetState();
            if(!isFocus)
                keyboard = Keyboard.GetState();
            this.playerStats = playerStats;



            if (display) 
            if (mouse.X > mainBoxCoord.X && mouse.X < mainBoxCoord.X + 300 && mouse.Y > mainBoxCoord.Y && mouse.Y < mainBoxCoord.Y + 300)
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


            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > mainBoxCoord.X && mouse.X < mainBoxCoord.X + 300 && mouse.Y > mainBoxCoord.Y && mouse.Y < mainBoxCoord.Y + 20 && display &&  isActive)
            {
                drag = true;
                this.block = true;
                if (prevMouse.LeftButton == ButtonState.Released)
                {
                    mouseInBox.X = mouse.X - mainBoxCoord.X;
                    mouseInBox.Y = mouse.Y - mainBoxCoord.Y;
                }
            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                drag = false;
                this.block = false;
            }

            if (drag)
            {
                mainBoxCoord = new Vector2(mouse.X - mouseInBox.X, mouse.Y - mouseInBox.Y);
                
            }

            if (mouse.X > mainBoxCoord.X + 280 && mouse.X < mainBoxCoord.X + 295 && mouse.Y > mainBoxCoord.Y + 2 && mouse.Y < mainBoxCoord.Y + 17 && prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released && isActive)
            {
                display = false;
                isActive = false;
            }

            if (prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y))
                if (display)
                {
                    display = false;
                    isActive = false;
                }
                else
                {
                    display = true;
                    isActive = true;
                    
                }
            else if (prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U) || prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I)|| prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
                isActive = false;

            if (isActive)
                transp = 1;
            else
                transp = .5f;


            if (display && isActive && keyboard.IsKeyDown(Keys.Escape))
            {
                display = false;
                isActive = false;
            }

            prevMouse = mouse;
            prevKeyboard = keyboard;
            block = this.block;
        }

        public void Draw()
        {

            spriteBatch.Begin();
            if (display)
            {
                spriteBatch.Draw(mainBox, color: Color.White * transp, destinationRectangle: new Rectangle((int)mainBoxCoord.X, (int)mainBoxCoord.Y, 300, 300));
                spriteBatch.Draw(header, mainBoxCoord,color: Color.White * transp);
                spriteBatch.Draw(xButton, new Vector2(mainBoxCoord.X + 280, mainBoxCoord.Y + 2), color:Color.White * transp);
                spriteBatch.DrawString(font, "Stats", new Vector2(mainBoxCoord.X + 150 - font.MeasureString("Stats").X / 2, mainBoxCoord.Y + 20), Color.Black * transp);

                string str = "Physical Power: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40), Color.Black * transp);
                str = Convert.ToString(playerStats.physPow);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y +40 ), Color.Black * transp);

                str = "Physical Defense: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40+15), Color.Black * transp);
                str = Convert.ToString(playerStats.physDef);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 15), Color.Black * transp);

                str = "Attack Speed: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 30), Color.Black * transp);
                str = Convert.ToString(playerStats.attSpeed);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 30), Color.Black * transp);


                str = "Physical Critical: " ;
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 45), Color.Black * transp);
                str = Convert.ToString(playerStats.physCrits + "%");
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 45), Color.Black * transp);

                str = "Evasion: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 60), Color.Black * transp);
                str = Convert.ToString(playerStats.eva + "%");
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 60), Color.Black * transp);

                 str = "Magical Power: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40+75), Color.Black * transp);
                str = Convert.ToString(playerStats.mPow);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40+75), Color.Black * transp);

                str = "Magical Defense: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 90), Color.Black * transp);
                str = Convert.ToString(playerStats.mDef);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 90), Color.Black * transp);

                str = "Casting Speed: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 105), Color.Black * transp);
                str = Convert.ToString(playerStats.castSpeed);
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 105), Color.Black * transp);


                str = "Magical Critical: ";
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 40 + 120), Color.Black * transp);
                str = Convert.ToString(playerStats.mCrits + "%");
                spriteBatch.DrawString(font, str, new Vector2(mainBoxCoord.X + 20 + 250 - font.MeasureString(str).X, mainBoxCoord.Y + 40 + 120), Color.Black * transp);

            }
            spriteBatch.End();
            
        }
    }
}
