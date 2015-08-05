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

        public void ResetLevel()
        {
            var list = db.Synsets.ToList();
            foreach (var s in list)
            {
                //s.Level = 0;
            }
        }

        public void ClearAndLoadWords()
        {
            db.Synsets.RemoveRange(db.Synsets);
            db.SaveChanges();

            var filename = path + "words.txt";

            string[] lines = System.IO.File.ReadAllLines(filename);
            var synsets = lines.Select(l =>
            {
                var split = l.Split(new char[] { '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() != 2) return new Synset { Wnid = "" };
                return new Synset { Wnid = split[0], Words = split[1] };
            }).Where(s => s.Wnid != "").ToList();

            db.Synsets.AddRange(synsets);
            db.SaveChanges();
        }


        public void AddIsAvailable()
        {
            var filename = path + "imagenet.synset.txt";

            string[] lines = System.IO.File.ReadAllLines(filename);
            var isa = lines.Where(l => l != null && l.Trim() != "").OrderBy(l => l).ToList();

            var list = db.Synsets.OrderBy(s => s.Wnid).ToList();

            var c = 0;
            for (int i = 0; i < isa.Count; i++)
            {
                while (list[c].Wnid != isa[i]) ++c;

                list[c].IsAvailable = true;
            }

            db.SaveChanges();

            Console.WriteLine("done.");
            Console.ReadLine();

        }

        public void AddParents()
        {
            var filename = path + "wordnet.is_a.txt";

            string[] lines = System.IO.File.ReadAllLines(filename);
            var isa = lines.Select(l =>
            {
                var split = l.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() != 2) return new Synset { Wnid = "" };
                return new Synset { Wnid = split[1], ParentId = split[0] };
            }).Where(s => s.Wnid != "").OrderBy(s => s.Wnid).ToList();

            var list = db.Synsets.OrderBy(s => s.Wnid).ToList();

            var c = 0;
            for (int i = 0; i < isa.Count; i++)
            {
                while (list[c].Wnid != isa[i].Wnid) ++c;

                list[c].ParentId = isa[i].ParentId;
            }

            db.SaveChanges();

            Console.WriteLine("done.");
            Console.ReadLine();

        }

        public void CheckParentIsSingle()
        {
            var filename = path + "wordnet.is_a.txt";

            string[] lines = System.IO.File.ReadAllLines(filename);
            var isa = lines.Select(l =>
            {
                var split = l.Split(new char[] { '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() != 2) return new Synset { Wnid = "" };
                return new Synset { Wnid = split[1], ParentId = split[0] };
            }).Where(s => s.Wnid != "").OrderBy(s => s.Wnid).ToList();

            var list = isa.Select(i => i.Wnid).ToList().Distinct();

            if (list.Count() != isa.Count) Console.WriteLine("not equal.");
            else Console.WriteLine("equal.");

            //db.SaveChanges();

            Console.WriteLine("done.");
            Console.ReadLine();

        }

        public void BatchLoadGlosses()
        {
            var filename = path + "gloss.txt";

            string[] lines = System.IO.File.ReadAllLines(filename);
            var glosses = lines.Select(l =>
            {
                var split = l.Split(new char[] { '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() != 2) return new Synset { Wnid = "" };
                return new Synset { Wnid = split[0], Glosses = split[1] };
            }).Where(s => s.Wnid != "").OrderBy(s=>s.Wnid).ToList();

            var synsets = db.Synsets.OrderBy(s => s.Wnid).ToList();

            var L = glosses.Count;
            if (synsets.Count != L) {
                Console.WriteLine("lines count not match");
                return;
            }

            for (int i = 0; i < L; i++)
            {
                if (synsets[i].Wnid == glosses[i].Wnid)
                    synsets[i].Glosses = glosses[i].Glosses;
                else
                    Console.WriteLine("not match line " + i.ToString());
            }

            db.SaveChanges();
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
