using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class UIhandler
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private List<Texture2D> UISprites = new List<Texture2D>();
    private List<(int, int)> spriteCoords = new List<(int, int)>();
    private Texture2D scoreboard, shop, hut, buildmenubox, closehammer, hammer, woodicon, ironicon, coinicon, gearicon;
    private SpriteFont font;

    public bool buildMenuOpen = false;

    private List<(Texture2D sprite, (int x, int y) coords)> buildMenuItems;

    // In LoadContent or your constructor:


    public UIhandler(GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch)
    {
        _graphics = graphicsDeviceManager;
        _spriteBatch = spriteBatch;
    }

    public void LoadUIContent(Microsoft.Xna.Framework.Content.ContentManager Content)
    {
        buildmenubox = Content.Load<Texture2D>("buildmenubox");
        scoreboard = Content.Load<Texture2D>("scoreboard");
        shop = Content.Load<Texture2D>("shop");
        hut = Content.Load<Texture2D>("hut");
        hammer = Content.Load<Texture2D>("hammer");
        closehammer = Content.Load<Texture2D>("closehammer");
        woodicon = Content.Load<Texture2D>("woodicon");
        ironicon = Content.Load<Texture2D>("ironicon");
        coinicon = Content.Load<Texture2D>("coinicon");
        gearicon = Content.Load<Texture2D>("gear"); // Load the gear icon
        font = Content.Load<SpriteFont>("score");



        buildMenuItems = new List<(Texture2D, (int, int))>
        {
            (closehammer, (50, 930)),
            (buildmenubox, (200, 770)),
            (buildmenubox, (530, 770))
        };
    }

    public void DrawUI(int wood, int iron, int coins)
    {
        // Draw resource pane
        _spriteBatch.Draw(scoreboard, new Vector2(50, 50), Color.White);
        _spriteBatch.Draw(woodicon, new Vector2(110, 75), Color.White);
        _spriteBatch.Draw(ironicon, new Vector2(380, 100), Color.White);
        _spriteBatch.Draw(coinicon, new Vector2(650, 80), Color.White);


        _spriteBatch.DrawString(font, wood.ToString(), new Vector2(180, 100), Color.Black);
        _spriteBatch.DrawString(font, iron.ToString(), new Vector2(465, 100), Color.Black);
        _spriteBatch.DrawString(font, coins.ToString(), new Vector2(725, 100), Color.Black);

        // Draw gear icon in bottom right corner
        int gearX = _graphics.PreferredBackBufferWidth - gearicon.Width - 20;
        int gearY = _graphics.PreferredBackBufferHeight - gearicon.Height - 20;
        _spriteBatch.Draw(gearicon, new Vector2(gearX, gearY), Color.White);

        // Draw build menu if open
        if (buildMenuOpen)
        {
            drawBuildMenu();
        }
    }


    protected void drawBuildMenu()
    {
        for (int i = 0; i < buildMenuItems.Count; i++)
        {
            _spriteBatch.Draw(buildMenuItems[i].sprite, new Vector2(buildMenuItems[i].coords.x, buildMenuItems[i].coords.y), Color.White);
        }

        _spriteBatch.Draw(shop, new Vector2(240, 790), Color.White);

        // prices
        _spriteBatch.DrawString(font, "10", new Vector2(265, 990), Color.Black); // wood
        _spriteBatch.DrawString(font, "10", new Vector2(365, 990), Color.Black); // iron
        _spriteBatch.DrawString(font, "0", new Vector2(465, 990), Color.Black); // coins

        _spriteBatch.Draw(hut, new Vector2(570, 790), Color.White);
        _spriteBatch.DrawString(font, "15", new Vector2(595, 990), Color.Black);
        _spriteBatch.DrawString(font, "5", new Vector2(695, 990), Color.Black);
        _spriteBatch.DrawString(font, "25", new Vector2(795, 990), Color.Black);
    }

    public void OpenBuildMenu()
    {
        buildMenuOpen = true;
        Console.WriteLine("Build menu opened.");
    }

    public void CloseBuildMenu()
    {
        buildMenuOpen = false;
        Console.WriteLine("Build menu closed.");
    }

    public int HandleBuildMenuClick(Point mousePoint, int lr)
    {

        for (int j = 0; j < buildMenuItems.Count; j++)
            {
                int xStart = buildMenuItems[j].coords.x;
                int yStart = buildMenuItems[j].coords.y;
                int width = buildMenuItems[j].sprite.Width;
                int height = buildMenuItems[j].sprite.Height;



                Rectangle sprRect = new Rectangle(xStart, yStart, width, height);

                if (sprRect.Contains(mousePoint))
                {
                if (j == 0)
                {
                    // clicked on hammer - nothing for now
                    return 0;
                }
                else if (j == 1)
                {
                    Console.WriteLine("Shop box clicked");
                    // clicked on shop box
                    return 1;
                }
                else if (j == 2)
                {
                    // clicked on hut box
                    return 2;
                }
                }
            }
        return 0;
    }
}