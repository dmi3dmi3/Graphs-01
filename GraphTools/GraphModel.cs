using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GraphTools
{
    public class GraphModel
    {
        #region Tests

        [TestFixture]
        public class GraphTests
        {
            public bool CompareCycles(List<List<int>> leftCycles, List<List<int>> rightCycles)
            {
                var graph = new GraphModel(2, new List<(int, int)> {(0, 1)});
                foreach (var rightCycle in rightCycles)
                    if (!graph.CheckCycleContains(leftCycles, rightCycle))
                        return false;

                return true;
            }

            [Test]
            public void CycleTest1()
            {
                var graph = new GraphModel(3, new List<(int, int)>
                {
                    (0, 1), (1, 2), (2, 0)
                });
                var t = graph.CyclesCatalog;
                Assert.True(CompareCycles(t, new List<List<int>>
                {
                    new List<int> {0, 1, 2}
                }));
            }

            [Test]
            public void CycleTest2()
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
                var t = graph.CyclesCatalog.First(ints =>
                    graph.CyclesCatalog.TrueForAll(list => ints.Count >= list.Count));

                Assert.True(
                    graph.CheckCycleContains(new List<List<int>> {t}, new List<int> {0, 1, 2, 3, 4, 6, 5})
                );
            }
        }

        #endregion

        #region Public properties

        public int VertexCount { get; private set; }
        public List<(int, int)> Edges { get; }
        public Dictionary<int, List<int>> AdjacencyList { get; }
        public List<List<int>> CyclesCatalog { get; }

        #endregion

        #region Public metods

        #region Constructors

        /// <summary>
        ///     Конструктор графа
        /// </summary>
        /// <param name="vertexCount">Количество вершин. Номера от 0 до vertexCount-1</param>
        /// <param name="edges">Список ребер</param>
        public GraphModel(int vertexCount, List<(int, int)> edges)
        {
            VertexCount = vertexCount;
            Edges = edges;
            AdjacencyList = new Dictionary<int, List<int>>(vertexCount);
            foreach (var edge in edges)
            {
                if (AdjacencyList.ContainsKey(edge.Item1))
                    AdjacencyList[edge.Item1].Add(edge.Item2);
                else
                    AdjacencyList.Add(edge.Item1, new List<int> {edge.Item2});

                if (AdjacencyList.ContainsKey(edge.Item2))
                    AdjacencyList[edge.Item2].Add(edge.Item1);
                else
                    AdjacencyList.Add(edge.Item2, new List<int> {edge.Item1});
            }


            CyclesCatalog = new List<List<int>>();
            CyclesSearch();
        }

        /// <summary>
        ///     Конструктор графа
        /// </summary>
        /// <param name="vertexCount">Количество вершин. Номера от 0 до vertexCount-1</param>
        /// <param name="adjacencyList">Список смежности графа</param>
        public GraphModel(int vertexCount, Dictionary<int, List<int>> adjacencyList)
        {
            VertexCount = vertexCount;
            AdjacencyList = adjacencyList;
            Edges = new List<(int, int)>();

            foreach (var v1 in adjacencyList)
            foreach (var v2 in v1.Value)
                if (!Edges.Contains((v1.Key, v2)) && !Edges.Contains((v2, v1.Key)))
                    Edges.Add((v1.Key, v2));

            CyclesCatalog = new List<List<int>>();
            CyclesSearch();
        }

        #endregion

        /// <summary>
        ///     Метод добавления нового ребра графа. Игнорирует повторное добавление.
        /// </summary>
        /// <param name="edge">Новое ребро</param>
        public void AddEdge((int, int) edge)
        {
            if (Edges.Contains(edge)) return;

            Edges.Add(edge);
            if (AdjacencyList.ContainsKey(edge.Item1))
                AdjacencyList[edge.Item1].Add(edge.Item2);
            else
                AdjacencyList.Add(edge.Item1, new List<int> {edge.Item2});

            if (AdjacencyList.ContainsKey(edge.Item2))
                AdjacencyList[edge.Item2].Add(edge.Item1);
            else
                AdjacencyList.Add(edge.Item2, new List<int> {edge.Item1});
        }

        /// <summary>
        ///     Метод удаления ребра из графа. Игнорирует удаление несуществующего ребра
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge((int, int) edge)
        {
            if (!Edges.Contains(edge)) return;

            Edges.Remove(edge);
            AdjacencyList[edge.Item1].Remove(edge.Item2);
            AdjacencyList[edge.Item2].Remove(edge.Item1);
        }

        /// <summary>
        ///     Метод добавления новых вершин в граф. Игнорирует не положительные значения.
        /// </summary>
        /// <param name="count">Количество вершин которые нужно добавить</param>
        public void AddVertex(int count)
        {
            if (count <= 0) return;

            VertexCount += count;
        }

        #endregion

        #region Private methods

        #region Cycles

        /// <summary>
        ///     Основной метод поиска циклов в графе. Заполняет свойство CyclesCatalog.
        /// </summary>
        private void CyclesSearch()
        {
            var color = new int[VertexCount];
            for (var i = 0; i < VertexCount; i++)
            {
                for (var k = 0; k < VertexCount; k++)
                    color[k] = 1;
                var cycle = new List<int>();
                DfsCycle(i, i, color, -1, cycle);
            }
        }

        /// <summary>
        ///     Измененный обход в глубину для поиска циклов
        /// </summary>
        /// <param name="u">Текущая вершина.</param>
        /// <param name="endV">Конечная вершина.</param>
        /// <param name="color">Масств цветов вершин графа.</param>
        /// <param name="unavailableEdge">Последняя пройденная грань.</param>
        /// <param name="cycle">Список для хранения цикла. Изначально пуст.</param>
        private void DfsCycle(int u, int endV, int[] color, int unavailableEdge, List<int> cycle)
        {
            if (u != endV)
            {
                color[u] = 2; 
            }
            else if (cycle.Count >= 2)
            {
                cycle.Reverse();
                var res = cycle.ToList();
                var flag = CyclesCatalog.Any(t => t.SequenceEqual(res));
                if (!flag)
                {
                    cycle.Reverse();
                    if (!CheckCycleContains(CyclesCatalog, res))
                    {
                        res.Reverse();
                        if (!CheckCycleContains(CyclesCatalog, res)) CyclesCatalog.Add(res);
                    }
                }

                return;
            }

            for (var i = 0; i < Edges.Count; i++)
            {
                if (i == unavailableEdge) continue;

                if (color[Edges[i].Item2] == 1 && Edges[i].Item1 == u)
                {
                    var cycleNew = new List<int>(cycle);
                    cycleNew.Add(Edges[i].Item2);
                    DfsCycle(Edges[i].Item2, endV, color, i, cycleNew);
                    color[Edges[i].Item2] = 1;
                }
                else if (color[Edges[i].Item1] == 1 && Edges[i].Item2 == u)
                {
                    var cycleNew = new List<int>(cycle);
                    cycleNew.Add(Edges[i].Item1);
                    DfsCycle(Edges[i].Item1, endV, color, i, cycleNew);
                }
            }
        }

        /// <summary>
        ///     Проверка наличия цикла newCycle и подобных ему в списке циклов cycles
        /// </summary>
        /// <param name="cycles">Список циклов в котором идет поиск</param>
        /// <param name="newCycle">Искомый цикл</param>
        /// <returns>Возвращает true в случае если цикл найден</returns>
        private bool CheckCycleContains(List<List<int>> cycles, List<int> newCycle)
        {
            foreach (var cycle in cycles)
            {
                if (cycle.Count != newCycle.Count)
                    continue;

                var flag = true;
                var f = cycle.IndexOf(newCycle[0]);
                if (f == -1)
                    continue;

                for (int i = f, k = 0; i < f + cycle.Count; i++, k++)
                {
                    var j = i % cycle.Count;
                    if (cycle[j] == newCycle[k])
                        continue;
                    flag = false;
                    break;
                }

                if (flag) return true;
            }

            return false;
        }

        #endregion

        #endregion
    }
}