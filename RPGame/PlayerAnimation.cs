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
    class PlayerAnimation
    {
        ContentManager Content;
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;

        Texture2D hair, head, arms, body, legs, boots,backArm,castSkillBar,magicTexture;
        Texture2D leftArm,rightArm,leftGloves,rightGloves,leftBody,middleBody,rightBody,legsArmor,bootsArmor,helm,frontHand,backHand;
        SpriteFont dmgFont;
        List<Triplet<int, Int16, Point>> dmgQue = new List<Triplet<int, Int16, Point>>();
        List<Triplet<int, Int16, Point>> removeList = new List<Triplet<int, Int16, Point>>();

        List<MagicAnimation> magicQue = new List<MagicAnimation>();
        List<MagicAnimation> removeMagicList = new List<MagicAnimation>();

        Items items;
        EqItems eqItems;
        Skills skills;

        int[] playerState;
        int playerX, playerY;
        int FPS,FPSMA;
        int animationIndex, handsIndex;
        bool handsNotOver;
        SpriteEffects spriteEffect;
        int maxHp, hp;
        Random randX, randY, randSign;
        int[] sign = new int[] { -1, 1 };
        int magicId,magicIndex;
        bool magicStarted;
        

        public PlayerAnimation(ContentManager Content,GraphicsDevice graphics)
        {
            this.Content = Content;
            this.graphics = graphics;
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics);
            spriteEffect = SpriteEffects.None;
            FPS = 0;
            animationIndex = 1;
            handsIndex = 0;
            handsNotOver = false;
            magicStarted = false;
            magicIndex = 0;
            FPSMA = 0;
            randX = new Random();
            randY = new Random();
            randSign = new Random();
        }

        public void LoadContent()
        {
            hair = Content.Load<Texture2D>("Game/Player/hair");
            head = Content.Load<Texture2D>("Game/Player/head");
            arms = Content.Load<Texture2D>("Game/Player/front arm");
            body = Content.Load<Texture2D>("Game/Player/body");
            legs = Content.Load<Texture2D>("Game/Player/legs");
            boots = Content.Load<Texture2D>("Game/Player/boots");
            backArm = Content.Load<Texture2D>("Game/Player/back arm");
            frontHand = Content.Load<Texture2D>("Game/Player/front hand");
            backHand = Content.Load<Texture2D>("Game/Player/back hand");
            castSkillBar = new Texture2D(graphics, 40, 2);

            XmlSerializer deserializer = new XmlSerializer(typeof(Items));
            TextReader reader = new StreamReader("Content/Game/Items.xml");
            object obj = deserializer.Deserialize(reader);
            items = (Items)obj;
            reader.Close();

             deserializer = new XmlSerializer(typeof(Skills));
            reader = new StreamReader("Content/Game/Skills.xml");
            obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();


            dmgFont = Content.Load<SpriteFont>("Game/dmgFont");
        }

        public void Update(int[] playerState,Stats playerStats, int playerDmg, int playerX,int playerY,EqItems eqItems,int attSkillIndex,int buffId)
        {
            this.playerState = playerState;
            this.playerX = playerX;
            this.playerY = playerY;
            this.eqItems = eqItems;
            

            Color[] data = new Color[80];
            for(int i=0;i<80;i++)
            {
                if (i % 40 < 40 * attSkillIndex / (600 / (float)playerStats.castSpeed))
                    data[i] = Color.Blue;
                else
                    data[i] = Color.Transparent;

            }
            castSkillBar.SetData(data);

            maxHp = playerStats.maxHp;
            hp = playerStats.hp;

            if (playerDmg > -1)
                dmgQue.Add(new Triplet<int, short, Point>(playerDmg, 0, new Point(randX.Next(1, 20) * sign[randSign.Next(0, 2)], randY.Next(1, 10) * sign[randSign.Next(0, 2)])));


            foreach (Triplet<int, short, Point> dmg in dmgQue)
            {
                if (dmg.Item2 == 255)
                    removeList.Add(dmg);
                else
                    dmg.Item2 += 3;

            }

            foreach (Triplet<int, short, Point> remove in removeList)
            {
                dmgQue.Remove(remove);
            }

            removeList.Clear();

            if (playerState[2] == 1)
                spriteEffect = SpriteEffects.None;
            if (playerState[3] == 1)
                spriteEffect = SpriteEffects.FlipHorizontally;
            

            if (FPS == 5)
            {
                if (playerState[2] == 1)
                {
                    if (playerState[3] + playerState[1] + playerState[0] == 0)
                        animationIndex = ((animationIndex + 1) % 7);
                    spriteEffect = SpriteEffects.None;
                }
                if (playerState[3] == 1 )
                {
                    if(playerState[2] + playerState[1] + playerState[0] == 0)
                        animationIndex = ((animationIndex + 1) % 7);
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }

                if(handsIndex!=0 && handsNotOver)
                {
                    handsIndex--;
                }
                else
                {
                    handsNotOver = false;
                }

                
                FPS = 0;
            }
            FPS++;
            if (playerState[4] == 1 && !handsNotOver)
            {
                handsIndex = 2;
                handsNotOver = true;
            }

            if (eqItems.leftArm.id != -1)
                leftArm = Content.Load<Texture2D>("ItemsAnimation/" + items.itemList[eqItems.leftArm.id].name);

            if(eqItems.helm.id!=-1)
                helm= Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.helm.id].name);

            if (eqItems.body.id!=-1)
            {
                leftBody = Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.body.id].set + "LeftArm");
                middleBody = Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.body.id].name);
                rightBody= Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.body.id].set + "RightArm");
            }

            if(eqItems.gloves.id!=-1)
            {
                leftGloves= Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.gloves.id].set + "LeftGloves");
                rightGloves = Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.gloves.id].set + "RightGloves");
            }

            if(eqItems.legs.id!=-1)
                legsArmor = Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.legs.id].name);

            if(eqItems.boots.id!=-1)
                bootsArmor = Content.Load<Texture2D>("Game/Armors/" + items.itemList[eqItems.boots.id].name);


            if (buffId != -1 && attSkillIndex<=0)
                magicQue.Add(new MagicAnimation(buffId, 0, 0, Content.Load<Texture2D>("SkillAnimation/" + skills.skillList[buffId].name)));

        }


        void playerAnimation(int coord1, int coord2, int coord3, int coord4)
        {
            Vector2 playerAnimationCoordinates = new Vector2(playerX , playerY+2 );

            if(eqItems.boots.id!=-1)
                spriteBatch.Draw(bootsArmor, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            else
                spriteBatch.Draw(boots, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

            if(eqItems.legs.id!=-1)
                spriteBatch.Draw(legsArmor, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            else
                spriteBatch.Draw(legs, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

            
            

            if (eqItems.helm.id != -1)
                spriteBatch.Draw(helm, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            else
            {
                if (playerState[2] == 1 || playerState[3] == 1)
                    spriteBatch.Draw(head, playerAnimationCoordinates, new Rectangle(32, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                else
                    spriteBatch.Draw(head, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                spriteBatch.Draw(hair, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
            }

            
                


            if (handsNotOver)
            {
                if (eqItems.leftArm.id != -1)
                {
                    if (items.itemList[eqItems.leftArm.id].eqType == "bow")
                    {

                        if (eqItems.body.id == -1)
                            spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                       
                            

                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(middleBody, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(body, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if (eqItems.gloves.id != -1)
                            spriteBatch.Draw(rightGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(backHand, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(rightBody, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);


                        

                        if (spriteEffect == SpriteEffects.None)
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X, playerAnimationCoordinates.Y - 5), new Rectangle(handsIndex * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X - 18, playerAnimationCoordinates.Y - 5), new Rectangle(handsIndex  * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        

                        if ( eqItems.body.id == -1)
                            spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        if (eqItems.gloves.id != -1)
                            spriteBatch.Draw(leftGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(frontHand, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(leftBody, playerAnimationCoordinates, new Rectangle((handsIndex + 14) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    }
                    else
                    {

                        if ( eqItems.body.id == -1)
                            spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(middleBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(body, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if (eqItems.gloves.id != -1)
                            spriteBatch.Draw(rightGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(backHand, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(rightBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);


                        if (spriteEffect == SpriteEffects.None)
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X, playerAnimationCoordinates.Y - 5), new Rectangle((handsIndex + 10) * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X - 18, playerAnimationCoordinates.Y - 5), new Rectangle((handsIndex + 10) * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                        if ( eqItems.body.id == -1)
                            spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        if (eqItems.gloves.id != -1)
                            spriteBatch.Draw(leftGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(frontHand, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        if (eqItems.body.id != -1)
                            spriteBatch.Draw(leftBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    }

                }
                else
                {
                    if ( eqItems.body.id == -1)
                        spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                    if (eqItems.body.id != -1)
                        spriteBatch.Draw(middleBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    else
                        spriteBatch.Draw(body, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                    if (eqItems.gloves.id != -1)
                        spriteBatch.Draw(rightGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    else
                        spriteBatch.Draw(backHand, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                    if (eqItems.body.id != -1)
                        spriteBatch.Draw(rightBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                    if (eqItems.body.id == -1)
                        spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    if (eqItems.gloves.id != -1)
                        spriteBatch.Draw(leftGloves, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    else
                        spriteBatch.Draw(frontHand, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    if (eqItems.body.id != -1)
                        spriteBatch.Draw(leftBody, playerAnimationCoordinates, new Rectangle((handsIndex + 10) * 32, 0, 32, 64), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                }
                
                
            }
            else
            {


                if ( eqItems.body.id == -1)
                    spriteBatch.Draw(backArm, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                if (eqItems.body.id != -1)
                    spriteBatch.Draw(middleBody, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                else
                    spriteBatch.Draw(body, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                if (eqItems.gloves.id != -1)
                    spriteBatch.Draw(rightGloves, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                else
                    spriteBatch.Draw(backHand, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);


                if (eqItems.body.id != -1)
                    spriteBatch.Draw(rightBody, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);

                if (eqItems.leftArm.id != -1)
                {
                    if (items.itemList[eqItems.leftArm.id].eqType != "bow")
                    {
                        if (spriteEffect == SpriteEffects.None)
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X, playerAnimationCoordinates.Y - 5), new Rectangle((coord1 / 32) * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                        else
                            spriteBatch.Draw(leftArm, new Vector2(playerAnimationCoordinates.X - 18, playerAnimationCoordinates.Y - 5), new Rectangle((coord1 / 32) * 50, 0, 50, 60), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                    }
                   

                }
                
                if( eqItems.body.id == -1)
                    spriteBatch.Draw(arms, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                if (eqItems.gloves.id != -1)
                    spriteBatch.Draw(leftGloves, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                else
                    spriteBatch.Draw(frontHand, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                if (eqItems.body.id != -1)
                    spriteBatch.Draw(leftBody, playerAnimationCoordinates, new Rectangle(coord1, coord2, coord3, coord4), Color.White, 0, Vector2.Zero, 1, spriteEffect, 1);
                
            }

            spriteBatch.Draw(castSkillBar, new Vector2(playerAnimationCoordinates.X + 16 - 20, playerAnimationCoordinates.Y - 4), Color.White);
            foreach (Triplet<int, short, Point> dmg in dmgQue)
                spriteBatch.DrawString(dmgFont, Convert.ToString(dmg.Item1), new Vector2(playerAnimationCoordinates.X + dmg.Item3.X, playerAnimationCoordinates.Y + dmg.Item3.Y - 30), new Color(dmg.Item2, dmg.Item2, dmg.Item2));
        }


        public void Draw()
        {
            spriteBatch.Begin();

            if (playerState != null && playerState[5]==0)
            {

                if (playerState[2] == 0 && playerState[3] == 0 && playerState[0] == 0 && playerState[1] == 0)
                {
                    playerAnimation(0, 0, 32, 64);
                }
                else if (playerState[0] == 1)
                {
                    playerAnimation(8 * 32, 0, 32, 64);
                   // playerAnimation(0, 0, 100, 100);
                }
                else if (playerState[1] == 1)
                {
                     playerAnimation(9 * 32, 0, 32, 64);
                    //playerAnimation(0, 0, 100, 100);
                }
                else if (playerState[2] == 1)
                {
                    playerAnimation((animationIndex ) * 32, 0, 32, 64);
                    //playerAnimation(0, 0, 100, 100);
                }
                else if (playerState[3] == 1)
                {
                    playerAnimation((animationIndex ) * 32, 0, 32, 64);
                   // playerAnimation(0, 0, 100, 100);
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
