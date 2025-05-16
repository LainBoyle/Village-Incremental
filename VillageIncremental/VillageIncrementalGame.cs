using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Buildings;
using System.Xml.XPath;

namespace VillageIncremental;

public class VillageIncrementalGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private UIhandler uiHandler;

    // Texture2D instance variables
    private Texture2D background;
    private Texture2D scoreboard;
    private Texture2D hammer;
    private Texture2D closehammer;
    private Texture2D mine;
    private Texture2D tree;
    private Texture2D shop;
    private Texture2D shopchoose;
    private Texture2D woodshop;
    private Texture2D ironshop;
    private Texture2D hut;
    private Texture2D woodHut;
    private Texture2D ironHut;
    private Texture2D hutchoose;
    private Texture2D buildmenubox;

    private List<Texture2D> mySprites;
    private List<Building> myBuildings;
    private List<Building> myShops;
    private List<(int, int)> spriteCoords;
    private List<Texture2D> buildMenuSprites;
    private List<(int, int)> buildMenuSpriteCoords;

    private double clockCount = 1;

    // Font variables
    private SpriteFont font;
    private int woodStock = 0;
    private int ironStock = 0;
    private int coins = 0;
    private int woodRate = 0;
    private int ironRate = 0;
    private int ironSellRate = 0;
    private int woodSellRate = 0;

    // UI variables
    private MouseState oldState;
    private bool buildMenuOpen;
    private int building;
    private bool menuOpen;

    public VillageIncrementalGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.PreferredBackBufferWidth = 1920;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        buildMenuOpen = false;
        building = 0;
        myBuildings = new List<Building>();
        myShops = new List<Building>();
        menuOpen = false;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        mySprites = new List<Texture2D>();
        buildMenuSprites = new List<Texture2D>();
        buildMenuSpriteCoords = new List<(int, int)>();

        background = Content.Load<Texture2D>("background");
        hammer = Content.Load<Texture2D>("hammer");
        mySprites.Add(hammer);
        closehammer = Content.Load<Texture2D>("closehammer");
        mine = Content.Load<Texture2D>("mine");
        mySprites.Add(mine);
        tree = Content.Load<Texture2D>("tree");
        mySprites.Add(tree);
        mySprites.Add(tree);
        hut = Content.Load<Texture2D>("hut");
        woodHut = Content.Load<Texture2D>("woodHut");
        ironHut = Content.Load<Texture2D>("ironHut");
        shop = Content.Load<Texture2D>("shop");
        shopchoose = Content.Load<Texture2D>("shopchoose");
        hutchoose = shopchoose;

        woodshop = Content.Load<Texture2D>("woodshop");
        ironshop = Content.Load<Texture2D>("ironshop");
        buildmenubox = Content.Load<Texture2D>("buildmenubox");

        buildMenuSpriteCoords.Add((50, 930));
        buildMenuSprites.Add(closehammer);
        buildMenuSpriteCoords.Add((200, 770));
        buildMenuSprites.Add(buildmenubox);
        buildMenuSpriteCoords.Add((530, 770));
        buildMenuSprites.Add(buildmenubox);

        uiHandler = new UIhandler(_graphics, _spriteBatch);
        uiHandler.LoadUIContent(Content);

        spriteCoords = new List<(int, int)>
        {
            (50, 930), // hammer
            (1300, 150), // mine
            (1500, 750), // tree
            (1400, 670) // tree
        };

        scoreboard = Content.Load<Texture2D>("scoreboard");
        font = Content.Load<SpriteFont>("score");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        MouseState newState = Mouse.GetState();

        if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
        {
            checkMouse(newState, 0);
        }
        else if (newState.RightButton == ButtonState.Pressed && oldState.RightButton == ButtonState.Released)
        {
            checkMouse(newState, 1);
        }

        clockCount += gameTime.ElapsedGameTime.TotalSeconds;

        if (clockCount >= 1)
        {
            clockCount -= 1;
            tickSec();
        }

        oldState = newState;
        base.Update(gameTime);
    }

    protected void checkMouse(MouseState curState, int lr)
    {
        // if lr 0, left click, otherwise right click
        Point mousePoint = new Point(curState.X, curState.Y);
        bool counting = true;

        if (menuOpen)
        {
            uiHandler.HandleClick(mousePoint, lr);
            return;
        }

        for (int i = 0; i < mySprites.Count; i++)
        {
            int xStart = spriteCoords[i].Item1;
            int yStart = spriteCoords[i].Item2;
            int width = mySprites[i].Width;
            int height = mySprites[i].Height;

            Rectangle sprRect = new Rectangle(xStart, yStart, width, height);
            if (counting && sprRect.Contains(mousePoint))
            {
                if (i == 0)
                {
                    // Clicked Hammer
                    counting = false;
                    if (buildMenuOpen == false)
                    {
                        openBuildMenu();
                    }
                    else
                    {
                        building = 0;
                        closeBuildMenu();
                    }
                }
                else if (i == 1)
                {
                    // Clicked mine
                    if (building == 0)
                    {
                        ironStock++;
                    }
                    counting = false;
                }
                else if (i == 2)
                {
                    // Clicked tree
                    if (building == 0)
                    {
                        woodStock++;
                    }
                    counting = false;
                }
                else if (i == 3)
                {
                    // Clicked tree
                    if (building == 0)
                    {
                        woodStock++;
                    }
                    counting = false;
                }
            }
        }

        foreach (Building build in myBuildings)
        {
            Rectangle sprRect = new Rectangle(build.getCoords().Item1, build.getCoords().Item2, build.width, build.height);
            if (counting && sprRect.Contains(mousePoint))
            {
                if (build is Shop shop)
                {
                    if (lr == 0)
                    {
                        (int first, int second) result = shop.lclick(mousePoint);
                        if (result != (0, 0))
                        {
                            woodRate -= ((result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0));
                            woodSellRate += ((result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0)) * shop.rate;
                            ironRate -= ((result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0));
                            ironSellRate += ((result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0)) * shop.rate;
                        }
                    }
                    else if (lr == 1)
                    {
                        shop.rclick(mousePoint);
                    }
                }
                else if (build is Hut hut)
                {
                    if (lr == 0)
                    {
                        (int first, int second) result = hut.lclick(mousePoint);
                        if (result != (0, 0))
                        {
                            woodRate += (result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0) * hut.rate;
                            ironRate += (result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0) * hut.rate;
                        }
                    }
                    else if (lr == 1)
                    {
                        hut.rclick(mousePoint);
                    }
                }
            }
        }

        if (buildMenuOpen)
        {
            for (int j = 0; j < buildMenuSprites.Count; j++)
            {
                int xStart = buildMenuSpriteCoords[j].Item1;
                int yStart = buildMenuSpriteCoords[j].Item2;
                int width = buildMenuSprites[j].Width;
                int height = buildMenuSprites[j].Height;

                if (building == 1)
                {
                    xStart -= shop.Width / 2;
                    yStart -= shop.Height / 2;
                    width += shop.Width / 2;
                    height += shop.Height / 2;
                }
                else if (building == 2)
                {
                    xStart -= hut.Width / 2;
                    yStart -= hut.Height / 2;
                    width += hut.Width / 2;
                    height += hut.Height / 2;
                }

                Rectangle sprRect = new Rectangle(xStart, yStart, width, height);

                if (counting && sprRect.Contains(mousePoint))
                {
                    if (j == 0)
                    {
                        // clicked on hammer - nothing for now
                    }
                    else if (j == 1)
                    {
                        // clicked on shop box
                        building = 1;
                        counting = false;
                    }
                    else if (j == 2)
                    {
                        // clicked on hut box
                        building = 2;
                        counting = false;
                    }
                }
            }
        }

        if (counting)
        {
            // mouse click no overlap
            handleBuild(curState);
        }
    }

    protected void tickSec()
    {
        if (woodRate > 0)
        {
            woodStock += woodRate;
            coins += woodSellRate;
        }
        else if (woodStock + woodRate > 0)
        {
            coins += woodSellRate;
            woodStock += woodRate;
        }
        if (ironRate > 0)
        {
            ironStock += ironRate;
            coins += ironSellRate;
        }
        else if (ironStock + ironRate > 0)
        {
            coins += ironSellRate;
            ironStock += ironRate;
        }
    }


    protected void openBuildMenu()
    {
        buildMenuOpen = true;
    }

    protected void drawBuildMenu()
    {
        for (int i = 0; i < buildMenuSprites.Count; i++)
        {
            _spriteBatch.Draw(buildMenuSprites[i], new Vector2(buildMenuSpriteCoords[i].Item1, buildMenuSpriteCoords[i].Item2), Color.White);
        }

        _spriteBatch.Draw(shop, new Vector2(240, 790), Color.White);

        // prices
        _spriteBatch.DrawString(font, "10", new Vector2(275, 1000), Color.Black); // wood
        _spriteBatch.DrawString(font, "10", new Vector2(375, 1000), Color.Black); // iron
        _spriteBatch.DrawString(font, "0", new Vector2(475, 1000), Color.Black); // coins

        _spriteBatch.Draw(hut, new Vector2(570, 790), Color.White);
        _spriteBatch.DrawString(font, "15", new Vector2(605, 1000), Color.Black);
        _spriteBatch.DrawString(font, "5", new Vector2(705, 1000), Color.Black);
        _spriteBatch.DrawString(font, "25", new Vector2(805, 1000), Color.Black);
    }

    protected void closeBuildMenu()
    {
        mySprites.Remove(buildmenubox);
        mySprites.Remove(buildmenubox);
        spriteCoords.Remove((200, 770));
        spriteCoords.Remove((530, 770));
        buildMenuOpen = false;
    }



    protected void handleBuild(MouseState newState)
    {
        if (building == 1)
        {
            // shop
            if (checkBuildReqs())
            {
                woodStock -= 10;
                ironStock -= 10;
                int xCoord = newState.X - (shop.Width / 2);
                int yCoord = newState.Y - (shop.Height / 2);
                Building thisShop = new Shop((xCoord, yCoord), shop.Width, shop.Height, shop, shopchoose, woodshop, ironshop);
                myBuildings.Add(thisShop);
                myShops.Add(thisShop);
                building = 0;
                closeBuildMenu();
            }
        }
        else if (building == 2)
        {
            // hut
            if (checkBuildReqs())
            {
                woodStock -= 15;
                ironStock -= 5;
                coins -= 25;
                int xCoord = newState.X - (hut.Width / 2);
                int yCoord = newState.Y - (hut.Height / 2);
                Building thisBuilding = new Hut((xCoord, yCoord), hut.Width, hut.Height, hut, hutchoose, woodHut, ironHut);
                myBuildings.Add(thisBuilding);
                building = 0;
                closeBuildMenu();
            }
        }
    }

    protected bool checkBuildReqs()
    {
        if (building == 1)
        {
            if ((woodStock >= 10) && (ironStock >= 10))
            {
                return true;
            }
        }
        else if (building == 2)
        {
            if ((woodStock >= 15) && (ironStock >= 5) && (coins >= 25))
            {
                return true;
            }
        }
        return false;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);

        int i = 0;
        foreach (Texture2D texture in mySprites)
        {
            _spriteBatch.Draw(texture, new Vector2(spriteCoords[i].Item1, spriteCoords[i].Item2), Color.White);
            i++;
        }

        _spriteBatch.Draw(scoreboard, new Vector2(50, 50), Color.White);
        _spriteBatch.DrawString(font, woodStock.ToString(), new Vector2(180, 100), Color.Black);
        _spriteBatch.DrawString(font, ironStock.ToString(), new Vector2(465, 100), Color.Black);
        _spriteBatch.DrawString(font, coins.ToString(), new Vector2(725, 100), Color.Black);

        foreach (Building build in myBuildings)
        {
            _spriteBatch.Draw(build.getTexture(), new Vector2(build.getCoords().Item1, build.getCoords().Item2), Color.White);
            i++;
        }

        if (buildMenuOpen)
        {
            drawBuildMenu();
        }
        
        MouseState newState = Mouse.GetState();


        if (building == 1)
        {
            _spriteBatch.Draw(shop, new Vector2(newState.X - shop.Width/2, newState.Y - shop.Height/2), Color.White);

        }
        else if (building == 2)
        {
            _spriteBatch.Draw(hut, new Vector2(newState.X - hut.Width/2, newState.Y - hut.Height/2), Color.White);
        }


        //uiHandler.DrawUI(woodStock, ironStock, coins);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}