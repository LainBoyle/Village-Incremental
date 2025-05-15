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


    public Building((int, int) coords, int width, int height, Texture2D texture){
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


  class Shop : Building 
  {
    Texture2D shopChooseTexture;
    Texture2D texture;
    private new (int, int) coords;

    private int oldState;

    public new int width;
    public new int height;
    public int shopState; // 0 is init, 1 is choosing, 2 is wood, 3 is iron


    public Shop((int, int) coords, int width, int height, Texture2D texture, Texture2D shopChooseTexture) : base(coords, width, height, texture)
    {
      this.coords = coords;
      this.width = width;
      this.height = height;
      this.texture = texture;
      this.shopChooseTexture = shopChooseTexture;
    }

    public new (int, int) getCoords(){
      return this.coords;
    }

    public override Texture2D getTexture(){
      if (this.shopState == 1){
        return this.shopChooseTexture;
      }
      else{return this.texture;}
    }

    public override (int, int) lclick(Point mouseCoords){
      if (this.shopState == 1){
        return (this.oldState, this.handleShopChoose(mouseCoords));
      }
      else{return (0,0);}
    }

    public override void rclick(Point mouseCoords){
      if (this.shopState != 1){
        this.oldState = this.shopState;
        this.shopState = 1;
      }
    }


    protected int handleShopChoose(Point mouseCoords){
      Rectangle leftRect = new Rectangle(this.coords.Item1, this.coords.Item2, this.width/2, this.height);
      Rectangle rightRect = new Rectangle(this.coords.Item1 + this.width/2, this.coords.Item2, this.width/2, this.height);

      if (leftRect.Contains(mouseCoords)){ //chose wood
        this.shopState = 2;
        return 2;
      }
      else if (rightRect.Contains(mouseCoords)){ //chose iron
        this.shopState = 3;
        return 3;
      }
      else{
        this.shopState = 0;
        return 0;
      }

    }

  }
}