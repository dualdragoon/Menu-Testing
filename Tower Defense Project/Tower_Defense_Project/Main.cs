using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Duality;
using Duality.Interaction;

namespace Tower_Defense_Project
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Game
    {
        SpriteBatch spriteBatch;
        static ContentManager content;
        static GraphicsDeviceManager graphics;
        static KeyboardManager keyboardManager;
        static KeyboardState keyboard;
        static MouseManager mouseManager;
        static MouseState mouse;

        string lastMenu = "Menu";
        Texture2D background;
        Vector2 intendedResolution;

        List<Button> buttons = new List<Button>();
        List<ButtonType> buttonTypes = new List<ButtonType>();
        List<int> buttonLeftResults = new List<int>();
        List<int> buttonRightResults = new List<int>();
        List<string> buttonLeftDestinations = new List<string>();
        List<string> buttonRightDestinations = new List<string>();
        List<Texture2D> textures = new List<Texture2D>();
        List<Vector2> textureLocations = new List<Vector2>();

        public string LastMenu
        {
            get { return lastMenu; }
        }

        #region Properties
        public static ContentManager GameContent
        {
            get { return content; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public static Vector2 Scale
        {
            get { return new Vector2(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight); }
        }

        public static KeyboardManager Keyboard
        {
            get { return keyboardManager; }
        }

        public static KeyboardState CurrentKeyboard
        {
            get { return keyboard; }
        }

        public static MouseManager Mouse
        {
            get { return mouseManager; }
        }

        public static MouseState CurrentMouse
        {
            get { return mouse; }
            set { mouse = value; }
        }
        #endregion

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mouseManager = new MouseManager(this);
            keyboardManager = new KeyboardManager(this);
            content = Content;
            IsMouseVisible = true;
            //graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 800;
            /*graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.IsFullScreen = true;
            IsMouseVisible = true;*/
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ErrorHandler.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadMenu("Menu");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void LoadMenu(string menuName)
        {
            try
            {
                XmlDocument read = new XmlDocument();
                read.Load("Content/Menus/" + menuName + ".xml");
                XmlNode node = read.SelectSingleNode("/Menu");
                XmlNode res = node.SelectSingleNode("IntendedResolution");
                XmlNode resX = res.SelectSingleNode("X"), resY = res.SelectSingleNode("Y");
                intendedResolution = new Vector2(float.Parse(resX.InnerText), float.Parse(resY.InnerText));

                foreach (XmlNode i in node)
                {
                    if (i.Name == "BackgroundTexture") background = GameContent.Load<Texture2D>("Menus/" + menuName + "/" + i.InnerText + ".png");
                    else if (i.Name == "Texture")
                    {
                        foreach (XmlNode l in i)
                        {
                            if (l.Name == "TextureName") textures.Add(GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png"));
                            else if (l.Name == "Position")
                            {
                                XmlNode x = l.SelectSingleNode("X"), y = l.SelectSingleNode("Y");
                                textureLocations.Add(new Vector2((float.Parse(x.InnerText) / intendedResolution.X) * Scale.X, (float.Parse(y.InnerText) / intendedResolution.Y) * Scale.Y));
                            }
                        }
                    }
                    else if (i.Name == "Button")
                    {
                        ButtonType b = ButtonType.Ellipse;
                        Texture2D normal = GameContent.Load<Texture2D>("Textures/help"), hovered = GameContent.Load<Texture2D>("Textures/help");
                        Vector2 pos = Vector2.Zero;
                        float w = 0, h = 0, diameter = 0;
                        int leftResultType = 0, rightResultType = 0;
                        foreach (XmlNode l in i)
                        {
                            if (l.Name == "ButtonType")
                            {
                                b = (ButtonType)int.Parse(l.InnerText);
                                buttonTypes.Add(b);
                            }
                            else if (l.Name == "NormalTexture") normal = GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "HoveredTexture") hovered = GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "Position")
                            {
                                XmlNode x = l.SelectSingleNode("X"), y = l.SelectSingleNode("Y");
                                pos = new Vector2(float.Parse(x.InnerText), float.Parse(y.InnerText));
                            }
                            else if (l.Name == "LeftClickResult")
                            {
                                XmlNode type = l.SelectSingleNode("ResultType"), name = l.SelectSingleNode("ResultName");
                                leftResultType = int.Parse(type.InnerText);
                                buttonLeftResults.Add(leftResultType);
                                buttonLeftDestinations.Add(name.InnerText);
                            }
                            else if (l.Name == "RightClickResult")
                            {
                                XmlNode type = l.SelectSingleNode("ResultType"), name = l.SelectSingleNode("ResultName");
                                rightResultType = int.Parse(type.InnerText);
                                buttonRightResults.Add(rightResultType);
                                buttonRightDestinations.Add(name.InnerText);
                            }
                            else if (b == ButtonType.Rectangle)
                            {
                                if (l.Name == "Size")
                                {
                                    XmlNode width = l.SelectSingleNode("Width"), height = l.SelectSingleNode("Height");
                                    w = (float.Parse(width.InnerText) / intendedResolution.X) * Scale.X;
                                    h = (float.Parse(height.InnerText) / intendedResolution.Y) * Scale.Y;
                                }
                            }
                            else if (b == ButtonType.Circle)
                            {
                                if (l.Name == "Diameter") diameter = float.Parse(l.InnerText);
                            }
                        }
                        pos = new Vector2((pos.X / intendedResolution.X) * Scale.X, (pos.Y / intendedResolution.Y) * Scale.Y);
                        if (b == ButtonType.Rectangle) buttons.Add(new Button(pos, (int)w, (int)h, buttons.Count, CurrentMouse, normal, hovered, true, Scale.X, Scale.Y));
                        else if (b == ButtonType.Circle) buttons.Add(new Button(pos, diameter, buttons.Count, CurrentMouse, normal, hovered, true, Scale.X, Scale.Y));
                        else if (b == ButtonType.Ellipse) buttons.Add(new Button(pos, buttons.Count, CurrentMouse, normal, hovered, true, Scale.X, Scale.Y));
                    }
                }

                foreach (Button i in buttons)
                {
                    #region LeftClickResults
                    if (buttonLeftResults[i.ButtonNum] == 0)
                    {
                        i.LeftClicked += (object sender, EventArgs e) =>
                        {
                            string name = buttonLeftDestinations[((Button)sender).ButtonNum];
                            background = GameContent.Load<Texture2D>("Textures/help");
                            Clear();
                            lastMenu = menuName;
                            LoadMenu(name);
                        };
                    }
                    else if (buttonLeftResults[i.ButtonNum] == 1)
                    {
                        i.LeftClicked += (object sender, EventArgs e) =>
                        {
                            string name = buttonLeftDestinations[((Button)sender).ButtonNum];
                            if (name != "null")
                            {
                                Clear();
                                lastMenu = menuName;
                                Console.WriteLine(string.Format("Would start level with name: {0}.", name));
                            }
                        };
                    }
                    else if (buttonLeftResults[i.ButtonNum] == 2)
                    {
                        i.LeftClicked += (object sender, EventArgs e) =>
                        {
                            Clear();
                            lastMenu = menuName;
                            Console.WriteLine("Would open the level designer.");
                        };
                    }
                    #endregion

                    #region RightClickResults
                    if (buttonRightResults[i.ButtonNum] == 0)
                    {
                        i.RightClicked += (object sender, EventArgs e) =>
                        {
                            string name = buttonRightDestinations[((Button)sender).ButtonNum];
                            background = GameContent.Load<Texture2D>("Textures/help");
                            Clear();
                            lastMenu = menuName;
                            LoadMenu(name);
                        };
                    }
                    else if (buttonRightResults[i.ButtonNum] == 1)
                    {
                        i.RightClicked += (object sender, EventArgs e) =>
                        {
                            string name = buttonRightDestinations[((Button)sender).ButtonNum];
                            if (name != "null")
                            {
                                Clear();
                                lastMenu = menuName;
                                Console.WriteLine(string.Format("Would start level with name: {0}.", name));
                            }
                        };
                    }
                    else if (buttonRightResults[i.ButtonNum] == 2)
                    {
                        i.RightClicked += (object sender, EventArgs e) =>
                        {
                            Clear();
                            lastMenu = menuName;
                            Console.WriteLine("Would open the level designer.");
                        };
                    }
                    #endregion
                }
            }
            catch (AssetNotFoundException e)
            {
                ErrorHandler.RecordError(2, 007, "A specified assest was not found, this could be due to it not existing, being a wrong format, or being misnamed.", e.Message);
                Console.WriteLine("There was a problem loading an asset. Full details on this error can be found in the error log.");
            }
            catch (FileNotFoundException e)
            {
                ErrorHandler.RecordError(3, 001, "The destination file could not be found; make sure that files are named correctly and that they are correctly referenced in other menus. If this is happening at start up make sure that the Menu.xml is correctly named and present.", e.Message);
            }
        }

        private void Clear()
        {
            buttons.Clear();
            buttonTypes.Clear();
            buttonLeftDestinations.Clear();
            buttonLeftResults.Clear();
            buttonRightDestinations.Clear();
            buttonRightResults.Clear();
            textures.Clear();
            textureLocations.Clear();
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentMouse = Mouse.GetState();

            try
            {
                foreach (Button i in buttons) i.Update(CurrentMouse);
            }
            catch { }

            if (CurrentKeyboard.IsKeyPressed(Keys.Enter))
            {
                background = GameContent.Load<Texture2D>("Textures/help");
                Clear();
                LoadMenu("Menu");
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            for (int i = 0; i < textures.Count; i++)
            {
                Vector2 textureSize = new Vector2((textures[i].Width / intendedResolution.X) * Scale.X, (textures[i].Height / intendedResolution.Y) * Scale.Y),
                    textureLocation = textureLocations[i];
                spriteBatch.Draw(textures[i], new RectangleF(textureLocation.X, textureLocation.Y, textureSize.X, textureSize.Y), Color.White);
            }

            foreach (Button i in buttons)
            {
                if (buttonTypes[i.ButtonNum] == ButtonType.Rectangle)
                {
                    spriteBatch.Draw(i.Texture, i.Collision, Color.White);
                }
                else
                {
                    spriteBatch.Draw(i.Texture, i.Position, Color.White);
                }
            }
        }
    }
}