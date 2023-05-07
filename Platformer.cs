//#define DEBUG_INFO //uncomment this to see debug information

//From Javidx9:
/*
OneLoneCoder.com - Code-It-Yourself! Simple Tile Based Platform Game
"Its-a meee-a Jario!" - @Javidx9

License
~~~~~~~
Copyright (C) 2018  Javidx9
This program comes with ABSOLUTELY NO WARRANTY.
This is free software, and you are welcome to redistribute it
under certain conditions; See license for details.
Original works located at:
https://www.github.com/onelonecoder
https://www.onelonecoder.com
https://www.youtube.com/javidx9

GNU GPLv3
https://github.com/OneLoneCoder/videos/blob/master/LICENSE

From Javidx9 :)
~~~~~~~~~~~~~~~
Hello! Ultimately I don't care what you use this for. It's intended to be
educational, and perhaps to the oddly minded - a little bit of fun.
Please hack this, change it and use it in any way you see fit. You acknowledge
that I am not responsible for anything bad that happens as a result of
your actions. However this code is protected by GNU GPLv3, see the license in the
github repo. This means you must attribute me if you use it. You can view this
license here: https://github.com/OneLoneCoder/videos/blob/master/LICENSE
Cheers!


Background
~~~~~~~~~~
Tile maps are fundamental to most 2D games. This program explores emulating a classic 2D platformer
using floating point truncation to implement robust collision between a moving tile and a tilemap
representing the level.

Controls
~~~~~~~~
Left and Right arrow keys move Jario, Space bar jumps.
(Up and Down also move jario)

Author
~~~~~~
Twitter: @javidx9
Blog: www.onelonecoder.com
YouTube: www.youtube.com/javidx9
Discord: https://discord.gg/WhwHUMV

Video:
~~~~~~
https://youtu.be/oJvJZNyW_rw

Last Updated: 04/02/2018
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//NOTE: 0.0164 is the amount of seconds it usually takes to complete a frame.
//Unlike OneLoneCoder's Console Game Engine, this has a fixed timestep, and it
//calls Draw less often to make up for the duration it takes to Update the game.

namespace OneLoneCoder_Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Platformer : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Level storage
        string sLevel;
        int nLevelWidth;
        int nLevelHeight;

        const float fGravityFactor = 20.0f;

        // Player Properties
        float fPlayerPosX = 1.0f;
        float fPlayerPosY = 1.0f;
        float fPlayerVelX = 0.0f;
        float fPlayerVelY = 0.0f;
        bool bPlayerOnGround = false;

        // Camera properties
        float fCameraPosX = 0.0f;
        float fCameraPosY = 0.0f;

        Texture2D spriteMan;
        Texture2D spriteTiles;

        float scale = 2f;
        SpriteFont sf;

        // Sprite selection flags
        int nDirModX = 0;
        int nDirModY = 0;

        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //IsFixedTimeStep = false; //IsFixedTimeStep defaults to true.

            IsFixedTimeStep = false;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            nLevelWidth = 64;
            nLevelHeight = 16;
            sLevel += "................................................................";
            sLevel += "................................................................";
            sLevel += ".......ooooo....................................................";
            sLevel += "........ooo.....................................................";
            sLevel += ".......................########.................................";
            sLevel += ".....BB?BBBB?BB.......###..............#.#......................";
            sLevel += "....................###................#.#......................";
            sLevel += ".......{}..........####.........................................";
            sLevel += "GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG.##############.....########";
            sLevel += "...................................#.#...............###........";
            sLevel += "........................############.#............###...........";
            sLevel += "........................#............#.........###..............";
            sLevel += "........................#.############......###.................";
            sLevel += "........................#................###....................";
            sLevel += "........................#################.......................";
            sLevel += "................................................................";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteMan = Content.Load<Texture2D>("minijario");
            spriteTiles = Content.Load<Texture2D>("leveljario");
            sf = Content.Load<SpriteFont>("hud");

            // TODO: use this.Content to load your game content here
        }

        char GetTile(float x, float y)
        {
            int nX = (int)Math.Floor(x);
            int nY = (int)Math.Floor(y);
            if (nX >= 0 && nX < nLevelWidth && y >= 0 && y < nLevelHeight)
                return sLevel[(int)(nY * nLevelWidth + nX)];
            else
                return ' ';
        }

        char GetTile(int x, int y)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
                return sLevel[y * nLevelWidth + x];
            else
                return ' ';
        }

        void SetTile(int x, int y, char c)
        {
            if (x >= 0 && x < nLevelWidth && y >= 0 && y < nLevelHeight)
            {
                char[] s = sLevel.ToCharArray();
                s[y * nLevelWidth + x] = c;
                sLevel = new string(s);
            }
            //sLevel[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }

        void SetTile(float x, float y, char c)
        {
            int nX = (int)Math.Floor(x); int nY = (int)Math.Floor(y);
            if (nX >= 0 && nX < nLevelWidth && nY >= 0 && nY < nLevelHeight)
            {
                char[] s = sLevel.ToCharArray();
                s[(int)(nY * nLevelWidth + nX)] = c;
                sLevel = new string(s);
            }
            //sLevel[(int)Math.Floor(y)*nLevelWidth + (int)Math.Floor(x)] = c;
        }

        KeyboardState prevKbState = new KeyboardState();

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool IsFocused = true;
        protected override void OnActivated(object sender, EventArgs args)
        {
            IsFocused = true;
            //base.OnActivated(sender, args);
        }
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            IsFocused = false;
            //base.OnActivated(sender, args);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Utility Lambdas

            //fPlayerVelX = 0.0f;
            //fPlayerVelY = 0.0f;

            //float fElapsedTime = 0.0164f;

            KeyboardState kbState = Keyboard.GetState();

            //fPlayerVelX = 0.0f;
            fPlayerVelY += fGravityFactor * 0.0164f;


            if (IsFocused)
            {
                if (kbState.IsKeyDown(Keys.Up))
                {
                    fPlayerVelY = -6.0f;
                }
                if (kbState.IsKeyDown(Keys.Down))
                {
                    fPlayerVelY = 6.0f;
                }
                if (kbState.IsKeyDown(Keys.Left))
                {
                    fPlayerVelX += -6.0f * 0.0164f;
                    nDirModY = 1;
                }
                if (kbState.IsKeyDown(Keys.Right))
                {
                    fPlayerVelX += 6.0f * 0.0164f;
                    nDirModY = 0;
                }
                if (kbState.IsKeyDown(Keys.Space) && !prevKbState.IsKeyDown(Keys.Space))
                {
                    if (fPlayerVelY >= -0.5f && fPlayerVelY <= 0.5f)
                    {
                        fPlayerVelY = -12.0f;
                        nDirModX = 1;
                    }
                    //fPlayerVelX += 6.0f * 0.0164f;
                }
                if (kbState.IsKeyDown(Keys.F1))
                {
                    scale = 1.0f;
                    //fPlayerVelX += 6.0f * 0.0164f;
                }
                if (kbState.IsKeyDown(Keys.F2))
                {
                    scale = 2.0f;
                    //fPlayerVelX += 6.0f * 0.0164f;
                }
                if (kbState.IsKeyDown(Keys.F3))
                {
                    scale = 3.0f;
                    //fPlayerVelX += 6.0f * 0.0164f;
                }
            }

            if (fPlayerVelX > 10.0f)
                fPlayerVelX = 10.0f;

            if (fPlayerVelX < -10.0f)
                fPlayerVelX = -10.0f;

            if (fPlayerVelY > 100.0f)
                fPlayerVelY = 100.0f;

            if (fPlayerVelY < -100.0f)
                fPlayerVelY = -100.0f;



            float fNewPlayerPosX = fPlayerPosX + fPlayerVelX * 0.0164f;
            float fNewPlayerPosY = fPlayerPosY + fPlayerVelY * 0.0164f;

            if (GetSolidity(GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 0.0f)) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)(int)fNewPlayerPosY + 0.0f, '.');

            if (GetSolidity(GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f)) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f, '.');

            if (GetSolidity(GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f)) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f, '.');

            if (GetSolidity(GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f)) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f, '.');

            //Collision
            if (fPlayerVelX <= 0)
            {
                if (GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.0f))) != '.' ||
                    GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.8f))) != '.')
                {
                    fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX + 1);
                    fPlayerVelX = 0;
                }
            }
            else
            {
                if (GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.0f))) != '.' ||
                    GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.8f))) != '.')
                {
                    fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX);
                    fPlayerVelX = 0;
                }
            }

            if (fPlayerVelY <= 0)
            {
                if (GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY - 0.1f)))!= '.' ||
                    GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY - 0.1f))) != '.')
                {
                    if (GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY - 0.1f))) == 'B')
                    {
                        SetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY - 0.1f), '.');
                    }
                    else if(GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY - 0.1f))) == 'B')
                    {
                        SetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY - 0.1f), '.');
                    }
                    fNewPlayerPosY = (float)Math.Floor(fNewPlayerPosY + 1);
                    fPlayerVelY = 0;
                }
            }
            else
            {
                if (GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 1f))) != '.' ||
                    GetSolidity(GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY + 1f))) != '.')
                {
                    fNewPlayerPosY = (int)Math.Floor(fNewPlayerPosY);
                    fPlayerVelY = 0;
                    nDirModX = 0;
                    bPlayerOnGround = true;
                }
            }

            if (bPlayerOnGround)
            {
                fPlayerVelX += -1.0f * fPlayerVelX * 0.0164f;
                if (Math.Abs(fPlayerVelX) < 0.01f)
                    fPlayerVelX = 0.0f;
            }

            fPlayerPosX = fNewPlayerPosX;
            fPlayerPosY = fNewPlayerPosY;

            fCameraPosX = fPlayerPosX;
            fCameraPosY = fPlayerPosY;

            prevKbState = kbState;

            base.Update(gameTime);
        }

        public int ScreenWidth()
        {

            return (int)(GraphicsDevice.Viewport.Width/scale);
        }
        public int ScreenHeight()
        {
            //return 240;
            return (int)(GraphicsDevice.Viewport.Height/scale);
        }

        public char GetSolidity(char t)
        {
            switch (t)
            {
                case '.':
                case '{':
                case '}':
                    return '.';
                case 'o':
                    return 'o';
                case 'B':
                    return 'B';
                default: return '#';
            }
        }

        public void DrawPartialSprite(int x, int y, Texture2D texture, int ox, int oy, int ow, int oh) { spriteBatch.Draw(texture, new Rectangle(x, y, ow, oh), new Rectangle(ox, oy, ow, oh), Color.White); }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0, 255, 255, 255));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap,
                null, null, null, Matrix.CreateScale(scale));
            // Draw Level
            int nTileWidth = 16;
            int nTileHeight = 16;
            int nVisibleTilesX = ScreenWidth() / nTileWidth;
            int nVisibleTilesY = ScreenHeight() / nTileHeight;

            // Calculate Top-Leftmost visible tile
            float fOffsetX = fCameraPosX - (float)nVisibleTilesX / 2.0f;
            float fOffsetY = fCameraPosY - (float)nVisibleTilesY / 2.0f;

            // Clamp camera to game boundaries
            if (fOffsetX < 0) fOffsetX = 0;
            if (fOffsetY < 0) fOffsetY = 0;
            if (fOffsetX > nLevelWidth - nVisibleTilesX) fOffsetX = nLevelWidth - nVisibleTilesX;
            if (fOffsetY > nLevelHeight - nVisibleTilesY) fOffsetY = nLevelHeight - nVisibleTilesY;

            // Get offsets for smooth movement
            float fTileOffsetX = (fOffsetX - (int)fOffsetX) * nTileWidth;
            float fTileOffsetY = (fOffsetY - (int)fOffsetY) * nTileHeight;

            // Draw visible tile map
            for (int x = -3; x < nVisibleTilesX + 3; x++)
            {
                for (int y = -3; y < nVisibleTilesY + 3; y++)
                {
                    char sTileID = GetTile(x + fOffsetX, y + fOffsetY);
                    switch (sTileID)
                    {
                        case '.': // Sky

                            //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_CYAN);
                            break;
                        case '#': // Solid Block
                            //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_RED);
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 2 * nTileWidth, 0 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case 'G': // Ground Block
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 0 * nTileWidth, 0 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case 'B': // Brick Block
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 0 * nTileWidth, 1 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case '?': // Question Block
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 1 * nTileWidth, 1 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case 'o': // Coin
                            //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_CYAN);
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 3 * nTileWidth, 0 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case '{': // Scenery 1
                            //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_CYAN);
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 2 * nTileWidth, 1 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        case '}':
                            DrawPartialSprite((int)(x * nTileWidth - fTileOffsetX), (int)(y * nTileHeight - fTileOffsetY), spriteTiles, 3 * nTileWidth, 1 * nTileHeight, nTileWidth, nTileHeight);
                            break;
                        default:

                            //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_BLACK);
                            break;
                    }
                }
            }

            // Draw Player
            DrawPartialSprite((int)((fPlayerPosX - fOffsetX) * nTileWidth), (int)((fPlayerPosY - fOffsetY) * nTileWidth), spriteMan, nDirModX * nTileWidth, nDirModY * nTileHeight, nTileWidth, nTileHeight);

            //Debug info
#if DEBUG_INFO
            spriteBatch.DrawString(sf, "POSX " + fPlayerPosX.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(sf, "POSY " + fPlayerPosY.ToString(), new Vector2(10, 20), Color.White);
            spriteBatch.DrawString(sf, "HSPD " + fPlayerVelX.ToString(), new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(sf, "VSPD " + fPlayerVelY.ToString(), new Vector2(10, 40), Color.White);
#endif

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
