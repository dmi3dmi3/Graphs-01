using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphTools
{
    public static class FacetTools
    {
        #region Public methods

        /// <summary>
        ///     Graph facet search method
        /// </summary>
        /// <param name="graph">Input graph</param>
        /// <returns></returns>
        public static List<List<int>> GetFacets(GraphModel graph)
        {
            //Установка изначальных значений необхомых для подсчетов

            #region Init

            var startCycle =
                graph.CyclesCatalog.First(ints => graph.CyclesCatalog.TrueForAll(list => ints.Count >= list.Count));
            var anotherEdges = new List<(int, int)>();
            var anotherVertexes = new List<int>();
            var startCycleEdges = new List<(int, int)>();

            for (var i = 0; i < startCycle.Count; i++)
            {
                int t;
                if (i == startCycle.Count - 1)
                    t = 0;
                else
                    t = i + 1;
                startCycleEdges.Add((startCycle[i], startCycle[t]));
            }

            foreach (var graphEdge in graph.Edges)
                if (!startCycleEdges.Contains(graphEdge) &&
                    !startCycleEdges.Contains((graphEdge.Item2, graphEdge.Item1)))
                    anotherEdges.Add(graphEdge);

            for (var i = 0; i < graph.VertexCount; i++)
                if (!startCycle.Contains(i))
                    anotherVertexes.Add(i);


            var facets = new List<List<int>>
            {
                startCycle,
                startCycle
            };

            var currentGraph = new GraphModel(startCycle.Count + anotherVertexes.Count, startCycleEdges);

            #endregion

            // Добавление гамма-цепей

            while (true)
            {
                if (anotherEdges.Count == 0) return facets;

                var currentChain = FindGammaChain(currentGraph, anotherEdges);
                for (var k = 0; k < currentChain.Count - 1; k++) currentGraph.AddEdge((currentChain[k], currentChain[k + 1]));
                var currentFacet = facets.First(ints =>
                    ints.Contains(currentChain[0]) && ints.Contains(currentChain[currentChain.Count - 1]));
                facets.Remove(currentFacet);
                var indexFirst = currentFacet.IndexOf(currentChain[0]);
                var indexLast = currentFacet.IndexOf(currentChain[currentChain.Count - 1]);
                var newFacet1 = new List<int>();
                var newFacet2 = new List<int>();

                var i = indexFirst;
                while (i != indexLast)
                {
                    newFacet1.Add(currentFacet[i]);
                    i = (i + 1) % currentFacet.Count;
                }

                var j = currentChain.Count - 1;
                while (j >= 0) newFacet1.Add(currentChain[j--]);


                i = indexLast;
                while (i != indexFirst)
                {
                    newFacet2.Add(currentFacet[i]);
                    i = (i + 1) % currentFacet.Count;
                }

                j = 0;
                while (j < currentChain.Count) newFacet2.Add(currentChain[j++]);
                newFacet1.RemoveAt(0);
                newFacet2.RemoveAt(0);
                facets.Add(newFacet1);
                facets.Add(newFacet2);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Метод поиска гамма цепей в графе построенных из конкретного набора ребер, отстутствующих в графе.
        /// </summary>
        /// <param name="graph">Граф для поиска</param>
        /// <param name="anotherEdges">Список ребер из которых состовляются гамма-цепи. Итоговые ребра будут удалены.</param>
        /// <returns>Возвращает список вершин гамма цепи. Первая и последняя принадлежат графу.</returns>
        private static List<int> FindGammaChain(GraphModel graph, List<(int, int)> anotherEdges)
        {
            foreach (var anotherEdge in anotherEdges)
            {
                if (!graph.AdjacencyList.ContainsKey(anotherEdge.Item1))
                    continue;

                var vertexFirst = anotherEdge.Item1;
                if (graph.AdjacencyList.ContainsKey(anotherEdge.Item2))
                {
                    anotherEdges.Remove(anotherEdge);
                    return new List<int> { anotherEdge.Item1, anotherEdge.Item2 };
                }

                var colors = new int[graph.VertexCount];
                for (var i = 0; i < colors.Length; i++) colors[i] = 1;
                colors[vertexFirst] = 3;

                var res = new List<int>() { vertexFirst };
                ChainDfs(vertexFirst, colors, anotherEdges, graph, res);

                if (!graph.AdjacencyList.ContainsKey(res[res.Count - 1]))
                    continue;
                for (var i = 0; i < res.Count - 1; i++)
                {
                    anotherEdges.Remove((res[i], res[i + 1]));
                    anotherEdges.Remove((res[i+1], res[i]));
                }

                return res;
            }

            //todo
            throw new Exception("Fatal error");
        }

        /// <summary>
        ///     Метод обхода в глубину переделанный для поиска гамма-цепей
        /// </summary>
        /// <param name="u">Исходная вершина</param>
        /// <param name="colors">Массив окраски вершин</param>
        /// <param name="edges">Список ребер, используемых для построения цепей</param>
        /// <param name="grap">Иноформация о графе для проверки условия окончания поиска</param>
        /// <param name="chain">Результирующий список</param>
        private static void ChainDfs(int u, int[] colors, List<(int, int)> edges, GraphModel grap, List<int> chain)
        {
            if (grap.AdjacencyList.ContainsKey(u) && colors[u] != 3)
                return;
            colors[u] = 2;

            for (var i = 0; i < edges.Count; i++)
                if (colors[edges[i].Item2] == 1 && edges[i].Item1 == u)
                {
                    chain.Add(edges[i].Item2);
                    ChainDfs(edges[i].Item2, colors, edges, grap, chain);
                    break;
                    colors[edges[i].Item2] = 1;
                }
                else if (colors[edges[i].Item1] == 1 && edges[i].Item2 == u)
                {
                    chain.Add(edges[i].Item1);
                    ChainDfs(edges[i].Item1, colors, edges, grap, chain);
                    break;
                    colors[edges[i].Item1] = 1;
                }
        }

        #endregion

        #region Tests

        [TestFixture]
        public class GraphTests
        {
            [Test]
            public void FacetTest1()
            {
                var graph = new GraphModel(7, new List<(int, int)>
                {
                    (0, 1),
                    (1, 2),
                    (2, 3),
                    (3, 4),
                    (4, 5),
                    (5, 0),
                    (0, 3),
                    (2, 4),
                    (5, 6),
                    (4, 6),
                    (1, 4)
                });
                var t = GetFacets(graph);
            }

            [Test]
            public void FacetTest2()
            {
                var graph = new GraphModel(13, new List<(int, int)>
                {
                    (0, 1),
                    (1, 2),
                    (2, 3),
                    (3, 4),
                    (4, 5),
                    (5, 6),
                    (6, 7),
                    (7, 8),
                    (8, 9),
                    (9, 0),
                    (0, 10),
                    (10, 11),
                    (10, 12),
                    (5, 11),
                    (11, 12),
                });
                var t = GetFacets(graph);
            }

            [Test]
            public void FacetTest3()
            {
                var graph = new GraphModel(9, new List<(int, int)>
                {
                    (0, 1),
                    (0, 4),
                    (0, 2),
                    (1, 4),
                    (1, 5),
                    (2, 3),
                    (2, 6),
                    (3, 4),
                    (3, 7),
                    (4, 5),
                    (4, 8),
                    (5, 8),
                    (6, 7),
                    (7, 8)
                });
                var t = GetFacets(graph);
            }
        }

        #endregion
    }
}