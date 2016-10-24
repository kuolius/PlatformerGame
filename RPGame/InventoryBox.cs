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
    class InventoryItem
    {
        public int id;
        public int val;
        

        public InventoryItem(int id, int val)
        {
            this.id = id;
            this.val = val;
            
        }
    }

    class EqItems
    {
        public InventoryItem helm, leftArm, rightArm, body, legs, gloves, boots;

        public EqItems()
        {
            helm = new InventoryItem(-1, 1);
            leftArm = new InventoryItem(-1, 1);
            rightArm = new InventoryItem(-1, 1);
            body = new InventoryItem(-1, 1);
            legs = new InventoryItem(-1, 1);
            gloves = new InventoryItem(-1, 1);
            boots = new InventoryItem(-1, 1);
                
        }
    }

    class InventoryBox
    {
        ContentManager Content;
        GraphicsDevice graphics;
        GameWindow Window;
        SpriteBatch spriteBatch;
        SpriteFont itemInfoFont;

        Vector2 mainBoxCoord,eqCoord,inventoryCoord,scrollerCoord;
        Vector2 mouseInBox;

        Texture2D mainBox,header,xButton;
        Texture2D defaultIcon;
        Texture2D arrow, scroller, path;

        MouseState mouse, prevMouse;
        KeyboardState keyboard, prevKeyboard;
        public List<InventoryItem> inventoryItems;
        EqItems eqItems;
       
        List<Texture2D> itemIcons;
        List<string> itemInfo;
        Items items;


        public bool display,isActive,isActivated,inBox;
        bool drag, block,scroll;
        int margins;
        int inventoryRows;
        int scrollerWidth;
        int scrollerUpBound, scrollerDownBound;
        int clicked, dbclicked,diseq;
        int clickedIndex,clickedTime;
        float transp;
        public bool displayCursor;

        public InventoryBox(ContentManager Content, GraphicsDevice graphics, GameWindow Window)
        {
            this.Content = Content;
            this.graphics = graphics;
            this.Window = Window;
        }

        public void Initialize()
        {
            
            mainBoxCoord = new Vector2(Window.ClientBounds.Width - 300, Window.ClientBounds.Height / 2 - 150);
            inventoryCoord = new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 142);
            scrollerCoord = new Vector2(inventoryCoord.X + 248, inventoryCoord.Y + 15);
            scrollerUpBound =(int) inventoryCoord.Y + 15;
            scrollerDownBound = (int)inventoryCoord.Y + 15 + 93;
            display = false;
            drag = false;
            block = false;
            scroll = false;
            margins = 1;
            inventoryItems = new List<InventoryItem>();
            itemIcons = new List<Texture2D>();
            itemInfo = new List<string>();
            isActive = false;
            isActivated = false;
            

            inventoryItems.Add(new InventoryItem(0, 1));
            inventoryItems.Add(new InventoryItem(1, 1));
            inventoryItems.Add(new InventoryItem(2, 1));
            inventoryItems.Add(new InventoryItem(3, 1));
            inventoryItems.Add(new InventoryItem(4, 1));
            inventoryItems.Add(new InventoryItem(5, 1));
            inventoryItems.Add(new InventoryItem(6, 1));
            inventoryItems.Add(new InventoryItem(7, 1));
            inventoryItems.Add(new InventoryItem(8, 1));


            XmlSerializer deserializer = new XmlSerializer(typeof(Items));
            TextReader reader = new StreamReader("Content/Game/Items.xml");
            object obj = deserializer.Deserialize(reader);
            items = (Items)obj;
            reader.Close();

            foreach(Item item in items.itemList)
            {
                itemIcons.Add(Content.Load<Texture2D>("Items/" + item.name));
            }

            eqItems = new EqItems();

            arrow = Content.Load<Texture2D>("scroller/arrow");
            scroller = Content.Load<Texture2D>("scroller/scroller");
            path = Content.Load<Texture2D>("scroller/path");
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics);
            mainBox = Content.Load<Texture2D>("Game/inventoryBg");
            defaultIcon = Content.Load<Texture2D>("Items/defaultEqIcon");
            header = Content.Load<Texture2D>("header");
            xButton = Content.Load<Texture2D>("x");
            itemInfoFont = Content.Load<SpriteFont>("Game/chatFont");

        }

        public void Update(out bool block, Stats playerStats, out Stats playerStatsOut, out EqItems eqItemsOut,out bool eqItemRanged,out string eqItemSet, bool inBoxIn,bool isFocus,List<Drop> dropList)
        {

            mouse = Mouse.GetState();
            if(!isFocus)
                keyboard = Keyboard.GetState();
            dbclicked = -1;
            diseq = -1;
            itemInfo.Clear();
            displayCursor = true;


            foreach (Drop drop in dropList)
            {
                if (items.itemList[drop.id].stackable)
                {
                    int o = 0;
                    foreach (InventoryItem inventoryItem in inventoryItems)
                        if (drop.id == inventoryItem.id)
                        {
                            inventoryItem.val += drop.val;
                            o++;
                        }
                    if (o == 0)
                        inventoryItems.Add(new InventoryItem(drop.id, drop.val));

                }
                else 
                {
                    for (int i = 0; i < drop.val; i++)
                        inventoryItems.Add(new InventoryItem(drop.id, i));
                }
                

            }

              

            if (display)
            if (mouse.X > mainBoxCoord.X && mouse.X < mainBoxCoord.X + 300 && mouse.Y > mainBoxCoord.Y && mouse.Y < mainBoxCoord.Y + 300)
            {
                inBox = true;
                    
                    if ((mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released|| mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released) && !inBoxIn)
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

           

            inventoryRows = (inventoryItems.Count - 32) / 8;
            if ((inventoryItems.Count - 32) % 8 > 0)
                inventoryRows++;
            if (inventoryRows < 0)
                inventoryRows = 0;

            scrollerWidth = 93 - 5 * inventoryRows;

            if (scrollerWidth + scrollerCoord.Y - scrollerUpBound > 93)
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

            //if(prevMouse.LeftButton==ButtonState.Pressed && mouse.LeftButton==ButtonState.Released && )



            if (prevMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed && mouse.X > mainBoxCoord.X && mouse.X < mainBoxCoord.X + 300 && mouse.Y > mainBoxCoord.Y && mouse.Y < mainBoxCoord.Y + 20 && display&& isActive)
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
                scrollerCoord.X += mouse.X - prevMouse.X;
                scrollerCoord.Y += mouse.Y - prevMouse.Y;
                scrollerUpBound += mouse.Y - prevMouse.Y;
            }

            if (display && isActive)
            {
                for (int i = 0; i < 32; i++)
                {
                    if (mouse.X > inventoryCoord.X + 31 * (i % 8) && mouse.X < inventoryCoord.X + 31 * (i % 8) + 30 && mouse.Y > inventoryCoord.Y + 31 * (i / 8) && mouse.Y < inventoryCoord.Y + 31 * (i / 8) + 30 )
                    {

                        if(prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                            dbclicked = (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i;
                        else if((int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i!=-1 && (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i<inventoryItems.Count)
                        {
                            displayCursor = false;
                            itemInfo.Add(items.itemList[inventoryItems[(int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i].id].name);
                            if(items.itemList[inventoryItems[(int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i].id].eqType=="gold")
                                itemInfo.Add(Convert.ToString(inventoryItems[(int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i].val));
                            foreach (Add add in items.itemList[inventoryItems[(int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8 + i].id].addList)
                                switch (add.stat)
                                {
                                    case "pstr":
                                        itemInfo.Add("P.Pow "+add.val);
                                        break;

                                    case "mstr":
                                        itemInfo.Add("M.Pow " + add.val);
                                        break;

                                    case "pdef":
                                        itemInfo.Add("P.Def " + add.val);
                                        break;

                                    case "mdef":
                                        itemInfo.Add("M.Def " + add.val);
                                        break;

                                    case "pas":
                                        itemInfo.Add("P.Speed " + add.val);
                                        break;

                                    case "mas":
                                        itemInfo.Add("C.Speed " + add.val);
                                        break;

                                    case "mp":
                                        itemInfo.Add("Max Mp " + add.val);
                                        break;

                                    case "hp":
                                        itemInfo.Add("Max Hp " + add.val);
                                        break;

                                    case "eva":
                                        itemInfo.Add("Evasion " + add.val);
                                        break;

                                    case "pc":
                                        itemInfo.Add("P.Crits " + add.val);
                                        break;

                                    case "mc":
                                        itemInfo.Add("M.Crits " + add.val);
                                        break;
                                    case "attackRange":
                                        itemInfo.Add("Range " + add.val);
                                        break;
                                }
                        }

                    }
                }

                if (dbclicked < inventoryItems.Count && dbclicked >= 0)
                {
                    int EqId = -1;

                    switch (items.itemList[inventoryItems[dbclicked].id].eqType)
                    {
                        case "weapon":
                            EqId = eqItems.leftArm.id;
                            break;
                        case "bow":
                            EqId = eqItems.leftArm.id;
                            break;
                        case "helm":
                            EqId = eqItems.helm.id;
                            break;
                        case "shield":
                            EqId = eqItems.rightArm.id;
                            break;
                        case "body":
                            EqId = eqItems.body.id;
                            break;
                        case "gloves":
                            EqId = eqItems.gloves.id;
                            break;
                        case "legs":
                            EqId = eqItems.legs.id;
                            break;
                        case "boots":
                            EqId = eqItems.boots.id;
                            break;
                    }

                    if (EqId != -1)
                        foreach (Add add in items.itemList[EqId].addList)
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


                    bool equiped = false;
                    switch (items.itemList[inventoryItems[dbclicked].id].eqType)
                    {
                        case "weapon":
                            if (eqItems.leftArm.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.leftArm.id, 1));
                            eqItems.leftArm = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "bow":
                            if (eqItems.leftArm.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.leftArm.id, 1));
                            eqItems.leftArm = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "helm":
                            if (eqItems.helm.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.helm.id, 1));
                            eqItems.helm = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "shield":
                            if (eqItems.rightArm.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.rightArm.id, 1));
                            eqItems.rightArm = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "body":
                            if (eqItems.body.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.body.id, 1));
                            eqItems.body = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "gloves":
                            if (eqItems.gloves.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.gloves.id, 1));
                            eqItems.gloves = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "legs":
                            if (eqItems.legs.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.legs.id, 1));
                            eqItems.legs = inventoryItems[dbclicked];
                            equiped = true;
                            break;
                        case "boots":
                            if (eqItems.boots.id != -1)
                                inventoryItems.Add(new InventoryItem(eqItems.boots.id, 1));
                            eqItems.boots = inventoryItems[dbclicked];
                            equiped = true;
                            break;

                    }

                    foreach (Add add in items.itemList[inventoryItems[dbclicked].id].addList)
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
                                playerStats.attSpeed+= add.val;
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

                        }

                    if(equiped)
                        inventoryItems.Remove(inventoryItems[dbclicked]);

                }


                int Id=-1;

                if ( mouse.X > eqCoord.X && mouse.X < eqCoord.X + 30 && mouse.Y > eqCoord.Y + 30 + margins && mouse.Y < eqCoord.Y + 30 + margins + 30)
                    if (eqItems.leftArm.id != -1)
                    {

                        Id = eqItems.leftArm.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.leftArm.id, 1));
                            eqItems.leftArm.id = -1;
                        }
                    }

                if ( mouse.X > eqCoord.X + 30 + margins && mouse.X < eqCoord.X + 30 + margins + 30 && mouse.Y > eqCoord.Y && mouse.Y < eqCoord.Y + 30)
                    if (eqItems.helm.id != -1)
                    {

                        Id = eqItems.helm.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.helm.id, 1));
                            eqItems.helm.id = -1;
                        }

                    }

                if ( mouse.X > eqCoord.X + 30 + margins && mouse.X < eqCoord.X + 30 + margins + 30 && mouse.Y > eqCoord.Y + 30 + margins && mouse.Y < eqCoord.Y + 30 + margins + 30)
                    if (eqItems.body.id != -1)
                    {

                        Id = eqItems.body.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.body.id, 1));
                            eqItems.body.id = -1;
                        }
                    }

                if ( mouse.X > eqCoord.X + 2 * (30 + margins) && mouse.X < eqCoord.X + 2 * (30 + margins) + 30 + margins + 30 && mouse.Y > eqCoord.Y + 30 + margins && mouse.Y < eqCoord.Y + 30 + margins + 30)
                    if (eqItems.rightArm.id != -1)
                    {

                        Id = eqItems.rightArm.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.rightArm.id, 1));
                            eqItems.rightArm.id = -1;
                        }

                    }

                if ( mouse.X > eqCoord.X && mouse.X < eqCoord.X + 30 && mouse.Y > eqCoord.Y + 2 * (30 + margins) && mouse.Y < eqCoord.Y + 2 * (30 + margins) + 30)
                    if (eqItems.gloves.id != -1)
                    {

                        Id = eqItems.gloves.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.gloves.id, 1));
                            eqItems.gloves.id = -1;
                        }

                    }

                if ( mouse.X > eqCoord.X + 30 + margins && mouse.X < eqCoord.X + 30 + margins + 30 && mouse.Y > eqCoord.Y + 2 * (30 + margins) && mouse.Y < eqCoord.Y + 2 * (30 + margins) + 30)
                    if (eqItems.legs.id != -1)
                    {

                        Id = eqItems.legs.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.legs.id, 1));
                            eqItems.legs.id = -1;
                        }
                    }

                if ( mouse.X > eqCoord.X + 2 * (30 + margins) && mouse.X < eqCoord.X + 2 * (30 + margins) + 30 && mouse.Y > eqCoord.Y + 2 * (30 + margins) && mouse.Y < eqCoord.Y + 2 * (30 + margins) + 30)
                    if (eqItems.boots.id != -1)
                    {

                        Id = eqItems.boots.id;
                        if (prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                        {
                            inventoryItems.Add(new InventoryItem(eqItems.boots.id, 1));
                            eqItems.boots.id = -1;
                        }

                    }

               


                if (Id!=-1 && prevMouse.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
                foreach (Add add in items.itemList[Id].addList)
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
                else if(Id!=-1)
                {

                    displayCursor = false;
                    itemInfo.Add(items.itemList[Id].name);
                    foreach (Add add in items.itemList[Id].addList)
                        switch (add.stat)
                        {
                            case "pstr":
                                itemInfo.Add("P.Pow " + add.val);
                                break;

                            case "mstr":
                                itemInfo.Add("M.Pow " + add.val);
                                break;

                            case "pdef":
                                itemInfo.Add("P.Def " + add.val);
                                break;

                            case "mdef":
                                itemInfo.Add("M.Def " + add.val);
                                break;

                            case "pas":
                                itemInfo.Add("P.Speed " + add.val);
                                break;

                            case "mas":
                                itemInfo.Add("C.Speed " + add.val);
                                break;

                            case "mp":
                                itemInfo.Add("Max Mp " + add.val);
                                break;

                            case "hp":
                                itemInfo.Add("Max Hp " + add.val);
                                break;

                            case "eva":
                                itemInfo.Add("Evasion " + add.val);
                                break;

                            case "pc":
                                itemInfo.Add("P.Crits " + add.val);
                                break;

                            case "mc":
                                itemInfo.Add("M.Crits " + add.val);
                                break;
                            case "attackRange":
                                itemInfo.Add("Range " + add.val);
                                break;
                        }
                }




            }


            if (mouse.X > mainBoxCoord.X + 280 && mouse.X < mainBoxCoord.X + 295 && mouse.Y > mainBoxCoord.Y + 2 && mouse.Y < mainBoxCoord.Y + 17 && prevMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released &&  isActive)
            {
                display = false;
                isActive = false;
            }

            if (prevKeyboard.IsKeyUp(Keys.I) && keyboard.IsKeyDown(Keys.I))
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
            else if (prevKeyboard.IsKeyUp(Keys.Y) && keyboard.IsKeyDown(Keys.Y) || prevKeyboard.IsKeyUp(Keys.U) && keyboard.IsKeyDown(Keys.U)|| prevKeyboard.IsKeyUp(Keys.O) && keyboard.IsKeyDown(Keys.O))
                isActive = false;


            if (isActive)
                transp = 1;
            else
                transp = .5f;


            if(display && isActive && keyboard.IsKeyDown(Keys.Escape))
            {
                display = false;
                isActive = false;
            }


            eqCoord = new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 25);
            inventoryCoord = new Vector2(mainBoxCoord.X + 20, mainBoxCoord.Y + 142);
            scrollerCoord.X = inventoryCoord.X + 248;
            prevMouse = mouse;
            prevKeyboard = keyboard;
            block = this.block;
            playerStatsOut = playerStats;
            eqItemsOut = eqItems;
            if (eqItems.leftArm.id != -1)
            {
                eqItemRanged = items.itemList[eqItems.leftArm.id].ranged;
                eqItemSet = items.itemList[eqItems.leftArm.id].set;
            }
            else
            {
                eqItemRanged = false;
                eqItemSet = "";
            }


        }

        public void Draw()
        {
            spriteBatch.Begin();
            if (display)
            {
                spriteBatch.Draw(mainBox, color:Color.White * transp,destinationRectangle:new Rectangle((int)mainBoxCoord.X, (int)mainBoxCoord.Y,300,300));
                spriteBatch.Draw(header, mainBoxCoord, color:Color.White *transp);
                spriteBatch.Draw(xButton, new Vector2(mainBoxCoord.X + 280, mainBoxCoord.Y+2), color:Color.White *transp);

                //helm
                if (eqItems.helm.id==-1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X+30+margins, eqCoord.Y),color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.helm.id], new Vector2(eqCoord.X + 30 + margins, eqCoord.Y), color: Color.White * transp);
                //leftArm
                if (eqItems.leftArm.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X , eqCoord.Y+30 + margins), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.leftArm.id], new Vector2(eqCoord.X , eqCoord.Y + 30 + margins), color: Color.White * transp);
                //body
                if (eqItems.body.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X + 30 + margins, eqCoord.Y + 30 + margins), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.body.id], new Vector2(eqCoord.X + 30 + margins, eqCoord.Y + 30 + margins), color: Color.White * transp);
                //rightArm
                if (eqItems.rightArm.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X + 2*(30 + margins), eqCoord.Y + 30 + margins), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.rightArm.id], new Vector2(eqCoord.X + 2 * (30 + margins), eqCoord.Y + 30 + margins),  color: Color.White * transp);
                //gloves
                if (eqItems.gloves.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X , eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.gloves.id], new Vector2(eqCoord.X , eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);
                //legs
                if (eqItems.legs.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X + 30 + margins, eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.legs.id], new Vector2(eqCoord.X + 30 + margins, eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);
                //boots
                if (eqItems.boots.id == -1)
                    spriteBatch.Draw(defaultIcon, new Vector2(eqCoord.X + 2*(30 + margins), eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);
                else
                    spriteBatch.Draw(itemIcons[eqItems.boots.id], new Vector2(eqCoord.X + 2*(30 + margins), eqCoord.Y + 2 * (30 + margins)), color: Color.White * transp);

                if (inventoryItems.Count-((scrollerCoord.Y-scrollerUpBound)/5)*8 <= 32)
                {
                    for (int i =0; i < inventoryItems.Count - (int)((scrollerCoord.Y - scrollerUpBound) / 5)*8; i++)
                        spriteBatch.Draw(itemIcons[inventoryItems[i+ (int)((scrollerCoord.Y - scrollerUpBound) / 5) * 8].id], new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);

                    for (int i = inventoryItems.Count - (int)((scrollerCoord.Y - scrollerUpBound) / 5)*8; i < 32; i++)
                        spriteBatch.Draw(defaultIcon, new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                }
                else
                {
                    for (int i = 0; i < 32; i++)
                        spriteBatch.Draw(itemIcons[inventoryItems[i+ (int)((scrollerCoord.Y - scrollerUpBound) / 5)*8].id], new Vector2(inventoryCoord.X + 31 * (i % 8), inventoryCoord.Y + 31 * (i / 8)), color: Color.White * transp);
                }

                


                spriteBatch.Draw(arrow, new Vector2(inventoryCoord.X + 248, inventoryCoord.Y), color: Color.White * transp);
                spriteBatch.Draw(path, new Vector2(inventoryCoord.X + 248, inventoryCoord.Y + 15), color: Color.White * transp);
                spriteBatch.Draw(arrow, new Vector2(inventoryCoord.X + 248, inventoryCoord.Y + 108), color: Color.White * transp, effects:SpriteEffects.FlipVertically);
                spriteBatch.Draw(scroller, color: Color.White * transp, destinationRectangle:new Rectangle((int)scrollerCoord.X, (int)scrollerCoord.Y,15,scrollerWidth));

                if (itemInfo.Count > 0)
                {
                    if (mouse.X + 100 <= Window.ClientBounds.Width)
                    {
                        spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X, mouse.Y, 100, itemInfo.Count * 16), color: Color.White * transp);
                        for (int i = 0; i < itemInfo.Count; i++)
                            spriteBatch.DrawString(itemInfoFont, itemInfo[i], new Vector2(mouse.X + 2, mouse.Y + 2 + i * 16), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(mainBox, destinationRectangle: new Rectangle(mouse.X-100,mouse.Y, 100, itemInfo.Count * 16), color: Color.White * transp);
                        for (int i = 0; i < itemInfo.Count; i++)
                            spriteBatch.DrawString(itemInfoFont, itemInfo[i], new Vector2(mouse.X + 2-100, mouse.Y + 2 + i * 16), Color.Black);
                    }
                }

            }

            spriteBatch.End();
        }

    }
}
