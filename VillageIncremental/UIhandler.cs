using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



public class UIhandler{

    private GraphicsDeviceManager _graphics;

    private SpriteBatch _spriteBatch;

    private List<Texture2D> UISprites;

    private List<(int, int)> spriteCoords;



    private Texture2D shop;
    private Texture2D hut;
    private Texture2D buildmenubox;

    public UIhandler(GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch) {
        _graphics = graphicsDeviceManager;
        _spriteBatch = spriteBatch;
    }

    public void loadUIContent(){
        

    }

    public void openBuildMenu(){
        Console.WriteLine("Build menu enabled.");


    }


}
