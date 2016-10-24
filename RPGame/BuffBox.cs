using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGame
{
    class BuffBox
    {
        ContentManager Content;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        GameWindow Window;


        Texture2D header,mainBox;
        List<Buff> buffs=new List<Buff>();
        List<string> buffInfo = new List<string>();
        Skills skills;

        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;
        Vector2 boxCoord;
        Vector2 mouseInBox;
        SpriteFont buffInfoFont;
        Point boxSize;
        int hpBarY;
        bool drag, block;
        public bool inBox, isActive, displayCursor;
        float transp;


        public BuffBox(ContentManager Content, GraphicsDevice graphics,GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
            
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            boxCoord = new Vector2(200, 0);

            boxSize = new Point(200, 60);
           
            

            drag = false;
            block = false;
            inBox = false;
            isActive = true;

        }

        public void LoadContent()
        {
            header = Content.Load<Texture2D>("header");

            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();

            buffInfoFont = Content.Load<SpriteFont>("Game/chatFont");
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");

        }

        public void Update(List<Buff> buffs, out bool block, bool inBoxIn)
        {

            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();
            this.buffs = buffs;

            buffInfo.Clear();
            displayCursor = true;

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

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && mouse.X > boxCoord.X && mouse.X < boxCoord.X + 15 && mouse.Y > boxCoord.Y && mouse.Y < boxCoord.Y + boxSize.Y && isActive)
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

            if (drag)
            {
                boxCoord = new Vector2(mouse.X - mouseInBox.X, mouse.Y - mouseInBox.Y);
            }


            for (int i = 0; i < buffs.Count; i++)
            {
                if(mouse.X> (int)(boxCoord.X + 15 + i % 10 * 21) && mouse.X< (int)(boxCoord.X + 15 + i % 10 * 21+21) && mouse.Y> (int)(boxCoord.Y + i / 10 * 21) && mouse.Y< (int)(boxCoord.Y + i / 10 * 21)+21)
                {
                    displayCursor = false;
                    buffInfo.Add(skills.skillList[buffs[i].buffId].name);
                    buffInfo.Add((buffs[i].time - buffs[i].cTime) / 60 + "s");
                }
            }



            if (prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U) || prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I) || prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y) || prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
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
            spriteBatch.Draw(header, destinationRectangle: new Rectangle((int)boxCoord.X+15, (int)boxCoord.Y, boxSize.Y, 15), color: Color.White * transp, rotation: (float)Math.PI / 2);
            for (int i=0;i<buffs.Count;i++)
            {
               
                spriteBatch.Draw(Content.Load<Texture2D>("Skills/" + skills.skillList[buffs[i].buffId].name), destinationRectangle: new Rectangle((int)(boxCoord.X+15 + i % 10 * 21), (int)(boxCoord.Y + i / 10 * 21), 20, 20), color: Color.White*transp);
            }


            if (buffInfo.Count > 0)
            {
                if (mouse.X + 100 <= Window.ClientBounds.Width)
                {
                    spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X, mouse.Y, 100, buffInfo.Count * 16), color: Color.White * transp);
                    for (int i = 0; i < buffInfo.Count; i++)
                        spriteBatch.DrawString(buffInfoFont, buffInfo[i], new Vector2(mouse.X + 2, mouse.Y + 2 + i * 16), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X - 100, mouse.Y, 100, buffInfo.Count * 16), color: Color.White * transp);
                    for (int i = 0; i < buffInfo.Count; i++)
                        spriteBatch.DrawString(buffInfoFont, buffInfo[i], new Vector2(mouse.X + 2 - 100, mouse.Y + 2 + i * 16), Color.Black);
                }
            }

            spriteBatch.End();
        }
    }
}
