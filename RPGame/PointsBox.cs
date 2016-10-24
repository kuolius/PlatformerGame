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
    class PointsBox
    {
        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;

        Texture2D mainBox,button1,pressedButton1,header, xButton;
        Point mainBoxSize;
        Vector2 mainBoxCoord;
        Vector2 mouseInBox;
        Points playerPoints;
        SpriteFont font;
        MouseState mouse,prevMouse;
        KeyboardState keyboard,prevKeyboard;

        int leftSpace,rightSpace,upSpace;
        int pressed,clicked;
        float prevHeight;
        public bool display,isActive,isActivated, inBox;
        bool drag, block;
        float transp;

        public PointsBox(ContentManager Content,GraphicsDevice graphics,GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
        }

        public void Initialize()
        {
            mainBoxSize = new Point(300, 300);
            mainBoxCoord = new Vector2(Window.ClientBounds.Width - mainBoxSize.X,Window.ClientBounds.Height/2-mainBoxSize.Y/2);
            leftSpace = 20;
            rightSpace = 10;
            upSpace = 20;
            isActive = false;
            isActivated = false;

            drag = false;
            block = false;
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics);
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            header = Content.Load<Texture2D>("header");
            button1 = Content.Load<Texture2D>("Game/button+1");
            pressedButton1 = Content.Load<Texture2D>("Game/pressedButton+1");
            font = Content.Load<SpriteFont>("Game/statFont");
            xButton = Content.Load<Texture2D>("x");


        }

        public void Update(Points playerPoints,Stats playerStats,out Stats playerStatsOut,out Points playerPointsOut,out bool block,bool inBoxIn,bool isFocus)
        {
            this.playerPoints = playerPoints;
            mouse = Mouse.GetState();
            if(!isFocus)
                keyboard = Keyboard.GetState();
            pressed = -1;
            clicked = -1;

            if(display)
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



            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > mainBoxCoord.X && mouse.X < mainBoxCoord.X + mainBoxSize.X && mouse.Y > mainBoxCoord.Y && mouse.Y < mainBoxCoord.Y + 20 && display &&  isActive)
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

            if (prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U))
                if (display)
                {
                    display = false;
                    isActive=false;
                }
                else
                {
                    display = true;
                    isActive = true;
                }
            else if (prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y) || prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I)|| prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
                isActive = false;

            if (playerPoints.points > 0 && display && isActive)
            {
                prevHeight = mainBoxCoord.Y + 45+ upSpace;
                for (int i = 0; i < 11; i++)
                {
                    if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed && mouse.X > mainBoxCoord.X + mainBoxSize.X - rightSpace - button1.Width && mouse.X < mainBoxCoord.X + mainBoxSize.X - rightSpace && mouse.Y > prevHeight + 15 * i - button1.Height / 2 && mouse.Y < prevHeight + 15 * i - button1.Height / 2 + button1.Height)
                        pressed = i;

                    if (prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released && mouse.X > mainBoxCoord.X + mainBoxSize.X - rightSpace - button1.Width && mouse.X < mainBoxCoord.X + mainBoxSize.X - rightSpace && mouse.Y > prevHeight + 15 * i - button1.Height / 2 && mouse.Y < prevHeight + 15 * i - button1.Height / 2 + button1.Height)
                        clicked = i;

                }

                switch (clicked)
                {
                    case 0:
                        playerPoints.pstr += 1;
                        playerStats.physPow += 10;
                        break;
                    case 1:
                        playerPoints.mstr += 1;
                        playerStats.mPow += 10;
                        break;
                    case 2:
                        playerPoints.pdef += 1;
                        playerStats.physDef += 10;
                        break;
                    case 3:
                        playerPoints.mdef += 1;
                        playerStats.mDef += 10;
                        break;
                    case 4:
                        playerPoints.pas += 1;
                        playerStats.attSpeed += 1;
                        break;
                    case 5:
                        playerPoints.mas += 1;
                        playerStats.castSpeed += 1;
                        break;
                    case 6:
                        playerPoints.hp += 1;
                        playerStats.maxHp += 100;
                        break;
                    case 7:
                        playerPoints.mp += 1;
                        playerStats.maxMana += 100;
                        break;
                    case 8:
                        playerPoints.pc += 1;
                        playerStats.physCrits += 1;
                        break;
                    case 9:
                        playerPoints.mc += 1;
                        playerStats.mCrits += 1;
                        break;
                    case 10:
                        playerPoints.eva += 1;
                        playerStats.eva += 1;
                        break;
                }
                if (clicked != -1)
                    playerPoints.points--;

            }

            if (isActive)
                transp = 1;
            else
                transp = .5f;


            if (display && isActive && keyboard.IsKeyDown(Keys.Escape))
            {
                display = false;
                isActive = false;
            }

            playerPointsOut = playerPoints;
            playerStatsOut = playerStats;
            prevMouse = mouse;
            prevKeyboard = keyboard;
            block = this.block;
        }

        public void Draw()
        {
            spriteBatch.Begin();
            if (display)
            {
                spriteBatch.Draw(mainBox, destinationRectangle:new Rectangle((int)mainBoxCoord.X,(int)mainBoxCoord.Y,mainBoxSize.X,mainBoxSize.Y), color:Color.White * transp);
                spriteBatch.Draw(header, mainBoxCoord,color: Color.White * transp);
                spriteBatch.Draw(xButton, new Vector2(mainBoxCoord.X + 280, mainBoxCoord.Y + 2), color:Color.White * transp);
                spriteBatch.DrawString(font, "Stats", new Vector2(mainBoxCoord.X + mainBoxSize.X / 2 - font.MeasureString("Stats").X / 2, mainBoxCoord.Y + 5+ upSpace), Color.Black * transp);
                if (playerPoints.points != 0)
                    spriteBatch.DrawString(font, "You have " + playerPoints.points + " points.", new Vector2(mainBoxCoord.X + mainBoxSize.X / 2 - font.MeasureString("You have " + playerPoints.points + " points.").X / 2 * .6f, mainBoxCoord.Y + 25+ upSpace), Color.Black * transp,0,Vector2.Zero,.7f,SpriteEffects.None,0);
                prevHeight = mainBoxCoord.Y + 25+ upSpace;
                spriteBatch.DrawString(font, "Physical Power: " + playerPoints.pstr, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 20 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 20;
                spriteBatch.DrawString(font, "Magical Power: " + playerPoints.mstr, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Physical Defense: " + playerPoints.pdef, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Magical Defense: " + playerPoints.mdef, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Attack Speed: " + playerPoints.pas, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Casting Speed: " + playerPoints.mas, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Health Points: " + playerPoints.hp, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Magic Points: " + playerPoints.mp, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Physical Critical: " + playerPoints.pc, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Magical Critical: " + playerPoints.mc, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);
                prevHeight += 15;
                spriteBatch.DrawString(font, "Evasion: " + playerPoints.eva, new Vector2(mainBoxCoord.X + leftSpace, prevHeight + 15 - font.MeasureString("T").Y / 2), Color.Black * transp);

                prevHeight = mainBoxCoord.Y + 45+ upSpace;

                if (playerPoints.points > 0)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        if (i != pressed)
                            spriteBatch.Draw(button1, new Vector2(mainBoxCoord.X + mainBoxSize.X - rightSpace - button1.Width, prevHeight + 15 * i - button1.Height / 2),Color.White * transp);
                        else
                            spriteBatch.Draw(pressedButton1, new Vector2(mainBoxCoord.X + mainBoxSize.X - rightSpace - button1.Width, prevHeight + 15 * i - button1.Height / 2),Color.White * transp);
                    }
                }

            }
            spriteBatch.End();
        }
    }
}
