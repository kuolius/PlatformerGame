using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RPGame
{
    class LoadMap
    {
        ContentManager Content;
        GraphicsDevice graphicsDevice;
        GameWindow Window;
        SpriteBatch spriteBatch;

        public int[][][][][] map;
        int[][] shaders; 
        int coordX;
        int coordY;
        KeyboardState keyboard;
        int scrollSpeed;
        int scrollWidth;
        Texture2D tileset,shader;

        public LoadMap(ContentManager Content,GraphicsDevice graphicsDevice,GameWindow Window)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            this.Window = Window;
        }

        public void Initialize()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            coordX = 0;
            coordY = 0;
            scrollSpeed = 3;
            scrollWidth = 50;
        }

        public void LoadContent(out int[][][][][] map)
        {
            tileset = Content.Load<Texture2D>("Game/tileset");
            shader = new Texture2D(graphicsDevice, 5, 5);
            Color[] data = new Color[25];
            for (int i = 0; i < 25; i++)
                data[i] = Color.Black;
            shader.SetData(data);
            Stream stream = File.Open("Content/shadersDefault.bin", FileMode.Open);
            BinaryFormatter bin = new BinaryFormatter();
            var obj = (Object[])(bin.Deserialize(stream));
            map = (int[][][][][])(obj[0]);
            shaders = (int[][])(obj[1]);
            stream.Close();
            this.map = map;
        }

        public void UnloadContent()
        {
            
        }

        public void Update(int coordXout, int coordYout)
        {

            // if (keyboard.IsKeyDown(Keys.Down) && coordY <= map[0].Length * 180 - Window.ClientBounds.Height - scrollSpeed + 50)
            //  coordY += scrollSpeed;
            //  if ( keyboard.IsKeyDown(Keys.Up) && coordY >= scrollSpeed)
            //   coordY -= scrollSpeed;
            coordX= coordXout ;
            coordY=coordYout;
           
             
        }

        public void Draw()
        {
            int squaresX = (Window.ClientBounds.Width + coordX % 180 - 180) / 180;
            int squaresY = (Window.ClientBounds.Height + coordY % 180 - 180) / 180;

            if ((Window.ClientBounds.Width + coordX % 180 - 180) % 180 != 0)
                squaresX++;
            if ((Window.ClientBounds.Height + coordY % 180 - 180) % 180 != 0)
                squaresY++;

            if (180 - coordX % 180 > 0) squaresX++;
            if (180 - coordY % 180 > 0) squaresY++;
            spriteBatch.Begin();

            for (int i = 0; i < squaresX; i++)
                for (int j = 0; j < squaresY; j++)
                    for (int k = 0; k < 10; k++)
                        for (int l = 0; l < 10; l++)
                            spriteBatch.Draw(tileset, new Vector2(180 * i + 18 * k - coordX % 180, 180 * j + 18 * l - coordY % 180), new Rectangle(new Point(map[i + coordX / 180][(j + coordY / 180)][k][l][0] * 18, map[i + coordX / 180][(j + coordY / 180)][k][l][1] * 18), new Point(18, 18)), Color.White);


            for (int i = 0; i < Window.ClientBounds.Width / 5; i++)
                for (int j = 0; j < Window.ClientBounds.Height / 5; j++)
                    spriteBatch.Draw(shader, new Vector2(i  * 5, j  * 5),new Color(Color.White, shaders[i + coordX / 5][j + coordY / 5] / (float)100));
                            

                      


            spriteBatch.End();
        }
    }
}
