using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Buildings;
using System.Xml.XPath;
using System.Security.Cryptography;

namespace VillageIncremental;

public class VillageIncrementalGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private UIhandler uiHandler;

    private Song[] musicTracks;
    private int currentTrack = 0;
    private TimeSpan currentTrackPosition = TimeSpan.Zero;



    // Texture2D instance variables
    private Texture2D background;
    private Texture2D scoreboard;
    private Texture2D hammer;
    private Texture2D closehammer;
    private Texture2D mine;
    private Texture2D tree;

    private Texture2D woodicon;
    private Texture2D ironicon;
    private Texture2D coinicon;
    private Texture2D shop;
    private Texture2D shopchoose;
    private Texture2D woodshop;
    private Texture2D ironshop;
    private Texture2D hut;
    private Texture2D woodHut;
    private Texture2D ironHut;
    private Texture2D hutchoose;

    SoundEffect chopSound;
    SoundEffect mineSound;
    SoundEffect errorSound;
    SoundEffect buildSound;
    SoundEffect ironSound;
    SoundEffect kachingSound;
    SoundEffect woodSound;


    private List<(Texture2D sprite, (int x, int y) coords)> clickables;


    private List<Building> myBuildings;

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
    private int building;
    private bool buildingQueued;
    //private bool menuOpen;

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
        building = 0;
        buildingQueued = false;
        myBuildings = new List<Building>();
        //menuOpen = false;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);



        chopSound = Content.Load<SoundEffect>("chopsound");
        mineSound = Content.Load<SoundEffect>("minesound");
        errorSound = Content.Load<SoundEffect>("errorsound");
        buildSound = Content.Load<SoundEffect>("buildsound");
        ironSound = Content.Load<SoundEffect>("ironsound");
        kachingSound = Content.Load<SoundEffect>("kaching");
        woodSound = Content.Load<SoundEffect>("woodsound");

        background = Content.Load<Texture2D>("background");
        hammer = Content.Load<Texture2D>("hammer");
        closehammer = Content.Load<Texture2D>("closehammer");
        mine = Content.Load<Texture2D>("mine");
        tree = Content.Load<Texture2D>("tree");
        hut = Content.Load<Texture2D>("hut");
        woodHut = Content.Load<Texture2D>("woodHut");
        ironHut = Content.Load<Texture2D>("ironHut");
        shop = Content.Load<Texture2D>("shop");
        shopchoose = Content.Load<Texture2D>("shopchoose");
        hutchoose = Content.Load<Texture2D>("hutchoose");

        woodshop = Content.Load<Texture2D>("woodshop");
        ironshop = Content.Load<Texture2D>("ironshop");

        woodicon = Content.Load<Texture2D>("woodicon");
        ironicon = Content.Load<Texture2D>("ironicon");
        coinicon = Content.Load<Texture2D>("coinicon");


        uiHandler = new UIhandler(_graphics, _spriteBatch);
        uiHandler.LoadUIContent(Content);


        clickables = new List<(Texture2D, (int, int))>
        {
            (hammer, (50, 930)), // hammer
            (mine, (1300, 150)), // mine
            (tree, (1500, 750)), // tree
            (tree, (1400, 670)) // tree
        };


        scoreboard = Content.Load<Texture2D>("scoreboard");
        font = Content.Load<SpriteFont>("score");

        musicTracks = new Song[3];
        musicTracks[0] = Content.Load<Song>("villageincrementalsong0");
        musicTracks[1] = Content.Load<Song>("villageincrementalsong1");
        musicTracks[2] = Content.Load<Song>("villageincrementalsong2");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(musicTracks[0]);
        currentTrack = 0;


    }
    

    
    private void switchMusic(int newTrack)
    {
        if (newTrack == currentTrack) return;
        TimeSpan ts1 = new TimeSpan(11300000);

        currentTrackPosition = MediaPlayer.PlayPosition;
        TimeSpan startPosition = currentTrackPosition > ts1 
            ? currentTrackPosition.Subtract(ts1) 
            : musicTracks[currentTrack].Duration.Subtract(ts1);
        MediaPlayer.Play(musicTracks[newTrack], startPosition);
        currentTrack = newTrack;
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

        int xmod = 0;
        int ymod = 0;

        Console.WriteLine("Building: " + building);
        if (building == 1)
        {
            xmod = shop.Width * 2;
            ymod = shop.Height * 2;
        }
        else if (building == 2)
        {
            xmod = hut.Width * 2;
            ymod = hut.Height * 2;
        }




        for (int i = 0; i < clickables.Count; i++)
        {
            int xStart = clickables[i].coords.x - xmod;
            int yStart = clickables[i].coords.y - ymod;
            int width = clickables[i].sprite.Width + xmod;
            int height = clickables[i].sprite.Height + ymod;

            Rectangle sprRect = new Rectangle(xStart, yStart, width, height);
            if (sprRect.Contains(mousePoint))
            {
                if (i == 0)
                {
                    toggleBuildMenu();
                    // Clicked hammer
                }
                else if (i == 1)
                {
                    // Clicked mine
                    if (building == 0)
                    {
                        ironStock++;
                        mineSound.Play();
                    }
                }
                else if (i == 2)
                {
                    // Clicked tree
                    if (building == 0)
                    {
                        woodStock++;
                        chopSound.Play();
                    }
                }
                else if (i == 3)
                {
                    // Clicked tree
                    if (building == 0)
                    {
                        woodStock++;
                        chopSound.Play();
                    }
                }
                return;
            }
        }

        foreach (Building build in myBuildings)
        {
            int checkingx = build.getCoords().Item1 - xmod;
            int checkingy = build.getCoords().Item2 - ymod;
            int checkingWidth = build.width + xmod;
            int checkingHeight = build.height + ymod;




            Rectangle sprRect = new Rectangle(checkingx, checkingy, checkingWidth, checkingHeight);
            if (sprRect.Contains(mousePoint))
            {
                if (build is Shop shop)
                {
                    if (lr == 0)
                    {
                        (int first, int second) result = shop.lclick(mousePoint);
                        if (result != (0, 0))
                        {

                            if (result.second == 3)
                            {
                                ironSound.Play();
                            }
                            else if (result.second == 2)
                            {
                                woodSound.Play();
                            }

                            woodRate -= ((result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0));
                            woodSellRate += ((result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0)) * shop.rate;
                            ironRate -= ((result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0));
                            ironSellRate += ((result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0)) * shop.rate;
                            
                            kachingSound.Play();
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
                            if (result.second == 3)
                            {
                                ironSound.Play();
                            }
                            else if (result.second == 2)
                            {
                                woodSound.Play();
                            }

                            woodRate += (result.second == 2 ? 1 : 0) - (result.first == 2 ? 1 : 0) * hut.rate;
                            ironRate += (result.second == 3 ? 1 : 0) - (result.first == 3 ? 1 : 0) * hut.rate;
                        }
                    }
                    else if (lr == 1)
                    {
                        hut.rclick(mousePoint);
                    }

                }
                return;

            }
        }
    
                
        if (uiHandler.buildMenuOpen)
        {
            if (buildingQueued)
            {
                handleBuild(curState);
                buildingQueued = false;
                return;
            }
            building = uiHandler.HandleBuildMenuClick(mousePoint, lr);
            if (building > 0 && checkBuildReqs())
            {
                buildingQueued = true;
            }
            return;
        }


    }

    protected void tickSec()
    {
        if (woodRate > 0)
        {
            woodStock += woodRate;
            coins += woodSellRate;
        }
        else if (woodStock + woodRate >= 0)
        {
            coins += woodSellRate;
            woodStock += woodRate;
        }
        if (ironRate > 0)
        {
            ironStock += ironRate;
            coins += ironSellRate;
        }
        else if (ironStock + ironRate >= 0)
        {
            coins += ironSellRate;
            ironStock += ironRate;
        }
    }







    protected void handleBuild(MouseState newState)
    {
        Console.WriteLine("Building: " + building);
        if (building == 1)
        {
            // shop
            if (checkBuildReqs())
            {
                buildSound.Play();



                woodStock -= 10;
                ironStock -= 10;
                int xCoord = newState.X - (shop.Width / 2);
                int yCoord = newState.Y - (shop.Height / 2);
                Building thisShop = new Shop((xCoord, yCoord), shop.Width, shop.Height, shop, shopchoose, woodshop, ironshop);
                myBuildings.Add(thisShop);
                building = 0;
                if (currentTrack == 0)
                {
                    switchMusic(1);
                }

                uiHandler.CloseBuildMenu();
            }
        }
        else if (building == 2)
        {
            // hut
            if (checkBuildReqs())
            {
                buildSound.Play();

                

                woodStock -= 15;
                ironStock -= 5;
                coins -= 25;
                int xCoord = newState.X - (hut.Width / 2);
                int yCoord = newState.Y - (hut.Height / 2);
                Building thisBuilding = new Hut((xCoord, yCoord), hut.Width, hut.Height, hut, hutchoose, woodHut, ironHut);
                myBuildings.Add(thisBuilding);
                building = 0;
                if (currentTrack == 1)
                {
                    switchMusic(2);
                }
                uiHandler.CloseBuildMenu();
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
        buildingQueued = false;
        building = 0;
        errorSound.Play();
        return false;
    }


    protected void toggleBuildMenu(){
        building = 0;
        buildingQueued = false;
        if (uiHandler.buildMenuOpen)
        {
            uiHandler.CloseBuildMenu();
        }
        else
        {
            uiHandler.OpenBuildMenu();
        }
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);



        for (int i = 0; i < clickables.Count; i++)
        {
            _spriteBatch.Draw(clickables[i].sprite, new Vector2(clickables[i].coords.Item1, clickables[i].coords.Item2), Color.White);
        }


        foreach (Building build in myBuildings)
        {
            _spriteBatch.Draw(build.getTexture(), new Vector2(build.getCoords().Item1, build.getCoords().Item2), Color.White);
        }


        MouseState newState = Mouse.GetState();


        if (building == 1)
        {
            _spriteBatch.Draw(shop, new Vector2(newState.X - shop.Width / 2, newState.Y - shop.Height / 2), Color.White);

        }
        else if (building == 2)
        {
            _spriteBatch.Draw(hut, new Vector2(newState.X - hut.Width / 2, newState.Y - hut.Height / 2), Color.White);
        }


        uiHandler.DrawUI(woodStock, ironStock, coins);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}