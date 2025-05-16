using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;



namespace Buildings{
  class Building
  {
    Texture2D texture; 
    public (int, int) coords;
    public int width;
    public int height;
    public int rate;


    public Building((int, int) coords, int width, int height, Texture2D texture)
    {
      this.coords = coords;
      this.width = width;
      this.height = height;
      this.texture = texture;
    }

    public virtual (int, int) lclick(Point mouseCoords){return (0,0);}

    public virtual void rclick(Point mouseCoords){}

    public virtual Texture2D getTexture(){return this.texture;}

    public (int, int) getCoords(){return this.coords;}


  }


  class Hut : Building
  {
    Texture2D texture;
    Texture2D woodHutTexture;
    Texture2D ironHutTexture;
    Texture2D hutChooseTexture;

    public (int, int) coords;
    public int width;
    public int height;
    public int rate;
    public int hutState; // 0 is init, 1 is choosing, 2 is wood, 3 is iron
    public int oldState;


    public Hut((int, int) coords, int width, int height, Texture2D texture, Texture2D hutChooseTexture, Texture2D woodHutTexture, Texture2D ironHutTexture) : base(coords, width, height, texture)
    {
      this.coords = coords;
      this.width = width;
      this.height = height;
      this.texture = texture;
      this.hutChooseTexture = hutChooseTexture;
      this.hutState = 0;
      this.woodHutTexture = woodHutTexture;
      this.ironHutTexture = ironHutTexture;
      this.rate = 1;
    }

    public override Texture2D getTexture()
    {
      if (this.hutState == 1)
      {
        return this.hutChooseTexture;
      }
      else if (this.hutState == 2)
      {
        return this.woodHutTexture;
      }
      else if (this.hutState == 3)
      {
        return this.ironHutTexture;
      }
      else { return this.texture;}
    }

    public override (int, int) lclick(Point mouseCoords)
    {
      if (this.hutState == 1)
      {
        return (this.oldState, this.handleHutChoose(mouseCoords));
      }
      else { return (0, 0); }
    }

    public override void rclick(Point mouseCoords)
    {
      if (this.hutState != 1)
      {
        this.oldState = this.hutState;
        this.hutState = 1;
      }
    }


    protected int handleHutChoose(Point mouseCoords)
    {
      Rectangle leftRect = new Rectangle(this.coords.Item1, this.coords.Item2, this.width / 2, this.height);
      Rectangle rightRect = new Rectangle(this.coords.Item1 + this.width / 2, this.coords.Item2, this.width / 2, this.height);

      if (leftRect.Contains(mouseCoords))
      { //chose wood
        this.hutState = 2;
        return 2;
      }
      else if (rightRect.Contains(mouseCoords))
      { //chose iron
        this.hutState = 3;
        return 3;
      }
      else
      {
        this.hutState = 0;
        return 0;
      }

    }



  }


  class Shop : Building
  {
    Texture2D shopChooseTexture;
    Texture2D texture;
    private new (int, int) coords;

    private int oldState;

    public new int width;
    public new int height;
    public int shopState; // 0 is init, 1 is choosing, 2 is wood, 3 is iron
    Texture2D woodShopTexture;
    Texture2D ironShopTexture;
    public int rate;



    public Shop((int, int) coords, int width, int height, Texture2D texture, Texture2D shopChooseTexture, Texture2D woodShopTexture, Texture2D ironShopTexture) : base(coords, width, height, texture)
    {
      this.coords = coords;
      this.width = width;
      this.height = height;
      this.texture = texture;
      this.shopChooseTexture = shopChooseTexture;
      this.woodShopTexture = woodShopTexture;
      this.ironShopTexture = ironShopTexture;
      this.rate = 1;
    }

    public new (int, int) getCoords()
    {
      return this.coords;
    }

    public override Texture2D getTexture()
    {
      if (this.shopState == 1)
      {
        return this.shopChooseTexture;
      }
      else if (this.shopState == 2)
      {
        return this.woodShopTexture;
      }
      else if (this.shopState == 3)
      {
        return this.ironShopTexture;
      }
      else { return this.texture; }
    }

    public override (int, int) lclick(Point mouseCoords)
    {
      if (this.shopState == 1)
      {
        return (this.oldState, this.handleShopChoose(mouseCoords));
      }
      else { return (0, 0); }
    }

    public override void rclick(Point mouseCoords)
    {
      if (this.shopState != 1)
      {
        this.oldState = this.shopState;
        this.shopState = 1;
      }
    }


    protected int handleShopChoose(Point mouseCoords)
    {
      Rectangle leftRect = new Rectangle(this.coords.Item1, this.coords.Item2, this.width / 2, this.height);
      Rectangle rightRect = new Rectangle(this.coords.Item1 + this.width / 2, this.coords.Item2, this.width / 2, this.height);

      if (leftRect.Contains(mouseCoords))
      { //chose wood
        this.shopState = 2;
        return 2;
      }
      else if (rightRect.Contains(mouseCoords))
      { //chose iron
        this.shopState = 3;
        return 3;
      }
      else
      {
        this.shopState = 0;
        return 0;
      }

    }

  }
}