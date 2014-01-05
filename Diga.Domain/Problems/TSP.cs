using Diga.Domain.Contracts;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Diga.Domain.Problems
{
    public class TSP : IProblem
    {
        private const string TSPLIB_URL = "http://www.iwr.uni-heidelberg.de/groups/comopt/software/TSPLIB95/tsp/";

        public bool Maximization { get { return false; } }

        public double[][] Coordinates { get; set; }

        public TSP() { }

        public TSP(string tspInstance)
        {
            string content;
            using (var wc = new WebClient())
            {
                byte[] data = wc.DownloadData(Path.Combine(TSPLIB_URL, tspInstance) + ".tsp.gz");
                using (var ms = new MemoryStream(data))
                using (var gzs = new GZipStream(ms, CompressionMode.Decompress))
                using (var result = new MemoryStream())
                {
                    gzs.CopyTo(result);
                    content = Encoding.ASCII.GetString(result.ToArray());
                }
            }
            string[] lines = content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] coordLines = lines.Where(x => Regex.IsMatch(x, @"^\d+")).ToArray();
            Coordinates = new double[coordLines.Length][];
            for (int i = 0; i < coordLines.Length; i++)
            {
                string[] coords = coordLines[i].Split();
                Coordinates[i] = new double[2];
                Coordinates[i][0] = double.Parse(coords[1]);
                Coordinates[i][1] = double.Parse(coords[2]);
            }
        }
    }
}
