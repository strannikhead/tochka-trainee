using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    private static List<List<char>> GetInput()
    {
        var data = new List<List<char>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }

        return data;
    }

    private static int Solve(List<List<char>> grid)
    {
        var nodes = GetStartPositions(grid);
        var keyIndex = CreateKeyIndexAndUpdateNodes(grid, nodes);

        var edges = GetEdges(grid, nodes, keyIndex);

        return DoDijkstra(grid, keyIndex, edges, nodes);
    }

    private static int DoDijkstra(List<List<char>> grid, Dictionary<(int, int), int> keyIndex,
        List<Edge>[] edges, List<(int y, int x)> nodes)
    {
        var distances = new Dictionary<string, int>();
        var priorityQueue = new SortedDictionary<int, Queue<string>>();

        var initPos = new[] { 0, 1, 2, 3 };
        var initState = Encode(initPos, "");
        distances[initState] = 0;
        priorityQueue[0] = new Queue<string>(new[] { initState });

        while (priorityQueue.Count > 0)
        {
            var first = priorityQueue.Keys.First();
            var queueAtD = priorityQueue[first];
            var state = queueAtD.Dequeue();
            if (queueAtD.Count == 0)
                priorityQueue.Remove(first);
            if (distances[state] != first)
                continue;

            var (posParts, haveKeys) = ParseState(state);
            if (haveKeys.Length == keyIndex.Count)
                return first;

            for (var r = 0; r < 4; r++)
            {
                var at = posParts[r];
                foreach (var edge in edges[at])
                {
                    var to = edge.To;
                    if (to - 4 < 0)
                        continue;
                    var keyChar = grid[nodes[to].y][nodes[to].x];
                    if (haveKeys.Contains(keyChar))
                        continue;

                    var isAllDoorsOpen = edge.NeededKeys.All(need => haveKeys.Contains(need));
                    if (!isAllDoorsOpen)
                        continue;

                    var newKeys = string.Concat((haveKeys + keyChar).OrderBy(c => c));

                    var newPos = (int[])posParts.Clone();
                    newPos[r] = to;
                    var newState = Encode(newPos, newKeys);
                    var nd = first + edge.Distance;
                    if (distances.TryGetValue(newState, out var oldD) && nd >= oldD)
                        continue;
                    distances[newState] = nd;
                    if (!priorityQueue.TryGetValue(nd, out var newQueue))
                    {
                        newQueue = new Queue<string>();
                        priorityQueue[nd] = newQueue;
                    }

                    newQueue.Enqueue(newState);
                }
            }
        }

        return -1;
    }

    private static (int[], string) ParseState(string state)
    {
        var parts = state.Split('#');
        var posParts = parts[0].Split(',').Select(int.Parse).ToArray();
        var haveKeys = parts[1];
        return (posParts, haveKeys);
    }

    private static List<Edge>[] GetEdges(List<List<char>> grid, List<(int y, int x)> nodes,
        Dictionary<(int, int), int> keyIndex)
    {
        var edges = new List<Edge>[nodes.Count];
        for (var i = 0; i < nodes.Count; i++)
            edges[i] = new List<Edge>();

        for (var i = 0; i < nodes.Count; i++)
        {
            var minDoorsSet = new Dictionary<(int, int), string>();
            var queue = new Queue<(int y, int x, int dist, string doors)>();
            queue.Enqueue((nodes[i].y, nodes[i].x, 0, ""));
            minDoorsSet[(nodes[i].y, nodes[i].x)] = "";

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.dist > 0 && grid[current.y][current.x] >= 'a' && grid[current.y][current.x] <= 'z')
                    edges[i].Add(new Edge(keyIndex[(current.y, current.x)], current.dist, current.doors));

                DoDirectionMove(grid, current, minDoorsSet, queue);
            }
        }

        return edges;
    }

    private static void DoDirectionMove(List<List<char>> grid, (int y, int x, int dist, string doors) current,
        Dictionary<(int, int), string> minDoorsSet, Queue<(int y, int x, int dist, string doors)> queue)
    {
        var directions = new[] { new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };
        foreach (var direction in directions)
        {
            var nx = current.x + direction[1];
            var ny = current.y + direction[0];
            if (ny < 0 || ny >= grid.Count || nx < 0 || nx >= grid[0].Count)
                continue;
            var ch = grid[ny][nx];
            if (ch == '#')
                continue;
            var neededDoors = current.doors;
            if (ch >= 'A' && ch <= 'Z')
            {
                var neededKey = char.ToLower(ch);
                if (!neededDoors.Contains(neededKey))
                    neededDoors += neededKey;
            }

            var key = (ny, nx);
            if (minDoorsSet.TryGetValue(key, out var previous) && previous.Length <= neededDoors.Length)
                continue;
            minDoorsSet[key] = neededDoors;
            queue.Enqueue((ny, nx, current.dist + 1, neededDoors));
        }
    }

    private static Dictionary<(int, int), int> CreateKeyIndexAndUpdateNodes(List<List<char>> grid,
        List<(int y, int x)> nodes)
    {
        var keyIndex = new Dictionary<(int, int), int>();
        for (var x = 0; x < grid[0].Count; x++)
        for (var y = 0; y < grid.Count; y++)
            if (grid[y][x] >= 'a' && grid[y][x] <= 'z')
            {
                keyIndex[(y, x)] = nodes.Count;
                nodes.Add((y, x));
            }

        return keyIndex;
    }

    private static List<(int y, int x)> GetStartPositions(List<List<char>> grid)
    {
        var nodes = new List<(int y, int x)>();
        for (var x = 0; x < grid[0].Count; x++)
        for (var y = 0; y < grid.Count; y++)
            if (grid[y][x] == '@')
                nodes.Add((y, x));

        return nodes;
    }

    private struct Edge
    {
        public readonly int To;
        public readonly int Distance;
        public readonly string NeededKeys;

        public Edge(int to, int distance, string neededKeys)
        {
            To = to;
            Distance = distance;
            NeededKeys = neededKeys;
        }
    }

    private static string Encode(int[] pos, string keys) => string.Join(",", pos) + "#" + keys;

    public static void Main()
    {
        var data = GetInput();
        var result = Solve(data);

        if (result == -1)
            Console.WriteLine("No solution found");
        else
            Console.WriteLine(result);
    }
}