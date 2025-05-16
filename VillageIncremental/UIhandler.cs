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
    private Texture2D shop, hut, buildmenubox, closeButton, hammer, woodIcon, ironIcon, coinIcon, gearIcon;
    private SpriteFont font;

    private bool buildMenuOpen = false;

    public UIhandler(GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch)
    {
        _graphics = graphicsDeviceManager;
        _spriteBatch = spriteBatch;
    }

public void LoadUIContent(Microsoft.Xna.Framework.Content.ContentManager Content)
{
    buildmenubox = Content.Load<Texture2D>("buildmenubox");
    shop = Content.Load<Texture2D>("shop");
    hut = Content.Load<Texture2D>("hut");
    hammer = Content.Load<Texture2D>("hammer");
    closeButton = Content.Load<Texture2D>("closehammer");
    woodIcon = Content.Load<Texture2D>("woodicon");
    ironIcon = Content.Load<Texture2D>("ironicon");
    coinIcon = Content.Load<Texture2D>("coinicon");
    gearIcon = Content.Load<Texture2D>("gear"); // Load the gear icon
    font = Content.Load<SpriteFont>("score");
}

// ...existing code...

public void DrawUI(int wood, int iron, int coins)
{
    // Draw resource panel
    _spriteBatch.Draw(buildmenubox, new Vector2(20, 20), Color.White);
    _spriteBatch.Draw(woodIcon, new Vector2(40, 40), Color.White);
    _spriteBatch.DrawString(font, wood.ToString(), new Vector2(80, 40), Color.Black);
    _spriteBatch.Draw(ironIcon, new Vector2(40, 80), Color.White);
    _spriteBatch.DrawString(font, iron.ToString(), new Vector2(80, 80), Color.Black);
    _spriteBatch.Draw(coinIcon, new Vector2(40, 120), Color.White);
    _spriteBatch.DrawString(font, coins.ToString(), new Vector2(80, 120), Color.Black);

    // Draw gear icon in bottom right corner
    int gearX = _graphics.PreferredBackBufferWidth - gearIcon.Width - 20;
    int gearY = _graphics.PreferredBackBufferHeight - gearIcon.Height - 20;
    _spriteBatch.Draw(gearIcon, new Vector2(gearX, gearY), Color.White);

    // Draw build menu if open
    if (buildMenuOpen)
    {
        _spriteBatch.Draw(buildmenubox, new Vector2(200, 200), Color.White);
        _spriteBatch.Draw(shop, new Vector2(220, 220), Color.White);
        _spriteBatch.Draw(hut, new Vector2(320, 220), Color.White);
        _spriteBatch.Draw(closeButton, new Vector2(370, 200), Color.White);
        // ...draw prices, highlights, etc.
    }
}

    public void OpenBuildMenu()
    {
        buildMenuOpen = true;
        Console.WriteLine("Build menu enabled.");
    }

    public void CloseBuildMenu()
    {
        buildMenuOpen = false;
        Console.WriteLine("Build menu closed.");
    }

    public void HandleClick(Point mousePoint, int lr)
    {
        // Check if mousePoint is within any UI element and handle accordingly
        // e.g., open/close menu, select building, etc.
    }
}