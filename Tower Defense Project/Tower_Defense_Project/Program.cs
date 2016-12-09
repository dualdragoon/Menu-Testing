using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using SharpDX;
using SharpDX.Serialization;
using Duality;
using Duality.Records;
using Duality.Encrypting;

namespace Tower_Defense_Project
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Uncomment for making new level serializations.
            /*Path path = new Path();

            path.points.Add(new Vector2(0, 105));
            path.points.Add(new Vector2(100, 105));
            path.points.Add(new Vector2(103.5f, 106.5f));
            path.points.Add(new Vector2(105, 110));
            path.points.Add(new Vector2(105, 200));
            path.points.Add(new Vector2(106.5f, 203.5f));
            path.points.Add(new Vector2(110, 205));
            path.points.Add(new Vector2(200, 205));
            path.points.Add(new Vector2(203.5f, 206.5f));
            path.points.Add(new Vector2(205, 210));
            path.points.Add(new Vector2(205, 400));

            for (int i = 0; i < path.points.Count; i++)
            {
                path.points[i] = new Vector2(path.points[i].X / 800, path.points[i].Y / 480);
            }

            path.pathSet.Add(new FloatingRectangle(0, 100, 110, 10));
            path.pathSet.Add(new FloatingRectangle(100, 100, 10, 110));
            path.pathSet.Add(new FloatingRectangle(100, 200, 110, 10));
            path.pathSet.Add(new FloatingRectangle(200, 200, 10, 210));

            for (int i = 0; i < path.pathSet.Count; i++)
            {
                path.pathSet[i] = new FloatingRectangle(path.pathSet[i].X/800f, path.pathSet[i].Y/480f, path.pathSet[i].Width/800f, path.pathSet[i].Height/480f);
            }

            StreamWriter temp = new StreamWriter("temp1.temp");
            StreamWriter write = new StreamWriter("Level1.path");

            temp.WriteLine(path.points.Count);
            temp.WriteLine(path.pathSet.Count);

            for (int i = 0; i < path.points.Count; i++)
            {
                temp.WriteLine(path.points[i].X);
                temp.WriteLine(path.points[i].Y);
            }

            for (int i = 0; i < path.pathSet.Count; i++)
            {
                temp.WriteLine(path.pathSet[i].X);
                temp.WriteLine(path.pathSet[i].Y);
                temp.WriteLine(path.pathSet[i].Width);
                temp.WriteLine(path.pathSet[i].Height);
            }

            temp.Close();

            StreamReader read = new StreamReader("temp1.temp");

            write.Write(StringCipher.Encrypt(read.ReadToEnd(), "temp2"));
            write.Close();
            read.Close();
            File.Delete("temp1.temp");*/

            /*StreamWriter write = new StreamWriter("Level1.enemies");
            write.Write(StringCipher.Encrypt(Serialization.SerializeToString<EnemyType>(EnemyType.Scout), "temp"));
            write.Close();*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }
#endif
}