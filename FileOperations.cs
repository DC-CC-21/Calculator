using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class FileOperations
    {
        private readonly List<string> data = new();

        /// <summary>
        /// This function overwrites the output file with a list of most recent previous equations
        /// </summary>
        /// <param name="value"></param>
        public void WriteFile(string value)
        {
            // Add the value to the data list and save it to the output file
            data.Add(value);
            Save();
        }

        /// <summary>
        /// This function writes the data line by line to the output file
        /// </summary>
        private void Save()
        {
            using StreamWriter sw = new("output.txt");
            foreach (string line in data)
            {
                sw.WriteLine(line);
            }
        }

        /// <summary>
        /// Read each line from the output file
        /// </summary>
        /// <returns></returns>
        public List<string> ReadFile()
        {
            if (!File.Exists("output.txt"))
            {
                using StreamWriter sw = new("output.txt");
                sw.WriteLine(string.Empty);
            }
            using StreamReader sr = new("output.txt");
            while (!sr.EndOfStream)
            {
                data.Add(sr.ReadLine() ?? "");
            }
            return data;
        }

        /// <summary>
        /// This function finds and deletes an equation from the data list and 
        /// then saves the file
        /// </summary>
        /// <param name="value"></param>
        public void DeleteEq(string? value = null)
        {
            if (value is null)
            {
                data.RemoveAt(0);
            }
            else
            {
                data.Remove(value);
            }
            Save();
        }
    }
}
