using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Travelling {
    class Traveler : ICloneable {
        private List<string> route = new List<string>();
        private string currentLocation = "";
        private string name = "";

        public string Name { get => name; set => name = value; }
        public string CurrentLocation { get => currentLocation; set => currentLocation = value; }
        public List<string> Route { get => route; set => route = value; }
        public Traveler(string name) => this.name = name;
        public void ClearRoute() => route.Clear();
        public bool HasCity(string city) { return route.Contains(Capitalize(city)); }
        public int GetStopCount() { return route.Count(); }
        public override string ToString() { string s = "Traveler: " + GetName() + " | Location: " + GetLocation() + " | Route: " + GetRoute(); return s; }

        public void PlanRouteTo(string destination, CityGraph map) {
            if (currentLocation != "") route = map.FindShortestPath(currentLocation, destination);
            else {
                if (route.Count == 0) throw new Exception("No route!");
                else
                {
                    route = map.FindShortestPath(destination, route[0]);
                }
            }
            if (route.Count == 0) throw new Exception("No route!");
        }
        public object Clone() {
            var clone = new Traveler(this.GetName());
            clone.SetLocation(this.GetLocation() ?? "");

            string routeStr = GetRoute();

            if (routeStr != "") {
                foreach (string city in routeStr.Split(" -> ")) {
                    clone.AddCity(city);
                }
            }
            return clone;
        }
        public bool RemoveCity(string city) {
            if (HasCity(Capitalize(city))) {
                route.Remove(Capitalize(city));
                return true;
            }
            else return false;
        }
        private static string Capitalize(string word) {
            string s = char.ToUpper(word[0])!.ToString();
            for (int i = 1; i < word.Length; i++) { if (word[i - 1] == ' ' || word[i - 1] == '-') { s += char.ToUpper(word[i]); } else { s += word[i]; } }
            return s;
        }
        public override bool Equals(object? obj) {
            if (obj is Traveler other) {
                return name == other.name && currentLocation == other.currentLocation && route.SequenceEqual(other.route);
            }
            return false;
        }
        public override int GetHashCode() {
            int hash = name.GetHashCode() ^ currentLocation.GetHashCode();
            foreach (var city in route)
                hash ^= city.GetHashCode();
            return hash;
        }

        public static bool operator ==(Traveler? a, Traveler? b) {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }
        public static bool operator !=(Traveler? a, Traveler? b) => !(a == b);
        public string? this[int index] {
            get {
                if (index < 0 || index > route.Count - 1) return null;         
                else  return route[index];               
            }
        }
        public string? GetNextStop() {
            if (route.Count >= 1) return route[0];        
            else return null;    
        }
        public void AddCity(string city) {
            if (string.IsNullOrWhiteSpace(city))
                throw new Exception("City name cannot be null, empty, or whitespace.");
            route.Add(Capitalize(city));
        }
        public void SortRoute() { route.Sort(); return; }
        public string GetRoute() {
            if (route.Count == 0) return "";
            string s = route[0];
            for (int i = 1; i < route.Count; i++) {
                s += " -> " + route[i];
            }
            return s;
        }
        public string GetName() { return name; }
        public void SetLocation(string location) { currentLocation = Capitalize(location); }
        public string GetLocation() { return currentLocation; }

        public void SaveToFile(string filePath){
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Cannot save traveler with empty name");

            foreach (var city in route) {
                if (string.IsNullOrWhiteSpace(city))
                    throw new Exception("Cannot save traveler with empty city in route");

                if (!IsValidName(city))
                    throw new Exception($"Cannot save traveler with invalid city name: '{city}'");
            }

            var Data = new {
                name,
                currentLocation,
                route
            };

            string fileName = filePath.EndsWith(".json") ? filePath : filePath + ".json";

            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(Data, options);

            jsonString = jsonString.Replace("[", "[")
                                   .Replace("\n  ]", "]")
                                   .Replace("\n    ", " ");

            File.WriteAllText(fileName, jsonString, Encoding.UTF8);
        }
        public static Traveler LoadFromFile(string filePath) {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File doesn't exist", filePath);

            string jsonString = File.ReadAllText(filePath);

            try {
                var temp = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString)!;

                if (!temp.TryGetValue("name", out JsonElement nameElement) ||
                    !temp.TryGetValue("currentLocation", out JsonElement locElement))
                    throw new FileLoadException("Invalid travel data - missing required fields");

                string name = nameElement.GetString() ?? "";
                string location = locElement.GetString() ?? "";

                if (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
                    throw new FileLoadException("Name can only contain letters, spaces and hyphens!");

                if (!string.IsNullOrWhiteSpace(location) && !IsValidName(location))
                    throw new FileLoadException("Location can only contain letters, spaces and hyphens!");

                var t = new Traveler(name);
                t.CurrentLocation = location;

                if (temp.TryGetValue("route", out JsonElement route) && route.ValueKind == JsonValueKind.Array) {
                    t.Route.Clear();
                    foreach (var city in route.EnumerateArray()) {
                        string c = city.GetString() ?? "";

                        if (string.IsNullOrWhiteSpace(c))
                            throw new FileLoadException("Empty city name in route!");

                        if (!IsValidName(c))
                            throw new FileLoadException($"City name '{c}' can only contain letters, spaces and hyphens!");

                        t.Route.Add(Capitalize(c));
                    }
                }
                return t;
            }
            catch (JsonException) {
                throw new FileLoadException("Invalid JSON format");
            }
        }
        private static bool IsValidName(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            foreach (char c in input) {
                bool isLatinLetter = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
                bool isCyrillicLetter = (c >= 'À' && c <= 'ß') || (c >= 'à' && c <= 'ÿ');
                bool isSpaceOrHyphen = c == ' ' || c == '-';

                if (!(isLatinLetter || isCyrillicLetter || isSpaceOrHyphen)) {
                    return false;
                }
            }
            return true;
        }
    }
}
