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

        Texture2D background;

        List<Button> buttons = new List<Button>();
        List<int> buttonResults = new List<int>();
        List<string> buttonDestinations = new List<string>();
        List<Texture2D> textures = new List<Texture2D>();
        List<Vector2> textureLocations = new List<Vector2>();

        #region Properties
        public static ContentManager GameContent
        {
            get { return content; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
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

        private void ButtonPressed(object sender, EventArgs e)
        {
            string name = buttonDestinations[((Button)sender).ButtonNum];
            background = GameContent.Load<Texture2D>("Textures/help");
            buttons.Clear();
            buttonDestinations.Clear();
            textures.Clear();
            textureLocations.Clear();
            LoadMenu(name);
        }

        private void LoadMenu(string menuName)
        {
            try
            {
                XmlDocument read = new XmlDocument();
                read.Load("Content/Menus/" + menuName + ".xml");
                XmlNode node = read.SelectSingleNode("/Menu");

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
                                textureLocations.Add(new Vector2(float.Parse(x.InnerText), float.Parse(y.InnerText)));
                            }
                        }
                    }
                    else if (i.Name == "Button")
                    {
                        ButtonType b = ButtonType.Ellipse;
                        Texture2D normal = GameContent.Load<Texture2D>("Textures/help"), hovered = GameContent.Load<Texture2D>("Textures/help");
                        Vector2 pos = Vector2.Zero;
                        float w = 0, h = 0, diameter = 0;
                        int resultType = 0;
                        foreach (XmlNode l in i)
                        {
                            if (l.Name == "ButtonType")
                            {
                                b = (ButtonType)int.Parse(l.InnerText);
                            }
                            else if (l.Name == "NormalTexture") normal = GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "HoveredTexture") hovered = GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "Position")
                            {
                                XmlNode x = l.SelectSingleNode("X"), y = l.SelectSingleNode("Y");
                                pos = new Vector2(float.Parse(x.InnerText), float.Parse(y.InnerText));
                            }
                            else if (l.Name == "Result")
                            {
                                foreach (XmlNode j in l)
                                {
                                    if (j.Name == "ResultType")
                                    {
                                        resultType = int.Parse(j.InnerText);
                                        buttonResults.Add(resultType);
                                    }
                                    else if (j.Name == "ResultName")
                                    {
                                        if (resultType == 0) buttonDestinations.Add(j.InnerText);
                                        else if (resultType == 1) buttonDestinations.Add(j.InnerText);
                                    }
                                }
                            }
                            else if (b == ButtonType.Rectangle)
                            {
                                if (l.Name == "Size")
                                {
                                    XmlNode width = l.SelectSingleNode("Width"), height = l.SelectSingleNode("Height");
                                    w = float.Parse(width.InnerText);
                                    h = float.Parse(height.InnerText);
                                }
                            }
                            else if (b == ButtonType.Circle)
                            {
                                if (l.Name == "Diameter") diameter = float.Parse(l.InnerText);
                            }
                        }
                        if (b == ButtonType.Rectangle) buttons.Add(new Button(pos, (int)w, (int)h, buttons.Count, CurrentMouse, normal, hovered, true, 800, 480));
                        else if (b == ButtonType.Circle) buttons.Add(new Button(pos, diameter, buttons.Count, CurrentMouse, normal, hovered, true, 800, 480));
                        else if (b == ButtonType.Ellipse) buttons.Add(new Button(pos, buttons.Count, CurrentMouse, normal, hovered, true, 800, 480));
                    }
                }

                foreach (Button i in buttons)
                {
                    if (buttonResults[i.ButtonNum] == 0)
                    {
                        i.LeftClicked += (object sender, EventArgs e) =>
                        {
                            string name = buttonDestinations[((Button)sender).ButtonNum];
                            background = GameContent.Load<Texture2D>("Textures/help");
                            buttons.Clear();
                            buttonDestinations.Clear();
                            textures.Clear();
                            textureLocations.Clear();
                            buttonResults.Clear();
                            LoadMenu(name);
                        };
                    }
                    else if (buttonResults[i.ButtonNum] == 0)
                    {
                        i.LeftClicked += (object sender, EventArgs e) =>
                        {

                        };
                    }
                }
            }
            catch (AssetNotFoundException e)
            {
                ErrorHandler.RecordError(2, 007, "A specified assest was not found, this could be due to it not existing, being a wrong format, or being misnamed.", e.Message);
                Console.WriteLine("There was a problem loading an asset. Full details on this error can be found in the error log.");
            }
            catch (FileNotFoundException e)
            {
                ErrorHandler.RecordError(3, 001, "The menu xml file could not be found; make sure that files are named correctly and that they are correctly referenced in other menus. If this is happening at start up make sure that the Menu.xml is correctly named and present.", e.Message);
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();

            try
            {
                foreach (Button i in buttons) i.Update(CurrentMouse);
            }
            catch { }

            if (CurrentKeyboard.IsKeyPressed(Keys.Enter))
            {
                background = GameContent.Load<Texture2D>("Textures/help");
                buttons.Clear();
                buttonDestinations.Clear();
                textures.Clear();
                textureLocations.Clear();
                LoadMenu("Menu");
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, graphics.GraphicsDevice.BlendStates.NonPremultiplied);

            Window.AllowUserResizing = false;

            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            for (int i = 0; i < textures.Count; i++)
            {
                spriteBatch.Draw(textures[i], textureLocations[i], Color.White);
            }

            foreach (Button i in buttons)
            {
                spriteBatch.Draw(i.Texture, i.Position, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}