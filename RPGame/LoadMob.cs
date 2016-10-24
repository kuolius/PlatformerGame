using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGame
{
    class LoadMob
    {

        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;
        KeyboardState keyboard, prevKeyboard;
        MouseState mouse;
        Stats mobStats;


        int[][][][][] map;
        int mobX, mobY;
        int walkingRange,walkingIndex;
        Random walkingRand;
        int mobFps;


        int fallingSpeed, jumpingSpeed;
        int[] mobState;
        bool fell;
        int rightCorrect, leftCorrect;

        int aggroRange,followRange,aggressiveFollowIndex;

        bool following,knockBacked,toSpawnPoint,aggressiveFollow;
        int knockBack, knockBackIndex;
        int attIndex;
        int magicId, magicIndex;


        public LoadMob(ContentManager Content, GraphicsDevice graphics, GameWindow Window,  int[][][][][] map,Mob mob)
        {

            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
            this.mobX = mob.coords.X;
            this.mobY = mob.coords.Y;
            this.map = map;
            Initialize(mob);
        }

        public void Initialize(Mob mob)
        {
            spriteBatch = new SpriteBatch(graphics);

            mobStats = new Stats(mob.maxHp,mob.hp,mob.mp,mob.maxMp,mob.physPow,mob.physDef,mob.pas,mob.attRange,10,2,new Point(mob.coords.X,mob.coords.Y),lvl:mob.lvl,xp:mob.xp,hpRegen:mob.hpRegen,manaRegen:mob.mpRegen,mDef:mob.mDef,mPow:mob.mPow,castSpeed:mob.mas,eva:mob.eva,physCrits:mob.pc,mCrits:mob.mc,fPen:mob.fPen,pen:mob.pen,acc:mob.acc);
            

            mobState = new int[] { 0, 1, 0, 0, 0 ,0};
            fallingSpeed = 0;
            jumpingSpeed = mobStats.jumpingHeight;
            fell = false;
            following = false;
            rightCorrect = 4;
            leftCorrect = 11;
            aggroRange = 100;
            followRange = 100;
            knockBack = 7;
            knockBackIndex = 0;
            knockBacked = false;
            toSpawnPoint = false;
            aggressiveFollow = false;
            attIndex = 0;
            aggressiveFollowIndex = 0;
            magicId = -1;
            magicIndex = 0;
            walkingRange = 200;
            walkingRand = new Random();
            walkingIndex = 0;
            mobFps = 0;

        }

        public void LoadContent()
        {


        }



        void Falling()
        {
            if (mobY <= map[0].Length * 180 - fallingSpeed)
            {
                if (map[(mobX + leftCorrect) / 180][(mobY + 1 + 63) / 180][((mobX + leftCorrect) % 180) / 18][((mobY + 1 + 63) % 180) / 18][2] != 1)
                {
                    // coordY+=2;
                    mobY += fallingSpeed;
                    if (fallingSpeed < 6)
                        fallingSpeed++;

                }
                else
                    fell = true;

                if (fell)
                {
                    if (map[(mobX + leftCorrect) / 180][(mobY + 63) / 180][((mobX + leftCorrect) % 180) / 18][((mobY + 63) % 180) / 18][2] == 0)
                        mobY += (18 - ((mobY + 63) % 180) % 18);
                    else
                        mobY -= ((mobY + 63) % 180) % 18;
                    fallingSpeed = 0;
                    fell = false;

                    mobState[1] = 0;
                }

            }


        }

        void Jumping()
        {
          
            if (mobY > jumpingSpeed)
            {
                if (map[(mobX + leftCorrect) / 180][(mobY - 1) / 180][((mobX + leftCorrect) % 180) / 18][((mobY - 1) % 180) / 18][2] == 1)
                {

                    jumpingSpeed = 0;

                }

                if (jumpingSpeed > 0)
                {
                    mobY -= jumpingSpeed;

                    jumpingSpeed--;
                }
                else
                {
                    mobState[1] = 1;
                    mobState[0] = 0;

                   
                        jumpingSpeed = mobStats.jumpingHeight;
                }
            }

        }

        void MovingRight()
        {
            if ( map[(mobX + 1 + 32 - rightCorrect) / 180][(mobY) / 180][((mobX + 1 + 32 - rightCorrect) % 180) / 18][((mobY) % 180) / 18][2] != 1 && map[(mobX + 1 + 32 - rightCorrect) / 180][(mobY + 18) / 180][((mobX + 1 + 32 - rightCorrect) % 180) / 18][((mobY + 18) % 180) / 18][2] != 1 && map[(mobX + 1 + 32 - rightCorrect) / 180][(mobY + 36) / 180][((mobX + 1 + 32 - rightCorrect) % 180) / 18][((mobY + 36) % 180) / 18][2] != 1 && map[(mobX + 1 + 32 - rightCorrect) / 180][(mobY + 54) / 180][((mobX + 1 + 32 - rightCorrect) % 180) / 18][((mobY + 54) % 180) / 18][2] != 1)
            {
                if (mobX <= map.Length * 180 - mobStats.scrollSpeed)
                    mobX += mobStats.scrollSpeed;
                
            }
        }

        void MovingLeft()
        {
            if (map[(mobX - 1) / 180][(mobY) / 180][((mobX - 1) % 180) / 18][((mobY) % 180) / 18][2] != 1 && map[(mobX - 1) / 180][(mobY + 18) / 180][((mobX - 1) % 180) / 18][((mobY + 18) % 180) / 18][2] != 1 && map[(mobX - 1) / 180][(mobY + 36) / 180][((mobX - 1) % 180) / 18][((mobY + 36) % 180) / 18][2] != 1 && map[(mobX - 1) / 180][(mobY + 54) / 180][((mobX - 1) % 180) / 18][((mobY + 54) % 180) / 18][2] != 1)
            {
                if (mobX >= mobStats.scrollSpeed)
                    mobX -= mobStats.scrollSpeed;


            }
        }


        void KnockBack()
        {
            if (knockBackIndex > 0)
            {


                MovingRight();
                if (mobState[1] == 0)
                    Jumping();
                knockBackIndex--;
                knockBacked = true;
            }
            else if (knockBackIndex < 0)
            {
               
                MovingLeft();
                if(mobState[1]==0)
                    Jumping();
                knockBackIndex++;
                knockBacked = true;
            }

            else if (knockBacked && mobState[1] == 0)
            {
                knockBacked = false;
                mobState[5] = 0;
            }

            }


        public void receiveDmg(int dmg)
        {
            mobStats.hp -= dmg;
            aggressiveFollow = true;
            aggressiveFollowIndex = 120;
            if (mobStats.hp < 0)
            {
                mobStats.hp = 0;
                aggressiveFollow = false;
                aggressiveFollowIndex = 0;

            }
        }

       

        public void Update( out int[] mobStateOut,out Stats mobStatsOut,int playerState,Stats playerStats,int playerDead,out bool attack)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
            mobState[4] = 0;
            attack = false;
            

           


            if (aggressiveFollowIndex <= 0)
                aggressiveFollow = false;
            else
            {
                aggressiveFollowIndex--;
            }
           

            if (playerState == 1 && playerStats.direction == 1 && Math.Pow(Math.Pow(mobX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobY - playerStats.modelY - playerStats.coordY, 2), .5) <= playerStats.attackRange && mobX > playerStats.modelX + playerStats.coordX && mobState[1] == 0)
            {
                jumpingSpeed = mobStats.jumpingHeight;
                mobState[5] = 0;
                knockBackIndex = knockBack;
            }
            else if (playerState == 1 && playerStats.direction == 0 && Math.Pow(Math.Pow(mobX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobY - playerStats.modelY - playerStats.coordY, 2), .5) <= playerStats.attackRange && mobX <= playerStats.modelX + playerStats.coordX && mobState[1] == 0)
            {
                jumpingSpeed = mobStats.jumpingHeight;
                mobState[5] = 0;
                knockBackIndex = -knockBack;
            }

            

                if (!knockBacked && playerDead==0)
            {
                if (mobState[1] == 0 && Math.Pow(Math.Pow(mobX - playerStats.modelX-playerStats.coordX,2)+ Math.Pow(mobY - playerStats.modelY - playerStats.coordY,2),.5) <= aggroRange && !following || following && Math.Pow(Math.Pow(mobX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobY - playerStats.modelY - playerStats.coordY, 2), .5) <= followRange || aggressiveFollow)
                {
                    following = true;
                    toSpawnPoint = false;
                    if (Math.Pow(Math.Pow(mobX - playerStats.modelX- playerStats.coordX,2)+ Math.Pow(mobY - playerStats.modelY - playerStats.coordY, 2),.5) > mobStats.attackRange && mobX - playerStats.modelX - playerStats.coordX>0)
                    {
                        if (map[(mobX + leftCorrect - 1) / 180][(mobY + 54) / 180][((mobX + leftCorrect - 1) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1 && mobState[1]==0)
                        {
                            mobY -= 18;
                            mobX -= 8;
                        }
                        mobState[3] = 1;
                        mobStats.direction = 0;
                        mobState[2] = 0;
                        mobState[4] = 0;
                       
                    }
                    else if (mobX - playerStats.modelX - playerStats.coordX < 0 && Math.Pow(Math.Pow(mobX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobY - playerStats.modelY - playerStats.coordY, 2), .5) > mobStats.attackRange)
                    {
                        if (map[(mobX + leftCorrect + 33) / 180][(mobY + 54) / 180][((mobX + leftCorrect + 33) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1&& mobState[1]==0)
                        {
                            mobY -= 18;
                            mobX += 8;
                        }
                        mobState[2] = 1;
                        mobStats.direction = 1;
                        mobState[3] = 0;
                        mobState[4] = 0;
                        
                    }
                    else
                    {
                        mobState[2] = 0;
                        mobState[3] = 0;

                        if (attIndex <= 0)
                        {
                            mobState[4] = 1;
                            attack = true;
                            attIndex =  600/mobStats.attSpeed;
                        }
                        else
                            attIndex--;
                        
                    }

                    


                }
                else
                {
                    
                    following = false;
                    aggressiveFollow = false;
                    mobState[2] = 0;
                    mobState[3] = 0;

                    if (mobState[0] + mobState[1] == 0)
                    {
                        if (mobStats.direction == 1 && mobStats.modelX - mobStats.spawnPoint.X < walkingRange && walkingIndex != 0 && mobStats.modelX < map.Length * 180 - 2)
                        {
                            if (map[(mobX + leftCorrect + 33) / 180][(mobY + 54) / 180][((mobX + leftCorrect + 33) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1 && mobState[1] == 0)
                            {
                                mobY -= 18;
                                mobX += 8;
                            }
                            mobState[2] = 1;
                            mobState[3] = 0;
                            walkingIndex--;
                        }
                        else if(mobStats.direction == 1 && mobStats.modelX - mobStats.spawnPoint.X < walkingRange && walkingIndex == 0 && mobStats.modelX < map.Length * 180 - 2)
                        {
                            mobStats.direction = 1;
                            if (mobFps == 120)
                            {
                                walkingIndex = walkingRand.Next(0, 201);
                                mobFps = 0;
                            }
                            else
                                mobFps++;
                        }
                        else if(mobStats.direction == 1 && (mobStats.modelX - mobStats.spawnPoint.X == walkingRange || mobStats.modelX>=map.Length*180-2))
                        {
                            mobStats.direction = 0;
                            if (mobFps == 120)
                            {
                                walkingIndex = walkingRand.Next(0, 201);
                                mobFps = 0;
                            }
                            else
                                mobFps++;
                        }

                        if(mobStats.direction==0 &&  mobStats.spawnPoint.X - mobStats.modelX < walkingRange && walkingIndex!=0 && mobStats.modelX > 2)
                        {
                            if (map[(mobX+leftCorrect - 1) / 180][(mobY + 54) / 180][((mobX + leftCorrect - 1) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1 && mobState[1] == 0)
                            {
                                mobY -= 18;
                                mobX -= 8;
                            }
                            mobState[2] = 0;
                            mobState[3] = 1;
                            walkingIndex--;
                        }
                        else if (mobStats.direction == 0 && mobStats.spawnPoint.X - mobStats.modelX < walkingRange && walkingIndex == 0 && mobStats.modelX >2)
                        {
                            mobStats.direction = 0;
                            if (mobFps == 120)
                            {
                                walkingIndex = walkingRand.Next(0, 201);
                                mobFps = 0;
                            }
                            else
                                mobFps++;
                        }
                        else if (mobStats.direction == 0 &&( mobStats.spawnPoint.X - mobStats.modelX == walkingRange || mobStats.modelX<=2))
                        {
                            mobStats.direction =1;
                            if (mobFps == 120)
                            {
                                walkingIndex = walkingRand.Next(0, 201);
                                mobFps = 0;
                            }
                            else
                                mobFps++;
                        }

                        if (Math.Abs(mobStats.modelX - mobStats.spawnPoint.X) > walkingRange)
                            toSpawnPoint = true;
                    }


                }
            }
            else
            {
               // toSpawnPoint = true;
                mobState[2] = 0;
                mobState[3] = 0;
            }


            


            if (toSpawnPoint && mobState[1] == 0 )
            {
                if(mobX-mobStats.spawnPoint.X>0)
                {
                    if (map[(mobX - 1) / 180][(mobY + 54) / 180][((mobX - 1) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1 && mobState[1] == 0)
                        mobState[0] = 1;
                    mobState[3] = 1;
                    mobStats.direction = 0;
                    mobState[2] = 0;
                }
                else if(mobX-mobStats.spawnPoint.X<0)
                {
                    if (map[(mobX + 33) / 180][(mobY + 54) / 180][((mobX + 33) % 180) / 18][((mobY + 54) % 180) / 18][2] == 1 && mobState[1] == 0)
                        mobState[0] = 1;
                    mobState[3] = 0;
                    mobStats.direction = 1;
                    mobState[2] = 1;
                }
                else
                {
                    toSpawnPoint = false;
                    mobState[3] = 0;
                    mobState[2] = 0;
                }
            }
            
           
           


            if (mobState[1] == 1)
                mobState[4] = 0;



            if (mobState[0] == 1)
                Jumping();
            if (mobState[1] == 1)
                Falling();
            if (mobState[2] == 1)
                MovingRight();
            if (mobState[3] == 1)
                MovingLeft();
            if (mobState[5] == 1)
                KnockBack();


            if (map[(mobX + leftCorrect) / 180][(mobY + 1 + 63) / 180][((mobX + leftCorrect) % 180) / 18][((mobY + 1 + 63) % 180) / 18][2] != 1 && mobState[0] == 0)
                mobState[1] = 1;


            prevKeyboard = keyboard;
            mobStateOut = mobState;

            mobStats.modelX = mobX;
            mobStats.modelY = mobY;

            mobStatsOut = mobStats;
        }

        public void Draw()
        {
        }

    }
}
