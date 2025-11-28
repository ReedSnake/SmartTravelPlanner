public struct TNode {
    public string city { get; }
    public TNode(string city) { this.city = city; }
}
public struct TEdge {
    public string city { get; }
    public int distance { get; }
    public TEdge(string city, string distance) { this.city = city; this.distance = int.Parse(distance); }
}
public class CityGraph {
    public Dictionary<TNode, List<TEdge>> adjacencyList = new();

    public int GetPathDistance(List<string> path) {
        if (path == null || path.Count < 2)
            return 0;

        int sum = 0;

        for (int i = 0; i < path.Count - 1; i++) {
            string currentCity = path[i];
            string nextCity = path[i + 1];

            var currentNode = new TNode(currentCity);

            if (!adjacencyList.ContainsKey(currentNode))
                return 0;

            bool found = false;

            foreach (var edge in adjacencyList[currentNode]) {
                if (edge.city == nextCity) {
                    sum += edge.distance;
                    found = true;
                    break;
                }
            }

            if (!found)
                return 0;
        }

        return sum;
    }
    public List<string> FindShortestPath(string from, string to) {
        var shortestPath = new List<string>();
        int minDistance = int.MaxValue;

        FindPaths(from, to, new HashSet<string>(), new List<string> { from }, 0, ref shortestPath, ref minDistance);
        if (shortestPath.Count == 0) {
            FindPaths(to, from, new HashSet<string>(), new List<string> { to }, 0, ref shortestPath, ref minDistance);
            var shortest = new List<string>();
            for (int i = shortestPath.Count - 1; i > -1; i--) shortest.Add(shortestPath[i]);
            return shortest;
        }
        return shortestPath;
    }

    private void FindPaths(string currentCity, string endCity, HashSet<string> visited, List<string> currentPath, int currentDistance, ref List<string> shortestPath, ref int minDistance) {
        if (currentCity.Equals(endCity)) {
            if (currentDistance < minDistance) {
                minDistance = currentDistance;
                shortestPath = new List<string>(currentPath);
            }
            return;
        }

        if (currentDistance >= minDistance) {
            return;
        }

        visited.Add(currentCity);

        var currentNode = new TNode(currentCity);
        if (adjacencyList.ContainsKey(currentNode)) {
            foreach (var edge in adjacencyList[currentNode]) {
                if (!visited.Contains(edge.city)) {
                    currentPath.Add(edge.city);
                    FindPaths(edge.city, endCity, visited, currentPath, currentDistance + edge.distance, ref shortestPath, ref minDistance);
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
            }
        }
        visited.Remove(currentCity);
    }
    public static CityGraph LoadFromFile(string filePath) {
        var file = File.ReadAllLines(filePath);
        var lineList = new List<string>(file);

        CityGraph graph = new CityGraph();
        string[] itemline;

        foreach (string line in lineList) {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            itemline = line.Split(new char[] { '-', ',' });

            if (itemline.Length != 3)
                throw new FormatException($"Invalid line format: {line}");

            if (!IsValidCityName(itemline[0]))
                throw new FormatException($"City name '{itemline[0]}' can only contain letters, spaces and hyphens!");

            if (!IsValidCityName(itemline[1]))
                throw new FormatException($"City name '{itemline[1]}' can only contain letters, spaces and hyphens!");

            if (!int.TryParse(itemline[2], out int distance) || distance <= 0)
                throw new FormatException($"Invalid distance in line: {line}");

            if (!graph.adjacencyList.ContainsKey(new TNode(itemline[0])))
                graph.adjacencyList[new TNode(itemline[0])] = new List<TEdge>();
            graph.adjacencyList[new TNode(itemline[0])].Add((new TEdge(itemline[1], itemline[2])));

            if (!graph.adjacencyList.ContainsKey(new TNode(itemline[1])))
                graph.adjacencyList[new TNode(itemline[1])] = new List<TEdge>();
            graph.adjacencyList[new TNode(itemline[1])].Add((new TEdge(itemline[0], itemline[2])));
        }

        return graph;
    }
    private static bool IsValidCityName(string name) {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        foreach (char c in name) {
            bool isLatinLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            bool isCyrillicLetter = (c >= 'À' && c <= 'ß') || (c >= 'à' && c <= 'ÿ');
            bool isSpaceOrHyphen = c == ' ' || c == '-';

            if (!(isLatinLetter || isCyrillicLetter || isSpaceOrHyphen))
            {
                return false;
            }
        }
        return true;
    }

    public void AddCity(List<string> itemline) {
        if (!IsValidCityName(itemline[0]))
            throw new Exception($"City name '{itemline[0]}' can only contain letters, spaces and hyphens!");

        if (!IsValidCityName(itemline[1]))
            throw new Exception($"City name '{itemline[1]}' can only contain letters, spaces and hyphens!");

        var fromNode = new TNode(itemline[0]);
        var toNode = new TNode(itemline[1]);
        var distance = itemline[2];

        if (!adjacencyList.ContainsKey(fromNode))
            adjacencyList[fromNode] = new List<TEdge>();
        adjacencyList[fromNode].Add(new TEdge(itemline[1], distance));

        if (!adjacencyList.ContainsKey(toNode))
            adjacencyList[toNode] = new List<TEdge>();
        adjacencyList[toNode].Add(new TEdge(itemline[0], distance));
    }
    public override string ToString() {
        string s = "";
        foreach (var key in adjacencyList) {
            TNode from = key.Key;
            List<TEdge> edges = key.Value;

            foreach (var edge in edges) {
                s += $"{from.city}-{edge.city},{edge.distance}\n";
            }
        }
        return s.Substring(0, s.Length - 1);
    }
    public bool RemoveCity(string cityName) {
        var cityNode = new TNode(cityName);
        if (!adjacencyList.ContainsKey(cityNode))
            return false;
        foreach (var edge in adjacencyList[cityNode]) {
            var connectedNode = new TNode(edge.city);
            if (adjacencyList.ContainsKey(connectedNode)) {
                adjacencyList[connectedNode] = adjacencyList[connectedNode]
                    .Where(e => e.city != cityName)
                    .ToList();
            }
        }
        adjacencyList.Remove(cityNode);
        return true;
    }
}
