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

    SpriteFont sf;

	// Sprite selection flags
	int nDirModX = 0;
	int nDirModY = 0;

        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;

            graphics.PreferredBackBufferWidth = 512;
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
		    sLevel += "...................####.........................................";
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

		char GetTile (float x, float y)
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

		void SetTile (float x, float y, char c)
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
            fPlayerVelY += fGravityFactor*0.0164f;


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
                    fPlayerVelX += -6.0f*0.0164f;
                    nDirModY = 1;
                }
                if (kbState.IsKeyDown(Keys.Right))
                {
                    fPlayerVelX += 6.0f*0.0164f;
                    nDirModY = 0;
                }
                if (kbState.IsKeyDown(Keys.Space) && !prevKbState.IsKeyDown(Keys.Space))
                {
                    if (fPlayerVelY >= -0.5f && fPlayerVelY <= 0.5f)
                    {
                        fPlayerVelY = -12.0f;
                    }
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

            if (fPlayerVelY >= -0.5f && fPlayerVelY <= 0.5f)
            {
                fPlayerVelX += -1.0f * fPlayerVelX * 0.0164f;
                if (Math.Abs(fPlayerVelX) < 0.01f)
                    fPlayerVelX = 0.0f;
            }

            float fNewPlayerPosX = fPlayerPosX + fPlayerVelX * 0.0164f;
            float fNewPlayerPosY = fPlayerPosY + fPlayerVelY * 0.0164f;

            if (GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 0.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)(int)fNewPlayerPosY + 0.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f, '.');

            //Collision
            if (fPlayerVelX <= 0)
            {
                if (GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.0f)) != '.' ||
                    GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 0.8f)) != '.')
                {
                    fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX + 1);
                    fPlayerVelX = 0;
                }
            }
            else
            {
                if (GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.0f)) != '.' ||
                    GetTile((int)Math.Floor(fNewPlayerPosX + 1.0f), (int)Math.Floor(fPlayerPosY + 0.8f)) != '.')
                {
                    fNewPlayerPosX = (int)Math.Floor(fNewPlayerPosX);
                    fPlayerVelX = 0;
                }
            }

            if (fPlayerVelY <= 0)
            {
                if (GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY-0.1f)) != '.' ||
                    GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY-0.1f)) != '.')
                {
                    fNewPlayerPosY = (float)Math.Floor(fNewPlayerPosY+1);
                    fPlayerVelY = 0;
                }
            }
            else
            {
                if (GetTile((int)Math.Floor(fNewPlayerPosX + 0.0f), (int)Math.Floor(fPlayerPosY + 1f)) != '.' ||
                    GetTile((int)Math.Floor(fNewPlayerPosX + 0.9f), (int)Math.Floor(fPlayerPosY + 1f)) != '.')
                {
                    fNewPlayerPosY = (int)Math.Floor(fNewPlayerPosY);
                    fPlayerVelY = 0;
                }
            }


            fPlayerPosX = fNewPlayerPosX;
            fPlayerPosY = fNewPlayerPosY;

            fCameraPosX = fPlayerPosX;
            fCameraPosY = fPlayerPosY;

            prevKbState = kbState;

		    // Handle Input
		    /*if (IsFocused)
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
				    fPlayerVelX += (bPlayerOnGround ? -25.0f : -15.0f)*0.01f;
				    nDirModY = 1;
			    }

			    if (kbState.IsKeyDown(Keys.Right))
			    {
				    fPlayerVelX += (bPlayerOnGround ? 25.0f : 15.0f)*0.01f;
				    nDirModY = 0;
			    }

			    if (kbState.IsKeyDown(Keys.Space) && prevKbState.IsKeyUp(Keys.Space))
			    {
				    if (fPlayerVelY == 0)
				    {
					    fPlayerVelY = -12.0f;
					    nDirModX = 1;
				    }
			    }
		    }

		    // Gravity
		    fPlayerVelY += 20.0f * fElapsedTime;

		    // Drag
		    if (bPlayerOnGround)
		    {
			    fPlayerVelX += -3.0f * fPlayerVelX * fElapsedTime;
			    if (Math.Abs(fPlayerVelX) < 0.01f)
				    fPlayerVelX = 0.0f;
		    }

		    // Clamp velocities
		    if (fPlayerVelX > 10.0f)
			    fPlayerVelX = 10.0f;

		    if (fPlayerVelX < -10.0f)
			    fPlayerVelX = -10.0f;

		    if (fPlayerVelY > 100.0f)
			    fPlayerVelY = 100.0f;

		    if (fPlayerVelY < -100.0f)
			    fPlayerVelY = -100.0f;

		    // Calculate potential new position
		    float fNewPlayerPosX = fPlayerPosX + fPlayerVelX * 0.1f;
            float fNewPlayerPosY = fPlayerPosY + fPlayerVelY * 0.1f;

		    // Check for pickups!
            if (GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 0.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)(int)fNewPlayerPosY + 0.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 0.0f, (int)fNewPlayerPosY + 1.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 0.0f, '.');

            if (GetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f) == 'o')
                SetTile((int)fNewPlayerPosX + 1.0f, (int)fNewPlayerPosY + 1.0f, '.');

		    // Check for Collision
		    if (fPlayerVelX <= 0) // Moving Left
		    {
			    if (GetTile(fNewPlayerPosX + 0.0f, fPlayerPosY + 0.0f) != '.' || GetTile(fNewPlayerPosX + 0.0f, fPlayerPosY + 0.9f) != '.')
			    {
				    fNewPlayerPosX = (int)fNewPlayerPosX + 1;
				    fPlayerVelX = 0;
			    }
		    }
		    else // Moving Right
		    {
			    if (GetTile(fNewPlayerPosX + 1.0f, fPlayerPosY + 0.0f) != '.' || GetTile(fNewPlayerPosX + 1.0f, fPlayerPosY + 0.9f) != '.')
			    {
				    fNewPlayerPosX = (int)fNewPlayerPosX;
				    fPlayerVelX = 0;

			    }
		    }

		    bPlayerOnGround = false;
		    if (fPlayerVelY <= 0) // Moving Up
		    {
			    if (GetTile(fPlayerPosX + 0.0f, fNewPlayerPosY) != '.' || GetTile(fPlayerPosX + 0.9f, fNewPlayerPosY) != '.')
			    {
				    fNewPlayerPosY = (int)fNewPlayerPosY + 1;
				    fPlayerVelY = 0;
			    }
		    }
		    else // Moving Down
		    {
			    if (GetTile(fNewPlayerPosX + 0.0f, fNewPlayerPosY + 1.0f) != '.' || GetTile(fNewPlayerPosX + 0.9f, fNewPlayerPosY + 1.0f) != '.')
			    {
				    fNewPlayerPosY = (int)fNewPlayerPosY;
				    fPlayerVelY = 0;
				    bPlayerOnGround = true; // Player has a solid surface underfoot
				    nDirModX = 0;
			    }
		    }

		    // Apply new position
		    fPlayerPosX = fNewPlayerPosX;
		    fPlayerPosY = fNewPlayerPosY;

		    // Link camera to player position
		    fCameraPosX = fPlayerPosX;
		    fCameraPosY = fPlayerPosY;
            */
		
	

            base.Update(gameTime);
        }

        public int ScreenWidth()
        {

            return 256;
            //return GraphicsDevice.Viewport.Width;
        }
        public int ScreenHeight()
        {
            return 240;
            //return GraphicsDevice.Viewport.Height;
        }

        public void DrawPartialSprite(int x, int y, Texture2D texture, int ox, int oy, int ow, int oh) { spriteBatch.Draw(texture, new Rectangle(x, y, ow, oh), new Rectangle(ox, oy, ow, oh), Color.White); }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0,255,255,255));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, 
                null, null, null, Matrix.CreateScale(2f));
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
            for (int x = -1; x < nVisibleTilesX + 1; x++)
            {
                for (int y = -1; y < nVisibleTilesY + 1; y++)
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
                    default:

                        //Fill(x * nTileWidth - fTileOffsetX, y * nTileHeight - fTileOffsetY, (x + 1) * nTileWidth - fTileOffsetX, (y + 1) * nTileHeight - fTileOffsetY, PIXEL_SOLID, FG_BLACK);
                        break;
                    }
                }
            }

            // Draw Player
            DrawPartialSprite((int)((fPlayerPosX - fOffsetX) * nTileWidth), (int)((fPlayerPosY - fOffsetY) * nTileWidth), spriteMan, nDirModX * nTileWidth, nDirModY * nTileHeight, nTileWidth, nTileHeight);
            
            //Debug info
            //spriteBatch.DrawString(sf, "POSX " + fPlayerPosX.ToString(), new Vector2(10, 10), Color.White);
            //spriteBatch.DrawString(sf, "POSY " + fPlayerPosY.ToString(), new Vector2(10, 20), Color.White);
            //spriteBatch.DrawString(sf, "HSPD " + fPlayerVelX.ToString(), new Vector2(10, 30), Color.White);
            //spriteBatch.DrawString(sf, "VSPD " + fPlayerVelY.ToString(), new Vector2(10, 40), Color.White);

            //spriteBatch.DrawString(sf, "POSX " + fPlayerPosX.ToString(), new Vector2(10, 10), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
