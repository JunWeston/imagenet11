using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imagenet
{
    public class Manager
    {
        ImgnetContext db = new ImgnetContext();
        string path = @"D:\Backup\Source\Repos\Imagenet\Imagenet\Imagenet\data\";


        public void LoadWords()
        {
            var filename = path + "words.txt";
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                var split = line.Split(new char[] { '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() != 2) continue;
                db.Synsets.AddOrUpdate(s => s.Wnid, new Synset { Wnid = split[0], Words = split[1] });
                if (counter % 100 == 0) db.SaveChanges();
                Console.WriteLine(counter++);
            }
            db.SaveChanges();

            file.Close();

            // Suspend the screen.
            Console.ReadLine();

        }

        public void BatchLoadWords()
        {
            var filename = path + "words.txt";
            int counter = 0;

            string[] lines = System.IO.File.ReadAllLines(filename);
            var synsets = lines.Select(l =>
              {
                  var split = l.Split(new char[] { '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                  if (split.Count() != 2) return new Synset { Wnid = "" };
                  return new Synset { Wnid = split[0], Words = split[1] };
              }).Where(s => s.Wnid != "").ToList();

            foreach (var s in synsets)
            {
                db.Synsets.AddOrUpdate(s);
                Console.WriteLine(counter++);
            }
            
            Console.ReadLine();

        }
    }
}
