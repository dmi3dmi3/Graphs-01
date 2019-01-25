using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphTools;

namespace GraphConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                ConsoleX.WriteLine("Введите граф (0 - список смежности, 1 - список дуг, 2 - выход)", ConsoleColor.White);
                var i = ConsoleX.GetValue<int>("Тип ввода", 1);
                GraphModel graph = null;
                if (i == 0)
                {
                    var nv = ConsoleX.GetValue<int>("Кол-во вершин");
                    var dict = new Dictionary<int, List<int>>();
                    for (int j = 0; j < nv; j++)
                    {
                        dict.Add(j, new List<int>());
                        var nv2 = ConsoleX.GetValue<int>($"Кол-во смежных вершин c {j}");
                        for (int k = 0; k < nv2; k++)
                        {
                            dict[j].Add(ConsoleX.GetValue<int>($"{k} вершина"));
                        }
                    }
                    graph = new GraphModel(nv, dict);
                }
                else if (i == 1)
                {
                    var nv = ConsoleX.GetValue<int>("Кол-во вершин");
                    var ne = ConsoleX.GetValue<int>("Кол-во дуг");
                    var edges = new List<(int,int)>();
                    for (int j = 0; j < ne; j++)
                    {
                        var f = ConsoleX.GetValue<int>($"Первый конец {j} дуги");
                        var l = ConsoleX.GetValue<int>($"Второй конец {j} дуги");
                        edges.Add((f,l));
                    }
                    graph = new GraphModel(nv, edges);
                }
                else if (i == 2)
                {
                    break;
                }

                var facets = FacetTools.GetFacets(graph);
                for (int j = 0; j < facets.Count; j++)
                {
                    ConsoleX.Write($"{j} поверхность: ", ConsoleColor.White);
                    foreach (var item in facets[j])
                    {
                        ConsoleX.Write($"{item} ", ConsoleColor.White);
                    }
                    ConsoleX.WriteLine("",ConsoleColor.Black);
                }
            } while (true);

        }
    }
}
