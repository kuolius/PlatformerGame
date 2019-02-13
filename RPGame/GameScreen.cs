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

    public struct Stats
    {
        public int maxHp,hp,physPow,physDef,mDef,mPow,attSpeed,castSpeed,eva,physCrits,mCrits,lvl,xp,maxXp,hpRegen,mana,maxMana,manaRegen,fPen,pen,acc;
        public int attackRange;
        public int jumpingHeight;
        public int scrollSpeed;
        public int coordX, coordY;
        public int modelX, modelY;
        public int direction;
        public Point spawnPoint;

        public Stats(int maxHp,int hp,int mana,int maxMana,int physPow,int physDef,int attSpeed,int attackRange,int jumpingHeight,int scrollSpeed, Point spawnPoint, int coordX=0,int coordY=0,int modelX=0,int modelY=0,int direction=1,int lvl=1,int xp=0,int maxXp=100,int hpRegen=1,int manaRegen=1,int mDef=100,int mPow=100,int castSpeed=60,int eva=1,int physCrits=1,int mCrits=1,int fPen=0,int pen=0,int acc=1)
        {
            this.maxHp = maxHp;
            this.hp = hp;
            this.attackRange = attackRange;
            this.jumpingHeight = jumpingHeight;
            this.scrollSpeed = scrollSpeed;
            this.coordX = coordX;
            this.coordY = coordY;
            this.modelX = modelX;
            this.modelY = modelY;
            this.direction = direction;
            this.physPow = physPow;
            this.attSpeed = attSpeed;
            this.spawnPoint = spawnPoint;
            this.physDef = physDef;
            this.lvl = lvl;
            this.xp = xp;
            this.maxXp = maxXp;
            this.hpRegen = hpRegen;
            this.mana = mana;
            this.maxMana = maxMana;
            this.manaRegen = manaRegen;
            this.mDef = mDef;
            this.mPow = mPow;
            this.castSpeed = castSpeed;
            this.eva = eva;
            this.physCrits = physCrits;
            this.mCrits = mCrits;
            this.fPen = fPen;
            this.pen = pen;
            this.acc = acc;
        }
    }

    public struct Points
    {
        public int pstr, mstr, pas, mas, hp, mp, mdef, pdef, eva, pc, mc,points;

        public Points(int pstr,int mstr,int pas,int mas,int hp,int mp,int mdef,int pdef,int eva,int pc, int mc,int points=0)
        {
            this.pstr = pstr;
            this.mstr = mstr;
            this.pas = pas;
            this.mas = mas;
            this.hp = hp;
            this.mp = mp;
            this.mdef = mdef;
            this.pdef = pdef;
            this.eva = eva;
            this.pc = pc;
            this.mc = mc;
            this.points = points;
        }

    }

    public class Mobs
    {
        [XmlElement("Mob")]
        public List<Mob> mobList = new List<Mob>();
    }

    public class Mob
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlElement("coords")]
        public Coord coords {get;set;}
        [XmlElement("respawnTime")]
        public int respawnTime { get; set; }
        public bool spawned;
        public int currentRespawnTime;
        [XmlElement("xp")]
        public int xp { get; set; }
        [XmlElement("hp")]
        public int hp { get; set; }
        [XmlElement("maxHp")]
        public int maxHp { get; set; }
        [XmlElement("lvl")]
        public int lvl { get; set; }
        [XmlElement("physPow")]
        public int physPow { get; set; }
        [XmlElement("physDef")]
        public int physDef { get; set; }
        [XmlElement("mPow")]
        public int mPow { get; set; }
        [XmlElement("mDef")]
        public int mDef { get; set; }
        [XmlElement("mp")]
        public int mp { get; set; }
        [XmlElement("maxMp")]
        public int maxMp { get; set; }
        [XmlElement("pas")]
        public int pas { get; set; }
        [XmlElement("mas")]
        public int mas { get; set; }
        [XmlElement("pc")]
        public int pc { get; set; }
        [XmlElement("mc")]
        public int mc { get; set; }
        [XmlElement("eva")]
        public int eva { get; set; }
        [XmlElement("acc")]
        public int acc { get; set; }
        [XmlElement("hpRegen")]
        public int hpRegen { get; set; }
        [XmlElement("mpRegen")]
        public int mpRegen { get; set; }
        [XmlElement("fPen")]
        public int fPen { get; set; }
        [XmlElement("pen")]
        public int pen { get; set; }
        [XmlElement("attackRange")]
        public int attRange { get; set; }
        [XmlElement("drop")]
        public List<Drop> dropList = new List<Drop>();
    }

   

    public class Coord
    {
        [XmlAttribute("X")]
        public int X { get; set; }
        [XmlAttribute("Y")]
        public int Y { get; set; }
    }

    public class Drop
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("val")]
        public int val { get; set; }
        [XmlAttribute("chance")]
        public int chance { get; set; }
        [XmlAttribute("from")]
        public int from { get; set; }
    }

    public class Projectile
    {
        public int Velocity=15;
        public int coordX, coordY;
        float fCoordX, fCoordY;
        Texture2D projectileTexture;
        SpriteBatch spriteBatch;
        public int startX, startY, endX, endY;
        public int direction;
        public int skillId;
        public int mobId;

        public Projectile(SpriteBatch spriteBatch,Texture2D projectileTexture,int startX,int startY,int mobId)
        {
            this.spriteBatch = spriteBatch;
            this.projectileTexture = projectileTexture;
            this.startX = startX;
            this.startY = startY;
            this.mobId = mobId;
            coordX = startX;
            coordY = startY;
            fCoordX = startX;
            fCoordY = startY;
            skillId = -1;
        }

        public void Update(int endX,int endY)
        {
            this.endX = endX;
            this.endY = endY;

            if (endX - startX <= 0)
                direction = 0;
            else
                direction = 1;

            fCoordX += (float)(Velocity * ((endX - startX) / (Math.Pow(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2), .5))));
            fCoordY += (float)(Velocity * ((endY - startY) / (Math.Pow(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2), .5))));

            coordX = (int)fCoordX;
            coordY = (int)fCoordY;
             
        }

        public void Draw(int playerX,int playerY)
        {
            spriteBatch.Begin();
            if(direction==1)
                spriteBatch.Draw(projectileTexture, new Vector2(coordX-playerX, coordY-playerY), color: Color.White, rotation: (float)(Math.Atan((endY - startY) / (float)(endX - startX))));
            else
                spriteBatch.Draw(projectileTexture, new Vector2(coordX - playerX, coordY - playerY), color: Color.White, rotation: (float)(Math.Atan((endY - startY) / (float)(endX - startX))),effects:SpriteEffects.FlipHorizontally);
            spriteBatch.End();
        }
    }

    class GameScreen
    {
        ContentManager Content;
        GraphicsDevice graphicsDevice;
        GameWindow Window;
        SpriteBatch spriteBatch;
        Cursor cursor;

        LoadMap loadMap;
        LoadPlayer loadPlayer;
        PlayerAnimation playerAnimation;
        StatusBox statusBox;
        PointsBox pointsBox;
        InventoryBox inventoryBox;
        StatsBox statsBox;
        SkillBar skillBar;
        BuffBox buffBox;
        SkillBox skillBox;
        ChatBox chatBox;



        int[][][][][] map;
        int[] mobState;
        int[] playerState;
        int mobDmg, playerDmg,xpEarned;
        int FPS;
        int prevCoordX, prevCoordY;
        int attSkillId,attSkillIndex,attSkillIdCasting,buffId;

        Mobs mobs;
        List<Tuple<LoadMob,MobAnimation,int>> spawnedMobs=new List<Tuple<LoadMob,MobAnimation,int>>();
        List<Tuple<LoadMob, MobAnimation,int>> despawnedMobs = new List<Tuple<LoadMob, MobAnimation,int>>();
        List<Projectile> projectiles = new List<Projectile>();
        List<Projectile> magicProjectiles = new List<Projectile>();
        List<Projectile> removeProjectiles = new List<Projectile>();
        List<Buff> buffs;
        List<Drop> dropList=new List<Drop>();

        int target;
        bool targeted;
        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;

        Stats playerStats,mobStats;
        Points playerPoints;
        EqItems eqItems;
        Skills skills;
        EqSkills eqSkills;
        Items items;
       
  
        bool attack,block,blockout,magicAttack;
        int attackMob;
        int mobDirection;
        bool firstTime,inBox;
        bool eqItemRanged;
        string eqItemSet;
        double range;
        bool rightDirection;
        bool startCasting,blockCasting,massCasting;
        string give,spawn;



        
 

        public GameScreen(ContentManager Content, GraphicsDevice graphicsDevice,GameWindow Window)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            this.Window = Window;
     
        }

        public void spawnMob(Mob mob,int num)
        {
            spawnedMobs.Add(new Tuple<LoadMob,MobAnimation, int>(new LoadMob(Content, graphicsDevice, Window,map ,mob),new MobAnimation(Content,graphicsDevice), num));
            
            
        }


        public Mob CopyMob(Mob mob)
        {
            Mob retMob = new Mob();
            retMob.id = mob.id;
            retMob.name = mob.name;
            retMob.coords = new Coord();
            retMob.respawnTime = mob.respawnTime;
            retMob.spawned = mob.spawned;
            retMob.xp = mob.xp;
            retMob.maxHp = mob.maxHp;
            retMob.hp = mob.hp;
            retMob.lvl = mob.lvl;
            retMob.physPow = mob.physPow;
            retMob.physDef = mob.physDef;
            retMob.mPow = mob.mPow;
            retMob.mDef = mob.mDef;
            retMob.mp = mob.mp;
            retMob.maxMp = mob.maxMp;
            retMob.pas = mob.pas;
            retMob.mas = mob.mas;
            retMob.pc = mob.pc;
            retMob.mc = mob.mc;
            retMob.eva = mob.eva;
            retMob.acc = mob.acc;
            retMob.hpRegen = mob.hpRegen;
            retMob.mpRegen = mob.mpRegen;
            retMob.fPen = mob.fPen;
            retMob.pen = mob.pen;
            retMob.attRange = mob.attRange;
            retMob.dropList = mob.dropList;
            return retMob;

        }

        public void Initialize()
        {
            cursor = new Cursor(Content, graphicsDevice);
            loadMap = new LoadMap(Content, graphicsDevice, Window);
            loadPlayer = new LoadPlayer(Content, graphicsDevice, Window);
            playerAnimation = new PlayerAnimation(Content, graphicsDevice);
            statusBox = new StatusBox(Content,  graphicsDevice, playerStats);
            pointsBox = new PointsBox(Content, graphicsDevice, Window);
            inventoryBox = new InventoryBox(Content, graphicsDevice, Window);
            statsBox=new StatsBox(Content, graphicsDevice, Window);
            skillBar = new SkillBar(Content, graphicsDevice, Window);
            buffBox = new BuffBox(Content, graphicsDevice,Window);
            skillBox=new SkillBox(Content, graphicsDevice, Window);
            chatBox = new ChatBox(Content, graphicsDevice, Window);

            loadMap.LoadContent(out map);
            mobDmg = -1;
            playerDmg= - 1;
            FPS = 0;
            xpEarned = 0;
            firstTime = true;
            inBox = false;
            targeted = false;
            massCasting = false;
            range = -1;
            target = -1;
            attSkillIndex = 0;
            startCasting = false;
            blockCasting = false;




            cursor.Initialize();
            loadMap.Initialize();
            loadPlayer.Initialize(map);
            playerAnimation.Initialize();
            statusBox.Initialize();
            pointsBox.Initialize();
            inventoryBox.Initialize();
            statsBox.Initialize();
            skillBar.Initialize();
            buffBox.Initialize();
            skillBox.Initialize();
            chatBox.Initialize();


            XmlSerializer deserializer = new XmlSerializer(typeof(Mobs));
            TextReader reader = new StreamReader("Content/Game/Mobs.xml");
            object obj = deserializer.Deserialize(reader);
            mobs = (Mobs)obj;
            reader.Close();




            foreach (Mob mob in mobs.mobList)
            {
                mob.currentRespawnTime = mob.respawnTime;
                mob.spawned = true;

            }
             SpawnAllMobs();





        }

        
        public void SpawnAllMobs()
        {
            foreach (Mob mob in mobs.mobList)
            {
                spawnMob(mob, mob.id);

            }
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            cursor.LoadContent();
            loadPlayer.LoadContent();
            playerAnimation.LoadContent();
            statusBox.LoadContent();
            pointsBox.LoadContent();
            inventoryBox.LoadContent();
            statsBox.LoadContent();
            skillBar.LoadContent();
            buffBox.LoadContent();
            skillBox.LoadContent();
            chatBox.LoadContent();

            XmlSerializer deserializer = new XmlSerializer(typeof(Skills));
            TextReader reader = new StreamReader("Content/Game/Skills.xml");
            object obj = deserializer.Deserialize(reader);
            skills = (Skills)obj;
            reader.Close();

             deserializer = new XmlSerializer(typeof(Items));
             reader = new StreamReader("Content/Game/Items.xml");
             obj = deserializer.Deserialize(reader);
            items = (Items)obj;
            reader.Close();


        }

        public void UnloadContent()
        {
            
        }

        public void Update()
        {
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();
            cursor.Update();

            if (keyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyUp(Keys.Enter) && !chatBox.isFocus)
            {
                chatBox.isFocus = true;
              
            }

            FPS++;
            if (FPS == 60)
            {
                
                foreach (Mob mob in mobs.mobList)
                {
                    if (mob.currentRespawnTime > 0 && !mob.spawned)
                        mob.currentRespawnTime--;
                    if (mob.currentRespawnTime == 0)
                    {

                        mob.currentRespawnTime = mob.respawnTime;
                        mob.spawned = true;
                        spawnMob(mob,mob.id);

                    }
                    

                }
                FPS = 0;
            }

            
            
            loadPlayer.Update(firstTime,out playerState,out playerStats,out playerPoints,playerStats,playerPoints,attack,mobDirection,xpEarned,block,magicAttack,attSkillIndex,buffId,out buffs,chatBox.isFocus);
            skillBar.Update(out attSkillId, range, rightDirection,playerStats.castSpeed,targeted,playerState[5],blockCasting,out eqSkills,eqSkills,firstTime,chatBox.isFocus);

            if (attSkillId != -1 && attackMob == 0 && skills.skillList[attSkillId].skillType == "dmg" )
            {
                startCasting = true;
                attSkillIndex = 600 / playerStats.castSpeed;
                attSkillIdCasting = attSkillId;
                blockCasting = true;
            }

            if (attSkillId != -1 && attackMob == 0 && skills.skillList[attSkillId].skillType == "mass")
            {
                
                attSkillIndex = 600 / playerStats.castSpeed;
                massCasting = true;
                attSkillIdCasting = attSkillId;
                blockCasting = true;
            }


            if (attSkillId != -1 && (skills.skillList[attSkillId].skillType == "buff" || skills.skillList[attSkillId].skillType == "heal"))
            {
                buffId = attSkillId;
                attSkillIndex = 600 / playerStats.castSpeed;
                blockCasting = true;
            }

           

           

            xpEarned = 0;
            attack = false;
            magicAttack = false;
           
            playerDmg = -1;

            int modelX, modelY;
            modelX = playerStats.modelX + playerStats.coordX;
            modelY = playerStats.modelY + playerStats.coordY;


            chatBox.Update(out block,out give,out spawn);
            if (give != "" && give.Split(' ').Length==2)
            {
                if (give.Split(' ')[0] == "skill" && Convert.ToInt32(give.Split(' ')[1]) <skills.skillList.Count)
                    skillBox.inventorySkills.Add(new InventorySkill(Convert.ToInt32(give.Split(' ')[1]), graphicsDevice));
                else if (give.Split(' ')[0] == "item" && Convert.ToInt32(give.Split(' ')[1]) < items.itemList.Count)
                    inventoryBox.inventoryItems.Add(new InventoryItem(Convert.ToInt32(give.Split(' ')[1]), 1));
            }
            if (spawn!="" && spawn.Split(' ').Length==1)
            {
                try
                {
                    if (Convert.ToInt32(spawn) < mobs.mobList.Count)
                    {
                        mobs.mobList.Add(CopyMob(mobs.mobList[Convert.ToInt32(spawn)]));
                        mobs.mobList[mobs.mobList.Count - 1].coords.X = playerStats.coordX + playerStats.modelX;
                        mobs.mobList[mobs.mobList.Count - 1].coords.Y = playerStats.coordY + playerStats.modelY;
                        mobs.mobList[mobs.mobList.Count - 1].currentRespawnTime = mobs.mobList[mobs.mobList.Count - 1].respawnTime;
                        mobs.mobList[mobs.mobList.Count - 1].id = mobs.mobList.Count - 1;
                        spawnMob(mobs.mobList[mobs.mobList.Count - 1], mobs.mobList.Count - 1);

                    }
                }
                catch
                {

                }
            }


            buffBox.Update(buffs, out block, inBox);
            block |= blockout;
            if (buffBox.isActive)
                inBox = buffBox.inBox;
            statusBox.Update(playerStats, out blockout, inBox, chatBox.isFocus);
            block |= blockout;
            if (statusBox.isActive)
                inBox = statusBox.inBox;
            skillBox.Update(out attSkillId,range,rightDirection,targeted,playerState[5],blockCasting, out blockout, inBox,eqSkills,out eqSkills, playerStats.castSpeed, chatBox.isFocus);
            block |= blockout;
            if (skillBox.isActive)
                inBox = skillBox.inBox;
            statsBox.Update(playerStats, out blockout, inBox, chatBox.isFocus);
            block |= blockout;
            if (statsBox.isActive)
                inBox = statsBox.inBox;
            inventoryBox.Update(out blockout, playerStats, out playerStats, out eqItems, out eqItemRanged, out eqItemSet, inBox, chatBox.isFocus,dropList);
            block |= blockout;
            if (inventoryBox.isActive)
                inBox = inventoryBox.inBox;
            pointsBox.Update(playerPoints, playerStats, out playerStats, out playerPoints, out blockout, inBox, chatBox.isFocus);
            block |= blockout;
            if (pointsBox.isActive)
                inBox = pointsBox.inBox;

            dropList.Clear();
            range = -1;

            if (attSkillId != -1 && attackMob == 0 && skills.skillList[attSkillId].skillType == "dmg")
            {
                startCasting = true;
                attSkillIndex = 600 / playerStats.castSpeed;
                attSkillIdCasting = attSkillId;
                blockCasting = true;
            }


            if (startCasting)
                attSkillIndex--;

            if (attSkillId != -1 && attackMob == 0 && skills.skillList[attSkillId].skillType == "mass")
            {

                attSkillIndex = 600 / playerStats.castSpeed;
                massCasting = true;
                attSkillIdCasting = attSkillId;
                blockCasting = true;
            }

            if (attSkillId != -1 && (skills.skillList[attSkillId].skillType == "buff" || skills.skillList[attSkillId].skillType == "heal"))
            {
                buffId = attSkillId;
                attSkillIndex = 600 / playerStats.castSpeed;
                blockCasting = true;
            }

            if (buffId != -1)
            {
                if (attSkillIndex <= 0)
                {
                    buffId = -1;
                    blockCasting = false;
                }
                else
                    attSkillIndex--;
            }

            if (massCasting)
            {
                if (attSkillIndex <= 0)
                {
                    massCasting = false;
                    blockCasting = false;
                }
                else
                    attSkillIndex--;
            }

            attackMob = playerState[4];
            if (attSkillId != -1)
            {
                attackMob = 0;
                playerState[4] = 0;
            }


            foreach (Tuple<LoadMob, MobAnimation, int> mob in spawnedMobs)
            {
                bool attackOut;

                block = false;

                


                mob.Item1.Update( out mobState,out mobStats,attackMob,playerStats,playerState[5], out attackOut);
                if (mouse.X > mobStats.modelX - playerStats.coordX && mouse.X < mobStats.modelX - playerStats.coordX + 32 && mouse.Y > mobStats.modelY - playerStats.coordY && mouse.Y < mobStats.modelY - playerStats.coordY + 64 && prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                {
                    targeted = true;
                    target = mob.Item3;
                }
                bool drawTarget = (target == mob.Item3);

                



                
                if ((mobStats.modelX - playerStats.coordX-playerStats.modelX<0 && playerStats.direction==0 && Math.Pow(Math.Pow(mobStats.modelX+32 - modelX,2)+ Math.Pow(mobStats.modelY - modelY, 2),.5f) < playerStats.attackRange || mobStats.modelX - playerStats.coordX - playerStats.modelX > 0 && playerStats.direction == 1 && Math.Pow(Math.Pow(mobStats.modelX - modelX-32, 2) + Math.Pow(mobStats.modelY - modelY, 2), .5f) < playerStats.attackRange) && (drawTarget || !eqItemRanged) && attackMob == 1 && attSkillIndex <= 0)
                {
                    if (eqItemRanged)
                    {

                        projectiles.Add(new Projectile(spriteBatch, Content.Load<Texture2D>("Projectiles/"+eqItemSet+"Arrow"), playerStats.coordX + playerStats.modelX + 16, playerStats.coordY + playerStats.modelY + 32,mob.Item3));
                    }
                    else
                    {
                        attackMob = 0;
                        Random critsId = new Random();
                        Random evaId = new Random();
                        int[] crits = new int[100];
                        int[] eva = new int[100];

                        for (int i = 0; i < playerStats.physCrits; i++)
                            crits[i] = 2;
                        for (int i = playerStats.physCrits; i < 100; i++)
                            crits[i] = 1;

                        for (int i = 0; i < (mobStats.eva * (100 - playerStats.acc)) / 100; i++)
                            eva[i] = 0;
                        for (int i = (mobStats.eva * (100 - playerStats.acc)) / 100; i < 100; i++)
                            eva[i] = 1;
                        mobDmg = (int)(crits[critsId.Next(0,100)] * eva[evaId.Next(0,100)] * (playerStats.physPow * .7) * (1 - ((mobStats.physDef - playerStats.fPen) / (float)1000) * (100 - playerStats.pen) / (float)100));
                        chatBox.WriteText("You successfully hit zombie " + mobDmg + "dmg.", Color.Blue);
                        mob.Item1.receiveDmg(mobDmg);
                    }

                    
                }

                if (massCasting && attSkillIndex <= 0 && Math.Pow(Math.Pow(mobStats.modelX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobStats.modelY - playerStats.modelY - playerStats.coordY, 2), .5f)<= skills.skillList[attSkillIdCasting].range)
                {
                    Random critsId = new Random();
                    Random evaId = new Random();
                    int[] crits = new int[100];
                    int[] eva = new int[100];

                    for (int i = 0; i < playerStats.mCrits; i++)
                        crits[i] = 2;
                    for (int i = playerStats.mCrits; i < 100; i++)
                        crits[i] = 1;

                    for (int i = 0; i < (mobStats.eva * (100 - playerStats.acc)) / 100; i++)
                        eva[i] = 0;
                    for (int i = (mobStats.eva * (100 - playerStats.acc)) / 100; i < 100; i++)
                        eva[i] = 1;

                    mobDmg = (int)(crits[critsId.Next(0, 100)] * eva[evaId.Next(0, 100)] * (playerStats.mPow * .8 + skills.skillList[attSkillIdCasting].power * 1.1) * (1 - ((mobStats.mDef - playerStats.fPen) / (float)1000) * (playerStats.pen / (float)100)));
                    mob.Item1.receiveDmg(mobDmg);
                    chatBox.WriteText("You successfully hit zombie " + mobDmg + "dmg.", Color.Blue);

                    magicAttack = true;
                    blockCasting = false;
                }


                if (drawTarget)
                {
                    range = Math.Pow(Math.Pow(mobStats.modelX - playerStats.modelX - playerStats.coordX, 2) + Math.Pow(mobStats.modelY - playerStats.modelY - playerStats.coordY, 2), .5f);
                    rightDirection = mobStats.modelX - playerStats.coordX - playerStats.modelX < 0 && playerStats.direction == 0 || mobStats.modelX - playerStats.coordX - playerStats.modelX > 0 && playerStats.direction == 1;
   
                }


                foreach (Projectile projectile in projectiles)
                {
                    if(projectile.mobId==mob.Item3)
                        projectile.Update(mobStats.modelX + 16, mobStats.modelY + 32);
                }



                foreach (Projectile projectile in projectiles)
                {
                    if (projectile.mobId == mob.Item3)
                        if (Math.Pow(Math.Pow(projectile.coordX - projectile.endX, 2) + Math.Pow(projectile.coordY - projectile.endY, 2), .5) < 32)
                        {
                            attackMob = 0;
                            Random critsId = new Random();
                            Random evaId = new Random();
                            int[] crits = new int[100];
                            int[] eva = new int[100];

                            for (int i = 0; i < playerStats.physCrits; i++)
                                crits[i] = 2;
                            for (int i = playerStats.physCrits; i < 100; i++)
                                crits[i] = 1;

                            for (int i = 0; i < (mobStats.eva * (100 - playerStats.acc)) / 100; i++)
                                eva[i] = 0;
                            for (int i = (mobStats.eva * (100 - playerStats.acc)) / 100; i < 100; i++)
                                eva[i] = 1;
                            mobDmg = (int)(crits[critsId.Next(0, 100)] * eva[evaId.Next(0, 100)] * (playerStats.physPow * .7) * (1 - ((mobStats.physDef - playerStats.fPen) / (float)1000) * (100 - playerStats.pen) / (float)100));
                            mob.Item1.receiveDmg(mobDmg);
                            chatBox.WriteText("You successfully hit zombie " + mobDmg + "dmg.", Color.Blue);
                            removeProjectiles.Add(projectile);
                        }
                }

                foreach (Projectile projectile in removeProjectiles)
                    projectiles.Remove(projectile);
                removeProjectiles.Clear();



                if (attSkillIndex <= 0 && startCasting && drawTarget)
                {
                    if (skills.skillList[attSkillIdCasting].projectile)
                    {
                        magicProjectiles.Add(new Projectile(spriteBatch, Content.Load<Texture2D>("Projectiles/" + skills.skillList[attSkillIdCasting].name), playerStats.coordX + playerStats.modelX + 16, playerStats.coordY + playerStats.modelY + 32, mob.Item3));
                        magicProjectiles[magicProjectiles.Count - 1].skillId = attSkillIdCasting;
                        startCasting = false;
                    }
                    else
                    {
                        Random critsId = new Random();
                        Random evaId = new Random();
                        int[] crits = new int[100];
                        int[] eva = new int[100];

                        for (int i = 0; i < playerStats.mCrits; i++)
                            crits[i] = 2;
                        for (int i = playerStats.mCrits; i < 100; i++)
                            crits[i] = 1;

                        for (int i = 0; i < (mobStats.eva * (100 - playerStats.acc)) / 100; i++)
                            eva[i] = 0;
                        for (int i = (mobStats.eva * (100 - playerStats.acc)) / 100; i < 100; i++)
                            eva[i] = 1;

                        mobDmg = (int)(crits[critsId.Next(0, 100)] * eva[evaId.Next(0, 100)] * (playerStats.mPow * .8 + skills.skillList[attSkillIdCasting].power * 1.1) * (1 - ((mobStats.mDef - playerStats.fPen) / (float)1000) * (playerStats.pen / (float)100)));
                        mob.Item1.receiveDmg(mobDmg);
                        chatBox.WriteText("You successfully hit zombie " + mobDmg + "dmg.", Color.Blue);

                        magicAttack = true;
                        startCasting = false;
                        blockCasting = false;
                    }
                }


                foreach (Projectile projectile in magicProjectiles)
                {
                    if (projectile.mobId == mob.Item3)
                    {
                        projectile.Update(mobStats.modelX + 16, mobStats.modelY + 32);

                        if (Math.Pow(Math.Pow(projectile.coordX - projectile.endX, 2) + Math.Pow(projectile.coordY - projectile.endY, 2), .5) < 32)
                        {
                            attackMob = 0;
                            Random critsId = new Random();
                            Random evaId = new Random();
                            int[] crits = new int[100];
                            int[] eva = new int[100];

                            for (int i = 0; i < playerStats.physCrits; i++)
                                crits[i] = 2;
                            for (int i = playerStats.physCrits; i < 100; i++)
                                crits[i] = 1;

                            for (int i = 0; i < (mobStats.eva * (100 - playerStats.acc)) / 100; i++)
                                eva[i] = 0;
                            for (int i = (mobStats.eva * (100 - playerStats.acc)) / 100; i < 100; i++)
                                eva[i] = 1;
                            mobDmg = (int)(crits[critsId.Next(0, 100)] * eva[evaId.Next(0, 100)] * (playerStats.mPow * .8 + skills.skillList[attSkillIdCasting].power * 1.1) * (1 - ((mobStats.mDef - playerStats.fPen) / (float)1000) * (playerStats.pen / (float)100)));
                            mob.Item1.receiveDmg(mobDmg);
                            chatBox.WriteText(skills.skillList[attSkillIdCasting].name + " successfully hit zombie " + mobDmg + "dmg.",Color.Blue);

                            attSkillIdCasting = projectile.skillId;
                            magicAttack = true;
                            blockCasting = false;
                            removeProjectiles.Add(projectile);
                        }

                    }
                }


                foreach (Projectile projectile in removeProjectiles)
                    magicProjectiles.Remove(projectile);
                removeProjectiles.Clear();





                mob.Item2.Update(mobState,mobStats,mobDmg, mobStats.modelX - playerStats.coordX, mobStats.modelY - playerStats.coordY,drawTarget,attSkillIdCasting,magicAttack);

                

                mobDmg = -1;

                if (attackOut)
                {

                    Random critsId = new Random();
                    Random evaId = new Random();
                    int[] crits = new int[100];
                    int[] eva = new int[100];

                    for (int i = 0; i < mobStats.physCrits; i++)
                        crits[i] = 2;
                    for (int i = mobStats.physCrits; i < 100; i++)
                        crits[i] = 1;

                    for (int i = 0; i < (playerStats.eva * (100 - mobStats.acc)) / 100; i++)
                        eva[i] = 0;
                    for (int i = (playerStats.eva * (100 - mobStats.acc)) / 100; i < 100; i++)
                        eva[i] = 1;

                    if (playerDmg == -1)
                        playerDmg = 0;
                    playerDmg += (int)(crits[critsId.Next(0, 100)] * eva[evaId.Next(0, 100)] * (mobStats.physPow * .7) * (1 - ((playerStats.physDef - mobStats.fPen) / (float)1000) * (100 -mobStats.pen) / (float)100));
                    chatBox.WriteText("Zombie hit you " + playerDmg + " dmg.", Color.Red);
                    mobDirection = mobStats.direction;
                }

                attack = attack || attackOut;


                if (mobStats.hp == 0)
                {
                    despawnedMobs.Add(mob);
                    mobs.mobList[mob.Item3].spawned = false;
                    if (drawTarget)
                    {
                        targeted = false;
                        target = -1;
                        magicProjectiles.Clear();
                        projectiles.Clear();
                    }
                    Random randChance = new Random();
                    foreach (Drop drop in mobs.mobList[mob.Item3].dropList)
                    {
                        
                        bool[] chance = new bool[drop.from];
                        for (int i = 0; i < drop.chance; i++)
                            chance[i] = true;
                        for (int i = drop.chance; i < drop.from; i++)
                            chance[i] = false;
                        if (chance[randChance.Next(0, drop.from)])
                        {
                            chatBox.WriteText(mobs.mobList[mob.Item3].name + " dropped " + drop.val + " " + items.itemList[drop.id].name+".",Color.Purple);
                            dropList.Add(drop);
                        }
                    }
                }

               

            }

            
            if(target==-1 && buffId==-1)
            {
                attSkillIndex = 0;
                massCasting = false;
                startCasting = false;
                blockCasting = false;
            }

            if (playerDmg>-1)
                loadPlayer.receiveDmg(playerDmg,out playerStats);

            foreach (Tuple<LoadMob, MobAnimation, int> mob in despawnedMobs)
            {
                spawnedMobs.Remove(mob);
                
                xpEarned += mobs.mobList[mob.Item3].xp;
            }

            despawnedMobs.Clear();

            if(prevCoordX!=playerStats.coordX || prevCoordY!=playerStats.coordY)
                loadMap.Update( playerStats.coordX, playerStats.coordY);



            
            





            if (!pointsBox.isActive && !inventoryBox.isActive && !statsBox.isActive && !statusBox.isActive && !buffBox.isActive && !skillBox.isActive)
            {
                inBox = false;
               // statusBox.isActive = true;
            }



            
 

            playerAnimation.Update(playerState, playerStats, playerDmg, playerStats.modelX, playerStats.modelY, eqItems,attSkillIndex,buffId);

            if (firstTime)
                firstTime = false;

            prevCoordX = playerStats.coordX;
            prevCoordY = playerStats.coordY;
            prevMouse = mouse;
            prevKeyboard = keyboard;
           
        }

        public void Draw()
        {
            loadMap.Draw();
            
            playerAnimation.Draw();
            foreach (Tuple<LoadMob, MobAnimation, int> mob in spawnedMobs)
                mob.Item2.Draw();

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(playerStats.coordX, playerStats.coordY);
            }
            foreach (Projectile projectile in magicProjectiles)
            {
                projectile.Draw(playerStats.coordX, playerStats.coordY);
            }

            statusBox.Draw();
            skillBar.Draw();
            chatBox.Draw();

            if(!pointsBox.isActive)
                pointsBox.Draw();
            if(!inventoryBox.isActive)
                inventoryBox.Draw();
            if(!statsBox.isActive)
                statsBox.Draw();
            if (!skillBox.isActive)
                skillBox.Draw();
            if (!statusBox.isActive)
                statusBox.Draw();
            if (!buffBox.isActive)
                buffBox.Draw();

            if (pointsBox.isActive)
                pointsBox.Draw();
            if (inventoryBox.isActive)
                inventoryBox.Draw();
            if (statsBox.isActive)
                statsBox.Draw();
            if (skillBox.isActive)
                skillBox.Draw();
            if (statusBox.isActive)
                statusBox.Draw();
            if (buffBox.isActive)
                buffBox.Draw();
            

            loadPlayer.Draw();
           
            if(inventoryBox.displayCursor && skillBox.displayCursor && buffBox.displayCursor)
                cursor.Draw();

        }
        
    }
}
