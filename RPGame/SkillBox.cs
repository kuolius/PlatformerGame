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

    class InventorySkill
    {
        public int skillId;
        public Texture2D reuseTexture;
        public int cReuse ;
        public bool startReuse;
        public int eqSkillId;

        public InventorySkill(int skillId,GraphicsDevice graphics)
        {
            this.skillId = skillId;
            cReuse = 0;
            startReuse = false;
            eqSkillId = -1;
           reuseTexture = new Texture2D(graphics, 30, 30);


        }

    }


    class SkillBox
    {


        ContentManager Content;
        SpriteBatch spriteBatch;
        GraphicsDevice graphics;
        GameWindow Window;

        SpriteFont skillInfoFont;
        Texture2D header,mainBox,xButton,defaultIcon;
        Texture2D arrow, scroller, path;

        List<Buff> buffs = new List<Buff>();
        public List<InventorySkill> inventorySkills = new List<InventorySkill>();
        List<string> skillInfo;
        Skills skills;

        Point barSize;
        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;
        Vector2 boxCoord;
        Vector2 mouseInBox,mouseInSkill;
        Vector2 inventoryCoord, scrollerCoord,barCoord;
        Point boxSize;
        int dragSkill;
        bool drag, block,scroll;
        public bool inBox, isActive,display;
        float transp;
        int inventoryRows;
        int scrollerWidth;
        int scrollerUpBound, scrollerDownBound;
        int inventorySkillId;
        public bool displayCursor;

        public SkillBox(ContentManager Content, GraphicsDevice graphics,GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;

        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            boxCoord = new Vector2(Window.ClientBounds.Width - 300, Window.ClientBounds.Height / 2 - 150);

            boxSize = new Point(300, 300);
            inventoryCoord = new Vector2(boxCoord.X + 20, boxCoord.Y + 30);
            scrollerCoord = new Vector2(inventoryCoord.X + 248, inventoryCoord.Y+15);
            scrollerUpBound = (int)inventoryCoord.Y + 15;
            scrollerDownBound = (int)inventoryCoord.Y + 15 + 154;
            barSize = new Point(319, 75);
            barCoord = new Vector2(Window.ClientBounds.Width  - barSize.X -20, Window.ClientBounds.Height - barSize.Y - 20);

            skillInfo = new List<string>();
          

            inventorySkills.Add(new InventorySkill(0,graphics));
            inventorySkills.Add(new InventorySkill(1, graphics));
            inventorySkills.Add(new InventorySkill(2, graphics));
            inventorySkills.Add(new InventorySkill(3, graphics));
            inventorySkills.Add(new InventorySkill(4, graphics));
            inventorySkills.Add(new InventorySkill(5, graphics));
            inventorySkills.Add(new InventorySkill(6, graphics));

            drag = false;
            block = false;
            inBox = false;
            isActive = true;
            display=false;
            scroll = false;
            dragSkill = -1;
        }

        public void LoadContent()
        {
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            header = Content.Load<Texture2D>("header");
            xButton = Content.Load<Texture2D>("x");
            defaultIcon = Content.Load<Texture2D>("Items/defaultEqIcon");
            arrow = Content.Load<Texture2D>("scroller/arrow");
            scroller = Content.Load<Texture2D>("scroller/scroller");
            path = Content.Load<Texture2D>("scroller/path");
            skillInfoFont = Content.Load<SpriteFont>("Game/chatFont");

            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();



        }

        public void Update(out int attSkillId, double range, bool rightDirection, bool targeted, int playerDead, bool blockCasting,out bool block, bool inBoxIn,EqSkills eqSkills,out EqSkills eqSkillsOut,int castSpeed,bool isFocus)
        {

            mouse = Mouse.GetState();
            if(!isFocus)
                keyboard = Keyboard.GetState();
            attSkillId = -1;

            skillInfo.Clear();
            displayCursor = true;

            if (display)
                foreach (InventorySkill inventorySkill in inventorySkills)
                {
                    if (inventorySkill.eqSkillId != -1)
                    {
                        inventorySkill.cReuse = eqSkills.cReuse[inventorySkill.eqSkillId];
                        inventorySkill.reuseTexture = eqSkills.reuseTexture[inventorySkill.eqSkillId];
                        inventorySkill.startReuse = eqSkills.startReuse[inventorySkill.eqSkillId];

                    }
                    else
                    {
                        if (inventorySkill.cReuse < skills.skillList[inventorySkill.skillId].reuse * (1 - castSpeed / (float)50) && inventorySkill.startReuse)
                            inventorySkill.cReuse++;
                        else
                        {

                            inventorySkill.startReuse = false;

                        }


                        Color[] data = new Color[30 * 30];
                        float alfa = (float)(2 * Math.PI * inventorySkill.cReuse / (skills.skillList[inventorySkill.skillId].reuse * (1 - castSpeed / (float)50)));

                        for (int j = 0; j < 30 * 30; j++)
                        {
                            if (!inventorySkill.startReuse)
                                data[j] = Color.Transparent;
                            else
                            {

                                if (alfa > 0 && alfa < Math.PI & j % 30 - 15 >= 0 && j / 30 - 15 >= (j % 30 - 15) * Math.Tan(Math.PI / 2 - alfa) || alfa > Math.PI && alfa < 2 * Math.PI && j % 30 - 15 < 0 && j / 30 - 15 <= (j % 30 - 15) * Math.Tan(Math.PI / 2 - alfa) || alfa >= Math.PI && alfa < 2 * Math.PI && j % 30 - 15 >= 0 || alfa >= 2 * Math.PI)
                                    data[j] = Color.Transparent;
                                else
                                    data[j] = Color.Gray;

                            }
                        }
                        inventorySkill.reuseTexture.SetData(data);
                    }


                }

            if (display)
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




            inventoryRows = (inventorySkills.Count - 8*6) / 8;
            if ((inventorySkills.Count - 8*6) % 8 > 0)
                inventoryRows++;
            if (inventoryRows < 0)
                inventoryRows = 0;

            scrollerWidth = 155 - 5 * inventoryRows;

            if (scrollerWidth + scrollerCoord.Y - scrollerUpBound > 155)
            {
                scrollerCoord.Y -= 5;
            }



            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > scrollerCoord.X && mouse.X < scrollerCoord.X + 15 && mouse.Y > scrollerCoord.Y && mouse.Y < scrollerCoord.Y + scrollerWidth && display && isActive)
            {
                scroll = true;
                this.block = true;

            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                scroll = false;
                this.block = false;
            }

            if (scroll && scrollerCoord.Y < inventoryCoord.Y + 15 + 5 * inventoryRows && mouse.Y - prevMouse.Y > 0 && ((mouse.Y - prevMouse.Y) / 2) * 5 <= inventoryRows * 5 - scrollerCoord.Y + scrollerUpBound)
            {

                scrollerCoord.Y += ((mouse.Y - prevMouse.Y) / 2) * 5;
            }
            else if (scroll && scrollerCoord.Y > inventoryCoord.Y + 15 && mouse.Y - prevMouse.Y < 0 && ((prevMouse.Y - mouse.Y) / 2) * 5 <= scrollerCoord.Y - scrollerUpBound)
            {
                scrollerCoord.Y += ((mouse.Y - prevMouse.Y) / 2) * 5;
            }




            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && mouse.X > boxCoord.X && mouse.X < boxCoord.X + 300 && mouse.Y > boxCoord.Y && mouse.Y < boxCoord.Y + 20 && isActive)
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
                scrollerCoord.X += mouse.X - prevMouse.X;
                scrollerCoord.Y += mouse.Y - prevMouse.Y;
                scrollerUpBound += mouse.Y - prevMouse.Y;
            }


            if (display )
            {
                for (int i = 0; i < 8*6; i++)
                {
                    if (i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 < inventorySkills.Count  && !blockCasting && mouse.X > inventoryCoord.X + 31 * (i % 8) && mouse.X < inventoryCoord.X + 31 * (i % 8) + 30 && mouse.Y > inventoryCoord.Y + 31 * (i / 8) && mouse.Y < inventoryCoord.Y + 31 * (i / 8) + 30 && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released && inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].startReuse == false && (targeted && range <= skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].range && range > 0 && rightDirection && playerDead == 0 || skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType == "buff" || skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType == "heal"))
                    {
                        attSkillId = inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId;
                        inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].cReuse = 0;
                        inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].startReuse = true;

                    }

                    if (isActive)
                    {
                        if (mouse.X > inventoryCoord.X + 31 * (i % 8) && mouse.X < inventoryCoord.X + 31 * (i % 8) + 31 && mouse.Y > inventoryCoord.Y + 31 * (i / 8) && mouse.Y < inventoryCoord.Y + 31 * (i / 8) + 31  && i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 < inventorySkills.Count)
                        {

                            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released)
                            {
                                dragSkill = inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId;
                                inventorySkillId = i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8;
                                mouseInSkill = new Vector2(mouse.X - inventoryCoord.X - 31 * (i % 8), mouse.Y - inventoryCoord.Y - 31 * (i / 8));
                            }
                            else
                            {
                                displayCursor = false;
                                skillInfo.Add(skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].name);
                                if(skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType=="dmg" || skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType == "mass")
                                {
                                    skillInfo.Add("Power " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].power);
                                    skillInfo.Add("Range " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].range);
                                    skillInfo.Add("Reuse " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].reuse/60+"s");
                                }
                                else if(skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType == "buff")
                                {
                                    
                                    foreach(Add add in skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].add)
                                    switch(add.stat)
                                    {
                                            case "pstr":
                                                skillInfo.Add("P.Pow " + add.val);
                                                break;

                                            case "mstr":
                                                skillInfo.Add("M.Pow " + add.val);
                                                break;

                                            case "pdef":
                                                skillInfo.Add("P.Def " + add.val);
                                                break;

                                            case "mdef":
                                                skillInfo.Add("M.Def " + add.val);
                                                break;

                                            case "pas":
                                                skillInfo.Add("P.Speed " + add.val);
                                                break;

                                            case "mas":
                                                skillInfo.Add("C.Speed " + add.val);
                                                break;

                                            case "mp":
                                                skillInfo.Add("Max Mp " + add.val);
                                                break;

                                            case "hp":
                                                skillInfo.Add("Max Hp " + add.val);
                                                break;

                                            case "eva":
                                                skillInfo.Add("Evasion " + add.val);
                                                break;

                                            case "pc":
                                                skillInfo.Add("P.Crits " + add.val);
                                                break;

                                            case "mc":
                                                skillInfo.Add("M.Crits " + add.val);
                                                break;
                                            case "attackRange":
                                                skillInfo.Add("Range " + add.val);
                                                break;
                                        }
                                    skillInfo.Add("Reuse " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].reuse / 60 + "s");
                                    skillInfo.Add("Time " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].time/60+"s");
                                }
                                else if(skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].skillType == "heal")
                                {
                                    
                                    foreach (Add add in skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].add)
                                        switch (add.stat)
                                        {
                                            case "rhp":
                                                skillInfo.Add("Power " + add.val);
                                                break;
                                        }
                                    
                                    skillInfo.Add("Reuse " + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].reuse / 60 + "s");
                                    
                                }
                            }
                        }
                    }
                }

            }

            for (int i = 0; i < 20; i++)
            {
                if (mouse.X > barCoord.X + 5 + 31 * (i % 10) && mouse.X < barCoord.X + 5 + 31 * (i % 10) + 31 && mouse.Y > barCoord.Y + 5 + 31 * (i / 10) && mouse.Y < barCoord.Y + 5 + 31 + 31 * (i / 10) && mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released && eqSkills.skill[i]!=-1)
                {
                    dragSkill = eqSkills.skill[i];
                    inventorySkillId = eqSkills.skillInventoryId[i];
                    mouseInSkill = new Vector2(mouse.X - barCoord.X - 5 - 31 * (i % 10), mouse.Y - barCoord.Y - 5 - 31 * (i / 10));
                }
            }

            if (dragSkill != -1 && mouse.RightButton == ButtonState.Released)
            {
                int o = 0;
                for (int i = 0; i < 20; i++)
                {
                    if(mouse.X> barCoord.X + 5 + 31 * (i%10) && mouse.X< barCoord.X + 5 + 31 * (i%10)+30 && mouse.Y>barCoord.Y+5 + 31 * (i / 10) && mouse.Y<barCoord.Y+5+30 + 31 * (i / 10))
                    {
                        if (eqSkills.skill[i] != -1 && inventorySkills[inventorySkillId].eqSkillId!=-1)
                        {
                            eqSkills.skill[inventorySkills[inventorySkillId].eqSkillId] = eqSkills.skill[i];
                            eqSkills.skillInventoryId[inventorySkills[inventorySkillId].eqSkillId] = eqSkills.skillInventoryId[i];
                            eqSkills.cReuse[inventorySkills[inventorySkillId].eqSkillId] = eqSkills.cReuse[i];
                            eqSkills.startReuse[inventorySkills[inventorySkillId].eqSkillId] = eqSkills.startReuse[i];

                            inventorySkills[eqSkills.skillInventoryId[i]].eqSkillId = inventorySkills[inventorySkillId].eqSkillId;

                        }
                        else if(inventorySkills[inventorySkillId].eqSkillId != -1)
                        {
                            eqSkills.skill[inventorySkills[inventorySkillId].eqSkillId] = -1;
                        }
                        

                        
                        inventorySkills[inventorySkillId].eqSkillId = i;
                        eqSkills.skill[i] = dragSkill;
                        eqSkills.skillInventoryId[i] = inventorySkillId;
                        eqSkills.cReuse[i] = inventorySkills[inventorySkillId].cReuse;
                        eqSkills.startReuse[i] = inventorySkills[inventorySkillId].startReuse;
                        o++;

                    }
                }
                if(o>0)
                    dragSkill = -1;
                else
                {
                    if (inventorySkills[inventorySkillId].eqSkillId != -1)
                    {
                        eqSkills.skill[inventorySkills[inventorySkillId].eqSkillId] = -1;
                        eqSkills.skillInventoryId[inventorySkills[inventorySkillId].eqSkillId] = -1;
                    }
                    inventorySkills[inventorySkillId].eqSkillId = -1;
                    dragSkill = -1;
                }
            }


            



            if (mouse.X > boxCoord.X + 280 && mouse.X < boxCoord.X + 295 && mouse.Y > boxCoord.Y + 2 && mouse.Y < boxCoord.Y + 17 && prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released && isActive)
            {
                display = false;
                isActive = false;
            }

            if (prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
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
            else if (prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U) || prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I) || prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y))
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

            inventoryCoord = new Vector2(boxCoord.X + 20,boxCoord.Y + 30);
            scrollerCoord.X = inventoryCoord.X + 248;
            block = this.block;
            eqSkillsOut = eqSkills;
            prevMouse = mouse;
            prevKeyboard = keyboard;

        }

        public void Draw()
        {
            spriteBatch.Begin();
            if(display)
            {
                spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle((int)boxCoord.X, (int)boxCoord.Y, 300, 300), color: Color.White * transp);
                spriteBatch.Draw(header, destinationRectangle: new Rectangle((int)boxCoord.X, (int)boxCoord.Y, 300, 20), color: Color.White * transp);
                spriteBatch.Draw(xButton, new Vector2(boxCoord.X + 280, boxCoord.Y + 2), color: Color.White * transp);

                if (inventorySkills.Count - ((scrollerCoord.Y - scrollerUpBound) / 5) * 8 <= 8*6)
                {
                    for (int i = 0; i < inventorySkills.Count - (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8; i++)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Skills/" + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].name), new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                        if( inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].cReuse!=0)
                        spriteBatch.Draw(inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].reuseTexture, new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                    }

                    for (int i = inventorySkills.Count - (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8; i < 8*6; i++)
                        spriteBatch.Draw(defaultIcon, new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                }
                else
                {
                    for (int i = 0; i < 8 * 6; i++)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>("Skills/" + skills.skillList[inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].skillId].name), new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                        if (inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].cReuse != 0)
                            spriteBatch.Draw(inventorySkills[i + (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].reuseTexture, new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                    }
                }

                spriteBatch.Draw(arrow, new Vector2(inventoryCoord.X + 248, inventoryCoord.Y), color: Color.White * transp);
                spriteBatch.Draw(path, destinationRectangle:new Rectangle((int)inventoryCoord.X + 248, (int)inventoryCoord.Y + 15,15,155), color: Color.White * transp);
                spriteBatch.Draw(arrow, new Vector2(inventoryCoord.X + 248, inventoryCoord.Y + 170), color: Color.White * transp, effects: SpriteEffects.FlipVertically);
                spriteBatch.Draw(scroller, color: Color.White * transp, destinationRectangle: new Rectangle((int)scrollerCoord.X, (int)scrollerCoord.Y, 15, scrollerWidth));

            }

            if (dragSkill != -1)
                spriteBatch.Draw(Content.Load<Texture2D>("Skills/" + skills.skillList[dragSkill].name), new Vector2(mouse.X - mouseInSkill.X, mouse.Y - mouseInSkill.Y), Color.White);



            if (skillInfo.Count > 0)
            {
                if (mouse.X + 100 <= Window.ClientBounds.Width)
                {
                    spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X, mouse.Y, 100, skillInfo.Count * 16), color: Color.White * transp);
                    for (int i = 0; i < skillInfo.Count; i++)
                        spriteBatch.DrawString(skillInfoFont, skillInfo[i], new Vector2(mouse.X + 2, mouse.Y + 2 + i * 16), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X - 100, mouse.Y, 100, skillInfo.Count * 16), color: Color.White * transp);
                    for (int i = 0; i < skillInfo.Count; i++)
                        spriteBatch.DrawString(skillInfoFont, skillInfo[i], new Vector2(mouse.X + 2 - 100, mouse.Y + 2 + i * 16), Color.Black);
                }
            }

            spriteBatch.End();


        }


    }
}
