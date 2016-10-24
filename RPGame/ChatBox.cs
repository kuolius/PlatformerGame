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

    class ChatContent
    {
        public string text;
        public Color color;

        public ChatContent(string text,Color color)
        {
            this.text = text;
            this.color = color;
        }
    }

    class ChatBox
    {


        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;
        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;

        Texture2D mainBox, chat, separator,focusPointer;
        SpriteFont chatFont;
        Point mainBoxSize, scrollerCoord, chatCoord,chatSize;
        float transp;
        Texture2D arrow, scroller, path;
        int scrollerWidth, chatRows, scrollerUpBound, padding;
        bool scroll, block;
        public bool isFocus,firstTime;
        int blinkFps;
        bool display;
        DateTime localTime = DateTime.Now;
        int pointCoordX;
        bool startedWriting;

        List<ChatContent> chatContent = new List<ChatContent>();
        
        string cText;
        string give,spawn;


        public ChatBox(ContentManager Content, GraphicsDevice graphics, GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            mainBoxSize = new Point(400, 250);
            
            focusPointer = new Texture2D(graphics, 1, 13);
            chatCoord = new Point(5, Window.ClientBounds.Height - mainBoxSize.Y + 5);
            chatSize = new Point(375, 220);
            scrollerCoord= new Point(chatCoord.X + chatSize.X, chatCoord.Y +chatSize.Y-15);
            scrollerUpBound = chatCoord.Y + 15;
            separator = new Texture2D(graphics, mainBoxSize.X-10, 1);
            padding =5;
            cText = "";
            blinkFps = 0;
            pointCoordX = 0;
            

            Color[] data = new Color[separator.Width];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Black;
            separator.SetData(data);

            data = new Color[13];
            for (int i = 0; i < 13; i++)
                data[i] = Color.Black;
            focusPointer.SetData(data);

            transp = .5f;
        }

        public void LoadContent()
        {
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            chat= Content.Load<Texture2D>("Game/chat");
            arrow = Content.Load<Texture2D>("scroller/arrow");
            scroller = Content.Load<Texture2D>("scroller/scroller");
            path = Content.Load<Texture2D>("scroller/path");
            chatFont = Content.Load<SpriteFont>("Game/chatFont");
        }


        public void WriteText(string text,Color color)
        {
            if (text.Length <= (chatSize.X-48)/8)
                chatContent.Add(new ChatContent(localTime.Hour + ":" + localTime.Minute + " " + text, color));
            else
            {
                chatContent.Add(new ChatContent(localTime.Hour + ":" + localTime.Minute + " " + text.Substring(0, (chatSize.X - 48) / 8), color));
                chatContent.Add(new ChatContent(text.Substring((chatSize.X - 48) / 8, text.Length - (chatSize.X - 48) / 8), color));
            }
            
        }

        public void Update(out bool blockKeyboard,out string giveOut,out string spawnOut)
        {
            mouse = Mouse.GetState();
            give = "";
            spawn = "";

            if(isFocus)
                keyboard = Keyboard.GetState();


            while (chatContent.Count > 100)
                chatContent.Remove(chatContent[0]);

            chatRows = chatContent.Count - (chatSize.Y - 5) / 15;
            
            if (chatRows < 0)
                chatRows = 0;

            scrollerWidth = chatSize.Y-30 -  chatRows;


            /*  while (scrollerCoord.Y - scrollerUpBound > 140)
              {
                  scrollerCoord.Y -= 1;
              }
              */

            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > 5 && mouse.X < 5 + 230 && mouse.Y > Window.ClientBounds.Height - 5 - 15 && mouse.Y < Window.ClientBounds.Height - 5)
            {
                isFocus = true;
            }
            else if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
                isFocus = false;
    

            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > scrollerCoord.X && mouse.X < scrollerCoord.X + 15 && mouse.Y > scrollerCoord.Y-scrollerWidth && mouse.Y < scrollerCoord.Y-scrollerWidth + scrollerWidth)
            {
                scroll = true;
                block = true;

            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                scroll = false;
                block = false;
            }

            if (scroll && scrollerCoord.Y-scrollerWidth <= chatCoord.Y + 15 + 1 * chatRows && mouse.Y - prevMouse.Y > 0 && ((mouse.Y - prevMouse.Y) / 2)  <= chatRows - scrollerCoord.Y+scrollerWidth + scrollerUpBound)
            {

                scrollerCoord.Y += ((mouse.Y - prevMouse.Y) / 2) ;
            }
            else if (scroll && scrollerCoord.Y-scrollerWidth > chatCoord.Y + 15 && mouse.Y - prevMouse.Y < 0 && ((prevMouse.Y - mouse.Y) / 2)  <= scrollerCoord.Y-scrollerWidth - scrollerUpBound)
            {
                scrollerCoord.Y += ((mouse.Y - prevMouse.Y) / 2) ;
            }


            if (cText.Length <= 60)
            {
                for (int i = 0; i < 26; i++)
                {
                    char charakter = (char)(65 + i);
                    if (keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), charakter.ToString())) && prevKeyboard.IsKeyUp((Keys)Enum.Parse(typeof(Keys), charakter.ToString())))
                    {
                        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
                            cText = cText.Substring(0, pointCoordX) + charakter.ToString().ToUpper() + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                        else
                            cText = cText.Substring(0, pointCoordX) + charakter.ToString().ToLower() + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                        pointCoordX++;
                    }
                }

                for (int i = 0; i < 10; i++)
                {



                    if (keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), "D" + (i + 1) % 10)) && prevKeyboard.IsKeyUp((Keys)Enum.Parse(typeof(Keys), "D" + (i + 1) % 10)) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                    {
                        string temp = "";

                        switch (i)
                        {
                            case 9:
                                temp = ")";
                                break;
                            case 0:
                                temp = "!";
                                break;
                            case 1:
                                temp = "@";
                                break;
                            case 2:
                                temp = "#";
                                break;
                            case 3:
                                temp = "$";
                                break;
                            case 4:
                                temp = "%";
                                break;
                            case 5:
                                temp = "^";
                                break;
                            case 6:
                                temp = "&";
                                break;
                            case 7:
                                temp = "*";
                                break;
                            case 8:
                                temp = "(";
                                break;
                        }

                        cText = cText.Substring(0, pointCoordX) + temp + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                        if (temp != "")
                            pointCoordX++;

                    }
                    else if (keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), "D" + (i + 1) % 10)) && prevKeyboard.IsKeyUp((Keys)Enum.Parse(typeof(Keys), "D" + (i + 1) % 10)) || keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), "NumPad" + (i + 1) % 10)) && prevKeyboard.IsKeyUp((Keys)Enum.Parse(typeof(Keys), "NumPad" + (i + 1) % 10)))
                    {
                        cText = cText.Substring(0, pointCoordX) + (i + 1) % 10 + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                        pointCoordX++;
                    }
                }


                if (keyboard.IsKeyDown(Keys.Space) && prevKeyboard.IsKeyUp(Keys.Space))
                {
                    cText = cText.Substring(0, pointCoordX) + " " + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Divide) && prevKeyboard.IsKeyUp(Keys.Divide))
                {
                    cText = cText.Substring(0, pointCoordX) + "/" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Multiply) && prevKeyboard.IsKeyUp(Keys.Multiply))
                {
                    cText = cText.Substring(0, pointCoordX) + "*" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Subtract) && prevKeyboard.IsKeyUp(Keys.Subtract))
                {
                    cText = cText.Substring(0, pointCoordX) + "-" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Add) && prevKeyboard.IsKeyUp(Keys.Add))
                {
                    cText = cText.Substring(0, pointCoordX) + "+" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Decimal) && prevKeyboard.IsKeyUp(Keys.Decimal))
                {
                    cText = cText.Substring(0, pointCoordX) + "." + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemQuestion) && prevKeyboard.IsKeyUp(Keys.OemQuestion) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "?" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemQuestion) && prevKeyboard.IsKeyUp(Keys.OemQuestion))
                {
                    cText = cText.Substring(0, pointCoordX) + "/" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemCloseBrackets) && prevKeyboard.IsKeyUp(Keys.OemCloseBrackets) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "}" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemCloseBrackets) && prevKeyboard.IsKeyUp(Keys.OemCloseBrackets))
                {
                    cText = cText.Substring(0, pointCoordX) + "]" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemOpenBrackets) && prevKeyboard.IsKeyUp(Keys.OemOpenBrackets) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "{" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemOpenBrackets) && prevKeyboard.IsKeyUp(Keys.OemOpenBrackets))
                {
                    cText = cText.Substring(0, pointCoordX) + "[" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemPipe) && prevKeyboard.IsKeyUp(Keys.OemPipe) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "|" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemPipe) && prevKeyboard.IsKeyUp(Keys.OemPipe))
                {
                    cText = cText.Substring(0, pointCoordX) + "\\" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemComma) && prevKeyboard.IsKeyUp(Keys.OemComma) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "<" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemComma) && prevKeyboard.IsKeyUp(Keys.OemComma))
                {
                    cText = cText.Substring(0, pointCoordX) + "," + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemPeriod) && prevKeyboard.IsKeyUp(Keys.OemPeriod) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + ">" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemPeriod) && prevKeyboard.IsKeyUp(Keys.OemPeriod))
                {
                    cText = cText.Substring(0, pointCoordX) + "." + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemQuotes) && prevKeyboard.IsKeyUp(Keys.OemQuotes) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "\"" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemQuotes) && prevKeyboard.IsKeyUp(Keys.OemQuotes))
                {
                    cText = cText.Substring(0, pointCoordX) + "'" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemSemicolon) && prevKeyboard.IsKeyUp(Keys.OemSemicolon) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + ":" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemSemicolon) && prevKeyboard.IsKeyUp(Keys.OemSemicolon))
                {
                    cText = cText.Substring(0, pointCoordX) + ";" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemPlus) && prevKeyboard.IsKeyUp(Keys.OemPlus) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "+" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemPlus) && prevKeyboard.IsKeyUp(Keys.OemPlus))
                {
                    cText = cText.Substring(0, pointCoordX) + "=" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemTilde) && prevKeyboard.IsKeyUp(Keys.OemTilde) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "`" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemTilde) && prevKeyboard.IsKeyUp(Keys.OemTilde))
                {
                    cText = cText.Substring(0, pointCoordX) + "~" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.OemMinus) && prevKeyboard.IsKeyUp(Keys.OemMinus) && (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift)))
                {
                    cText = cText.Substring(0, pointCoordX) + "_" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }
                else if (keyboard.IsKeyDown(Keys.OemMinus) && prevKeyboard.IsKeyUp(Keys.OemMinus))
                {
                    cText = cText.Substring(0, pointCoordX) + "-" + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                    pointCoordX++;
                }

                if (keyboard.IsKeyDown(Keys.Back) && prevKeyboard.IsKeyUp(Keys.Back))
                {
                    if (cText.Length >= 1)
                    {
                        cText = cText.Substring(0, pointCoordX - 1) + cText.Substring(pointCoordX, cText.Length - pointCoordX);
                        pointCoordX--;
                    }

                }

            }

            if (isFocus)
            {
                if (keyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyUp(Keys.Enter))
                {

                    if (cText != "")
                    {
                        if (cText.Length > 5 && cText.Substring(0, 5) == "/give")
                            give = cText.Substring(6, cText.Length - 6);
                        else if (cText.Length > 6 && cText.Substring(0, 6) == "/spawn")
                            spawn = cText.Substring(7, cText.Length - 7);
                        else
                        {
                            if (cText.Length <= (chatSize.X - 48) / 8)
                                chatContent.Add(new ChatContent(localTime.Hour + ":" + localTime.Minute + " " + cText, Color.Black));
                            else
                            {
                                chatContent.Add(new ChatContent(localTime.Hour + ":" + localTime.Minute + " " + cText.Substring(0, (chatSize.X - 48) / 8), Color.Black));
                                chatContent.Add(new ChatContent(cText.Substring((chatSize.X - 48) / 8, cText.Length - (chatSize.X - 48) / 8), Color.Black));
                            }
                        }
                        cText = "";
                        pointCoordX = 0;
                        isFocus = false;
                    }
                    
                    

                }

                if (keyboard.IsKeyDown(Keys.Escape) && prevKeyboard.IsKeyUp(Keys.Escape))
                    isFocus = false;

            }

            if (keyboard.IsKeyDown(Keys.Right) && prevKeyboard.IsKeyUp(Keys.Right))
                if (pointCoordX < cText.Length)
                    pointCoordX++;
               
            if (keyboard.IsKeyDown(Keys.Left) && prevKeyboard.IsKeyUp(Keys.Left))
                if (pointCoordX > 0)
                    pointCoordX--;

           


            prevKeyboard = keyboard;
            prevMouse = mouse;
            blockKeyboard = block;
            giveOut = give;
            spawnOut = spawn;
        }

        public void Draw()
        {
            spriteBatch.Begin();
           
                spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(0, Window.ClientBounds.Height - mainBoxSize.Y, mainBoxSize.X, mainBoxSize.Y), color: Color.White * transp);
                spriteBatch.Draw(chat, destinationRectangle: new Rectangle(chatCoord.X, chatCoord.Y, chatSize.X, chatSize.Y), color: Color.White * transp);
                spriteBatch.Draw(separator, new Vector2(5, Window.ClientBounds.Height - 5 - 18), Color.White * transp);
            
                spriteBatch.Draw(chat, destinationRectangle: new Rectangle(5, Window.ClientBounds.Height - 5 - 15, mainBoxSize.X - 10, 15), color: Color.White * transp);

           
                spriteBatch.Draw(arrow, new Vector2(chatCoord.X + chatSize.X, chatCoord.Y), color: Color.White * transp);
                spriteBatch.Draw(path, destinationRectangle: new Rectangle(chatCoord.X + chatSize.X, chatCoord.Y + 15, 15, chatSize.Y - 30), color: Color.White * transp);
                spriteBatch.Draw(arrow, new Vector2(chatCoord.X + chatSize.X, chatCoord.Y + chatSize.Y - 15), color: Color.White * transp, effects: SpriteEffects.FlipVertically);
                spriteBatch.Draw(scroller, color: Color.White * transp, destinationRectangle: new Rectangle(scrollerCoord.X, scrollerCoord.Y - scrollerWidth, 15, scrollerWidth));

                if (chatContent.Count <= (chatSize.Y - 5) / 15)
                    for (int i = 0; i < chatContent.Count; i++)
                        spriteBatch.DrawString(chatFont, chatContent[i].text, new Vector2(padding + chatCoord.X, padding + chatCoord.Y + 15 * i), chatContent[i].color);
                else
                {

                    for (int i = 0; i < (chatSize.Y - 5) / 15; i++)
                        //  if (i + scrollerCoord.Y - scrollerWidth - scrollerUpBound < chatContent.Count) 
                        spriteBatch.DrawString(chatFont, chatContent[i + scrollerCoord.Y - scrollerWidth - scrollerUpBound].text, new Vector2(padding + chatCoord.X, padding + chatCoord.Y + 15 * i), chatContent[i + scrollerCoord.Y - scrollerWidth - scrollerUpBound].color);
                }

                if (cText.Length < (mainBoxSize.X - 10) / 8)
                    spriteBatch.DrawString(chatFont, cText, new Vector2(5 + padding, Window.ClientBounds.Height - 5 - 15), Color.Black);
                else
                {
                    if (pointCoordX - cText.Length + (mainBoxSize.X - 10) / 8 < 0)
                        spriteBatch.DrawString(chatFont, cText.Substring(cText.Length - (mainBoxSize.X - 10) / 8 + (pointCoordX - cText.Length + (mainBoxSize.X - 10) / 8), (mainBoxSize.X - 10) / 8), new Vector2(5 + padding, Window.ClientBounds.Height - 5 - 15), Color.Black);
                    else
                        spriteBatch.DrawString(chatFont, cText.Substring(cText.Length - (mainBoxSize.X - 10) / 8, (mainBoxSize.X - 10) / 8), new Vector2(5 + padding, Window.ClientBounds.Height - 5 - 15), Color.Black);

                }
            

            if (isFocus)
            {
                if (blinkFps >= 30)
                {
                    if (display)
                    {
                        
                        display = false;
                    }
                    else
                        display = true;
                    blinkFps = 0;
                }
                else
                    blinkFps++;

                if (display)
                {
                    if (cText.Length < (mainBoxSize.X - 10) / 8)
                        spriteBatch.Draw(focusPointer, new Vector2(5 + padding + pointCoordX * 8 + 1, Window.ClientBounds.Height - 5 - 15 + 1), Color.White);
                    else
                    {
                        if (pointCoordX * 8 - (cText.Length - (mainBoxSize.X - 10) / 8) * 8 < 0)
                            spriteBatch.Draw(focusPointer, new Vector2(5 + padding  + 1, Window.ClientBounds.Height - 5 - 15 + 1), Color.White);
                        else

                            spriteBatch.Draw(focusPointer, new Vector2(5 + padding + pointCoordX * 8 - (cText.Length - (mainBoxSize.X - 10) / 8) * 8 + 1, Window.ClientBounds.Height - 5 - 15 + 1), Color.White);

                    }
                }
            }
            spriteBatch.End();
        }
    }
}
