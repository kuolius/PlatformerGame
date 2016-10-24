using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RPGame
{

    public class Triplet<T1,T2,T3>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public Triplet(T1 par1,T2 par2,T3 par3)
            {
            Item1 = par1;
            Item2 = par2;
            Item3 = par3;
            }
    }

    public class MagicAnimation
    {
        public int magicId { get; set; }
        public int magicIndex { get; set; }
        public int cFps { get; set; }
        public Texture2D texture { get; set; }

        public MagicAnimation(int magicId, int magicIndex, int cFps,Texture2D texture)
        {
            this.magicId = magicId;
            this.magicIndex = magicIndex;
            this.cFps = cFps;
            this.texture = texture;
        }
    }

    class MobAnimation
    {
        ContentManager Content;
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;


        List<Triplet<int, Int16,Point>> dmgQue = new List<Triplet<int, Int16,Point>>();
        List<Triplet<int, Int16,Point>> removeList = new List<Triplet<int, Int16,Point>>();

        List<MagicAnimation> magicQue = new List<MagicAnimation>();
        List<MagicAnimation> removeMagicList = new List<MagicAnimation>();

        Texture2D hair, head, arms, body, legs, boots, backArm,hpBar,targetBar;
        SpriteFont dmgFont,statFont;
        Random randX, randY,randSign;
        int[] sign = new int[] { -1, 1 };


        int[] playerState;
        int playerX, playerY;
        int FPS,FPSMA,FPSA;
        int animationIndex, handsIndex;
        bool handsNotOver,drawTarget;
        int maxHp, hp;

        Skills skills;
        Texture2D magicTexture;
        int magicId,magicIndex;
        bool magicStarted;
  
        SpriteEffects spriteEffect;
        Stats mobStats;

        public MobAnimation(ContentManager Content, GraphicsDevice graphics)
        {
            this.Content = Content;
            this.graphics = graphics;
            Initialize();
            LoadContent();
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            spriteEffect = SpriteEffects.None;
            FPS = 0;
            FPSMA = 0;
            FPSA = 0;
            animationIndex = 0;
            handsIndex = 0;
            

            magicStarted = false;
            magicIndex = 0;
            

            randX = new Random();
            randY = new Random();
            randSign = new Random();   
        }

        public void LoadContent()
        {
            hair = Content.Load<Texture2D>("Game/Zombie/Zombie Brain");
            head = Content.Load<Texture2D>("Game/Zombie/Zombie Head");
            arms = Content.Load<Texture2D>("Game/Zombie/Zombie Front Arm");
            body = Content.Load<Texture2D>("Game/Zombie/Zombie Body");
            legs = Content.Load<Texture2D>("Game/Zombie/Zombie Legs");
            boots = Content.Load<Texture2D>("Game/Zombie/Zombie Feet");
            backArm = Content.Load<Texture2D>("Game/Zombie/Zombie Back Arm");
            

            hpBar = new Texture2D(graphics, 32,5);
            dmgFont = Content.Load<SpriteFont>("Game/dmgFont");
            statFont = Content.Load<SpriteFont>("Game/statFont");
            targetBar = new Texture2D(graphics, 15, 15);
            Color[] data = new Color[225];
            for (int i = 0; i < 225; i++)
                data[i] = Color.Blue;
            targetBar.SetData(data);



            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();

        }

        public void Update(int[] playerState,Stats mobStats,int mobDmg, int playerX, int playerY,bool drawTarget,int magicId,bool magicAttack)
        {
            this.playerState = playerState;
            this.playerX = playerX;
            this.playerY = playerY;
            this.mobStats = mobStats;
            this.drawTarget = drawTarget;
            this.magicId = magicId;

            maxHp = mobStats.maxHp;
            hp = mobStats.hp;

            if (mobDmg > -1)
                dmgQue.Add(new Triplet<int, short,Point>(mobDmg, 0,new Point(randX.Next(1, 20)*sign[randSign.Next(0,2)], randY.Next(1, 10) * sign[randSign.Next(0, 2)])));

            Color[] data = new Color[32*5];

            for (int i = 0; i < 32 * 5; i++)
            {
                if(i%32<(32*hp)/maxHp)
                    data[i] = Color.Green;
                else
                    data[i] = Color.Red;

            }


            hpBar.SetData(data);

            foreach(Triplet<int,short,Point> dmg in dmgQue)
            {
                if (dmg.Item2 == 255)
                    removeList.Add(dmg);
                else
                    dmg.Item2+=3;
                
            }

            foreach(Triplet<int, short,Point> remove in removeList)
            {
                dmgQue.Remove(remove);
            }

            removeList.Clear();

            if (playerState[2] == 1)
                spriteEffect = SpriteEffects.None;
            if (playerState[3] == 1)
                spriteEffect = SpriteEffects.FlipHorizontally;


            if (FPS == 10)
            {
                if (playerState[2] == 1)
                {
                    if (playerState[3] + playerState[1] + playerState[0] == 0)
                        animationIndex = ((animationIndex + 1) % 6);
                    spriteEffect = SpriteEffects.None;
                }
                if (playerState[3] == 1)
                {
                    if (playerState[2] + playerState[1] + playerState[0] == 0)
                        animationIndex = ((animationIndex + 1) % 6);
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }

               
                FPS = 0;
            }
            FPS++;

            if (playerState[4] == 1 && !handsNotOver)
            {
                handsIndex = 0;
                handsNotOver = true;
            }

            if (magicId != -1 && (drawTarget || skills.skillList[magicId].skillType == "mass") && magicAttack && mobDmg!=-1)
                magicQue.Add(new MagicAnimation(magicId, 0, 0, Content.Load<Texture2D>("SkillAnimation/" + skills.skillList[magicId].name)));


            
            
        }

        

       public void playerAnimation(int coord1, int coord2, int coord3, int coord4)
        {
            Vector2 playerAnimationCoordinates = new Vector2(playerX - 4, playerY + 1);

            spriteBatch.Draw(boots, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            spriteBatch.Draw(legs, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);



            spriteBatch.Draw(body, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

            if (playerState[2] == 1 || playerState[3] == 1)
                spriteBatch.Draw(head, playerAnimationCoordinates, new Rectangle(32, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            else
                spriteBatch.Draw(head, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

            spriteBatch.Draw(hair, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);





            if (handsNotOver)
            {
                if (handsIndex <= 4)
                {
                    spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle((handsIndex + 11) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle((handsIndex + 11) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    if (FPSA >= 5)
                    {
                        handsIndex++;
                        FPSA = 0;
                    }
                    else
                        FPSA++;
                }
                else
                {
                    FPSA = 0;
                    handsNotOver = false;
                }
            }

            if(!handsNotOver)
            {
                spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

            }

            spriteBatch.DrawString(statFont, "LvL " + mobStats.lvl,new Vector2(playerAnimationCoordinates.X+16- statFont.MeasureString("LvL " + mobStats.lvl).X / 2*.7f, playerAnimationCoordinates.Y - 15-statFont.MeasureString("LvL " + mobStats.lvl).Y/2*.7f),Color.Black,0,Vector2.Zero,.7f,SpriteEffects.None,1f);
            spriteBatch.Draw(hpBar,new Vector2(playerAnimationCoordinates.X,playerAnimationCoordinates.Y-5), Color.White);
            foreach(Triplet<int,short,Point> dmg in dmgQue)
                spriteBatch.DrawString(dmgFont, Convert.ToString(dmg.Item1), new Vector2(playerAnimationCoordinates.X+dmg.Item3.X, playerAnimationCoordinates.Y -dmg.Item3.Y-30), new Color(dmg.Item2, dmg.Item2, dmg.Item2));

        }


        public void Draw()
        {
            spriteBatch.Begin();

            if (playerState != null)
            {
                if (drawTarget)
                    spriteBatch.Draw(targetBar, new Vector2(playerX - 4 - 15, playerY + 1), Color.White);

                if (playerState[2] == 0 && playerState[3] == 0 && playerState[0] == 0 && playerState[1] == 0)
                {
                    playerAnimation(0, 0, 32, 64);
                }
                else if (playerState[0] == 1)
                {
                    playerAnimation(8 * 32, 0, 32, 64);
                }
                else if (playerState[1] == 1)
                {
                    playerAnimation(9 * 32, 0, 32, 64);
                }
                else if (playerState[2] == 1)
                {
                    playerAnimation((animationIndex + 1) * 32, 0, 32, 64);
                }
                else if (playerState[3] == 1)
                {
                    playerAnimation((animationIndex + 1) * 32, 0, 32, 64);
                }


                foreach (MagicAnimation magicAnimation in magicQue)
                {
                    if (magicAnimation.magicIndex < skills.skillList[magicAnimation.magicId].animationLength)
                    {
                        spriteBatch.Draw(magicAnimation.texture, new Vector2(playerX + 16 - 4 - 20, playerY + 64 + 1 - 70), new Rectangle(magicAnimation.magicIndex * 40, 0, 40, 70), Color.White);
                        if (magicAnimation.cFps >= skills.skillList[magicAnimation.magicId].fps)
                        {
                            magicAnimation.magicIndex++;
                            magicAnimation.cFps = 0;
                        }
                        else
                            magicAnimation.cFps++;

                    }
                    else
                    {
                        removeMagicList.Add(magicAnimation);
                    }
                }

                foreach (MagicAnimation magicAnimation in removeMagicList)
                    magicQue.Remove(magicAnimation);
                removeMagicList.Clear();


            }

            

            spriteBatch.End();
        }
    }
}
