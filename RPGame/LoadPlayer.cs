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
   
    class Buff
    {
        public List<Add> add;
        public int time;
        public int cTime;
        public int buffId;

        public Buff(List<Add> add,int time,int buffId)
        {
            this.buffId = buffId;
            this.add = add;
            this.time = time;
            cTime = 0;
        }
    }

    class LoadPlayer
    {
        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;
        Stats playerStats;
        Points playerPoints;

        
        int[][][][][] map;
        int coordX, coordY;
        int playerX, playerY;
        int centerX, centerY;
        KeyboardState keyboard,prevKeyboard;
        MouseState mouse;
        SpriteFont font;
        int fallingSpeed, jumpingSpeed;
        int[] playerState;
        List<Buff> buffs = new List<Buff>();
        List<Buff> cancelBuffs = new List<Buff>();
        Skills skills;
        

        bool fell,knockBacked,attackPenalty;
        int rightCorrect, leftCorrect;
        int direction;
        int attIndex,penaltyIndex,hpRegenIndex,manaRegenIndex;
        int respawnTime, currentRespawn;
        int knockBack, knockBackIndex;
        int maxSpeed;
        int penaltyTime;

        public LoadPlayer(ContentManager Content,GraphicsDevice graphics,GameWindow Window)
        {
            this.Content=Content;
            this.graphics = graphics;
            this.Window = Window;
            

        }

        public void Initialize(int[][][][][] map)
        {

            spriteBatch = new SpriteBatch(graphics);
            this.map = map;
            
            coordX = 0-Window.ClientBounds.Width+800;
            coordY = 600-Window.ClientBounds.Height+600;

            if (coordX < 0)
                coordX = 0;
            if (coordY < 0)
                coordY = 0;

            playerStats = new Stats(maxHp:1000, hp:1000,maxMana:1000,mana:1000, physPow:10,physDef:10, attSpeed:10,castSpeed:5, attackRange:30, jumpingHeight:10, scrollSpeed:3, spawnPoint:new Point(coordX, coordY),hpRegen:3,pen:70);
            playerPoints = new Points(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            
            




            InitializePlayer();
            //attackRange = 50;
           // jumpingHeight = 17;
          //  scrollSpeed = 3;

        }

        public void InitializePlayer()
        {
            coordX = playerStats.spawnPoint.X;
            coordY = playerStats.spawnPoint.Y;

            playerX = 32+Window.ClientBounds.Width-800;
            playerY = 3+Window.ClientBounds.Height-600;

            centerX = Window.ClientBounds.Width / 2 - 16;
            centerY = Window.ClientBounds.Height / 2 - 32;

            playerState = new int[] { 0, 1, 0, 0, 0, 0 ,0};
            fallingSpeed = 0;
            jumpingSpeed = playerStats.jumpingHeight;
            fell = false;
            rightCorrect = 0;
            leftCorrect = 0;
            direction = 1;
            attIndex = 0;
            respawnTime = 10;
            currentRespawn = 0;
            knockBack = 7;
            knockBackIndex = 0;
            knockBacked = false;
            penaltyIndex = 0;
            penaltyTime = 30;
            maxSpeed = playerStats.scrollSpeed;
            attackPenalty = false;
            hpRegenIndex = 0;
            manaRegenIndex = 0;
        }

        public void LoadContent()
        {
           

            font = Content.Load<SpriteFont>("Game/chatFont");
            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();
        }

        

         void Falling()
        {
            if (coordY <= map[0].Length * 180 - Window.ClientBounds.Height - fallingSpeed && playerY==centerY)
            {
                if (playerStats.direction==1 && map[(coordX + playerX+leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + leftCorrect+18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect+18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 || playerStats.direction == 0 && map[(coordX + playerX + 32 - leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + 32 - leftCorrect - 18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect - 18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1)
                {
                    // coordY+=2;
                    coordY += fallingSpeed;
                    if (fallingSpeed < playerStats.jumpingHeight)
                        fallingSpeed++;
                    
                }
                else
                    fell = true;

                if (fell)
                {
                    if (playerStats.direction == 1 && map[(coordX + playerX+leftCorrect) / 180][(coordY + playerY + 64) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY + 64) % 180) / 18][2] == 0 && map[(coordX + playerX + leftCorrect+18) / 180][(coordY + playerY + 64) / 180][((coordX + playerX + leftCorrect+18) % 180) / 18][((coordY + playerY + 64) % 180) / 18][2] == 0 || playerStats.direction == 0 && map[(coordX + playerX + 32 - leftCorrect) / 180][(coordY + playerY  + 64) / 180][((coordX + playerX + 32 - leftCorrect) % 180) / 18][((coordY + playerY  + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + 32 - leftCorrect - 18) / 180][(coordY + playerY  + 64) / 180][((coordX + playerX + 32 - leftCorrect - 18) % 180) / 18][((coordY + playerY  + 64) % 180) / 18][2] != 1)
                        coordY += (18 - ((coordY + playerY + 64) % 180) % 18);
                    else
                        coordY -= ((coordY + playerY + 64) % 180) % 18;
                    fallingSpeed = 0;
                    fell = false;
                    
                    playerState[1] = 0;
                }

            }

            else
            {
                if (playerY <= Window.ClientBounds.Height)
                {
                    if (playerStats.direction == 1 && map[(coordX + playerX + leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + leftCorrect + 18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect + 18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 || playerStats.direction == 0 && map[(coordX + playerX + 32 - leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + 32 - leftCorrect - 18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect - 18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1)
                    {

                        if(playerY<centerY && playerY+fallingSpeed>=centerY)
                        {
                            coordY += playerY + fallingSpeed - centerY;
                            playerY = centerY;
                            if (fallingSpeed < playerStats.jumpingHeight)
                                fallingSpeed++;
                            return;
                        }
                        // coordY+=2;
                        playerY += fallingSpeed;
                        
                        if (fallingSpeed < 8)
                            fallingSpeed++;

                        

                    }
                    else
                        fell = true;

                    if (fell)
                    {
                        if (playerStats.direction == 1 && map[(coordX + playerX + leftCorrect) / 180][(coordY + playerY + 64) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY + 64) % 180) / 18][2] == 0 && map[(coordX + playerX + leftCorrect+18) / 180][(coordY + playerY + 64) / 180][((coordX + playerX + leftCorrect+18) % 180) / 18][((coordY + playerY + 64) % 180) / 18][2] == 0 || playerStats.direction == 0 && map[(coordX + playerX + 32 - leftCorrect) / 180][(coordY + playerY  + 64) / 180][((coordX + playerX + 32 - leftCorrect) % 180) / 18][((coordY + playerY  + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + 32 - leftCorrect - 18) / 180][(coordY + playerY  + 64) / 180][((coordX + playerX + 32 - leftCorrect - 18) % 180) / 18][((coordY + playerY  + 64) % 180) / 18][2] != 1)
                            playerY += (18 - ((coordY + playerY + 64) % 180) % 18);
                        else
                            playerY -= ((coordY + playerY + 64) % 180) % 18;
                        fallingSpeed = 0;
                        fell = false;
                        playerState[1] = 0;
                    }


                }
            }
        }

        void Jumping()
        {
            if (coordY >jumpingSpeed && playerY==centerY)
            {
                if (map[(coordX + playerX+leftCorrect) / 180][(coordY + playerY - 1) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY - 1) % 180) / 18][2] == 1)
                {
                   
                    jumpingSpeed = 0;

                }

                    if (jumpingSpeed>0)
                {
                    coordY -= jumpingSpeed;
                      
                    jumpingSpeed--;
                }
                else
                {
                   
                    playerState[0] = 0;

                    
                    jumpingSpeed = playerStats.jumpingHeight;
                }
            }
            else
            {
                if (map[(coordX + playerX + leftCorrect) / 180][(coordY + playerY - 1) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY - 1) % 180) / 18][2] == 1)
                    jumpingSpeed = 0;


                if(playerY>centerY && playerY-jumpingSpeed<=centerY)
                {
                    coordY -= (centerY - playerY + jumpingSpeed);
                    playerY = centerY;
                    jumpingSpeed--;
                    return;
                }

                

                if (jumpingSpeed>0)
                {
                    playerY -= jumpingSpeed;

                    jumpingSpeed--;
                       
                }
                else
                {
                    
                    playerState[0] = 0;

                    jumpingSpeed= playerStats.jumpingHeight;
                }
            }
        }

        void MovingRight()
        {
            if (playerX + 1 + 32 - rightCorrect+playerStats.scrollSpeed<=Window.ClientBounds.Width && map[(coordX + playerX + 1+32-rightCorrect) / 180][(coordY + playerY) / 180][((coordX + playerX + 1 + 32 - rightCorrect ) % 180) / 18][((coordY + playerY) % 180) / 18][2] != 1 && map[(coordX + playerX + 1 + 32- rightCorrect ) / 180][(coordY + playerY+18) / 180][((coordX + playerX + 1 + 32- rightCorrect ) % 180) / 18][((coordY + playerY+18) % 180) / 18][2] != 1 && map[(coordX + playerX + 1 + 32 - rightCorrect ) / 180][(coordY + playerY+36) / 180][((coordX + playerX + 1 + 32 - rightCorrect ) % 180) / 18][((coordY + playerY+36) % 180) / 18][2] != 1 && map[(coordX + playerX + 1 + 32 - rightCorrect ) / 180][(coordY + playerY+54) / 180][((coordX + playerX + 1 + 32 - rightCorrect ) % 180) / 18][((coordY + playerY+54) % 180) / 18][2] != 1 )
            {
                if (coordX <= map.Length * 180 - Window.ClientBounds.Width - playerStats.scrollSpeed && playerX >= centerX)
                {
                    coordX += playerStats.scrollSpeed;
                    
                }
                else if (playerX <= Window.ClientBounds.Width - rightCorrect)
                {
                    if (playerX < centerX && playerX + playerStats.scrollSpeed >= centerX)
                    {
                        coordX += playerX + playerStats.scrollSpeed - centerX;
                        playerX = centerX;
                        
                        return;
                    }
                    playerX += playerStats.scrollSpeed;
                    

                }
            }
        }

        void MovingLeft()
        {
            if (playerX - 1 + rightCorrect - playerStats.scrollSpeed >= 0 && map[(coordX + playerX - 1+rightCorrect ) / 180][(coordY + playerY) / 180][((coordX + playerX - 1 + rightCorrect) % 180) / 18][((coordY + playerY) % 180) / 18][2] != 1 && map[(coordX + playerX - 1 + rightCorrect) / 180][(coordY + playerY + 18) / 180][((coordX + playerX -1 + rightCorrect) % 180) / 18][((coordY + playerY + 18) % 180) / 18][2] != 1 && map[(coordX + playerX - 1 + rightCorrect) / 180][(coordY + playerY + 36) / 180][((coordX + playerX - 1 + rightCorrect) % 180) / 18][((coordY + playerY + 36) % 180) / 18][2] != 1 && map[(coordX + playerX - 1 +rightCorrect) / 180][(coordY + playerY + 54) / 180][((coordX + playerX - 1 + rightCorrect) % 180) / 18][((coordY + playerY + 54) % 180) / 18][2] != 1 )
            {
                if (coordX >= playerStats.scrollSpeed && playerX <= centerX)
                    coordX -= playerStats.scrollSpeed;
                else if (playerX >= playerStats.scrollSpeed-rightCorrect)
                {
                    if(playerX>centerX && playerX- playerStats.scrollSpeed <=centerX)
                    {
                        coordX -= (centerX - playerX + playerStats.scrollSpeed);
                        playerX = centerX;
                        return;
                    }
                    playerX -= playerStats.scrollSpeed;

                }

            }
        }

        public void receiveDmg(int dmg,out Stats playerStatsOut)
        {
            playerStats.hp -= dmg;
            if (playerStats.hp <= 0)
            {
                playerStats.hp = 0;
                playerState[5] = 1;
            }
            playerStatsOut = playerStats;
        }

        void KnockBack()
        {
            if (knockBackIndex > 0)
            {


                MovingRight();
                if (playerState[1] == 0)
                    Jumping();
                knockBackIndex--;
                knockBacked = true;
            }
            else if (knockBackIndex < 0)
            {

                MovingLeft();
                if (playerState[1] == 0)
                    Jumping();
                knockBackIndex++;
                knockBacked = true;
            }

            else if (knockBacked && playerState[1] == 0)
            {
                knockBacked = false;
                playerState[6] = 0;
            }

        }

        public void Update(bool firstTime,out int[] playerStateOut,out Stats playerStatsOut,out Points outPlayerPoints,Stats playerStatsIn,Points playerPointsIn, bool attack,int direction,int xpEarned,bool block,bool magicAttack,int attackSkillIndex,int buffId,out List<Buff> buffsOut,bool isFocus)
        {
            if(!isFocus)
                keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
            playerPoints = playerPointsIn;

            if (!firstTime)
                playerStats = playerStatsIn;

            


                if (attackSkillIndex<=0 && buffId!=-1)
            {
                int o = 0;
                foreach (Buff buff in buffs)
                    if (buff.buffId == buffId)
                    {
                        o = 1;
                        buff.cTime = 0;
                    }

                if (o == 0)
                {
                    if(skills.skillList[buffId].time!=0)
                        buffs.Add(new Buff(skills.skillList[buffId].add, skills.skillList[buffId].time, buffId));
                    foreach (Add add in skills.skillList[buffId].add)
                        switch (add.stat)
                        {
                            case "pstr":
                                playerStats.physPow += add.val;
                                break;

                            case "mstr":
                                playerStats.mPow += add.val;
                                break;

                            case "pdef":
                                playerStats.physDef += add.val;
                                break;

                            case "mdef":
                                playerStats.mDef += add.val;
                                break;

                            case "pas":
                                playerStats.attSpeed += add.val;
                                break;

                            case "mas":
                                playerStats.castSpeed += add.val;
                                break;

                            case "mp":
                                playerStats.maxMana += add.val;
                                break;

                            case "hp":
                                playerStats.maxHp += add.val;
                                break;

                            case "eva":
                                playerStats.eva += add.val;
                                break;

                            case "pc":
                                playerStats.physCrits += add.val;
                                break;

                            case "mc":
                                playerStats.mCrits += add.val;
                                break;
                            case "attackRange":
                                playerStats.attackRange += add.val;
                                break;
                            case "rhp":
                                playerStats.hp += add.val;
                                if (playerStats.hp > playerStats.maxHp)
                                    playerStats.hp = playerStats.maxHp;
                                break;


                        }
                }
            }

            foreach(Buff buff in buffs)
            {
                if(buff.cTime<buff.time)
                    buff.cTime++;
                else
                {
                    cancelBuffs.Add(buff);
                    foreach (Add add in buff.add)
                        switch (add.stat)
                        {
                            case "pstr":
                                playerStats.physPow -= add.val;
                                break;

                            case "mstr":
                                playerStats.mPow -= add.val;
                                break;

                            case "pdef":
                                playerStats.physDef -= add.val;
                                break;

                            case "mdef":
                                playerStats.mDef -= add.val;
                                break;

                            case "pas":
                                playerStats.attSpeed -= add.val;
                                break;

                            case "mas":
                                playerStats.castSpeed -= add.val;
                                break;

                            case "mp":
                                playerStats.maxMana -= add.val;
                                break;

                            case "hp":
                                playerStats.maxHp -= add.val;
                                break;

                            case "eva":
                                playerStats.eva -= add.val;
                                break;

                            case "pc":
                                playerStats.physCrits -= add.val;
                                break;

                            case "mc":
                                playerStats.mCrits -= add.val;
                                break;
                            case "attackRange":
                                playerStats.attackRange -= add.val;
                                break;

                        }
                }
            }

            foreach (Buff buff in cancelBuffs)
                buffs.Remove(buff);

            cancelBuffs.Clear();


            

            if (hpRegenIndex < 60)
            {
                hpRegenIndex++;
            }
            else
            {
                if (playerStats.hp < playerStats.maxHp)
                {
                    playerStats.hp += playerStats.hpRegen;
                }

                if (playerStats.hp > playerStats.maxHp)
                    playerStats.hp = playerStats.maxHp;

                hpRegenIndex = 0;

            }

            if (manaRegenIndex < 60)
            {
                manaRegenIndex++;
            }
            else
            {
                if (playerStats.mana < playerStats.maxMana)
                {
                    playerStats.mana += playerStats.manaRegen;
                }

                if (playerStats.mana > playerStats.maxMana)
                    playerStats.mana = playerStats.maxMana;

                manaRegenIndex = 0;

            }


            playerStats.xp += xpEarned;

            while(playerStats.xp>=playerStats.maxXp)
            {
                playerStats.xp -= playerStats.maxXp;
                playerStats.maxXp = (int)(playerStats.maxXp*1.05);
                playerStats.lvl++;
                playerStats.maxHp = (int)(playerStats.maxHp * 1.01);
                playerStats.maxMana = (int)(playerStats.maxMana * 1.01);
                playerStats.hp = playerStats.maxHp;
                playerStats.mana = playerStats.maxMana;
                playerPoints.points ++;
            }
           
            if(playerState[5]==1 )
            {
                currentRespawn++;
                if (currentRespawn / 60 >= respawnTime)
                {
                    InitializePlayer();
                    playerStats.hp = (int)(0.8 * playerStats.maxHp);
                }
            }



            if (attack && playerState[1] == 0 && direction==1)
            {
                jumpingSpeed = playerStats.jumpingHeight;
                playerState[6] = 1;
                knockBackIndex = knockBack;
            }
            else if(attack && playerState[1] == 0 && direction == 0)
            {
                jumpingSpeed = playerStats.jumpingHeight;
                playerState[6] = 1;
                knockBackIndex = -knockBack;
            }

            if (playerState[5] == 0 )
            {
                if (attackSkillIndex <= 0)
                {
                    if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                    {
                        playerState[2] = 1;
                        this.direction = 1;
                    }
                    else
                        playerState[2] = 0;

                    if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                    {
                        this.direction = 0;
                        playerState[3] = 1;
                    }
                    else
                        playerState[3] = 0;

                    if (keyboard.IsKeyDown(Keys.Space) && playerState[1] == 0 && keyboard != prevKeyboard)
                        playerState[0] = 1;

                    playerState[4] = 0;

                    if (mouse.LeftButton == ButtonState.Pressed && attIndex <= 0 && !block)
                    {
                        playerState[4] = 1;
                        attackPenalty = true;
                        attIndex = 600 / playerStats.attSpeed;
                    }
                    else
                    {
                        attIndex--;

                    }

                    if (magicAttack)
                    {
                        attackPenalty = true;
                    }

                    if (penaltyIndex >= penaltyTime)
                        attackPenalty = false;

                    if (attackPenalty)
                    {
                        playerStats.scrollSpeed = 1;
                        penaltyIndex++;
                    }
                    else
                    {
                        playerStats.scrollSpeed = 3;
                        penaltyIndex = 0;
                    }

                }
                else
                {
                    playerState[3] = 0;
                    playerState[2] = 0;
                    playerState[4] = 0;
                }
                


                if (playerState[0] == 1)
                    Jumping();
                if (playerState[1] == 1)
                    Falling();
                if (playerState[2] == 1)
                    MovingRight();
                if (playerState[3] == 1)
                    MovingLeft();
                if (playerState[6] == 1)
                    KnockBack();


                if (playerStats.direction==1 && map[(coordX + playerX +leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + leftCorrect+18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + leftCorrect+18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && playerState[0] == 0 || playerStats.direction == 0 && map[(coordX + playerX +32- leftCorrect) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && map[(coordX + playerX + 32 - leftCorrect - 18) / 180][(coordY + playerY + 1 + 64) / 180][((coordX + playerX + 32 - leftCorrect - 18) % 180) / 18][((coordY + playerY + 1 + 64) % 180) / 18][2] != 1 && playerState[0] == 0)
                    playerState[1] = 1;

                if (playerStats.direction == 1 && playerState[2]==1 && map[(coordX + playerX + 32 - rightCorrect + 1) / 180][(coordY + playerY) / 180][((coordX + playerX - rightCorrect + 32 + 1) % 180) / 18][((coordY + playerY) % 180) / 18][2] != 1 && map[(coordX + playerX - rightCorrect + 32 + 1) / 180][(coordY + playerY + 18) / 180][((coordX + playerX - rightCorrect + 32 + 1) % 180) / 18][((coordY + playerY + 18) % 180) / 18][2] != 1 && map[(coordX + playerX - rightCorrect + 32 + 1) / 180][(coordY + playerY + 36) / 180][((coordX + playerX - rightCorrect + 32 + 1) % 180) / 18][((coordY + playerY + 36) % 180) / 18][2] != 1 && map[(coordX + playerX - rightCorrect + 32 + 1) / 180][(coordY + playerY + 54) / 180][((coordX + playerX - rightCorrect + 32 + 1) % 180) / 18][((coordY + playerY + 54) % 180) / 18][2] == 1  || playerStats.direction == 0 && playerState[3]==1 && map[(coordX + playerX +  rightCorrect - 1) / 180][(coordY + playerY) / 180][((coordX + playerX + rightCorrect - 1) % 180) / 18][((coordY + playerY) % 180) / 18][2] != 1 && map[(coordX + playerX + rightCorrect - 1) / 180][(coordY + playerY + 18) / 180][((coordX + playerX + rightCorrect - 1) % 180) / 18][((coordY + playerY + 18) % 180) / 18][2] != 1 && map[(coordX + playerX + rightCorrect - 1) / 180][(coordY + playerY + 36) / 180][((coordX + playerX + rightCorrect - 1) % 180) / 18][((coordY + playerY + 36) % 180) / 18][2] != 1 && map[(coordX + playerX + rightCorrect - 1) / 180][(coordY + playerY + 54) / 180][((coordX + playerX + rightCorrect - 1) % 180) / 18][((coordY + playerY + 54) % 180) / 18][2] == 1 )
                {
                    jumpingSpeed = 5;
                    playerState[0] = 1;
                }
            }

            prevKeyboard = keyboard;

           
            playerStats.modelX = playerX;
            playerStats.modelY = playerY;
            playerStats.coordX = coordX;
            playerStats.coordY = coordY;
            playerStats.direction = this.direction;

            playerStatsOut = playerStats;
            playerStateOut = playerState;
            outPlayerPoints = playerPoints;
            buffsOut = buffs;
            
           
        }

        public void Draw()
        {
            spriteBatch.Begin();
           
            if(playerState[5]==1)
            {
                string str = "You have died and will respawn in " + (respawnTime-currentRespawn/60)+"s";
                spriteBatch.DrawString(font,str,new Vector2(Window.ClientBounds.Width/2-font.MeasureString(str).X/2, Window.ClientBounds.Height / 2 - font.MeasureString(str).Y/2),Color.Black);
            }

            spriteBatch.DrawString(font, "X:" + (playerStats.coordX+playerStats.modelX) + "Y:" + (playerStats.coordY+playerStats.modelY), new Vector2(Window.ClientBounds.Width - font.MeasureString("X:" + playerStats.coordX + playerStats.modelX + "Y:" + playerStats.coordY + playerStats.modelY).X,font.MeasureString("X:" + playerStats.coordX + playerStats.modelX + "Y:" + playerStats.coordY + playerStats.modelY).Y), Color.Black);
            spriteBatch.End();
        }

    }
}
