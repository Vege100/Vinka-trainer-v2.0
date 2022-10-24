using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Solution
{
    class Program
    {
        static void Main(string[] args)
        {
            int cou = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < cou; i++)
            {
                string result = $"Case #{i + 1}: ";
                int a = Convert.ToInt32(Console.ReadLine());
                string[] tau = new string[a];
                string[] karsittu = new string[a];
                
                for (int j = 0; j < a; j++)
                {
                    tau[j] = Console.ReadLine();

                }
                for (int j = 0; j < tau.Length; j++)
                {
                    string tauk = tau[j];
                    karsittu[j] = " ";
                    for (int k = 0; k < tau[j].Length; k++)
                    {
                        if (!karsittu[j].Contains(tauk[k]))
                        {
                            karsittu[j] += tau[j][k];
                        }

                    }

                }
                int voittaja = 0;
                for (int j = 0; j < a; j++)
                {

                    if (karsittu[j].Length < karsittu[voittaja].Length) continue;
                    if (karsittu[j].Length == karsittu[voittaja].Length)
                        {
                        for (int k = 1; k < karsittu[j].Length; k++)
                        {
                            if (karsittu[j][k] > karsittu[voittaja][k])
                            {

                            }
                    
                        
                    voittaja = j;
                }
                Console.WriteLine($"{result}{tau[voittaja]}");
            }
        }
    }
}
