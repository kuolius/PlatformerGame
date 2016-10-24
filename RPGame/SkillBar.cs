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
    class EqSkills
    {
        public int[] skill=new int[20];
        public Texture2D[] reuseTexture=new Texture2D[20];
        public int[] cReuse = new int[20];
        public bool[] startReuse = new bool[20];
        public int[] skillInventoryId=new int[20];

        public EqSkills(GraphicsDevice graphics)
        {
            for (int i = 0; i < 20; i++)
            {
                skill[i] = -1;
                cReuse[i] = 0;
                startReuse[i] = false;
                reuseTexture[i] = new Texture2D(graphics, 30, 30);
                skillInventoryId[i] = -1;
            }
        }
    }
    
    class SkillBar
    {

        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;

        Point barSize;
        Vector2 barCoord;
        Texture2D mainBar,defaultIcon;
        EqSkills eqSkills;
        Skills skills;
        KeyboardState keyboard;
        MouseState mouse,prevMouse;

        int attSkillId;
        

        public SkillBar(ContentManager Content, GraphicsDevice graphics, GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
        }


        public void Initialize()
        {
            eqSkills = new EqSkills(graphics);
            barSize = new Point(319, 75);
            
            barCoord = new Vector2(Window.ClientBounds.Width  - barSize.X-20, Window.ClientBounds.Height - barSize.Y - 20);
            eqSkills = new EqSkills(graphics);
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics);
            mainBar = Content.Load<Texture2D>("Game/InventoryBg");
            defaultIcon = Content.Load<Texture2D>("Skills/default");

            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();
        }

        public void Update(out int attSkillIdOut,double range,bool rightDirection,int castSpeed,bool targeted,int playerDead,bool blockCasting,out EqSkills eqSkillsOut,EqSkills eqSkillsIn,bool firstTime,bool isFocus)
        {
            mouse = Mouse.GetState();
            if(!isFocus)
             keyboard = Keyboard.GetState();
            attSkillId = -1;
            if(!firstTime)
                eqSkills = eqSkillsIn;
            



            for(int i=0;i<20;i++)
            {
                if ( eqSkills.skill[i]!=-1 && !blockCasting && (i < 10 && keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), "D" + (i+1)%10, false)) && keyboard.IsKeyUp(Keys.LeftAlt) || i <20 && i>9 && keyboard.IsKeyDown((Keys)Enum.Parse(typeof(Keys), "D" + (i + 1) % 10, false)) && keyboard.IsKeyDown(Keys.LeftAlt) || mouse.X> barCoord.X + 5 + 31 * (i % 10) && mouse.X< barCoord.X + 5 + 31 * (i % 10)+30 && mouse.Y > barCoord.Y + 5 + 31 * (i / 10) && mouse.Y < barCoord.Y + 5 + 31 * (i / 10)+30 && mouse.LeftButton==ButtonState.Pressed && prevMouse.LeftButton==ButtonState.Released) && eqSkills.startReuse[i] == false &&( targeted && range <= skills.skillList[eqSkills.skill[i]].range && range > 0 && rightDirection && playerDead == 0 || skills.skillList[eqSkills.skill[i]].skillType=="buff" || skills.skillList[eqSkills.skill[i]].skillType == "heal"))
                {
                    attSkillId = eqSkills.skill[i];
                    eqSkills.cReuse[i] = 0;
                    eqSkills.startReuse[i] = true;

                }

                if (eqSkills.skill[i]!=-1)
                {
                    if (eqSkills.cReuse[i] < skills.skillList[eqSkills.skill[i]].reuse*(1-castSpeed/(float)50) && eqSkills.startReuse[i])
                        eqSkills.cReuse[i]++;
                    else
                    {

                        eqSkills.startReuse[i] = false;

                    }

                    Color[] data = new Color[30 * 30];
                    float alfa = (float)(2 * Math.PI * eqSkills.cReuse[i] / (skills.skillList[eqSkills.skill[i]].reuse * (1 - castSpeed / (float)50)));

                    for (int j=0;j<30*30;j++)
                    {
                        if (!eqSkills.startReuse[i])
                            data[j] = Color.Transparent;
                        else
                        {

                            if (alfa > 0 && alfa < Math.PI & j % 30 - 15 >= 0 && j / 30 - 15 >= (j % 30 - 15) * Math.Tan(Math.PI / 2 - alfa) || alfa > Math.PI && alfa < 2 * Math.PI && j % 30 - 15 < 0 && j / 30 - 15 <= (j % 30 - 15) * Math.Tan(Math.PI / 2 - alfa) || alfa >= Math.PI && alfa < 2 * Math.PI && j % 30 - 15 >= 0 || alfa >= 2 * Math.PI)
                                data[j] = Color.Transparent;
                            else
                                data[j] = Color.Gray;

                        }
                    }
                    eqSkills.reuseTexture[i].SetData(data);
                }
            }


            prevMouse = mouse;
            attSkillIdOut = attSkillId;
            eqSkillsOut = eqSkills;
        }

        public void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mainBar, new Rectangle((int)barCoord.X, (int)barCoord.Y, barSize.X, barSize.Y), Color.White);

            for(int i=0;i<20;i++)
            {
                if (eqSkills.skill[i] != -1)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("Skills/" + skills.skillList[eqSkills.skill[i]].name), new Vector2(barCoord.X + 5 + 31 * (i % 10) , barCoord.Y + 5 + 31 * (i / 10)), Color.White);
                    spriteBatch.Draw(eqSkills.reuseTexture[i], new Vector2(barCoord.X + 5 + 31 * (i % 10) , barCoord.Y + 5 + 31 * (i / 10)), Color.White * .8f);
                }
                else
                    spriteBatch.Draw(Content.Load<Texture2D>("Skills/default"), new Vector2(barCoord.X + 5 + 31 * (i % 10) , barCoord.Y + 5 + 31 * (i / 10)), Color.White);

            }
            
            spriteBatch.End();
        }
    }
}
